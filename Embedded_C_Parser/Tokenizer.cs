using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Globalization;

namespace Embedded_C_Parser
{
    public class Token
    {

    }

    public class VariableToken : Token
    {
        public string name;
        public VariableToken(string variableName)
        {
            name = variableName;
        }
    }

    public class OperatorToken : Token
    {
        public Oper oper;

        public OperatorToken(string operatorCode)
        {
            oper = OperatorFactory.createOperator(operatorCode);
        }
    }

    public class FunctionToken : Token
    {
        public string name;
        public FunctionToken(string functionName)
        {
            name = functionName;
        }
    }

    public class ConstantToken : Token
    {
        public dynamic val;
        public Type type
        {
            get
            {
                return val.GetType();
            }
        }


        public ConstantToken(string ConstantTokenCode)
        {
            if(ConstantTokenCode[0] == '\'') //char token
            {
                    if (ConstantTokenCode.Length != 3)
                        throw new ApplicationException("wrong characte constant size");

                    if (ConstantTokenCode[2] != '\'')
                        throw new ApplicationException("there is no ending \' in character constant");

                    val = ConstantTokenCode[1];
            }
            else if(ConstantTokenCode[0] == '\"') //string token
            {
                if (ConstantTokenCode[ConstantTokenCode.Length-1] != '\"')
                        throw new ApplicationException("there is no ending \" in character constant");

                    val =  ConstantTokenCode.Substring(1, ConstantTokenCode.Length - 2);
            }
            else if (char.IsDigit(ConstantTokenCode[0]))
            {
                if (ConstantTokenCode.Contains('.')) //double
                {
                    val = Convert.ToDouble(ConstantTokenCode, CultureInfo.InvariantCulture.NumberFormat);
                }
                else //int
                {
                    val = Convert.ToInt32(ConstantTokenCode);
                }
            }
            else
            {
                try
                {
                    val = Convert.ToInt32(ConstantTokenCode);
                }
                catch (Exception)
                {
                    throw new ApplicationException("unrecognised constant token");
                }
            }
        }

        public ConstantToken(Object _val, Type _type)
        {
            val = _val;
        }
    }

    /// <summary>
    /// assumption: variables and functions always have 1 operator between them
    /// </summary>
    public class Tokenizer
    {
        private static char[] operatorOnlyChars = new char[]
        {
           '(', ')',
           '+', '-', '*', '/', '%', 
           '&', '|', '^',
           '>', '<',
           '=',
           ','
        };

        public static List<Token> Tokenize(string code)
        {
            List<Token> tokensList = new List<Token>();

            //dont need any white chars
            code = code.Replace(" ", String.Empty);

            bool operatorIsLastFoundToken = operatorOnlyChars.Any(x => x == code[0]); //like 1st char
            int lastNotTokenizedCharIndex = 0;
            int i;
            for (i = 0; i < code.Length; i++)
            {
                if (operatorOnlyChars.Any(x => x == code[i])) //if char is an operator
                {
                    if (operatorIsLastFoundToken == false)
                    {
                        operatorIsLastFoundToken = true;

                        if (code[i] == '(')
                        {
                            tokensList.Add(new FunctionToken(code.Substring(lastNotTokenizedCharIndex, i - lastNotTokenizedCharIndex)));
                        }
                        else
                        {
                            if (code[lastNotTokenizedCharIndex] == '\"' || code[lastNotTokenizedCharIndex] == '\'' || char.IsDigit(code[lastNotTokenizedCharIndex]))
                            {
                                tokensList.Add(new ConstantToken(code.Substring(lastNotTokenizedCharIndex, i - lastNotTokenizedCharIndex)));
                            }
                            else
                            {
                                tokensList.Add(new VariableToken(code.Substring(lastNotTokenizedCharIndex, i - lastNotTokenizedCharIndex)));
                            }
                        }

                        lastNotTokenizedCharIndex = i;
                    }
                }
                else //char is not an operator
                {
                    if (operatorIsLastFoundToken == true)
                    {
                        operatorIsLastFoundToken = false;

                        tokensList.Add(new OperatorToken(code.Substring(lastNotTokenizedCharIndex, i - lastNotTokenizedCharIndex)));

                        lastNotTokenizedCharIndex = i;
                    }
                }
            }

            //last thing can be only operator or variable (coz function would have ")" at end - so it'd be operator there anyway
            if (operatorIsLastFoundToken)
            {
                tokensList.Add(new OperatorToken(code.Substring(lastNotTokenizedCharIndex, i - lastNotTokenizedCharIndex)));
            }
            else
            {
                if (code[lastNotTokenizedCharIndex] == '\"' || code[lastNotTokenizedCharIndex] == '\'' || char.IsDigit(code[lastNotTokenizedCharIndex]))
                {
                    tokensList.Add(new ConstantToken(code.Substring(lastNotTokenizedCharIndex, i - lastNotTokenizedCharIndex)));
                }
                else
                {
                    tokensList.Add(new VariableToken(code.Substring(lastNotTokenizedCharIndex, i - lastNotTokenizedCharIndex)));
                }
            }

            return tokensList;
        }

