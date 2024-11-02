using System;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace GenerateSelfSignedCertificate;

internal class Program
{
    static void Main()
    {
        using X509Certificate2 cert = GenerateSelfSignedCertificate();
        string publicKeyPem = GetRSAPublickKeyPem(cert);
        string privateKeyPem = GetRSAPrivateKeyPem(cert);

        Console.WriteLine(publicKeyPem);
        Console.WriteLine();
        Console.WriteLine(privateKeyPem);
    }

    public static X509Certificate2 GenerateSelfSignedCertificate()
    {
        using RSA rsa = RSA.Create(2048);
        CertificateRequest req = new CertificateRequest("CN=Test", rsa, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);

        return req.CreateSelfSigned(DateTimeOffset.Now, DateTimeOffset.Now.AddDays(30));
    }

    private static string GetRSAPublickKeyPem(X509Certificate2 cert)
    {
        RSA publicKey = cert.GetRSAPublicKey();
        return publicKey.ExportRSAPublicKeyPem();
    }

    private static string GetRSAPrivateKeyPem(X509Certificate2 cert)
    {
        RSA privateKey = cert.GetRSAPrivateKey();
        return privateKey.ExportRSAPrivateKeyPem();
    }
}