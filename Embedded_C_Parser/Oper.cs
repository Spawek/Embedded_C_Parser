using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Embedded_C_Parser
{
    public class Oper //operator
    {
        private string code;
        private variable LSVar;
        private variable RSVar;

        public abstract Object Execute();
    }

    public class Bracket : Oper
    {
        public enum BracketType
        {
            beginning,
            closing
        }
        BracketType type;
    }

    public class LeftSideOper : Oper
    {

    }

    public class RightSideOper : Oper
    {

    }

    public static class OperatorFactory
    {
        public static Oper createOperator(string code)
        {
            //switch (code)
            //{
            //    case "+":
            //        ;
            //}
            throw new NotImplementedException();
        }

    }
}
