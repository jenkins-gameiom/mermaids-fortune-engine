using AGS.Slots.PeacockBeauty.Logic.Engine.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AGS.Slots.PeacockBeauty.Commons
{

    public class PlatformRequest
    {
        public SpinPublicStateRequest PublicState { get; set; }
        public SpinPrivateState PrivateState { get; set; }

        public Platform Platform {get;set;}
        public Config Config;
    }
    public enum SpinType
    {
        spin,
        freeSpin,
        pick,
        freeSpinsAmount
    }

    public class Transaction
    {
        public long[] debits { get; set; }
        public long[] credits { get; set; }
    }
    public enum EvaluationType
    {
        PowerXStream = 1,
        freeSpin = 2,
        random
    }
    public enum FeatureType
    {
        PowerXStream = 1,
        pick,
        freeSpinWin

    }


    public class Win
    {
        public long winAmount { get; set; }
        public int[][] winningSymbolsPositions { get; set; }
        public int? lineNumber { get; set; }
        //public int? ways { get; set; }//TODO, Added because request from raven didnt worked cause the property wasnt in this class
        public int? symbolId { get; set; }
        public long multiplier { get; set; }
        public int freeSpinsAmount { get; set; }
        public int wildIndex { get; set; }
        public int wildLevel { get; set; }
        public string evaluationType { get; set; }
        public string featureType { get; set; }
        public string JackpotNameIfWon { get; set; }
        public int ReelNumber { get; set; }
        public List<string> AllPrizes { get; set; }

    }

    public class Event
    {
        public string id { get; set; }
        public int[][] positions { get; set; }
        public bool? upgrade { get; set; }
        public int? state { get; set; }
        public string gender { get; set; }
    }



    public class Spin
    {
        public int[][] reels { get; set; }
        public string type { get; set; }
        public List<Win> wins { get; set; }
        public long? sumWins { get; set; }
        public List<Event> events { get; set; }
        public List<Spin> childFeature { get; set; }
        public List<MCSymbol> MCSymbols { get; set; }
        public bool complete { get; set; }
        public int? index { get; set; }
        public long? cashWon { get; set; }
        public int value { get; set; }
        public long? totalSpinWin { get; set; }
        public int? freeSpinsLeft { get; set; }
        public int? totalFreeSpins { get; set; }
        public long? sumWinsFreeSpins { get; set; }
        public List<List<List<int>>> WildSpinsLeftForFSReel { get; set; }
        public List<List<List<int>>> WildSpinsLeftForBaseReel { get; set; }
        public List<List<List<int>>> listCoins { get; set; }
        public int RevealanceLevel { get; set; }
        public List<string> RevealanceSymbols { get; set; }
        public string jackpotTypeIfWon { get; set; }

    }


    public class SpinPublicStateRequest
    {
        public string simulator { get; set; }
        public bool? cleanState { get; set; }
        public string action { get; set; }
        public int? gameId { get; set; }
        public int? betAmount { get; set; }
        public int? denom { get; set; }
        public dynamic force { get; set; }
        public int index { get; set; }
        //public List<List<List<int>>> listCoins { get; set; }
        //public int totalBetAmountForRevealance { get; set; }
        //public List<int> WildSpinsLeftForFSReel { get; set; }


    }


    public class SpinPublicStateResponse
    {
        public string action { get; set; }
        public int? gameId { get; set; }
        public int? betAmount { get; set; }
        public int? denom { get; set; }
        public Spin spin { get; set; }
        //public List<List<List<int>>> listCoins { get; set; }
        //public int totalBetAmountForRevealance { get; set; }
        //public List<List<List<int>>> WildSpinsLeftForFSReel { get; set; }
        //public List<List<List<int>>> WildSpinsLeftForBaseReel { get; set; }
        


    }

    public class BonusGame
    {
        public List<string> symbols { get; set; }
        public string jackpotTypeIfWon { get; set; }
        public int selectedIndex { get; set; }
        public long winAmount { get; set; }
        public int betAmount { get; set; }
        //public int caseNumber { get; set; }

    }

    public class McFS
    {
        public List<MCSymbol> Symbols { get; set; }

    }
    public class MCSymbol
    {
        public int index { get; set; }
        public int[] coordinate { get; set; }
        public int symbol { get; set; }
        public long winAmount { get; set; }
        public bool IsLocked { get; set; }

        public TableTypeEnum Type { get; set; }
        public string JPSymbolIfString { get; set; }

    }


    public class SpinPrivateState
    {
        public BonusGame bonusGame { get; set; }
        public string state { get; set; }
        public string index { get; set; }
        public bool? completed { get; set; }
        public int? freeSpinsLeft { get; set; }
        public int? wildLevel { get; set; }
        public int binaryNumber { get; set; }
        public List<List<List<int>>> listCoins { get; set; }
        public int totalBetAmountForRevealance { get; set; }
        public List<List<List<int>>> WildSpinsLeftForFSReel { get; set; }
        public List<List<List<int>>> WildSpinsLeftForBaseReel { get; set; }
        public int? totalFreeSpins { get; set; }
        public long? sumWinsFreeSpins { get; set; }
        public Guid? transactionId { get; set; }
        public string userName { get; set; }
        public string sessionId { get; set; }
        public string mode { get; set; }
        public SpinPublicStateResponse lastState { get; set; }

        public int caseNumber { get; set; }


    }

    public class Platform
    {
          public double? balance { get; set; }
    }

    public class Config
    {
        public List<int> stakes { get; set; }
        public int defaultStake { get; set; }
        public List<int> denominations { get; set; }
        public int defaultDenomination { get; set; }
        public double rtp { get; set; }
    }


}
