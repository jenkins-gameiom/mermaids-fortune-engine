using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace AGS.Slots.MermaidsFortune.Common
{
    public  class Configs
    {
       

        private static int animationLevels = 0;
        public static int AnimationLevels { get
            {
                if (animationLevels == 0)
                {
          //          animationLevels = Convert.ToInt32( ConfigurationManager.AppSettings["AnimationLevels"]);
                }
                return animationLevels;
            }
        }

        private static int defaultBet = 0;
        public static int DefaultBet
        {
            get
            {
                if (defaultBet == 0)
                {
                   // defaultBet = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultBet"]);
                }
                return defaultBet;
            }
        }

        private static int defaultDenom = 0;
        public static int DefaultDenom
        {
            get
            {
                if (defaultDenom == 0)
                {
                //    defaultDenom = Convert.ToInt32(ConfigurationManager.AppSettings["DefaultDenom"]);
                }
                return defaultDenom;
            }
        }

        private static Dictionary<string, int> stopDurationByJur = null;
        public static Dictionary<string, int> StopDurationByJur
        {
            get
            {
                if (stopDurationByJur == null)
                {
                    stopDurationByJur = new Dictionary<string, int>();
                //    string n = ConfigurationManager.AppSettings["StopDuration"];
                    //foreach (var jur in n.Split('|'))
                    //{
                    //    var a = jur.Split(',');
                    //    stopDurationByJur.Add(a[0], Convert.ToInt32(a[1]));
                    //}
                }
                return stopDurationByJur;
            }
        }

        private static HashSet<string> noAutoPlay = null;
        public static HashSet<string> NoAutoPlay
        {
            get
            {
                if (noAutoPlay == null)
                {
                    noAutoPlay = new HashSet<string>();
                    //string n = ConfigurationManager.AppSettings["NoAutoSpin"];
                    //foreach (var jur in n.Split(','))
                    //{
                    //    noAutoPlay.Add(jur.Trim());
                    //}
                }
                return noAutoPlay;
            }

        }

        public  string WalletUrl
        {
            get;set;
        }

        public  bool IsTest
        {
            get;set;
        }

        public string RTP96
        {
            get; set;
        }

        public string RTP94
        {
            get; set;
        }

    }
}
