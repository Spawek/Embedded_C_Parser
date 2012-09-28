using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Embedded_C_Parser.Instructions;

namespace Embedded_C_Parser
{
    public class ArythmeticStatement : IInstruction
    {
        private string code;
        private InstructionsBlock parent;

        public IInstruction NextInstruction { get; private set; }

        private static string[] operatorsList = new string[]
        {
             "(",    ")",
             "!",    "~",
             "*",    "/",    "%",                                                
             "+",    "-",                                                      
             "<<",   ">>",                                                     
             "<",    "<=",   ">",    ">=",                                         
             "==",   "!=",                                                     
             "&",                                                            
             "^",                                                            
             "|",                                                            
             "&&",                                                           
             "||",                                                                                                             
             "=",    "+=",   "-=",   "*=",   "/=",  "%=",  ">>=",  "<<=",  "&=",  "^=",  "|="
        };

        public ArythmeticStatement(string instructionCode, InstructionsBlock instructionParent)
        {
            parent = instructionParent;
            code = instructionCode;
        }

        // white-space characters MUST be removed,
        // ";" from end MUST be removed
        public variable Execute()
        {
            List<Token> tokens = Tokenizer.Tokenize(code);
            List<Token> tokensInRPN = Tokenizer.RPN(tokens);
            List<Token> resolvedTokensRPN = Tokenizer.ResolveTokens(tokensInRPN, parent.localVariables, parent.inheritedVariables, parent.knownFunctions); //get variables, operators and functions from tokens

            return CalulateRPNStatement(resolvedTokensRPN, parent.localVariables, parent.inheritedVariables);      
        }

        //http://pl.wikipedia.org/wiki/Odwrotna_notacja_polska#Algorytm_obliczenia_warto.C5.9Bci_wyra.C5.BCenia_ONP
        private variable CalulateRPNStatement(List<Token> tokensInRPN, IDictionary<string, variable> localVariables, IDictionary<string, variable> inheritedVariables)
        {
            Stack<Token> stack = new Stack<Token>();

            foreach (Token token in tokensInRPN)
            {
                if (token.GetType() == typeof(variable))
                {
                    stack.Push(token);
                }
                else if (token.GetType() == typeof(LeftSideOper) || token.GetType() == typeof(RightSideOper))
                {
                    if (stack.Count < ((Oper)token).noOfArguments)
                        throw new ApplicationException("no of needed arguments is bigger than no of tokens on stack");

                    variable[] parameters = new variable[((Oper)token).noOfArguments];
                    for (int i = 0; i < ((Oper)token).noOfArguments; i++)
                    {
                        if(stack.Peek().GetType() != typeof(variable))
                            throw new ApplicationException("wrong no of arguments in stack");

                        parameters[i] = (variable)stack.Pop();
                    }

                    stack.Push(((Oper)token).Execute(parameters));
                }
                else if (token.GetType() == typeof(Function))
                {
                    if (stack.Count < ((Function)token).agrumgensNo)
                        throw new ApplicationException("no of needed arguments is bigger than no of tokens on stack");

                    variable[] arguments = new variable[((Function)token).agrumgensNo];
                    for (int i = 0; i < ((Function)token).agrumgensNo; i++)
                    {
                        if (stack.Peek().GetType() != typeof(Function))
                            throw new ApplicationException("wrong no of arguments in stack");

                        arguments[i] = (variable)stack.Pop();
                    }

                    stack.Push(((Function)token).Execute(arguments));
                }
                else
                {
                    throw new ApplicationException("unhandled token");
                }
            }

            if (stack.Count != 1)
                throw new ApplicationException("there should be only 1 value on stack");

            if (stack.Peek().GetType() != typeof(variable))
                throw new ApplicationException("last token on stack should be a variable");

            return (variable)stack.Pop();
        }
    }
}
