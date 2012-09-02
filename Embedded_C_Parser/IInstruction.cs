using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Embedded_C_Parser;

namespace Embedded_C_Parser
{
    public abstract class IInstruction
    {
        private IInstruction nextInstruction;

        public abstract variable Execute();
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
        public IDictionary<string, Function> knownFunctions;

        public override variable Execute()
        {
            throw new NotImplementedException();
        }
    }

    public class ConditionalInstruction : IInstruction
    {
        private ArythmeticStatement Condition;

        private IInstruction nextInstructionIfCondFailed;
        private IInstruction nextInstructiobIfCondFulfilled;

        public override variable Execute()
        {
            throw new NotImplementedException();
        }

    }

    public class VariableDeclarationInstruction : IInstruction
    {
        public override variable Execute()
        {
            throw new NotImplementedException();
        }
    }
}
