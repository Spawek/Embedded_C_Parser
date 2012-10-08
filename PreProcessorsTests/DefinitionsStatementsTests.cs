using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Embedded_C_Parser.Instructions;

namespace Embedded_C_ParserTests
{
    [TestClass]
    public class DefinitionsStatementsTests
    {
        [TestMethod]
        [DeploymentItem("Embedded_C_Parser.exe")]
        public void VariableDefiningTest()
        {
            string variableDefinition = "double x = 4";

            InstructionsBlock block = new InstructionsBlock();

            var declaration = new VariableDeclarationInstruction(variableDefinition, block);

            declaration.Execute();

            Assert.AreEqual(typeof(double), block.localVariables["x"].type);
            Assert.AreEqual(4.0, block.localVariables["x"].value);
        }
    }
}
