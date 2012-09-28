using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Embedded_C_Parser;

namespace Embedded_C_Parser.Instructions
{
    public interface IInstruction
    {
        variable Execute();
        IInstruction NextInstruction
        {
            get;
        }
    }
}
