namespace mTlsPingApi.Test
{
	[TestClass]
	public sealed class ApiTests
	{
		[TestMethod]
		[DataRow("lab.tagroot.io", 8088, "Data\\certificate.pfx", "testexamplecom",
			"E=test@example.com, OU=D, O=example.com, L=Stockholm, S=Stockholm, C=SE, CN=localhost",
			"E=test@example.com, OU=D, O=example.com, L=Stockholm, S=Stockholm, C=SE, CN=localhost",
			"00947fd2f9ef86d010")]
		public async Task Test_01_Ping(string Host, int Port, string CertificateFileName, 
			string Password, string Issuer, string Subject, string SerialNumber)
		{
			MTlsInfo Info = await mTlsPingClient.Ping(Host, Port, CertificateFileName, Password);

			Assert.IsFalse(string.IsNullOrEmpty(Info.ClientEndpoint));
			Assert.AreEqual(Host, Info.Host);
			Assert.IsTrue(Info.Encrypted);
			Assert.IsTrue(Info.CipherStrength > 0);
			Assert.IsTrue(Info.ClientCertificate);
			Assert.AreEqual(Issuer, Info.Issuer);
			Assert.AreEqual(Subject, Info.Subject);
			Assert.AreEqual(SerialNumber, Info.SerialNumber);
			Assert.IsFalse(Info.Valid);
		}
	}
}
