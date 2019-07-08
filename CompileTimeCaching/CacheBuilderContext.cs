using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Reflection;

namespace CompileTimeCaching
{
    internal class CacheBuilderContext
    {
        public IDictionary<object, object> ObjectCache { get; set; } = new Dictionary<object, object>();
        public IDictionary<object, string> VariableMap { get; set; } = new Dictionary<object, string>();
        public IList<StatementSyntax> Statements { get; set; } = new List<StatementSyntax>();
        public IDictionary<string, int> ExistingVariables { get; set; } = new Dictionary<string, int>();
        public IDictionary<Type, ICollection<FieldInfo>> TypeFieldsCache { get; set; } = new Dictionary<Type, ICollection<FieldInfo>>();
    }
}
