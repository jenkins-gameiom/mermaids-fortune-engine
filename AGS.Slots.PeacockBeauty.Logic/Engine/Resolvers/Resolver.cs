using AGS.Slots.PeacockBeauty.Logic.Engine.Interfaces;

namespace AGS.Slots.PeacockBeauty.Logic.Resolvers
{
    public class Resolver : IPayoutResolver
    {

      


        public void EvaluateResult(Bet bet,Result result,IRandom random)
        {
            
        }

        public bool IsWildCard(ItemOnReel item)
        {
            return item.Symbol == 99;
        }

        public bool IsScatter(ItemOnReel item)
        {
            return item.Symbol == 88;
        }

        public void EvaluateResultFaster(Bet bet, Result result, IRandom random)
        {
            
        }
    }


}


