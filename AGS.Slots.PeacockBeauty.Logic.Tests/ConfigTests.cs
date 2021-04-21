
using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune;
using Xunit;

namespace AGS.Slots.MermaidsFortune.Logic.Tests
{
    public class ConfigTests
    {
        //AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune
        private readonly Mock<IRandom> _random;
        private readonly Config _config;
        private Mock<IRequestContext> _contextInstance;



        public ConfigTests()
        {
            _random = new Mock<IRandom>();
            //_random.Setup(a =>  a.Next(0, 100)).Returns(6);
            //_random.Setup(a => a.Next(0, 50)).Returns(4);
            _random.Setup(a => a.GetRandomNumbers(It.IsAny<List<RandomNumber>>()))
                .Returns(new List<RandomNumber>(new RandomNumber[] {
                new RandomNumber() {Min=0,Max=57,Quantity=1,Values=new List<int>(new int[]{56 })},
                new RandomNumber() {Min=0,Max=61,Quantity=1,Values=new List<int>(new int[]{59 })},
                new RandomNumber() {Min=0,Max=71,Quantity=1,Values=new List<int>(new int[]{65 })},
                new RandomNumber() {Min=0,Max=111,Quantity=1,Values=new List<int>(new int[]{60 })},
                new RandomNumber() {Min=0,Max=104,Quantity=1,Values=new List<int>(new int[]{78 })} }));
            _config = new Config("Math96");
            _contextInstance = new Mock<IRequestContext>();



        }



    }
}
