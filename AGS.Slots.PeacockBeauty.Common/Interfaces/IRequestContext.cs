using AGS.Slots.MermaidsFortune.Common.Entities;

namespace AGS.Slots.MermaidsFortune.Common.Interfaces
{
    public interface IRequestContext
    {
        IStateItems State {get;set;}
        Config Config { get; set; }

        IMathFile MathFile { get; set; }
        RequestItems RequestItems { get; set; }
        int GetDenom();
        int GetBetAmount();
    }
}