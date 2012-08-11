using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Embedded_C_Parser
{
    public class ArythmeticStatement : IInstruction
    {
        private string code;
        private InstructionsBlock parent;

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

        ArythmeticStatement(string instructionCode, InstructionsBlock instructionParent)
        {
            parent = instructionParent;
            code = instructionCode;
        }


        // white-space characters MUST be removed,
        // ";" from end MUST be removed
        public Object Execute()
        {
            //changing:
            //local variables to $lVarname
            //inherited variables to $iVarname
            //functions to $fFuncname
            string[] tokens = code.Split(operatorsList, StringSplitOptions.None);
            for(int i = 0; i < tokens.Length; i++)
            {
                if(!(parent.localVariables.Any(x => (x.Key == tokens[i]))))
                    tokens[i].Replace(
            }

            tokens.fo
    
            
        }
    }
}
