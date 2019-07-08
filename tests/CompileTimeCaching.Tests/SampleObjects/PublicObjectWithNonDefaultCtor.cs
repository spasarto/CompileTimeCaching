using System;
using System.Collections.Generic;
using System.Text;

namespace CompileTimeCaching.Tests.SampleObjects
{
    public class PublicObjectWithNonDefaultCtor
    {
        public int Value { get; set; }

        public PublicObjectWithNonDefaultCtor(int value)
        {
            Value = value;
        }
    }
}
