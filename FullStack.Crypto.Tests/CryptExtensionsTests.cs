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
            var fi = new FileInfo(@"C:\temp\1.txt");
            fi.Encrypt(TestKey);
        }

        [Fact]
        public void Decrypt_Ok()
        {
            var fi = new FileInfo(@"C:\temp\437651da0e539de3c510a2c52864e53867bbc4fec0b5452acc2adb51ec85ddda.txt");
            using var fs = File.OpenWrite(@"c:\temp\1.txt");
            fi.Decrypt(TestKey, fs);
        }
    }
}
