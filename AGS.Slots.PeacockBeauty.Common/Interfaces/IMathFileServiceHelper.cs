using AGS.Slots.MermaidsFortune.Common.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGS.Slots.MermaidsFortune.Common.Interfaces
{
    public interface IMathFileService
    {
        IMathFile GetMathFile(MathFileType type);


    }
}
