
using Autofac;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace AGS.Slots.MermaidsFortune.Common
{
    public class DependencyModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);

            
          

            var configRoot = new ConfigurationBuilder()
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                //.AddJsonFile($"appsettings.{_args.environment}.json", optional: true, reloadOnChange: true)
                .Build();

            builder.RegisterInstance(configRoot);


            builder.Register(ctx =>
            {
                if (ctx.IsRegistered(typeof(Configs)))
                {
                    return ctx.Resolve<IConfigurationRoot>().Get<Configs>();
                }
                else
                {
                    return ctx.Resolve<IOptions<Configs>>().Value;
                }
            }).As<Configs>().AsSelf();


        }
    }
}
