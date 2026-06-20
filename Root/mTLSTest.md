Title: mTLS Test
Description: Web Page that can be used to test mTLS connectivity.
Author: Peter Waher
Date: 2026-06-20
Master: /Master.md

=============================================

mTLS Test
============

| Publicly available information                                 ||
|:-------------------------|:-------------------------------------|
| Client Endpoint          | `{{Request.RemoteEndPoint}}`         |
| Host                     | `{{Request.Host}}`                   |
| Encrypted                | `{{Request.Encrypted}}`              |
| Cipher Strength          | `{{Request.CipherStrength}}`         |
| Client Certificate       | {{
if exists(Request.RemoteCertificate) then
(
	]]Available. |
| Issuer                   | ((Request.RemoteCertificate.Issuer)) |
| Subject                  | ((Request.RemoteCertificate.Subject)) |
| Serial Number            | ((Waher.Security.Hashes.BinaryToString(Request.RemoteCertificate.SerialNumberBytes.ToArray() ) )) |
| Certificate Valid        | ((Request.RemoteCertificateValid))[[
)
else
	]]Not available.[[
}} |
