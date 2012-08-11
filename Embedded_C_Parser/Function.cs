using System;
using System.Collections.Generic;

namespace Embedded_C_Parser
{
    public class variable
    {
        public Type varType;
        public string varTypeName; //if type not known in C#
        public Object varValue;
    }

    public class Function
    {
        //function arguments
        private List<variable> agruments;

        //function return
        private variable returnVariable;

        //instructions
        private InstructionsBlock instructuionBlock;

        private IDictionary<string, variable> localVariables;
        private IDictionary<string, variable> globalVariables;

        private Function() { }

        public Function(List<string> inputCode)
        {
            throw new NotImplementedException();
        }
    }
}