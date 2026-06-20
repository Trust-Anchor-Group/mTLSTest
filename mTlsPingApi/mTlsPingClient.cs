using System;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace mTlsPingApi
{
	/// <summary>
	/// Class for testing mTLS connectivity by calling a `mTLSTest.ws` endpoint on a Neuron.
	/// </summary>
	public static class mTlsPingClient
	{
		/// <summary>
		/// TLS 1.2 &amp; 1.3
		/// </summary>
		public const SslProtocols SecureTls = (SslProtocols)((int)SslProtocols.Tls12 | 12288 /* TLS 1.3 */);

		/// <summary>
		/// TLS 1.0, 1.1, 1.2 &amp; 1.3
		/// </summary>
		public const SslProtocols TlsOnly = (SslProtocols)((int)SslProtocols.Tls | (int)SslProtocols.Tls11 | (int)SslProtocols.Tls12 | 12288 /* TLS 1.3 */);

		/// <summary>
		/// Calls the `mTLSTest.ws` endpoint, attempting to use the provided client
		/// certificate. The certificate must contain a private key.
		/// </summary>
		/// <param name="Host">Host name of Neuron</param>
		/// <param name="Port">HTTPS port where mTLS is enabled.</param>
		/// <param name="CertificateFileName">File name of client certificate.</param>
		/// <param name="CertificatePassword">Password for the client certificate.</param>
		/// <param name="TrustServer">If server certificate should be trusted.
		/// Default is false.</param>
		/// <param name="UseProxy">If a proxy should be used if available.
		/// Default is false.</param>
		/// <param name="TimeoutMs">Timeout, in milliseconds.
		/// Default is 10000 (10 seconds).</param>
		/// <returns>Information about the connection.</returns>
		/// <exception cref="ArgumentException">If the certificate do not contain
		/// a private key.</exception>
		public static Task<MTlsInfo> Ping(string Host, int Port, string CertificateFileName,
			string CertificatePassword, bool TrustServer = false, bool UseProxy = false,
			int TimeoutMs = 10000)
		{
			X509Certificate2 Certificate = new X509Certificate2(CertificateFileName,
				CertificatePassword);

			return Ping(Host, Port, Certificate, TrustServer, UseProxy, TimeoutMs);
		}

		/// <summary>
		/// Calls the `mTLSTest.ws` endpoint, attempting to use the provided client
		/// certificate. The certificate must contain a private key.
		/// </summary>
		/// <param name="Host">Host name of Neuron</param>
		/// <param name="Port">HTTPS port where mTLS is enabled.</param>
		/// <param name="Certificate">Client certificate.</param>
		/// <param name="TrustServer">If server certificate should be trusted.
		/// Default is false.</param>
		/// <param name="UseProxy">If a proxy should be used if available.
		/// Default is false.</param>
		/// <param name="TimeoutMs">Timeout, in milliseconds.
		/// Default is 10000 (10 seconds).</param>
		/// <returns>Information about the connection.</returns>
		/// <exception cref="ArgumentException">If the certificate do not contain
		/// a private key.</exception>
		public static async Task<MTlsInfo> Ping(string Host, int Port, X509Certificate2 Certificate, 
			bool TrustServer = false, bool UseProxy = false, int TimeoutMs = 10000)
		{
			if (!Certificate.HasPrivateKey)
			{
				throw new ArgumentException("Certificate must have a private key.",
					nameof(Certificate));
			}

			bool RemoteCertificateValidationCallback(object Sender,
				X509Certificate Certificate, X509Chain Chain, SslPolicyErrors SslPolicyErrors)
			{
				if (TrustServer)
					return true;
				else if (SslPolicyErrors == SslPolicyErrors.None)
					return true;
				else
				{
					// Check for incomplete revocation check in the chain

					if (SslPolicyErrors.HasFlag(SslPolicyErrors.RemoteCertificateChainErrors) && !(Chain is null))
					{
						foreach (X509ChainStatus Status in Chain.ChainStatus)
						{
							// Apple-specific error code for incomplete revocation check

							if (Status.Status == X509ChainStatusFlags.RevocationStatusUnknown ||
								Status.Status == X509ChainStatusFlags.OfflineRevocation)
							{
								continue; // Ignore this error
							}

							if (Status.Status != X509ChainStatusFlags.NoError)
							{
								if (Certificate is X509Certificate2 Certificate2)
									return Certificate2.Verify();

								Certificate2 = new X509Certificate2(Certificate.GetRawCertData());

								return Certificate2.Verify(); // Check if certificate fails on other errors
							}
						}

						return true; // Only revocation check failed, allow
					}

					return false;
				}
			}

			HttpClientHandler Handler = new HttpClientHandler()
			{
				AllowAutoRedirect = true,
				CheckCertificateRevocationList = true,
				ClientCertificateOptions = ClientCertificateOption.Automatic,
				ServerCertificateCustomValidationCallback = RemoteCertificateValidationCallback,
				AutomaticDecompression = (DecompressionMethods)(-1),    // All
				UseProxy = UseProxy
			};

			try
			{
				Handler.SslProtocols = TlsOnly;
			}
			catch (PlatformNotSupportedException)
			{
				// Ignore
			}

			if (!(Certificate is null))
			{
				Handler.ClientCertificateOptions = ClientCertificateOption.Manual;
				Handler.ClientCertificates.Add(Certificate);
			}

			StringBuilder Endpoint = new StringBuilder();

			Endpoint.Append("https://");
			Endpoint.Append(Host);
			Endpoint.Append(':');
			Endpoint.Append(Port);
			Endpoint.Append("/mTLSTest.ws");

			using HttpClient HttpClient = new HttpClient(Handler, true)
			{
				Timeout = TimeSpan.FromMilliseconds(TimeoutMs)
			};

			using HttpRequestMessage Request = new HttpRequestMessage()
			{
				RequestUri = new Uri(Endpoint.ToString()),
				Method = HttpMethod.Post,
			};

			if (!Request.Headers.Accept.TryParseAdd("application/json"))
				throw new InvalidOperationException("Unable to add JSON Accept header.");

			HttpResponseMessage Response = await HttpClient.SendAsync(Request);
			Response.EnsureSuccessStatusCode();

			byte[] Bin = await Response.Content.ReadAsByteArrayAsync();
			string s = Encoding.UTF8.GetString(Bin);
			JsonDocument Doc = JsonDocument.Parse(s);

			return new MTlsInfo(Doc);
		}
	}
}
