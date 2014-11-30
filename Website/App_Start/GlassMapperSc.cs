/*************************************

DO NOT CHANGE THIS FILE - UPDATE GlassMapperScCustom.cs

**************************************/

using System;
using System.Linq;
using Glass.Mapper.Sc.CastleWindsor;
using Glass.Mapper.Sc.Configuration.Attributes;
using System.Web.Optimization;
using System.Web.Mvc;

[assembly: WebActivatorEx.PostApplicationStartMethod(typeof(Website.App_Start.GlassMapperSc), "Start")]

namespace Website.App_Start
{
	public static class  GlassMapperSc
	{
		public static void Start()
		{
            AreaRegistration.RegisterAllAreas();
            BundleConfig.RegisterBundles(BundleTable.Bundles);
			//create the resolver
			var resolver = Glass.Mapper.Sc.CastleWindsor.DependencyResolver.CreateStandardResolver();

			//install the custom services
			GlassMapperScCustom.CastleConfig(resolver.Container);

			//create a context
			var context = Glass.Mapper.Context.Create(resolver);
			context.Load(      
				GlassMapperScCustom.GlassLoaders()        				
				);

			GlassMapperScCustom.PostLoad();
		}
	}
}