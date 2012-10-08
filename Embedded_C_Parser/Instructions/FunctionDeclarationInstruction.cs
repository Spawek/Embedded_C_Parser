using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Embedded_C_Parser.Instructions
{
    class FunctionDeclarationInstruction : IInstruction
    {
        public IInstruction NextInstruction { get; private set; }

        private string code;
        private InstructionsBlock functionScope;
        private InstructionsBlock parentScope;
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="instructionCode"></param>
        /// <param name="fooScope">only foo with function scope should be passed here -> in other case expection should be thrown</param>
        /// <param name="parentScope">this should be next scope to execute or any scope before it (they are passing foos anyway)</param>
        public FunctionDeclarationInstruction(string instructionCode, InstructionsBlock fooScope, Program parent) 
        {
            code = instructionCode;
            functionScope = fooScope;
            parentScope = parent;
        }

        public variable Execute()
        {
            throw new NotImplementedException();
        }

    }
}
