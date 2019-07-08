using System;
using System.Collections.Generic;
using System.Text;

namespace CompileTimeCaching.Tests.SampleObjects
{
    public class PublicObjectWithGenerics<T>
    {
        public T Value { get; set; }
    }
}
