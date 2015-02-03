using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Cmd.Net.Tests
{
    [TestClass]
    public class CommandContextTests
    {
        #region Public Methods

        [TestMethod]
        public void ExecuteChild()
        {
            bool commandExecuted = false;

            new CommandContext(
                "root",
                new DelegateCommand(
                    "child",
                    new Action(() => commandExecuted = true)
                    )
                )
                .Execute("child");

            Assert.IsTrue(commandExecuted);
        }

        [TestMethod]
        public void NavigateToChild()
        {
            CommandContext childContext2 = new CommandContext("child2");
            CommandContext childContext1 = new CommandContext("child1", childContext2);
            CommandContext rootContext = new CommandContext("root", childContext1);

            using (new CommandContextScope(rootContext))
            {
                CommandContext.Current.Execute("child1");
                Assert.AreEqual(CommandContext.Current, childContext1);
            }
        }

        [TestMethod]
        public void NavigateToChildDirect()
        {
            CommandContext childContext2 = new CommandContext("child2");
            CommandContext childContext1 = new CommandContext("child1", childContext2);
            CommandContext rootContext = new CommandContext("root", childContext1);

            using (new CommandContextScope(rootContext))
            {
                CommandContext.Current.Execute("child1 child2");
                Assert.AreEqual(CommandContext.Current, childContext2);
            }
        }

        [TestMethod]
        public void NavigateToParent()
        {
            CommandContext childContext2 = new CommandContext("child2");
            CommandContext childContext1 = new CommandContext("child1", childContext2);
            CommandContext rootContext = new CommandContext("root", childContext1);

            using (new CommandContextScope(rootContext))
            {
                CommandContext.Current.Execute("child1");
                CommandContext.Current.Execute("..");
                Assert.AreEqual(CommandContext.Current, rootContext);
            }
        }

        [TestMethod]
        public void NavigateToParentDirect()
        {
            CommandContext childContext2 = new CommandContext("child2");
            CommandContext childContext1 = new CommandContext("child1", childContext2);
            CommandContext rootContext = new CommandContext("root", childContext1);

            using (new CommandContextScope(rootContext))
            {
                CommandContext.Current.Execute("child1 child2");
                CommandContext.Current.Execute(".. ..");
                Assert.AreEqual(CommandContext.Current, rootContext);
            }
        }

        [TestMethod]
        public void NavigateToSibling()
        {
            CommandContext childContext3 = new CommandContext("child3");
            CommandContext childContext2 = new CommandContext("child2");
            CommandContext childContext1 = new CommandContext("child1", childContext2, childContext3);
            CommandContext rootContext = new CommandContext("root", childContext1);

            using (new CommandContextScope(rootContext))
            {
                CommandContext.Current.Execute("child1 child2");
                CommandContext.Current.Execute(".. child3");
                Assert.AreEqual(CommandContext.Current, childContext3);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(CommandException))]
        public void NavigateToUnknown()
        {
            new CommandContext(
                "root",
                new CommandContext("child2"),
                new CommandContext("child1")
                )
                .Execute("child3");
        }

        #endregion
    }
}
