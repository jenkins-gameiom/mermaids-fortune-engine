//using AGS.Slots.MermaidsFortune.Common.Enums;
using AGS.Slots.MermaidsFortune.Common.Enums;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace AGS.Slots.MermaidsFortune.Common.Entities
{

    public class PlatformRequest 
    {

        [JsonProperty(PropertyName = "publicState")]
        public SpinPublicStateRequest PublicState { get; set; }
        [JsonProperty(PropertyName = "privateState")]
        public SpinPrivateState PrivateState { get; set; }

        [JsonProperty(PropertyName = "platform")]
        public Platform Platform { get; set; }

        [JsonProperty(PropertyName = "config")]
        public Config Config { get; set; }
    }

    public class PlatformResponse
    {

        [JsonProperty(PropertyName = "publicState")]
        public SpinPublicStateResponse PublicState { get; set; }
        [JsonProperty(PropertyName = "privateState")]
        public SpinPrivateState PrivateState { get; set; }

        [JsonProperty(PropertyName = "platform")]
        public Platform Platform { get; set; }

        [JsonProperty(PropertyName = "config")]
        public Config Config { get; set; }
        [JsonProperty(PropertyName = "transactions")]
        public Transaction Transactions { get; set; }
    }
    public enum SpinType
    {
        spin,
        freeSpin,
        pick,
        freeSpinsAmount,
        jackpotPick

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
    


    public class Win
    {
        public long winAmount { get; set; }
        public int[][] winningSymbolsPositions { get; set; }
        public int? lineNumber { get; set; }
        public int? symbolId { get; set; }
        public long multiplier { get; set; }
        public int? ways { get; set; }
        public int freeSpinsAmount { get; set; }
        public int wildLevel { get; set; }
        public string evaluationType { get; set; }
        public string featureType { get; set; }
        public bool gate { get; set; }
        public int[] indexes { get; set; }
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
        public long? cashCurrentlyAwarded { get; set; }
        public string value { get; set; }
        public long? totalSpinWin { get; set; }
        public int? freeSpinsLeft { get; set; }
        public int? freeSpinsspun { get; set; }
        public int? totalFreeSpins { get; set; }
        public long? sumWinsFreeSpins { get; set; }
        public int treasureChestLevel { get; set; }

    }


    public class SpinPublicStateRequest
    {
        public string simulator { get; set; }
        public string userName { get; set; }
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
        //public List<List<List<int>>> WildSpinsLeftForBaseReel { get; set; }C:\Projects\MermaidsFortune\AGS.Slots.MermaidsFortune.Common\Json.cs
        


    }

    public class BonusGame
    {
        //public List<string> symbols { get; set; }
        //public string jackpotTypeIfWon { get; set; }
        //public int selectedIndex { get; set; }
        //public long winAmount { get; set; }
        //public int betAmount { get; set; }
        //public int caseNumber { get; set; }

        //public MCSymbol[] MCSymbolsFromSpin { get; set; }
        public List<MCSymbol> MCSymbols { get; set; }
        public long winAmount { get; set; }
        public int freeSpinsCount { get; set; }
        public int fsLeft { get; set; }
        public int fsDone { get; set; }
        public int totalSpin { get; set; }
        public int denom { get; set; }
        public int bet { get; set; }
        public bool complete { get; set; }
        public RemoveX2Enum X2IndexesToRemove { get; set; }
        public bool removeX8 { get; set; }
        public int specialIndex { get; set; }
        public long oldWonAmount { get; set; }

    }

    public class JackpotGame
    {
        public JackpotGame()
        {
            selectedItems = new List<JackpotItem>();
        }
        public List<JackpotItem> selectedItems { get; set; }
        public Dictionary<string, int> valuesDictionary { get; set; }
        public long winAmount { get; set; }
        public string outcome { get; set; }
        public int leftItems { get; set; }
        public bool complete
        {
            get
            {
                return leftItems == 0;
            }
        }
        public int index;

    }

    public class JackpotItem
    {
        public int Index { get; set; }
        public string Symbol { get; set; }
    }

    public class McFS
    {
        public List<MCSymbol> Symbols { get; set; }

    }
    public class MCSymbol
    {
        public int index { get; set; }
        public Tuple<int, int> coordinate { get; set; }
        public int symbol { get; set; }
        public long winAmount { get; set; }
        public bool IsLocked { get; set; }

        public TableTypeEnum Type { get; set; }
        public string JPSymbolIfString { get; set; }

        public MCSymbol(int index, int reel, int row, int symbol, bool isLocked, TableTypeEnum type, long winAmount = 0)
        {
            this.index = index;
            this.coordinate = new Tuple<int, int>(reel, row);
            this.symbol = symbol;
            this.IsLocked = isLocked;
            this.Type = type;
            this.winAmount = winAmount;
        }

    }

    public class RequestItems
    {
        public bool isFreeSpin { get; set; }

        public int betAmount { get; set; }
        public int denom { get; set; }
        public bool? cleanState { get; set; }
        public dynamic force { get; set; }
        public string action { get; set; }

    }


    public class SpinPrivateState : IStateItems
    {
        //public string force { get; set; }
        public int animationState { get; set; }
        public int TreasureChestTurnOver { get; set; }
        public int TreasureChestTurnState { get; set; }
        public BonusGame BonusGame { get; set; }
        public JackpotGame JackpotGame { get; set; }
        public string state { get; set; }
        public bool? completed { get; set; }
        public int? freeSpinsLeft { get; set; }
        public int? totalFreeSpins { get; set; }
        public long? sumWinsFreeSpins { get; set; }
        public Guid? transactionId { get; set; }
        public string userName { get; set; }
        public string sessionId { get; set; }
        public string mode { get; set; }
        public SpinPublicStateResponse lastState { get; set; }

    }

    public class State : IStateItems
    {
        public string state { get; set; }
        public int animationState { get; set; }
        public bool? completed { get; set; }
        public int? freeSpinsLeft { get; set; }
        public int? totalFreeSpins { get; set; }
        public int TreasureChestTurnOver { get; set; }
        public int TreasureChestTurnState { get; set; }
        public long? sumWinsFreeSpins { get; set; }
        public Guid? transactionId { get; set; }
        public string userName { get; set; }
        public string sessionId { get; set; }
        public string mode { get; set; }
        public BonusGame BonusGame { get; set; }
        public JackpotGame JackpotGame { get; set; }
        public SpinPublicStateResponse lastState { get; set; }
        public State()
        {
        }
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
