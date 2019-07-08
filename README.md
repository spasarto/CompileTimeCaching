# Compile Time Caching
Takes an in memory object and generates a factory class synatx tree (Rosyln) that produces an exact copy of that object when invoked. Pass the resulting SyntaxTree to your favorite compile time code generation library such as [Uno.SourceGeneration](https://github.com/unoplatform/Uno.SourceGeneration) or [CodeGeneration.Roslyn](https://github.com/AArnott/CodeGeneration.Roslyn) to generate the cache when the project builds.

## Supported Objects
* Public and private objects
* Objects with parameterless constructor
* Objects without parameterless constructor
* Objects with private constructor
* Objects with type parameters
* Objects with readonly fields

And everything in between.

## Example
Assume you have a class like this:

```csharp
    public class Foo
    {
        public int Value { get; set; }
    }
```

We can create an instance of that class and initialize it to our favorite value:
```csharp
    var target = new Foo
    {
        Value = 42
    };
```

We can then use CacheBuilder to generate a ```SyntaxTree``` that represents how to generate an instance of that object in the same state:
```csharp
    var syntax = CacheBuilder.CreateCache(target);

    var syntaxString = syntax.ToString();
```

Value of ```syntaxString```:
```csharp
namespace CompiledCache
{
    static class FooFactory
    {
        static T CreateObject<T>()
        {
            return (T)System.Runtime.Serialization.FormatterServices.GetUninitializedObject(typeof(T));
        }

        public static Foo Create()
        {
            var var0 = CreateObject<Foo>();
            var type0 = typeof(Foo);
            var field0 = type0.GetField("<Value>k__BackingField", System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            field0.SetValue(var0, 42);
            return var0;
        }
    }
}
```

## Why
If you are of sound mind, you may be asking yourself, "Why would I ever want to do this?" One possible response is fluent configurations. Frameworks that use a fluent style configuration provide compile time framework usage validation at the cost of a one time runtime penality, typically during application startup. While this is great, most frameworks don't actually validate the configuration until runtime. Additionally, when using multiple fluent frameworks, your project's startup time could begin to get away from you. 

Consider the following tech stack:
* ASP .NET Core 2.2
* Microsoft.Extensions.DependencyInjection
* ASP .NET Core OData
* Auto Mapper
* Fluent Assertions
* Entity Framework

Using that setup, you are able to provide a RESTful OData service which accesses the database using an ORM and transforms and validates the data seemlessly. Pretty nice. However, each of those frameworks require some form of configuration at startup. In my personal profiling experience, I've seen upwards to 10 seconds added to the startup time. That could be considered acceptable since it is a one time cost; however, each of these frameworks report configuration problems at runtime. Meaning some poor developer (you) is going to have to suffer through that start up time needless to track down that they forgot to register their type to their interface (again).

Additionally, while these configurations may change during development, they never/rarely change once deployed. Why rebuild them?

## Future
Some potential TODOs for this project:
* Add type blacklisting
* Add a sample project using the above stack
* Integrate more seemlessly with a compilation project


