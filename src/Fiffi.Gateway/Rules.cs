using Microsoft.AspNetCore.Http;

namespace Fiffi.Gateway
{
	public static class Rules
	{
		public static bool Match(HttpContext context, string rule)
			=> context.Request.Path.Value.Contains(rule);
	}
}
