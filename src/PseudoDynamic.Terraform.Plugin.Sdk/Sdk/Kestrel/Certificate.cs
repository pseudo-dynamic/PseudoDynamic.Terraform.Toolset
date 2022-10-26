using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Org.BouncyCastle.Asn1;
using Org.BouncyCastle.Asn1.Pkcs;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Generators;
using Org.BouncyCastle.Crypto.Operators;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Crypto.Prng;
using Org.BouncyCastle.Math;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Pkcs;
using Org.BouncyCastle.Security;

namespace PseudoDynamic.Terraform.Plugin.Sdk.Kestrel
{
    internal static class Certificate
    {
        private readonly static SecureRandom secureRandom = new SecureRandom(new CryptoApiRandomGenerator());

        internal static X509Certificate2 GenerateSelfSignedCertificate(
            Org.BouncyCastle.X509.X509Certificate bouncyCastleX509,
            AsymmetricKeyParameter subjectPrivate)
        {
            var x509 = new X509Certificate2(bouncyCastleX509.GetEncoded(), default(string), X509KeyStorageFlags.Exportable);
            var subjectPrivateInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(subjectPrivate);
            var subjectPrivateAsn = (Asn1Sequence)Asn1Object.FromByteArray(subjectPrivateInfo.ParsePrivateKey().GetDerEncoded());

            if (subjectPrivateAsn.Count != 9) {
                throw new PemException("Invalid RSA private key");
            }

            var subjectPrivateRsa = RsaPrivateKeyStructure.GetInstance(subjectPrivateAsn);

            var subjectPrivateRsaCertParameters = new RsaPrivateCrtKeyParameters(
                subjectPrivateRsa.Modulus,
                subjectPrivateRsa.PublicExponent,
                subjectPrivateRsa.PrivateExponent,
                subjectPrivateRsa.Prime1,
                subjectPrivateRsa.Prime2,
                subjectPrivateRsa.Exponent1,
                subjectPrivateRsa.Exponent2,
                subjectPrivateRsa.Coefficient);

            var subjectPrivateRsaParameters = DotNetUtilities.ToRSAParameters(subjectPrivateRsaCertParameters);

            var rsa = RSA.Create();
            rsa.ImportParameters(subjectPrivateRsaParameters);

            // https://github.com/dotnet/runtime/issues/23749
            var exportableCertificate = x509.CopyWithPrivateKey(rsa);
            return new X509Certificate2(exportableCertificate.Export(X509ContentType.Pkcs12));
        }

        internal static class BouncyCastle
        {
            internal static AsymmetricCipherKeyPair GenerateRsaKeyPair(int length)
            {
                var keygenParam = new KeyGenerationParameters(secureRandom, length);
                var keyGenerator = new RsaKeyPairGenerator();
                keyGenerator.Init(keygenParam);
                return keyGenerator.GenerateKeyPair();
            }

            internal static Org.BouncyCastle.X509.X509Certificate GenerateSelfSignedCertificate(
                X509Name issuer,
                X509Name subject,
                AsymmetricKeyParameter issuerPrivate,
                AsymmetricKeyParameter subjectPublic,
                GeneralNames? subjectAltName)
            {
                var signatureFactory = new Asn1SignatureFactory(PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString(), issuerPrivate);
                var certificateGenerator = new Org.BouncyCastle.X509.X509V3CertificateGenerator();
                certificateGenerator.SetIssuerDN(issuer);
                certificateGenerator.SetSubjectDN(subject);

                if (subjectAltName != null) {
                    certificateGenerator.AddExtension(X509Extensions.SubjectAlternativeName, critical: false, subjectAltName);
                }

                var randomSerialNumber = new BigInteger(sizeInBits: 128, secureRandom); // 128 bit
                certificateGenerator.SetSerialNumber(randomSerialNumber);

                var now = DateTime.UtcNow;
                certificateGenerator.SetNotBefore(now);
                certificateGenerator.SetNotAfter(now.AddYears(1));

                certificateGenerator.SetPublicKey(subjectPublic);
                return certificateGenerator.Generate(signatureFactory);
            }

            internal static bool ValidateSelfSignedCertificate(Org.BouncyCastle.X509.X509Certificate certificate, ICipherParameters publicKey)
            {
                certificate.CheckValidity(DateTime.UtcNow);
                var tbsCertificate = certificate.GetTbsCertificate();
                var signature = certificate.GetSignature();

                var signer = SignerUtilities.GetSigner(certificate.SigAlgName);
                signer.Init(false, publicKey);
                signer.BlockUpdate(tbsCertificate, 0, tbsCertificate.Length);
                return signer.VerifySignature(signature);
            }
        }
    }
}
