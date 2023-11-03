using System;
using System.Security.Cryptography;

namespace SafeShare.RSA
{
    class Program
    {
        static void Main(string[] args)
        {
            var (publicKey, privateKey) = GenerateRsaKeyPair();

            Console.WriteLine("Public Key:");
            Console.WriteLine(publicKey);

            Console.WriteLine("\nPrivate Key:");
            Console.WriteLine(privateKey);
        }

        public static (string publicKey, string privateKey) GenerateRsaKeyPair()
        {
            using var rsa = new RSACryptoServiceProvider(2048); // 2048 bits is a common key size
            var publicKey = rsa.ToXmlString(false); // false indicates exporting only the public key
            var privateKey = rsa.ToXmlString(true); // true indicates exporting both the public and private keys
            return (publicKey, privateKey);
        }
    }
}