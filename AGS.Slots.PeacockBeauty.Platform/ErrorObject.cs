using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGS.Slots.MermaidsFortune.Common
{
    public class Error
    {
        public int code { get; set; }
        public string message { get; set; }
        public string stackTrace { get; set; }
    }
    public class ErrorObject
    {
        public Error error { get; set; }
    }
}
