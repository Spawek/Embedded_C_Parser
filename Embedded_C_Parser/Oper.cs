using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Embedded_C_Parser
{
    public abstract class Oper : Token //operator
    {
        public string code;
        public int priority;
        public int noOfArguments;

        public abstract variable Execute(variable[] arguments);
    }

    public class Bracket : Oper
    {
        public enum BracketType
        {
            oppeningBracket,
            closingBracket
        }
        public BracketType type;

        public Bracket(BracketType _type)
        {
            type = _type;
        }

        override public variable Execute(variable[] arguments)
        {
            throw new ApplicationException("you cannot execute bracket!");
        }
    }

    public class LeftSideOper : Oper
    {
        override public variable Execute(variable[] arguments)
        {
            switch (code)
            {
                case "+":
                    
                    return new variable(
                        arguments[0].value + arguments[1].value, 
                        arguments[0].type
                    );

                case "*":
                    return new variable((int)arguments[0].value * (int)arguments[1].value, typeof(int));

                default:
                    throw new ApplicationException("unresolved operator");
            }
        }

        public LeftSideOper(string _code, int _priority, int _noOfArguments)
        {
            code = _code;
            priority = _priority;
            noOfArguments = _noOfArguments;
        }
    }

    public class RightSideOper : Oper
    {
        override public variable Execute(variable[] arguments)
        {
            throw new NotImplementedException();
        }

        public RightSideOper(string _code, int _priority, int _noOfArguments)
        {
            code = _code;
            priority = _priority;
            noOfArguments = _noOfArguments;
        }
    }

    /// <summary>
    /// priority, right/left side from:
    /// http://www.csee.umbc.edu/portal/help/C++/summary.shtml
    /// </summary>
    public static class OperatorFactory
    {
        public static Oper createOperator(string code)
        {
            switch (code)
            {
                case "+":
                    return new LeftSideOper(code, 11, 2);
                    break;

                case "*":
                    return new LeftSideOper(code, 12, 2);
                    break;

                case "=":
                    return new RightSideOper(code, 1, 2); 
                    break;

                default:
                    throw new NotImplementedException();
               
            }

            
        }

    }
}
