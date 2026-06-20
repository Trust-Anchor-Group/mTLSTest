using System.Text.Json;

namespace mTlsPingApi
{
	/// <summary>
	/// Response class containing information returned from the mTLSTest Web Service.
	/// </summary>
	public class MTlsInfo
	{
		/// <summary>
		/// Response class containing information returned from the mTLSTest Web Service.
		/// </summary>
		/// <param name="Response">Response from web service.</param>
		public MTlsInfo(JsonDocument Response)
		{
			JsonElement Root = Response.RootElement;

			this.ClientEndpoint = Root.GetProperty("clientEndpoint").GetString() ?? string.Empty;
			this.Host = Root.GetProperty("host").GetString() ?? string.Empty;
			this.Encrypted = Root.GetProperty("encrypted").GetBoolean();
			this.CipherStrength = (int)Root.GetProperty("cipherStrength").GetDouble();
			this.ClientCertificate = Root.GetProperty("clientCertificate").GetBoolean();

			if (this.ClientCertificate)
			{
				this.Issuer = Root.GetProperty("issuer").GetString();
				this.Subject = Root.GetProperty("subject").GetString();
				this.SerialNumber = Root.GetProperty("serialNumber").GetString();
				this.Valid = Root.GetProperty("valid").GetBoolean();
			}
		}

		/// <summary>
		/// Endpoint of client, as seend by server
		/// </summary>
		public string ClientEndpoint { get; private set; }

		/// <summary>
		/// Host name as described by the Host header in the request to the web service.
		/// </summary>
		public string Host { get; private set; }

		/// <summary>
		/// If the request was made over an encrypted channel.
		/// </summary>
		public bool Encrypted { get; private set; }

		/// <summary>
		/// Strength of the encryption cipher used for the request.
		/// </summary>
		public int CipherStrength { get; private set; }

		/// <summary>
		/// If a client certificate was detected by the server.
		/// </summary>
		public bool ClientCertificate { get; private set; }

		/// <summary>
		/// Issuer of client certificate, as seen by the server.
		/// </summary>
		public string? Issuer { get; private set; }

		/// <summary>
		/// Subject of client certificate, as seen by the server.
		/// </summary>
		public string? Subject { get; private set; }

		/// <summary>
		/// Serial number of client certificate, as seen by the server.
		/// </summary>
		public string? SerialNumber { get; private set; }

		/// <summary>
		/// If the server was able to validate the client certificate.
		/// </summary>
		public bool? Valid { get; private set; }
	}
}
