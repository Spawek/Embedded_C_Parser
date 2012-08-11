using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Embedded_C_Parser
{
    public interface IInstruction
    {
        private IInstruction nextInstruction;

        public abstract variable Execute();

        IInstruction(string instructionCode)
        {
            throw new NotImplementedException();
        }
    }

    /* { 
     *  instructions
     * }
     * 
     * only instruction blocks can storage variables and functions for statements
     */
    public class InstructionsBlock : IInstruction
    {
        public IDictionary<string, variable> inheritedVariables;
        public IDictionary<string, variable> localVariables = new Dictionary<string, variable>(); //its always empty at start
        private IInstruction firstInnerInstruction;
    }

    public class ConditionalInstruction : IInstruction
    {
        private ArythmeticStatement Condition;

        private IInstruction nextInstructionIfCondFailed;
        private IInstruction nextInstructiobIfCondFulfilled;
    }

    public class VariableDeclarationInstruction : IInstruction
    {

    }
}
