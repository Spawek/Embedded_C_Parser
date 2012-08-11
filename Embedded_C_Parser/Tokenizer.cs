using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Embedded_C_Parser
{
    private class Token
    {
        
    }

    private class variableToken : Token
    {
        public string name;
        public variableToken(string variableName)
        {
            name = variableName;
        }
    }

    private class operatorToken : Token
    {
        public Oper oper;

        public operatorToken(string operatorCode)
        {
            oper = OperatorFactory.createOperator(operatorCode);
        }
    }

    private class functionToken : Token
    {
        public string name;
        public functionToken(string functionName)
        {
            name = functionName;
        }
    }

    /// <summary>
    /// assumption: variables and functions always have 1 operator between them
    /// </summary>
    public static class Tokenizer
    {
        static char[] operatorOnlyChars = new char[]
        {
           '(', ')',
           '+', '-', '*', '/', '%', 
           '&', '|', '^',
           '>', '<',
           '=',
           ','
        };

        static List<Token> Tokenize(string code)
        {
            List<Token> tokensList = new List<Token>();

            //dont need any white chars
            code = code.Trim();

            bool operatorIsLastFoundToken = operatorOnlyChars.Any(x => x == code[0]); //like 1st char
            int lastNotTokenizedCharIndex = 0;
            int i;
            for (i = 0; i < code.Length; i++)
            {
                if (operatorOnlyChars.Any(x => x == code[i]))
                {
                    if (operatorIsLastFoundToken == false)
                    {
                        operatorIsLastFoundToken = true;

                        if (code[i] != '(')
                        {
                            tokensList.Add(new variableToken(code.Substring(lastNotTokenizedCharIndex, i - lastNotTokenizedCharIndex)));
                        }
                        else
                        {
                            tokensList.Add(new functionToken(code.Substring(lastNotTokenizedCharIndex, i - lastNotTokenizedCharIndex)));
                        }

                        lastNotTokenizedCharIndex = i;
                    }
                }
                else
                {
                    if (operatorIsLastFoundToken == true)
                    {
                        operatorIsLastFoundToken = false;

                        tokensList.Add(new operatorToken(code.Substring(lastNotTokenizedCharIndex, i - lastNotTokenizedCharIndex)));

                        lastNotTokenizedCharIndex = i;
                    }
                }
            }

            //last thing can be only operator or varuable (coz function would have ")" at end - so it'd be operator there anyway
            if (operatorIsLastFoundToken)
            {
                tokensList.Add(new operatorToken(code.Substring(lastNotTokenizedCharIndex, i - lastNotTokenizedCharIndex)));
            }
            else
            {
                tokensList.Add(new variableToken(code.Substring(lastNotTokenizedCharIndex, i - lastNotTokenizedCharIndex)));
            }

            return tokensList;
        }
    }
}
