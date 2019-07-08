using CompileTimeCaching.Tests.SampleObjects;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace CompileTimeCaching.Tests
{
    [TestClass]
    public class ObjectCreationAcceptenceTests
    {
        private static readonly Random _random = new Random();

        [TestMethod]
        public void PublicObject_WithDefaultCtor_IsSuccess()
        {
            var target = new PublicObjectWithDefaultCtor()
            {
                Value = _random.Next()
            };

            var syntax = CacheBuilder.CreateCache(target);

            var syntaxString = syntax.ToString();

            Assert.IsNotNull(syntaxString);
        }

        [TestMethod]
        public void PublicObject_WithNonDefaultCtor_IsSuccess()
        {
            var target = new PublicObjectWithNonDefaultCtor(_random.Next());

            var syntax = CacheBuilder.CreateCache(target);

            var syntaxString = syntax.ToString();

            Assert.IsNotNull(syntaxString);
        }

        [TestMethod]
        public void PublicObject_WithPrivateCtor_IsSuccess()
        {
            var target = PublicObjectWithPrivateCtor.Create(_random.Next());

            var syntax = CacheBuilder.CreateCache(target);

            var syntaxString = syntax.ToString();

            Assert.IsNotNull(syntaxString);
        }

        [TestMethod]
        public void PublicObject_WithReadonlyField_IsSuccess()
        {
            var target = new PublicObjectWithReadonlyField(_random.Next());

            var syntax = CacheBuilder.CreateCache(target);

            var syntaxString = syntax.ToString();

            Assert.IsNotNull(syntaxString);
        }
        
        [TestMethod]
        public void PrivateObject_WithDefaultCtor_IsSuccess()
        {
            var target = new PrivateObjectWithDefaultCtorWrapper();
            target.Intialize(_random.Next());

            var syntax = CacheBuilder.CreateCache(target);

            var syntaxString = syntax.ToString();

            Assert.IsNotNull(syntaxString);
        }

        [TestMethod]
        public void PublicObject_WithGenerics_IsSuccess()
        {
            var target = new PublicObjectWithGenerics<int>()
            {
                Value = _random.Next()
            };

            var syntax = CacheBuilder.CreateCache(target);

            var syntaxString = syntax.ToString();

            Assert.IsNotNull(syntaxString);
        }

        [TestMethod]
        public void Inheritance_IsSuccess()
        {
            var target = new Inheritance()
            {
                BaseValue = _random.Next(),
                Value = _random.Next()
            };

            var syntax = CacheBuilder.CreateCache(target);

            var syntaxString = syntax.ToString();

            Assert.IsNotNull(syntaxString);
        }
        
        [TestMethod]
        public void PublicField_IsSuccess()
        {
            var target = new PublicField();
            target.Value = _random.Next();

            var syntax = CacheBuilder.CreateCache(target);

            var syntaxString = syntax.ToString();

            Assert.IsNotNull(syntaxString);
        }
    }
}
