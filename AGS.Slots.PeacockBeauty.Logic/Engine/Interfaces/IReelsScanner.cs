
using AGS.Slots.PeacockBeauty.Logic.Resolvers;
using System.Linq;

namespace AGS.Slots.PeacockBeauty.Logic.Engine.Interfaces
{
   

    public abstract class ReelsScanner<T>
    {


        protected Result _result = new Result();
        protected IPayoutResolver _resolver = new Resolver();
        protected int _numOfReels;
        ReelItemComparer itemComparer = new ReelItemComparer();

        public abstract  void Scan(string force);
        public abstract bool ScanBackWards { get;  }

        public  Result Result
        {
            get { return _result; }
            set { _result = value; }
        }


        public ReelsScanner(IPayoutResolver resolver,int numOfReels)
        {
            _resolver = resolver;
           _numOfReels = numOfReels;
        }

        protected bool HandleItem(ItemOnReel item, bool scanBackWards)
        {
            bool ret = false;
            ItemOnReel.HitCount++;
            if (IsFirstReel(item, scanBackWards))
            {
                if (!_result.Sequence.Contains((item), itemComparer))
                    _result.Sequence.Add(item);
                return true;
            }
            if (_resolver.IsWildCard(item))
            {
                if (IsFollowingReel(_result.LastSequenceItem,item, scanBackWards))
                {
                    _result.Sequence.Add(item);
                    ret = true;
                }
            }
            else
            {

                if (_result.Sequence.Contains(item, itemComparer) &&
                    IsFollowingReel(_result.LastSequenceItem, item, scanBackWards))
                {
                    _result.Sequence.Add(item);
                    ret = true;
                }

            }
            return ret;
        }

        public bool IsFollowingReel(ItemOnReel item, ItemOnReel followingReel,bool scanBackWards)
        {
            if (scanBackWards)
                return (item.Reel == followingReel.Reel + 1);
            return (item.Reel == followingReel.Reel - 1);
        }

        public bool IsFirstReel(ItemOnReel item,bool scanBackWards)
        {
            if (scanBackWards)
                return item.Reel == _numOfReels - 1;
            return item.Reel == 0;
        }


        public Result GetResult()
        {
            return Result;
        }

        public void EvaluateResult(Bet bet,IRandom random)
        {
            _resolver.EvaluateResult(bet, Result,random);
        }


        public void AddSpecialItems(ItemOnReel itemOnReel)
        {
            if (_resolver.IsScatter(itemOnReel))
            {
                _result.Scatter.Add(itemOnReel);
            }
            if (_resolver.IsWildCard(itemOnReel))
            {
                _result.WildCards.Add(itemOnReel);
            }
        }
    }
}
