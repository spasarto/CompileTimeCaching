using System;
using System.Collections.Generic;
using System.Text;

namespace CompileTimeCaching.Tests.SampleObjects
{
    public class PrivateObjectWithDefaultCtorWrapper
    {
        public object Value { get; set; }

        private class PrivateObjectWithDefaultCtor
        {
            public int Value { get; set; }
        }

        public void Intialize(int value)
        {
            Value = new PrivateObjectWithDefaultCtor()
            {
                Value = value
            };
        }
    }
}
