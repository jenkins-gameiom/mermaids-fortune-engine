using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace AGS.Slots.MermaidsFortune.Platform.Tests
{
    class JsonsClass
    {
        public static string GetReq(string req)
        {
            var json = File.ReadAllText($"Requests/{req}.json");

            return json;
        }
    }
}
