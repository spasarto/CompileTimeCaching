using System;
using System.Collections.Generic;
using System.Text;

namespace CompileTimeCaching.Tests.SampleObjects
{
    public class Inheritance : BaseInheritance
    {
        public int Value { get; set; }
    }

    public class BaseInheritance
    {
        public int BaseValue { get; set; }
    }
}
