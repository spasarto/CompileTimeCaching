using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.Serialization;

namespace CompileTimeCaching
{
    public static class CacheBuilder
    {
        public static SyntaxTree CreateCache<T>(T item, CacheBuilderOptions options = default)
        {
            var type = typeof(T);
            options = InitializeOptions(options, type);

            var newClass = SyntaxFactory.ClassDeclaration(options.GeneratedTypeName)
                                        .AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                                        .AddMembers(
                                            GenerateCreateObjectMethod(),
                                            SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName(type.FullName), "Create")
                                                .AddModifiers(SyntaxFactory.Token(SyntaxKind.PublicKeyword))
                                                .AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                                                .WithBody(SyntaxFactory.Block(GenerateStatements(item))));

            var newNamespace = SyntaxFactory.NamespaceDeclaration(SyntaxFactory.IdentifierName(options.GeneratedNamespace))
                                            .AddMembers(newClass)
                                            .NormalizeWhitespace();

            return SyntaxFactory.SyntaxTree(newNamespace);
        }

        private static CacheBuilderOptions InitializeOptions(CacheBuilderOptions options, Type type)
        {
            options = options ?? new CacheBuilderOptions();
            options.GeneratedTypeName = options.GeneratedTypeName ?? $"{type.Name}Factory";
            options.GeneratedNamespace = options.GeneratedNamespace ?? "CompiledCache";

            return options;
        }

        private static MethodDeclarationSyntax GenerateCreateObjectMethod()
        {
            return SyntaxFactory.MethodDeclaration(SyntaxFactory.ParseTypeName("T"), "CreateObject")
                                .AddModifiers(SyntaxFactory.Token(SyntaxKind.StaticKeyword))
                                .WithTypeParameterList(SyntaxFactory.TypeParameterList(SyntaxFactory.SeparatedList<TypeParameterSyntax>(new List<TypeParameterSyntax>() { SyntaxFactory.TypeParameter("T") })))
                                .WithBody(SyntaxFactory.Block(SyntaxFactory.ParseStatement("return (T)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));")))
                                ;
        }

        private static SyntaxList<StatementSyntax> GenerateStatements<T>(T item)
        {
            var context = new CacheBuilderContext();
            var variableName = Buildup(item, context);

            context.Statements.Add(SyntaxFactory.ParseStatement($"return {variableName};"));
            return SyntaxFactory.List(context.Statements);
        }

        private static string Buildup(object target, CacheBuilderContext context)
        {
            var type = target.GetType();

            if (!context.TypeFieldsCache.TryGetValue(type, out ICollection<FieldInfo> fields))
            {
                fields = GetAllFields(type);
                context.TypeFieldsCache[type] = fields;
            }

            // Create an instance of the type, and generate a variable name for that instance.
            object obj = FormatterServices.GetUninitializedObject(type);
            string variableName = CreateVariableName("var", context.ExistingVariables);
            context.ObjectCache[target] = obj;
            context.VariableMap[obj] = variableName;
            context.Statements.Add(SyntaxFactory.ParseStatement($"var {variableName} = CreateObject<{type.FullName}>();"));

            foreach (var field in fields)
            {
                string fieldVariableName = null;

                // If the field is not public, we have to set it via reflection. Initialize the Type and FieldInfo variables.
                if (!field.IsPublic)
                {
                    if (!context.VariableMap.TryGetValue(field.DeclaringType, out string typeVariableName))
                    {
                        typeVariableName = CreateVariableName("type", context.ExistingVariables);
                        context.VariableMap[type] = typeVariableName;
                        context.Statements.Add(SyntaxFactory.ParseStatement($"var {typeVariableName} = typeof({field.DeclaringType.FullName});"));
                    }

                    if (!context.VariableMap.TryGetValue(field, out fieldVariableName))
                    {
                        fieldVariableName = CreateVariableName("field", context.ExistingVariables);
                        context.VariableMap[field] = fieldVariableName;
                        context.Statements.Add(SyntaxFactory.ParseStatement($"var {fieldVariableName} = {typeVariableName}.GetField(\"{field.Name}\", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);"));
                    }
                }

                var fieldValue = field.GetValue(target);
                string variableNameOrValue = null;

                // Generate the syntax for the field value.
                if (field.FieldType.IsPrimitive || fieldValue == null)
                {
                    variableNameOrValue = fieldValue?.ToString() ?? "null";
                }
                else if (field.FieldType == typeof(string))
                {
                    variableNameOrValue = $"\"{fieldValue}\"";
                }
                else if (!context.ObjectCache.ContainsKey(fieldValue))
                {
                    Buildup(fieldValue, context);
                }

                if (variableNameOrValue == null)
                {
                    variableNameOrValue = context.VariableMap[context.ObjectCache[fieldValue]];
                }

                if (fieldVariableName == null)
                {
                    context.Statements.Add(SyntaxFactory.ParseStatement($"{variableName}.{field.Name} = {variableNameOrValue};"));
                }
                else
                {
                    context.Statements.Add(SyntaxFactory.ParseStatement($"{fieldVariableName}.SetValue({variableName}, {variableNameOrValue});"));
                }
            }

            return variableName;
        }

        private static ICollection<FieldInfo> GetAllFields(Type type)
        {
            Type t = type;
            var fields = new List<FieldInfo>();
            while (t != typeof(object))
            {
                fields.AddRange(t.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance));
                t = t.BaseType;
            }

            return fields;
        }

        private static string CreateVariableName(string typeName, IDictionary<string, int> existingVariables)
        {
            existingVariables.TryGetValue(typeName, out int i);

            string variableName = $"{typeName}{i++}";
            existingVariables[typeName] = i;

            return variableName;
        }
    }
}
