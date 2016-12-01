using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Fiffi.Gateway;
using static Fiffi.Gateway.Request;
using Microsoft.Extensions.Configuration;

namespace Gateway
{
	public class Program
	{
		public static void Main(string[] args)
		{
			var rules = new ConfigurationBuilder()
			.SetBasePath(Directory.GetCurrentDirectory())
			.AddJsonFile($"gateway.json", true)
			.AddEnvironmentVariables()
			.Build()
			.Get<Service[]>()
			.ToDictionary(s => s.Rule, service => new ProxyOptions
			{
				BackChannelMessageHandler = new RequestHandler(Decorate(r => ApiKey(r, service.Key.Name, service.Key.Value), Retry, CircutBreaker)),
				Host = new Uri(service.Url).Host,
				Port = new Uri(service.Url).Port.ToString(),
				Scheme = new Uri(service.Url).Scheme
			});
			
			var host = new WebHostBuilder()
				.UseKestrel()
				.UseContentRoot(Directory.GetCurrentDirectory())
				.UseIISIntegration()
				.Configure(app =>
				{
					app.UseProxy(rules);

					app.Run(async (context) =>
					{
						await context.Response.WriteAsync("Hello World!");
					});
				})
				.Build();

			host.Run();
		}


	}

}



