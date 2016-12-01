using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Proxy;

namespace Fiffi.Gateway
{
	public static class Extensions
	{
		public static void UseProxy(this IApplicationBuilder app, IDictionary<string, ProxyOptions> rules) =>
			rules.ToList().ForEach(rule => app.UseWhen(context => Rules.Match(context, rule.Key), builder => builder.RunProxy(rule.Value)));

	}
}
