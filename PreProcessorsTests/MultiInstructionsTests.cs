using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Embedded_C_Parser.Instructions;

namespace Embedded_C_ParserTests
{
    [TestClass]
    public class MultiInstructionsTests
    {
        [TestMethod]
        public void UsingPreDeclaredVarInArythemicStatement()
        {
            InstructionsBlock block = new InstructionsBlock();

            string variableDefinition = "int x = 4";
            var declaration = new VariableDeclarationInstruction(variableDefinition, block);

            string variableDefinition2 = "int y = 7 + x";
            var declaration2 = new VariableDeclarationInstruction(variableDefinition2, block);

            declaration.Execute();
            declaration2.Execute();

            Assert.AreEqual(typeof(int), block.localVariables["x"].type);
            Assert.AreEqual(4.0, block.localVariables["x"].value);

            Assert.AreEqual(typeof(int), block.localVariables["y"].type);
            Assert.AreEqual(11.0, block.localVariables["y"].value);

        }
    }
}
