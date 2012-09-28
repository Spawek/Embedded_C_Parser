using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Embedded_C_Parser.Instructions
{
    public class VariableDeclarationInstruction : IInstruction
    {
        public IInstruction NextInstruction { get; private set; }

        private string code;
        private InstructionsBlock parent;

        public VariableDeclarationInstruction(string instructionCode, InstructionsBlock instructionParent) 
        {
            code = instructionCode;
            parent = instructionParent;
        }

        public variable Execute()
        {
            //get type and name from declaration
            Type declarationVarType = GetDeclarationVarType(code);
            string declarationVarName = GetDeclarationVarName(code);
            
            //create variable in parent variables map
            parent.localVariables.Add(declarationVarName, new variable("undefined variable"));

            //execute statement
            var instructionArythemics = new ArythmeticStatement(CutVarType(code), parent);
            instructionArythemics.Execute();

            //type control
            if ((parent.localVariables[declarationVarName]).type != declarationVarType)
            {
                //it should throw an exception if something went bad
                parent.localVariables[declarationVarName].value = Convert.ChangeType(parent.localVariables[declarationVarName].value, declarationVarType); 
            }

            return parent.localVariables[declarationVarName];
        }

        private string GetDeclarationVarName(string code)
        {
            string name = code.Split(' ')[1];

            //if last char is '=' cut it
            if (name[name.Length - 1] == '=')
                name = code.Substring(0, name.Length - 1);

            return name;
        }

        private string CutVarType(string code)
        {
            return code.Substring(code.IndexOf(' ') + 1);
        }

        private Type GetDeclarationVarType(string code)
        {

            switch (code.Split(' ')[0])
            {
                case "int":
                    return typeof(int);

                case "double":
                    return typeof(double);

                case "char":
                    return typeof(char);

                case "string":
                    return typeof(string);

                default:
                    throw new ApplicationException("unknown var declaration type");
            }
        }
    }
}