        /// <summary>
        /// implementation of:
        /// http://pl.wikipedia.org/wiki/Odwrotna_notacja_polska#Algorytm_konwersji_z_notacji_infiksowej_do_ONP
        /// </summary>
        /// <param name="tokens"></param>
        /// <returns></returns>
        public static List<Token> RPN(List<Token> tokens)
        {
            List<Token> outputQueue = new List<Token>();
            Stack<Token> stack = new Stack<Token>();

            foreach (Token token in tokens)
            {
                /*
                 * Jeśli symbol jest liczbą dodaj go do kolejki wyjście.
                 */
                if(token.GetType() == typeof(VariableToken) || token.GetType() == typeof(ConstantToken))
                {
                    outputQueue.Add(token);
                }
                /*
                 * Jeśli symbol jest funkcją włóż go na stos.
                 */
                else if(token.GetType() == typeof(FunctionToken))
                {
                    stack.Push(token);
                }
                else if (token.GetType() == typeof(OperatorToken) && token.GetType() != typeof(Bracket)) //jeśli symboljest operatorem i nie jest nawiasem
                {
                    if (((OperatorToken)token).oper.code == ",") //Jeśli symbol jest znakiem oddzielającym argumenty funkcji (np. przecinek):
                    {
                        /* 
                         * Dopóki najwyższy element stosu nie jest lewym nawiasem, 
                         * zdejmij element ze stosu i dodaj go do kolejki wyjście. 
                         */

                        while (stack.Peek().GetType() == typeof(OperatorToken)
                            &&
                            ((OperatorToken)stack.Peek()).oper.code != "(")
                        {
                            outputQueue.Add(stack.Pop());

                            /*
                             * Jeśli lewy nawias nie został napotkany oznacza to, 
                             * że znaki oddzielające zostały postawione w 
                             * złym miejscu lub nawiasy są źle umieszczone.
                             */
                            if (stack.Count == 0)
                                throw new ApplicationException("znaki oddzielające zostały postawione w złym miejscu lub nawiasy są źle umieszczone");
                        }
                    }
                    /*
                     * Jeśli symbol jest operatorem, o1, wtedy:
                     */
                    else //it is non-"," operator
                    {
                        /*
                         * 1) dopóki na górze stosu znajduje się operator, o2 taki, że:
                         *      o1 jest lewostronnie łączny i jego kolejność wykonywania jest mniejsza lub równa kolejności wyk. o2,
                         *      lub
                         *      o1 jest prawostronnie łączny i jego kolejność wykonywania jest mniejsza od o2
                         */
                        while (stack.Count > 0 && RPMHelperMethod(token, stack))
                        {
                            /*
                             * zdejmij o2 ze stosu i dołóż go do kolejki wyjściowej i wykonaj jeszcze raz 1)
                             */
                             outputQueue.Add(stack.Pop());
                        }
                        /*
                         * 2) włóż o1 na stos operatorów.
                         */
                        stack.Push(token);
                    }
                }
                /*
                 * Jeżeli symbol jest lewym nawiasem to włóż go na stos.
                 */
                else if(tokenIsOppeningBracket(token))
                {
                    stack.Push(token);
                }
                /*
                 * Jeżeli symbol jest prawym nawiasem to zdejmuj operatory ze stosu 
                 * i dokładaj je do kolejki wyjście, dopóki symbol na górze stosu 
                 * nie jest lewym nawiasem, kiedy dojdziesz do tego miejsca zdejmij 
                 * lewy nawias ze stosu bez dokładania go do kolejki wyjście. 
                 * Teraz, jeśli najwyższy element na stosie jest funkcją, 
                 * także dołóż go do kolejki wyjście. Jeśli stos zostanie opróżniony 
                 * i nie napotkasz lewego nawiasu, oznacza to, że nawiasy zostały źle umieszczone.
                 */
                else if(tokenIsClosingBracket(token))
                {
                    if(stack.Count == 0)
                        throw new ApplicationException("nawiasy zostały źle umieszczone");

                    while(!tokenIsOppeningBracket(token))
                    {
                        outputQueue.Add(stack.Pop());

                        if(stack.Count == 0)
                            throw new ApplicationException("nawiasy zostały źle umieszczone");

                    }
                }
            }
            while (stack.Count != 0)
            {
                outputQueue.Add(stack.Pop());
            }
            return outputQueue;
        }

