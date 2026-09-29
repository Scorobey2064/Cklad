using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cklad.Core.Models;
using Cklad.Core.Services;

namespace Cklad.Tests
{
    [TestClass]
    public class DataServiceTests
    {
        [TestInitialize]
        public void Setup()
        {
            DataService.ResetInstance();
        }

        [TestMethod]
        public void Instance_ShouldBeSingleton()
        {
            var a = DataService.Instance;
            var b = DataService.Instance;
            Assert.AreSame(a, b);
        }

        [TestMethod]
        public void ResetInstance_ShouldGiveNewInstance()
        {
            var a = DataService.Instance;
            DataService.ResetInstance();
            var b = DataService.Instance;

            Assert.AreNotSame(a, b);
        }

        [TestMethod]
        public void ResetInstance_NewInstanceHasFreshData()
        {
            DataService.Instance.Organizations.Add(new Organization { Name = "Temp" });
            DataService.ResetInstance();
            Assert.IsTrue(DataService.Instance.Organizations.Count > 0);
        }
    }
}
