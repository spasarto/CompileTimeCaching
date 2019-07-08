using System;
using System.Collections.Generic;
using System.Text;

namespace CompileTimeCaching.Tests.SampleObjects
{
    public class PublicObjectWithReadonlyField
    {
        private readonly int _value;
        public int Value => _value;

        public PublicObjectWithReadonlyField(int value)
        {
            _value = value;
        }
    }
}
