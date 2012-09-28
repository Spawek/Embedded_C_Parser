using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Embedded_C_Parser.Instructions
{
    public class ConditionalInstruction : IInstruction
    {
        private ArythmeticStatement Condition;

        private IInstruction nextInstructionIfCondFailed;
        private IInstruction nextInstructiobIfCondFulfilled;

        private bool? conditionFulfilled = null;

        public IInstruction NextInstruction 
        {
            get
            {
                //not so easy in here   
                if (conditionFulfilled == null)
                {
                    throw new ApplicationException("Condition was not checked before getting next instruction");
                }
                else
                {
                    if (conditionFulfilled == true)
                    {
                        return nextInstructiobIfCondFulfilled;
                    }
                    else if (conditionFulfilled == false) //if needed coz it can be NULL and compiler does not like it :D
                    {
                        return nextInstructionIfCondFailed;
                    }
                }

                throw new Exception("Something bad has just happened"); //compiled does not like logic in this getter
            }
        }


        public variable Execute()
        {
            throw new NotImplementedException();
        }

    }
}
