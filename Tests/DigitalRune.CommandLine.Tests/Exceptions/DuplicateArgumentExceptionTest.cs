﻿using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using NUnit.Framework;


namespace DigitalRise.CommandLine.Tests
{
    [TestFixture]
    public class DuplicateArgumentExceptionTest
    {
        [Test]
        public void ConstructorTest0()
        {
            var exception = new DuplicateArgumentException();
        }


        [Test]
        public void ConstructorTest1()
        {
            const string message = "message";
            var exception = new DuplicateArgumentException(message);
            Assert.AreEqual(message, exception.Message);
        }


        [Test]
        public void ConstructorTest2()
        {
            const string message = "message";
            var innerException = new Exception();
            var exception = new DuplicateArgumentException(message, innerException);
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
        }


        [Test]
        public void ConstructorTest3()
        {
            var argument = new ValueArgument<string>("arg", "");
            var exception = new DuplicateArgumentException(argument);
            Assert.AreEqual(argument.Name, exception.Argument);
        }


        [Test]
        public void ConstructorTest4()
        {
            var argument = new ValueArgument<string>("arg", "");
            const string message = "message";
            var exception = new DuplicateArgumentException(argument, message);
            Assert.AreEqual(argument.Name, exception.Argument);
            Assert.AreEqual(message, exception.Message);
        }


        [Test]
        public void ConstructorTest5()
        {
            var argument = new ValueArgument<string>("arg", "");
            const string message = "message";
            var innerException = new Exception();
            var exception = new DuplicateArgumentException(argument, message, innerException);
            Assert.AreEqual(argument.Name, exception.Argument);
            Assert.AreEqual(message, exception.Message);
            Assert.AreEqual(innerException, exception.InnerException);
        }


        [Test]
        public void ConstructorExceptions()
        {
            Assert.Throws<ArgumentNullException>(() => new DuplicateArgumentException((Argument)null));
        }


        [Test]
        public void SerializationTest()
        {
            var argument = new ValueArgument<string>("arg", "");
            const string message = "message";
            const string innerMessage = "Inner exception";
            var innerException = new Exception(innerMessage);
            var exception = new DuplicateArgumentException(argument, message, innerException);

            using (var memoryStream = new MemoryStream())
            {
                // Serialize exception.
                var formatter = new BinaryFormatter();
                formatter.Serialize(memoryStream, exception);

                memoryStream.Position = 0;

                // Deserialize exception.
                formatter = new BinaryFormatter();
                var deserializedException = (DuplicateArgumentException)formatter.Deserialize(memoryStream);

                Assert.AreEqual(argument.Name, deserializedException.Argument);
                Assert.AreEqual(message, deserializedException.Message);
                Assert.AreEqual(innerMessage, deserializedException.InnerException.Message);
            }
        }
    }
}
