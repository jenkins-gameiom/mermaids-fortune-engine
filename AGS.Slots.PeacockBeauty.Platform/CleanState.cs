
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGS.Slots.MermaidsFortune.Platform
{
    public class CleanState
    {
        public static dynamic Clean(dynamic obj)
        {
            try
            {
                    obj.config = null;
                    obj.privateState = Json.ObjectToDynamic(new SpinPrivateState());
            }
            catch (Exception ex)
            {
                
            }

            return obj;
        }
    }
}
