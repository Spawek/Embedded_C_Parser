using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Embedded_C_Parser.Instructions
{
    /* { 
     *  instructions
     * }
     * 
     * only instruction blocks can storage variables and functions for statements
     */
    public class InstructionsBlock : IInstruction
    {
        public IInstruction NextInstruction { get; private set; }

        public IDictionary<string, variable> inheritedVariables;
        public IDictionary<string, variable> localVariables = new Dictionary<string, variable>(); //its always empty at start
        private List<IInstruction> instructionsList;
        public IDictionary<string, Function> knownFunctions;
        private List<string> list;

        public InstructionsBlock(List<string> list)
        {

        }

        /// <summary>
        /// tests needed only
        /// DO NOT USE!
        /// </summary>
        public InstructionsBlock()
        {

        }

        public variable Execute()
        {
            throw new NotImplementedException();
        }
    }

}
