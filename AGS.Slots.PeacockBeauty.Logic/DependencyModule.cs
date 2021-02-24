using AGS.Slots.MermaidsFortune.Common.Enums;
using AGS.Slots.MermaidsFortune.Logic.Engine;
using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine.Providers;
using Autofac;
using System;
using System.Collections.Generic;
using System.Text;
using AGS.Slots.MermaidsFortune.Common.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune;

namespace AGS.Slots.MermaidsFortune.Logic
{
    public class DependencyModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<RandomGeneratorCrypro>().Keyed<IRandom>(RandomizerType.Local);
            builder.RegisterType<IgamingRandomize>().Keyed<IRandom>(RandomizerType.Remote);
            builder.RegisterType<MermaidsFortuneResolver>().AsImplementedInterfaces();
            builder.RegisterType<MermaidsFortuneScanner>().AsSelf();
            builder.RegisterType<GameEngine>().AsSelf();
            builder.RegisterType<MathFileServiceProvider>().AsImplementedInterfaces();
           

            var builder1 = new ContainerBuilder();
            builder1.RegisterInstance(new Config("Math94")).AsImplementedInterfaces().Keyed<IMathFile>(MathFileType.Config94);
            builder1.RegisterInstance(new Config("Math96")).AsImplementedInterfaces().Keyed<IMathFile>(MathFileType.Config96);
            builder.RegisterInstance<IContainer>(builder1.Build());
           

            builder.RegisterModule(new AGS.Slots.MermaidsFortune.Common.DependencyModule());
        }
    }
}
