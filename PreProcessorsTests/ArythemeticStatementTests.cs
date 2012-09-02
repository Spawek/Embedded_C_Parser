using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Embedded_C_Parser;

namespace Embedded_C_ParserTests
{
    [TestClass]
    public class ArythemeticStatementTests
    {
        [TestMethod]
        [DeploymentItem("Embedded_C_Parser.exe")]
        public void ArythemeticStetementBasicTest()
        {
            string basicInput = "2 + 2";
            ArythmeticStatement testStatement = new ArythmeticStatement(basicInput, new InstructionsBlock());

            variable result = testStatement.Execute();

            Assert.AreEqual(typeof(int), result.type);
            Assert.AreEqual(4, (int)(result.value));
        }

        [TestMethod]
        [DeploymentItem("Embedded_C_Parser.exe")]
        public void ArythemeticStetementPriorityTest()
        {
            string basicInput = "2 + 2 * 2";
            ArythmeticStatement testStatement = new ArythmeticStatement(basicInput, new InstructionsBlock());

            variable result = testStatement.Execute();

            Assert.AreEqual(typeof(int), result.type);
            Assert.AreEqual(6, (int)(result.value));
        }

        [TestMethod]
        [DeploymentItem("Embedded_C_Parser.exe")]
        public void ArythemeticStetementDoubleTypeTest()
        {
            string basicInput = "2.0 + 2.0 * 2.0";
            ArythmeticStatement testStatement = new ArythmeticStatement(basicInput, new InstructionsBlock());

            variable result = testStatement.Execute();

            Assert.AreEqual(typeof(double), result.type);
            Assert.AreEqual(6.0, (double)(result.value));
        }
    }
}
