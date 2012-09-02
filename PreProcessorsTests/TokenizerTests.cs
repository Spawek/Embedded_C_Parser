using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Embedded_C_Parser;

namespace Embedded_C_ParserTests
{
    [TestClass()]
    public class TokenizerTests
    {
        [TestMethod()]
        [DeploymentItem("Embedded_C_Parser.exe")]
        public void TokenizerTokenizeExampleTest()
        {
            string exampleStatement = " a = b + c + 2 ";
            List<Token> tokensList = Tokenizer.Tokenize(exampleStatement);

            Assert.AreEqual(7, tokensList.Count);

            Assert.AreEqual(typeof(VariableToken), tokensList[0].GetType());
            Assert.AreEqual("a", ((VariableToken)tokensList[0]).name);

            Assert.AreEqual(typeof(OperatorToken), tokensList[1].GetType());
            Assert.AreEqual("=", ((OperatorToken)tokensList[1]).oper.code);

            Assert.AreEqual(typeof(VariableToken), tokensList[2].GetType());
            Assert.AreEqual("b", ((VariableToken)tokensList[2]).name);

            Assert.AreEqual(typeof(OperatorToken), tokensList[3].GetType());
            Assert.AreEqual("+", ((OperatorToken)tokensList[3]).oper.code);

            Assert.AreEqual(typeof(VariableToken), tokensList[4].GetType());
            Assert.AreEqual("c", ((VariableToken)tokensList[4]).name);

            Assert.AreEqual(typeof(OperatorToken), tokensList[5].GetType());
            Assert.AreEqual("+", ((OperatorToken)tokensList[5]).oper.code);

            Assert.AreEqual(typeof(ConstantToken), tokensList[6].GetType());
            Assert.AreEqual(typeof(int), ((ConstantToken)tokensList[6]).type);
            Assert.AreEqual(2, (int)((ConstantToken)tokensList[6]).val);
        }

        /// <summary>
        /// should be a,b,c,+,2,+,=
        /// </summary>
        [TestMethod()]
        [DeploymentItem("Embedded_C_Parser.exe")]
        public void TokenizerRPNExampleTest()
        {
            string exampleStatement = " a = b + c + 2 ";
            List<Token> tokensList = Tokenizer.Tokenize(exampleStatement);
            List<Token> tokensListInRPNOrder = Tokenizer.RPN(tokensList);

            Assert.AreEqual(typeof(VariableToken), tokensListInRPNOrder[0].GetType());
            Assert.AreEqual("a", ((VariableToken)tokensListInRPNOrder[0]).name);

            Assert.AreEqual(typeof(VariableToken), tokensListInRPNOrder[1].GetType());
            Assert.AreEqual("b", ((VariableToken)tokensListInRPNOrder[1]).name);

            Assert.AreEqual(typeof(VariableToken), tokensListInRPNOrder[2].GetType());
            Assert.AreEqual("c", ((VariableToken)tokensListInRPNOrder[2]).name);

            Assert.AreEqual(typeof(OperatorToken), tokensListInRPNOrder[3].GetType());
            Assert.AreEqual("+", ((OperatorToken)tokensListInRPNOrder[3]).oper.code);

            Assert.AreEqual(typeof(ConstantToken), tokensListInRPNOrder[4].GetType());
            Assert.AreEqual(typeof(int), ((ConstantToken)tokensListInRPNOrder[4]).type);
            Assert.AreEqual(2, (int)((ConstantToken)tokensListInRPNOrder[4]).val);

            Assert.AreEqual(typeof(OperatorToken), tokensListInRPNOrder[5].GetType());
            Assert.AreEqual("+", ((OperatorToken)tokensListInRPNOrder[5]).oper.code);

            Assert.AreEqual(typeof(OperatorToken), tokensListInRPNOrder[6].GetType());
            Assert.AreEqual("=", ((OperatorToken)tokensListInRPNOrder[6]).oper.code);

        }

    }
}
