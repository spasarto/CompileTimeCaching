using CompileTimeCaching;
using System;
using Uno.SourceGeneration;

namespace UnoGeneratorSample
{
    public class SampleGenerator : SourceGenerator
    {
        public override void Execute(SourceGeneratorContext context)
        {
            var target = new PublicObjectWithDefaultCtor
            {
                Value = new Random().Next()
            };
            var syntax = CacheBuilder.CreateCache(target);

            context.AddCompilationUnit("Test", syntax);
        }
    }

    public class PublicObjectWithDefaultCtor
    {
        public int Value { get; set; }
    }
}
