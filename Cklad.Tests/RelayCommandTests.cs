using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Cklad.Core.ViewModels;

namespace Cklad.Tests
{
    [TestClass]
    public class RelayCommandTests
    {
        [TestMethod]
        public void Execute_ShouldInvokeAction()
        {
            bool called = false;
            var cmd = new RelayCommand(_ => called = true);

            cmd.Execute(null);

            Assert.IsTrue(called);
        }

        [TestMethod]
        public void CanExecute_NullPredicate_ReturnsTrue()
        {
            var cmd = new RelayCommand(_ => { });

            Assert.IsTrue(cmd.CanExecute(null));
        }

        [TestMethod]
        public void CanExecute_UsesPredicate()
        {
            var cmd = new RelayCommand(_ => { }, p => p != null);

            Assert.IsFalse(cmd.CanExecute(null));
            Assert.IsTrue(cmd.CanExecute("test"));
        }

        [TestMethod]
        public void RaiseCanExecuteChanged_ShouldFireEvent()
        {
            var cmd = new RelayCommand(_ => { });
            bool fired = false;
            cmd.CanExecuteChanged += (s, e) => fired = true;

            cmd.RaiseCanExecuteChanged();

            Assert.IsTrue(fired);
        }

        [TestMethod]
        public void Constructor_NullAction_Throws()
        {
            Assert.ThrowsException<ArgumentNullException>(() => new RelayCommand(null));
        }
    }
}
