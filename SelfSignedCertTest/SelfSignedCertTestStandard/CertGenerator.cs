using System.Collections.ObjectModel;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace SelfSignedCertTestStandard;

public class CertGenerator
{
    private readonly Type? CertificateRequestType;
    private readonly ConstructorInfo? CertificateRequestConstructor;
    private readonly PropertyInfo? CertificateExtensionsProperty;
    private readonly MethodInfo? CreateSelfSignedMethod;
    private readonly Type? SanBuilderType;
    private readonly MethodInfo? AddDnsNameMethod;

    public CertGenerator()
    {
        // These are all public methods within the System.Security.Cryptography.X509Certificates namespace.
        // They are not available in .NET Standard 2.0, but are in .NET Framework 4.7.2 and .NET.
        this.CertificateRequestType =
            typeof(RSACertificateExtensions).Assembly.GetType(
                "System.Security.Cryptography.X509Certificates.CertificateRequest");
        this.CertificateRequestConstructor = CertificateRequestType?.GetConstructor(
        [
            typeof(string),
            typeof(RSA),
            typeof(HashAlgorithmName),
            typeof(RSASignaturePadding)
        ]);
        this.CertificateExtensionsProperty = CertificateRequestType?.GetProperty("CertificateExtensions");
        this.CreateSelfSignedMethod = CertificateRequestType?.GetMethod("CreateSelfSigned");
        this.SanBuilderType = typeof(X509Extension).Assembly.GetType("System.Security.Cryptography.X509Certificates.SubjectAlternativeNameBuilder");
        this.AddDnsNameMethod = SanBuilderType?.GetMethod("AddDnsName", new Type[] { typeof(string) });
    }
    
    public X509Certificate2 GenerateCert()
    {
        if (CertificateRequestType == null)
        {
            throw new InvalidOperationException("CertificateRequest type not found.");
        }

        object request = CertificateRequestConstructor!.Invoke(new object[]
        {
            "CN=testServer",
            RSA.Create(),
            HashAlgorithmName.SHA256,
            RSASignaturePadding.Pkcs1
        });

        Collection<X509Extension> extensions =
            (Collection<X509Extension>)CertificateExtensionsProperty!.GetValue(request)!;
        extensions.Add(new X509KeyUsageExtension(X509KeyUsageFlags.KeyEncipherment, true));

        object? sanBuilder2 = Activator.CreateInstance(SanBuilderType!);
        this.AddDnsNameMethod!.Invoke(sanBuilder2, new object[] { "localhost" });
        this.AddDnsNameMethod!.Invoke(sanBuilder2, new object[] { "0.0.0.0" });
        this.AddDnsNameMethod!.Invoke(sanBuilder2, new object[] { IPAddress.Loopback });
        foreach(var ip in NetworkUtils.DeviceIps())
        {
            this.AddDnsNameMethod!.Invoke(sanBuilder2, new object[] { ip });
        }

        MethodInfo? buildMethod = SanBuilderType!.GetMethod("Build");
        X509Extension sanExtension = (X509Extension)buildMethod!.Invoke(sanBuilder2, new object?[] { false })!;
        extensions.Add(sanExtension);

        DateTimeOffset now = DateTimeOffset.UtcNow;
        X509Certificate2? cert = (X509Certificate2?)CreateSelfSignedMethod!.Invoke(
            request,
            new object[]
            {
                now.AddDays(-1),
                now.AddDays(7)
            });

        if (cert is null)
        {
            throw new InvalidOperationException("CreateSelfSignedMethod return null.");
        }

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            using (cert)
            {
                return new X509Certificate2(cert.Export(X509ContentType.Pfx), "", X509KeyStorageFlags.UserKeySet);
            }
        }

        var bytes = cert!.Export(X509ContentType.Pfx);
        return new X509Certificate2(bytes);
    }
}