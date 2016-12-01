using System;
using System.Collections.Generic;
using System.Text;

namespace Fiffi.Gateway
{
	public class Service
	{
		public string Url { get; set; }
		public string Name { get; set; }
		public string Rule { get; set; }
		public ApiKey Key { get; set; }
	}

	public class ApiKey
	{
		public string Name { get; set; }
		public string Value { get; set; }

		public static IDictionary<string, string> ToHeader(ApiKey key)
			=> key == null ? null : new Dictionary<string, string> { { key.Name, key.Value } };

	}
}
