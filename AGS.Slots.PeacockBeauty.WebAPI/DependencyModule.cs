using AGS.Slots.MermaidsFortune.Platform;
using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AGS.Slots.MermaidsFortune.Common.Interfaces;

namespace AGS.Slots.MermaidsFortune.WebAPI
{
    public class DependencyModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            base.Load(builder);
            builder.RegisterType<RequestManager>().AsSelf();
            builder.RegisterType<RequestExecutionContext>().As<IRequestContext>();


            builder.RegisterType<Game>().AsImplementedInterfaces();
           
            builder.RegisterModule(new Platform.DependencyModule());
            builder.RegisterModule(new Common.DependencyModule());

           
          




        }
    }
}
