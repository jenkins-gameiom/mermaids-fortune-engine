using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Security.Cryptography;
using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;
using Newtonsoft.Json.Linq;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Interfaces;

namespace AGS.Slots.MermaidsFortune.Logic.Engine.Providers
{
    public class IgamingRandomize : IRandom
    {


        public IgamingRandomize(Configs config)
        {
            _url = config.WalletUrl;
        }


        private string _url;
        public List<RandomNumber> GetRandomNumbers(List<RandomNumber> rnds)
        {
            string req = "{\"ranges\": [";
            for (int i = 0; i < rnds.Count; i++)
            {
                req += string.Format("{{\"minRange\":{0}, \"maxRange\":{1}, \"quantity\":{2} }},", rnds[i].Min, rnds[i].Max, rnds[i].Quantity);
            }
            req = req.TrimEnd(',') + "]}";
            try
            {
                var res = SendPostRequest(req, _url);
                dynamic en = JObject.Parse(res);
                for (int i = 0; i < rnds.Count; i++)
                {
                    var rndVals = ((IEnumerable<dynamic>)en.rangeValues[i].values).Select(x => (int)x.value).ToList();
                    rnds[i].Values = rndVals;
                }

            }
            catch (Exception ex)
            {
                throw new Exception("Error in getting random numbers from Gameiom " + ex.Message, ex.InnerException);
            }
            return rnds;
        }

        public double NextPercentage()
        {
            try
            {
                var rndNumber = GetRandomNumbers(new List<RandomNumber>() { new RandomNumber() { Max = Int32.MaxValue, Min = 0, Quantity = 1 } });
                return rndNumber[0].Values[0] / (double)int.MaxValue * 100;
            }
            catch (Exception ex)
            {
                throw new Exception("Error in getting random numbers from Gameiom " + ex.Message, ex.InnerException);
            }
        }


        public int Next(int minValue, int maxValue)
        {
            try
            {
                var rndNumber = GetRandomNumbers(new List<RandomNumber>() { new RandomNumber() { Max = maxValue, Min = minValue, Quantity = 1 } });
                return rndNumber[0].Values[0];
            }
            catch (Exception ex)
            {
                throw new Exception("Error in getting random numbers from Gameiom " + ex.Message, ex.InnerException);
            }
        }
        public static string SendPostRequest(string postData, string url)
        {
            WebRequest request = WebRequest.Create(url);
            // Set the Method property of the request to POST.  
            request.Method = "POST";
            // Create POST data and convert it to a byte array.  
            byte[] byteArray = Encoding.UTF8.GetBytes(postData);
            // Set the ContentType property of the WebRequest.  
            request.ContentType = "application/json";
            // Set the ContentLength property of the WebRequest.  
            request.ContentLength = byteArray.Length;
            // Get the request stream.  
            Stream dataStream = request.GetRequestStream();
            // Write the data to the request stream.  
            dataStream.Write(byteArray, 0, byteArray.Length);
            // Close the Stream object.  
            dataStream.Close();
            // Get the response.  
            WebResponse response = request.GetResponse();
            // Display the status.  
            Console.WriteLine(((HttpWebResponse)response).StatusDescription);
            // Get the stream containing content returned by the server.  
            dataStream = response.GetResponseStream();
            // Open the stream using a StreamReader for easy access.  
            StreamReader reader = new StreamReader(dataStream);
            // Read the content.  
            string responseFromServer = reader.ReadToEnd();
            // Display the content.  
            Console.WriteLine(responseFromServer);
            // Clean up the streams.  
            reader.Close();
            dataStream.Close();
            response.Close();
            return responseFromServer;
        }
    }

