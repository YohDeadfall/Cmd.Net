using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cmd.Net.Tests
{
    [TestClass]
    public class CommandExtensionsTests
    {
        #region Nested Types

        private sealed class InfinityStringReader : TextReader
        {
            #region Fields

            private readonly IEnumerator<string> lines;

            #endregion

            #region Constructors

            internal InfinityStringReader(IEnumerable<string> lines)
            {
                this.lines = lines.GetEnumerator();
            }

            #endregion

            #region TextReader Members

            public override string ReadLine()
            {
                if (lines.MoveNext())
                { return lines.Current; }

                lines.Reset();

                if (lines.MoveNext())
                { return lines.Current; }

                return null;
            }

            #endregion
        }

        #endregion

        #region Public Methods

        [TestMethod]
        public void CommandExecuteAll()
        {
            int executionCount = 0;

            new DelegateCommand(
                "command",
                new Action(() => { if (++executionCount == 4) { throw new CommandCanceledException(); } })
                )
                .ExecuteAll(
                    new InfinityStringReader(new string[] { string.Empty }),
                    TextWriter.Null
                );

            Assert.AreEqual(4, executionCount);
        }

        [TestMethod]
        public void CommandExecuteSingle()
        {
            int executionCount = 0;

            new DelegateCommand(
                "command",
                new Action(() => { if (++executionCount == 4) { throw new CommandCanceledException(); } })
                )
                .ExecuteSingle(
                    new InfinityStringReader(new string[] { string.Empty }),
                    TextWriter.Null
                );

            Assert.AreEqual(1, executionCount);
        }

        [TestMethod]
        public void ContextExecuteAll()
        {
            int executionCount = 0;

            new CommandContext(
                "root",
                new CommandContext(
                    "child",
                    new DelegateCommand(
                        "command",
                        new Action(() => { if (++executionCount == 4) { throw new CommandCanceledException(); } })
                        )
                    )
                )
                .ExecuteAll(
                    new InfinityStringReader(new string[] { "child", "command", ".." }),
                    TextWriter.Null
                );

            Assert.AreEqual(4, executionCount);
        }

        [TestMethod]
        public void ContextExecuteSingle()
        {
            int executionCount = 0;

            new CommandContext(
                "root",
                new DelegateCommand(
                    "command",
                    new Action(() => { if (++executionCount == 4) { throw new CommandCanceledException(); } })
                    )
                )
                .ExecuteSingle(
                    new InfinityStringReader(new string[] { "command" }),
                    TextWriter.Null
                );

            Assert.AreEqual(1, executionCount);
        }

        #endregion
    }
}
