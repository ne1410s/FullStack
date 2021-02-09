using System.IO;
using Xunit;

namespace FullStack.Crypto.Tests
{
    public class CryptExtensionsTests
    {
        private static readonly byte[] TestKey = new byte[] { 3, 44, 201, 0, 6 };

        [Fact]
        public void Encrypt_Ok()
        {
            var fi = new FileInfo(@"C:\temp\1.flv");
            fi.Encrypt(TestKey);
        }

        [Fact]
        public void Decrypt_Ok()
        {
            var fi = new FileInfo(@"C:\temp\66dfea4105cf5744e7955d9bd41e2f377c3510da605905f0076e31e18c3858a2.flv");
            using var fs = File.OpenWrite(@"c:\temp\1.flv");
            fi.Decrypt(TestKey, fs);
        }
    }
}
