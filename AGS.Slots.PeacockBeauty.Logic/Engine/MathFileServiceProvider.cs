using AGS.Slots.MermaidsFortune.Common.Enums;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using Autofac;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGS.Slots.MermaidsFortune.Logic.Engine
{
    public class MathFileServiceProvider : IMathFileService
    {

        private readonly IContainer _container;

        public MathFileServiceProvider(IContainer container)
        {
            _container = container;
        }
        public IMathFile GetMathFile(MathFileType type)
        {
           return  _container.ResolveKeyed<IMathFile>(type); 
        }
    }
}