        private static bool tokenIsClosingBracket(Token token)
        {
            return token.GetType() == typeof(OperatorToken) 
                && ((OperatorToken)token).oper.GetType() == typeof(Bracket) 
                && ((Bracket)((OperatorToken)token).oper).type == Bracket.BracketType.closingBracket;
        }

        private static bool tokenIsOppeningBracket(Token token)
        {
            return token.GetType() == typeof(OperatorToken) 
                && ((OperatorToken)token).oper.GetType() == typeof(Bracket) 
                && ((Bracket)((OperatorToken)token).oper).type == Bracket.BracketType.oppeningBracket;
        }

    

        /// <summary>
        /// na górze stosu znajduje się operator, o2 taki, że: 
        /// o1 jest lewostronnie łączny i jego kolejność wykonywania jest mniejsza lub równa kolejności wyk. o2,
        /// lub
        /// o1 jest prawostronnie łączny i jego kolejność wykonywania jest mniejsza od o2,
        /// </summary>
        /// <param name="stack"></param>
        /// <param name="outputQueue"></param>
        /// <returns></returns>
        private static bool RPMHelperMethod(Token currToken, Stack<Token> stack)
        {
            return  (stack.Peek().GetType() == typeof(OperatorToken)
                        &&// o1 jest lewostronnie łączny i jego kolejność wykonywania jest mniejsza lub równa kolejności wyk. o2,
                            ((((OperatorToken)currToken).oper.GetType() == typeof(LeftSideOper) //token is left-side operator
                            &&
                            ((OperatorToken)currToken).oper.priority <= ((OperatorToken)stack.Peek()).oper.priority
                            || //o1 jest prawostronnie łączny i jego kolejność wykonywania jest mniejsza od o2,
                            ((OperatorToken)currToken).oper.GetType() == typeof(RightSideOper)
                            &&
                            ((OperatorToken)currToken).oper.priority < ((OperatorToken)stack.Peek()).oper.priority))
                        );
        }

        public static List<Token> ResolveTokens(List<Token> tokens, IDictionary<string, variable> localVariables, IDictionary<string, variable> inheritedVariables, IDictionary<string, Function> knownFunctions)
        {
            List<Token> listWithResolvedTokens = new List<Token>();
            foreach(Token token in tokens)
            {
                if (token.GetType() == typeof(VariableToken))
                {
                   variable var;
                   if (!localVariables.TryGetValue(((VariableToken)token).name, out var))
                       if (!localVariables.TryGetValue(((VariableToken)token).name, out var))
                           throw new ApplicationException(String.Format("Unresolved variable: {0}", ((VariableToken)token).name));

                   listWithResolvedTokens.Add(var);
                }
                else if (token.GetType() == typeof(ConstantToken))
                {
                    listWithResolvedTokens.Add(new variable(((ConstantToken)token).val));
                }
                else if (token.GetType() == typeof(FunctionToken))
                {
                   Function foo;
                   if (!knownFunctions.TryGetValue(((FunctionToken)token).name, out foo))
                       throw new ApplicationException(String.Format("Unresolved variable: {0}", ((FunctionToken)token).name));

                   listWithResolvedTokens.Add(foo);
                }
                else if (token.GetType() == typeof(OperatorToken))
                {
                    listWithResolvedTokens.Add(((OperatorToken)token).oper);
                }
                else
                {
                    throw new ApplicationException("Unresolved token");
                }
            }

            return listWithResolvedTokens;
        }
    }
}

