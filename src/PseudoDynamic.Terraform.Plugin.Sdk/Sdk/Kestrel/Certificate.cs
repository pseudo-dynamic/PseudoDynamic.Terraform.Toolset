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
        private readonly static SecureRandom secureRandom = new(new CryptoApiRandomGenerator());

        internal static X509Certificate2 GenerateSelfSignedCertificate(
            Org.BouncyCastle.X509.X509Certificate bouncyCastleX509,
            AsymmetricKeyParameter subjectPrivate)
        {
            X509Certificate2 x509 = new(bouncyCastleX509.GetEncoded(), default(string), X509KeyStorageFlags.Exportable);
            PrivateKeyInfo subjectPrivateInfo = PrivateKeyInfoFactory.CreatePrivateKeyInfo(subjectPrivate);
            Asn1Sequence subjectPrivateAsn = (Asn1Sequence)Asn1Object.FromByteArray(subjectPrivateInfo.ParsePrivateKey().GetDerEncoded());

            if (subjectPrivateAsn.Count != 9) {
                throw new PemException("Invalid RSA private key");
            }

            RsaPrivateKeyStructure subjectPrivateRsa = RsaPrivateKeyStructure.GetInstance(subjectPrivateAsn);

            RsaPrivateCrtKeyParameters subjectPrivateRsaCertParameters = new(
                subjectPrivateRsa.Modulus,
                subjectPrivateRsa.PublicExponent,
                subjectPrivateRsa.PrivateExponent,
                subjectPrivateRsa.Prime1,
                subjectPrivateRsa.Prime2,
                subjectPrivateRsa.Exponent1,
                subjectPrivateRsa.Exponent2,
                subjectPrivateRsa.Coefficient);

            RSAParameters subjectPrivateRsaParameters = DotNetUtilities.ToRSAParameters(subjectPrivateRsaCertParameters);

            RSA rsa = RSA.Create();
            rsa.ImportParameters(subjectPrivateRsaParameters);

            // https://github.com/dotnet/runtime/issues/23749
            X509Certificate2 exportableCertificate = x509.CopyWithPrivateKey(rsa);
            return new X509Certificate2(exportableCertificate.Export(X509ContentType.Pkcs12));
        }

        internal static class BouncyCastle
        {
            internal static AsymmetricCipherKeyPair GenerateRsaKeyPair(int length)
            {
                KeyGenerationParameters keygenParam = new(secureRandom, length);
                RsaKeyPairGenerator keyGenerator = new();
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
                Asn1SignatureFactory signatureFactory = new(PkcsObjectIdentifiers.Sha256WithRsaEncryption.ToString(), issuerPrivate);
                Org.BouncyCastle.X509.X509V3CertificateGenerator certificateGenerator = new();
                certificateGenerator.SetIssuerDN(issuer);
                certificateGenerator.SetSubjectDN(subject);

                if (subjectAltName != null) {
                    certificateGenerator.AddExtension(X509Extensions.SubjectAlternativeName, critical: false, subjectAltName);
                }

                BigInteger randomSerialNumber = new(sizeInBits: 128, secureRandom); // 128 bit
                certificateGenerator.SetSerialNumber(randomSerialNumber);

                DateTime now = DateTime.UtcNow;
                certificateGenerator.SetNotBefore(now);
                certificateGenerator.SetNotAfter(now.AddYears(1));

                certificateGenerator.SetPublicKey(subjectPublic);
                return certificateGenerator.Generate(signatureFactory);
            }

            internal static bool ValidateSelfSignedCertificate(Org.BouncyCastle.X509.X509Certificate certificate, ICipherParameters publicKey)
            {
                certificate.CheckValidity(DateTime.UtcNow);
                byte[] tbsCertificate = certificate.GetTbsCertificate();
                byte[] signature = certificate.GetSignature();

                ISigner signer = SignerUtilities.GetSigner(certificate.SigAlgName);
                signer.Init(false, publicKey);
                signer.BlockUpdate(tbsCertificate, 0, tbsCertificate.Length);
                return signer.VerifySignature(signature);
            }
        }
    }
}
