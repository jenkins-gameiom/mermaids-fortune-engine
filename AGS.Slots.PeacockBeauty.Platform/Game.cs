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
        private readonly Init _init;

        public Game(Spins spins,Init init)
        {
            _spins = spins;
            _init = init;
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

    }
}
