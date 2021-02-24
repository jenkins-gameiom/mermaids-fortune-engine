using Microsoft.CSharp.RuntimeBinder;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGS.Slots.MermaidsFortune.Common
{
   public class Json
    {
        public static dynamic Clone(dynamic val)
        {
            return JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(val));
        }

        public static dynamic ObjectToDynamic(object val)
        {
            
            return JsonConvert.DeserializeObject<dynamic>(JsonConvert.SerializeObject(val, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore }),new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }

        public static dynamic Decode(string val)
        {
            return JObject.Parse(val);
        }

        public static dynamic DecodeNS(string obj)
        {
            return Newtonsoft.Json.JsonConvert.DeserializeObject(obj);
        }

        public static string Encode(dynamic obj)
        {
            return Newtonsoft.Json.JsonConvert.SerializeObject(obj);
        }

        public static int GetValueOrDefault(dynamic obj,string name, int defaultVal = 0)
        {
            try
            {
                var value = obj[name].Value;
                return Convert.ToInt32(value);
            }
            catch (RuntimeBinderException)
            {
                return defaultVal;
            }
        }

        public static string GetValueOrDefault(dynamic obj, string name, string defaultVal = "")
        {
            try
            {
                var value = obj[name].Value;
                return (string)(value);
            }
            catch (RuntimeBinderException)
            {
                return defaultVal;
            }
        }

        public static bool GetValueOrDefault(dynamic obj, string name, bool defaultVal = false)
        {
            try
            {
                var value = obj[name].Value;
                return Convert.ToBoolean(value);
            }
            catch (RuntimeBinderException)
            {
                return defaultVal;
            }
        }

        public static T ConvertDynamic<T>(dynamic data)
        {
            try
            {
                return data.ToObject<T>();
            }
            catch(Exception ex)
            {
                //Logger.Error("error converting dynamic to object Type " + typeof(T).Name +" " + ex.Message   , ex);
                return default(T);
            }
        }

    public static bool HasProperty(dynamic obj, string name)
        {

                try
                {
               
                    var value = obj[name].Value;
                    return true;
                }
                catch (RuntimeBinderException ex)
                {

                    return false;
                }

        }
    }
}
