using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGS.Slots.MermaidsFortune.Logic.Exceptions
{
    public class MismatchWithConfigException : Exception
    {
        private readonly string _configNeeded;
        private readonly string _actualValue;

        public MismatchWithConfigException(string configNeeded,string actualValue)
        {
            _configNeeded = configNeeded;
            _actualValue = actualValue;
        }

        public override string Message
        {
            get { return String.Format("Needed configuration: {0} Actual value is: {1}",_configNeeded,_actualValue); } 
        }
    }

    
    
}
