using System;
using System.Collections.Generic;
using System.Text;

namespace CompileTimeCaching.Tests.SampleObjects
{
    public class PublicObjectWithPrivateCtor
    {
        public int Value { get; set; }

        private PublicObjectWithPrivateCtor()
        {
        }

        public static PublicObjectWithPrivateCtor Create(int value)
        {
            return new PublicObjectWithPrivateCtor
            {
                Value = value
            };
        }
    }
}
