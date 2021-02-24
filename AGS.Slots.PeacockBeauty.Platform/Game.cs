using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using AGS.Slots.MermaidsFortune.Logic;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;

namespace AGS.Slots.MermaidsFortune.Platform
{
    public class Game 
    {
        //public static SlotGame CurrentGame = new RedSilk();
        private readonly Spins _spins;
        private readonly BonusPick _bonusPick;
        private readonly JackpotPick _jackpotPick;
        private readonly Init _init;

        public Game(Spins spins,BonusPick pick,Init init, JackpotPick jackpotPick)
        {
            _spins = spins;
            _bonusPick = pick;
            _init = init;
            _jackpotPick = jackpotPick;
        }

        public dynamic InitSlot(dynamic obj)
        {
            try
            {
                return _init.InitSlot(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public dynamic Spin(dynamic obj)
        {
            try
            {
                return _spins.Spin(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public dynamic Pick(dynamic obj)
        {
            try
            {
                return _bonusPick.Pick(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public dynamic JackpotPick(dynamic obj)
        {
            try
            {
                return _jackpotPick.PickJackpot(obj);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


    }
}
