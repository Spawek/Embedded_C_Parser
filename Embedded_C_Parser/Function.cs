using System;
using System.Collections.Generic;

namespace Embedded_C_Parser
{
    public class variable : Token
    {
        public Type type;
        public dynamic value; //was Object

        public variable(Object _value, Type _type)
        {
            value = _value;
            type = _type;
        }
    }

    public class Function : Token
    {
        //function arguments
        public List<variable> agruments;
        public List<Type> neededArgumentsType;
        public int agrumgensNo;

        //function return type
        private Type returnType;

        //instructions
        private InstructionsBlock instructuionBlock;

        private IDictionary<string, variable> localVariables;
        private IDictionary<string, variable> globalVariables;

        private Function() { }

        public Function(List<string> inputCode)
        {
            throw new NotImplementedException();
        }

        public variable Execute(variable[] arguments)
        {
            throw new NotImplementedException();
        }
    }
}