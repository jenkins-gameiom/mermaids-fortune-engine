using System;
using System.Collections.Generic;
using System.Text;
using AGS.Slots.MermaidsFortune.Common;
using AGS.Slots.MermaidsFortune.Common.Entities;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune;
using Autofac.Features.Indexed;
using Moq;
using Xunit;

namespace AGS.Slots.MermaidsFortune.Logic.Tests
{
    public class JackpotServiceTests
    {
        private readonly IJackpotService _jackpotServiceInstance;
        private readonly IRequestContext _context;

        public JackpotServiceTests()
        {
            _context = new BonusGameServiceTests.RequestConextImpl { State = new State(), RequestItems = new RequestItems { betAmount = 88, denom = 1 } }; ;
            _jackpotServiceInstance = new JackpotService(_context);
        }

        [Theory]//1000, 2500, 80000, 1000000
        [InlineData("322004")]//02 - 81000
        [InlineData("3201014")]//01 - 3500
        public void HandleJackpot_CheckItemsReturnedOrder(string items)
        {
            _context.State.JackpotGame = new JackpotGame();
            _context.State.JackpotGame.selectedItems = new List<JackpotItem>();
            _context.State.JackpotGame.leftItems = items.Length;
            var itemCharArray = items.ToCharArray();
            for (int i = 0; i < itemCharArray.Length; i++)
            {
                _context.State.JackpotGame.selectedItems.Add(new JackpotItem{Symbol = itemCharArray[i].ToString()});
            }
            for (int i = 0; i < itemCharArray.Length; i++)
            {
                var returnedItem = _jackpotServiceInstance.HandleJackpot();
                Assert.Equal(returnedItem, _context.State.JackpotGame.selectedItems[i].Symbol);
            }
            

        }
        [Theory]//1000, 2500, 80000, 1000000
        [InlineData("322004")]//02 - 81000
        [InlineData("3201014")]//01 - 3500
        public void HandleJackpot_CheckLeftItemsValid(string items)
        {
            _context.State.JackpotGame = new JackpotGame();
            _context.State.JackpotGame.selectedItems = new List<JackpotItem>();
            _context.State.JackpotGame.leftItems = items.Length;
            var itemCharArray = items.ToCharArray();
            for (int i = 0; i < itemCharArray.Length; i++)
            {
                _context.State.JackpotGame.selectedItems.Add(new JackpotItem { Symbol = itemCharArray[i].ToString() });
            }
            for (int i = 0; i < itemCharArray.Length; i++)
            {
                _jackpotServiceInstance.HandleJackpot();
                Assert.Equal(_context.State.JackpotGame.leftItems, _context.State.JackpotGame.selectedItems.Count - (i + 1));
            }


        }
    }
}
