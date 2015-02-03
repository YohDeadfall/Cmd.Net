using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Cmd.Net.Tests
{
    [TestClass]
    public class ArgumentEnumeratorTests
    {
        #region Nested Types

        private struct Argument
        {
            #region Fields

            internal readonly string Name;
            internal readonly string Value;
            internal readonly string Representation;

            #endregion

            #region Constructors

            internal Argument(string name, string value, string representation)
            {
                this.Name = name;
                this.Value = value;
                this.Representation = representation;
            }

            #endregion
        }

        #endregion

        #region Public Methods

        [TestMethod]
        public void ContinueFromCurrent()
        {
            string[] arguments = new string[] { "/first", "/second" };
            ArgumentEnumerator enumerator1 = new ArgumentEnumerator(arguments);

            Assert.IsTrue(enumerator1.MoveNext());
            Assert.IsTrue(enumerator1.MoveNext());

            ArgumentEnumerator enumerator2 = enumerator1.ContinueFromCurrent();

            Assert.IsTrue(enumerator2.MoveNext());
            Assert.AreEqual(enumerator1.CurrentName, "second");
            Assert.AreEqual(enumerator1.CurrentName, enumerator2.CurrentName);
            Assert.AreEqual(enumerator1.CurrentValue, enumerator2.CurrentValue);
            Assert.IsFalse(enumerator2.MoveNext());
        }

        [TestMethod]
        public void ParseArguments()
        {
            Argument[] arguments = new Argument[]
            {
                new Argument("check", bool.TrueString, "/check"),
                new Argument("uncheck", bool.FalseString, "/-uncheck"),
                new Argument("named", "value", "/named:value"),
                new Argument(string.Empty, "unnamed", "unnamed"),
                new Argument("named", "quoted value\"\"", "/named:\"quoted value\"\"\""),
                new Argument(string.Empty, "quoted value\"\"", "\"quoted value\"\"\""),
            };

            foreach (IEnumerable<Argument> permutation in arguments.Permutations())
            {
                ArgumentEnumerator enumerator = new ArgumentEnumerator(string.Join(" ", permutation.Select(p => p.Representation)));

                foreach (Argument argument in permutation)
                {
                    Assert.IsTrue(enumerator.MoveNext());
                    Assert.AreEqual(enumerator.CurrentName, argument.Name);
                    Assert.AreEqual(enumerator.CurrentValue, argument.Value);
                }

                Assert.IsFalse(enumerator.MoveNext());
            }
        }

        [TestMethod]
        public void ParseArgumentsArray()
        {
            Argument[] arguments = new Argument[]
            {
                new Argument("check", bool.TrueString, "/check"),
                new Argument("uncheck", bool.FalseString, "/-uncheck"),
                new Argument("named", "value", "/named:value"),
                new Argument(string.Empty, "unnamed", "unnamed"),
                new Argument("named", "quoted value\"\"", "/named:quoted value\"\""),
                new Argument(string.Empty, "quoted value\"\"", "quoted value\"\""),
            };

            foreach (IEnumerable<Argument> permutation in arguments.Permutations())
            {
                ArgumentEnumerator enumerator = new ArgumentEnumerator(permutation.Select(p => p.Representation).ToArray());

                foreach (Argument argument in permutation)
                {
                    Assert.IsTrue(enumerator.MoveNext());
                    Assert.AreEqual(enumerator.CurrentName, argument.Name);
                    Assert.AreEqual(enumerator.CurrentValue, argument.Value);
                }

                Assert.IsFalse(enumerator.MoveNext());
            }
        }

        #endregion
    }
}
