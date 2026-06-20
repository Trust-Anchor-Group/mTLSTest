using mTlsPingApi;

internal class Program
{
	private static void Main(string[] Args)
	{
		try
		{
			int i = 0;
			int c = Args.Length;
			string? Host = null;
			int? Port = null;
			string? FileName = null;
			string? Password = null;
			bool TrustServer = false;
			bool UseProxy = false;
			bool Help = c == 0;
			int Timeout = 10000;

			while (i < c)
			{
				switch (Args[i++].ToLower())
				{
					case "-h":
						if (!string.IsNullOrEmpty(Host))
							throw new Exception("Host already defined.");

						if (i >= c)
							throw new Exception("Missing host.");

						Host = Args[i++];
						break;

					case "-p":
						if (Port.HasValue)
							throw new Exception("Port already defined.");

						if (i >= c)
							throw new Exception("Missing port.");

						if (!int.TryParse(Args[i++], out int j) ||
							j <= 0 || j > ushort.MaxValue)
						{
							throw new Exception("Invalid port number.");
						}

						Port = j;
						break;

					case "-f":
						if (!string.IsNullOrEmpty(FileName))
							throw new Exception("Certificate file name already defined.");

						if (i >= c)
							throw new Exception("Missing certificate file name.");

						FileName = Args[i++];
						break;

					case "-w":
						if (!string.IsNullOrEmpty(Password))
							throw new Exception("Certificate password already defined.");

						if (i >= c)
							throw new Exception("Missing certificate password.");

						Password = Args[i++];
						break;

					case "-t":
						TrustServer = true;
						break;

					case "-x":
						UseProxy = true;
						break;

					case "-?":
						Help = true;
						break;

					case "-m":
						if (i >= c)
							throw new Exception("Missing timeout.");

						if (!int.TryParse(Args[i++], out j) ||
							j <= 0 || j > ushort.MaxValue)
						{
							throw new Exception("Invalid timeout.");
						}

						Timeout = j;
						break;
				}
			}

			if (Help)
			{
				Console.WriteLine("Usage: mTlsPing -h <host> -p <port> -f <certificate file name> " +
					"-w <certificate password> [-t] [-x] [-m <timeout>] [-?]");
				Console.WriteLine();
				Console.WriteLine("Options:");
				Console.WriteLine("  -h <host>                  Host name of Neuron.");
				Console.WriteLine("  -p <port>                  HTTPS port where mTLS is enabled.");
				Console.WriteLine("  -f <certificate file name> File name of client certificate.");
				Console.WriteLine("  -w <certificate password>  Password for the client certificate.");
				Console.WriteLine("  -t                         Trust server certificate.");
				Console.WriteLine("  -x                         Use proxy if available.");
				Console.WriteLine("  -m <timeout>               Timeout in milliseconds. Default is 10000 (10 seconds).");
				Console.WriteLine("  -?                         Show this help message.");

				if (c == 0)
					return;
			}

			if (string.IsNullOrEmpty(Host))
				throw new Exception("Host not defined.");

			if (!Port.HasValue)
				throw new Exception("Port not defined.");

			if (string.IsNullOrEmpty(FileName))
				throw new Exception("Certificate file name not defined.");

			if (Password is null)
				throw new Exception("Certificate password not defined.");

			MTlsInfo Info = mTlsPingClient.Ping(Host, Port.Value, FileName, Password,
				TrustServer, UseProxy, Timeout).Result;

			Console.WriteLine("Client endpoint: " + Info.ClientEndpoint);
			Console.WriteLine("Host: " + Info.Host);
			Console.WriteLine("Encrypted: " + Info.Encrypted);
			Console.WriteLine("Cipher strength: " + Info.CipherStrength);
			Console.WriteLine("Client certificate: " + Info.ClientCertificate);
			
			if (Info.ClientCertificate)
			{
				Console.WriteLine("Issuer: " + Info.Issuer);
				Console.WriteLine("Subject: " + Info.Subject);
				Console.WriteLine("Serial number: " + Info.SerialNumber);
				Console.WriteLine("Valid: " + Info.Valid);
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("ERROR: " + ex.ToString());
		}
	}
}