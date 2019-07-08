using System;

namespace UnoGeneratorSampleConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            var target = CompiledCache.PublicObjectWithDefaultCtorFactory.Create();
            Console.WriteLine($"created a {target.GetType()} with Value of {target.Value}!");
            
        }
    }
}
