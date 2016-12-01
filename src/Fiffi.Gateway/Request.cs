using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Polly;

namespace Fiffi.Gateway
{
	using System.Linq;
	using Req = Func<HttpRequestMessage, CancellationToken, Task<HttpResponseMessage>>;

	public static class Request {
		public static Func<Req, Req> Decorate(params Func<Req, Req>[] policyRequests)
		=> req => policyRequests.Aggregate((left, right) => inputReq => left(right(inputReq)))(req);

		public static Func<Req, Req> RuleUrlModifier(string rule) =>
			r => RuleUrlModifier(r, rule);

		public static Req RuleUrlModifier(Req r, string rule) =>
		 (message, token) => r(message, token); //TODO fix url

		public static Func<Req, Req> ApiKey(string keyName, string key) =>
			r => ApiKey(r, keyName, key);

		public static Req ApiKey(Req r, string keyName, string key)
			=> (message, token) =>
			{
				message.Headers.Add(keyName, key);
				return r(message, token);
			};
	

		public static Req Retry(Req r)
			=> (message, token) => Policy
				.Handle<HttpRequestException>()
				.Retry(2)
				.ExecuteAsync(() => r(message, token));

		public static Req CircutBreaker(Req r) =>
			(message, token) => Policy
				.Handle<Exception>()
				.CircuitBreakerAsync(2, TimeSpan.FromSeconds(5))
				.ExecuteAsync(() => r(message, token));
	}

	public class RequestHandler : HttpClientHandler
	{
		private readonly Func<Req, Req> _send;

		public RequestHandler(Func<Req, Req> send)
		{
			_send = send;
		}

		protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
			=> _send(base.SendAsync)(request, cancellationToken);

	}
}