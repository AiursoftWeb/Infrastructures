using Aiursoft.XelNaga.Tools;
using Aiursoft.XelNaga.Attributes;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;


namespace Aiursoft.XelNaga.Tests.Tools
{
    [TestClass]
    public class CSVGeneratorTests
    {
        private class Model
        {
            [CSVProperty("Id")]
            public int Id { get; set; }
        }
        private class Person : Model
        {
            [CSVProperty("Name")]
            public string Name { get; set; }
        }

        [TestMethod]
        public void BuildFromListEmpty()
        {
            Person[] persons = Array.Empty<Person>();
            byte[] expect = "\"Name\",\"Id\"".ToUTF8WithDom();
            CSVGenerator generator = new();
            CollectionAssert.AreEqual(expect, generator.BuildFromList(persons));
        }

        [TestMethod]
        public void BuildFromList()
        {
            Person[] persons = new [] {
                new Person { Id = 1, Name = "alice" },
                new Person { Id = 2, Name = "bob" },
            };
            byte[] expect = "\"Name\",\"Id\"\r\n\"alice\",\"1\"\r\n\"bob\",\"2\"".ToUTF8WithDom();
            CSVGenerator generator = new();
            CollectionAssert.AreEqual(expect, generator.BuildFromList(persons));
        }
    }
}