    //Randoms for simulators
    //public class RegRandomCrypto : IRandom
    //{
    //    RNGCryptoServiceProvider provider;
    //    public RegRandomCrypto(int seed)
    //    {
    //        provider = new RNGCryptoServiceProvider();
    //    }
    //
    //    public RegRandomCrypto()
    //    {
    //        provider = new RNGCryptoServiceProvider();
    //    }
    //    public double NextPercentage()
    //    {
    //        var byteArray = new byte[4];
    //        provider.GetBytes(byteArray);
    //
    //        var randomInteger = BitConverter.ToUInt32(byteArray, 0);
    //        return randomInteger / (double)uint.MaxValue * 100;
    //    }
    //
    //    public List<RandomNumber> GetRandomNumbers(List<RandomNumber> rnds)
    //    {
    //        try
    //        {
    //
    //            for (int i = 0; i < rnds.Count; i++)
    //            {
    //                List<int> rndVals = new List<int>();
    //                for (int j = 0; j < rnds[i].Quantity; j++)
    //                {
    //                    rndVals.Add(Next(0, rnds[i].Max));
    //                }
    //                rnds[i].Values = rndVals;
    //            }
    //        }
    //        catch (Exception ex)
    //        {
    //
    //        }
    //        return rnds;
    //    }
    //
    //
    //
    //    public int Next(int minValue, int maxValue)
    //    {
    //        if (maxValue > 10000)
    //        {
    //            var byteArray = new byte[8];
    //            provider.GetBytes(byteArray);
    //
    //            var randomInteger = BitConverter.ToUInt64(byteArray, 0);
    //            ulong answer = (ulong)minValue + randomInteger % ((ulong)maxValue - (ulong)minValue);
    //            return Convert.ToInt32(answer);
    //        }
    //        else
    //        {
    //            var byteArray = new byte[4];
    //            provider.GetBytes(byteArray);
    //            var randomInteger = BitConverter.ToUInt32(byteArray, 0);
    //            long answer = minValue + randomInteger % (maxValue - minValue);
    //            return Convert.ToInt32(answer);
    //        }
    //    }
    //
    //
    //
    //}

    public class RandomGeneratorCrypro : IRandom
    {
        readonly RNGCryptoServiceProvider csp;


        public List<RandomNumber> GetRandomNumbers(List<RandomNumber> rnds)
        {
            try
            {

                for (int i = 0; i < rnds.Count; i++)
                {
                    List<int> rndVals = new List<int>();
                    for (int j = 0; j < rnds[i].Quantity; j++)
                    {
                        rndVals.Add(Next(0, rnds[i].Max));
                    }
                    rnds[i].Values = rndVals;
                }
            }
            catch (Exception ex)
            {

            }
            return rnds;
        }

        public RandomGeneratorCrypro(int seed)
        {
            csp = new RNGCryptoServiceProvider();

        }

        public RandomGeneratorCrypro()
        {
            csp = new RNGCryptoServiceProvider();

        }



        public double NextPercentage()
        {
            var byteArray = new byte[4];
            csp.GetBytes(byteArray);

            var randomInteger = BitConverter.ToUInt32(byteArray, 0);
            return randomInteger / (double)uint.MaxValue * 100;
        }

        public int Next(int minValue, int maxValue)
        {
            if (minValue > maxValue)
                throw new ArgumentOutOfRangeException("minValue");

            if (minValue == maxValue)
                return minValue;

            long diff = maxValue - minValue;

            while (true)
            {
                uint rand = GetRandomUInt32();

                long max = 1 + (long)uint.MaxValue;
                long remainder = max % diff;

                if (rand < max - remainder)
                    return (int)(minValue + (rand % diff));
            }
        }


        private uint GetRandomUInt32()
        {
            var randomBytes = GenerateRandomBytes(sizeof(uint));
            return BitConverter.ToUInt32(randomBytes, 0);
        }


        private ulong GetRandomULong()
        {
            var randomBytes = GenerateRandomBytes(sizeof(ulong));
            return BitConverter.ToUInt64(randomBytes, 0);
        }

        private byte[] GenerateRandomBytes(int bytesNumber)
        {
            byte[] buffer = new byte[bytesNumber];
            csp.GetBytes(buffer);
            return buffer;
        }
    }


}
