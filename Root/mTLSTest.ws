Result:=
{
	"clientEndpoint":Str(Request.RemoteEndPoint),
	"host":Request.Host,
	"encrypted":Request.Encrypted,
	"cipherStrength":Request.CipherStrength
};

if exists(Request.RemoteCertificate) then
(
	Result["clientCertificate"]:=true;
	Result["issuer"]:=Request.RemoteCertificate.Issuer;
	Result["subject"]:=Request.RemoteCertificate.Subject;
	Result["serialNumber"]:=Waher.Security.Hashes.BinaryToString(Request.RemoteCertificate.SerialNumberBytes.ToArray());
	Result["valid"]:=Request.RemoteCertificateValid;
)
else
	Result["clientCertificate"]:=false;

Result;