using AGS.Slots.MermaidsFortune.Logic.Engine.Interfaces;
using AGS.Slots.MermaidsFortune.Logic.Engine.Providers;
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using AGS.Slots.MermaidsFortune.Logic.Engine.MermaidsFortune;

namespace AGS.Slots.MermaidsFortune.Platform
{
    public class DependencyModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);



            builder.RegisterType<Spins>().AsSelf();
            builder.RegisterType<Init>().AsSelf();
            builder.RegisterType<Game>().AsSelf();



            builder.RegisterModule(new AGS.Slots.MermaidsFortune.Logic.DependencyModule());
            builder.RegisterModule(new AGS.Slots.MermaidsFortune.Common.DependencyModule());


            var configRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                //.AddJsonFile($"appsettings.{_args.environment}.json", optional: true, reloadOnChange: true)
                .Build();

            builder.RegisterInstance(configRoot);




        }
    }
}
