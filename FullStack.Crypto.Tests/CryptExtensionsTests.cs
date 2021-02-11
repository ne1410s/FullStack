using System.IO;
using System.Linq;
using System.Security.Cryptography;
using Xunit;

namespace FullStack.Crypto.Tests
{
    public class CryptExtensionsTests
    {
        private static readonly byte[] TestKey = new byte[] { 3, 44, 201, 0, 6 };

        [Fact]
        public void CryptoE2E_Authless_Ok()
        {
            var fileName = "test.txt";
            var plainText = "hello world";
            File.WriteAllText(fileName, plainText);
            var file = new FileInfo(fileName);
            file.Encrypt(TestKey);
            var cipherText = File.ReadAllText(file.FullName);

            Assert.False(File.Exists(fileName));
            Assert.NotEqual(fileName, file.Name);
            Assert.NotEqual(plainText, cipherText);

            using (var decStr = File.OpenWrite(fileName))
            {
                file.Decrypt(TestKey, decStr);
            }
             
            var plainAgain = File.ReadAllText(fileName);

            Assert.Equal(plainText, plainAgain);
            file.Delete();
            File.Delete(fileName);
        }

        [Fact]
        public void CryptoE2E_WithAuth_Ok()
        {
            var fileName = "test.txt";
            var macFile = "test.txt.mac";
            var plainText = "hello world";
            File.WriteAllText(fileName, plainText);
            var file = new FileInfo(fileName);
            using (var mac = File.OpenWrite(macFile))
            {
                file.Encrypt(TestKey, mac: mac);
            }

            var cipherText = File.ReadAllText(file.FullName);

            Assert.False(File.Exists(fileName));
            Assert.NotEqual(fileName, file.Name);
            Assert.NotEqual(plainText, cipherText);

            using (var mac = File.OpenRead(macFile))
            using (var decStr = File.OpenWrite(fileName))
            {
                file.Decrypt(TestKey, decStr, mac: mac);
            }

            var plainAgain = File.ReadAllText(fileName);

            Assert.Equal(plainText, plainAgain);
            file.Delete();
            File.Delete(fileName);
            File.Delete(macFile);
        }

        [Fact]
        public void Encrypt_Repeat_UniqueCipherSameName()
        {
            var fileName = "test.txt";
            File.WriteAllText(fileName, "some text");
            var file = new FileInfo(fileName);
            file.Encrypt(TestKey);
            var fileName1 = file.FullName;
            var result1 = File.ReadAllText(fileName1);
            File.WriteAllText(fileName1, "some text");
            file.Encrypt(TestKey);
            var fileName2 = file.FullName;
            var result2 = File.ReadAllText(fileName2);

            Assert.Equal(fileName1, fileName2);
            Assert.NotEqual(result1, result2);

            file.Delete();
            File.Delete(fileName);
            File.Delete(fileName1);
            File.Delete(fileName2);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public void Decrypt_AuthlessBadKey_Mismatch(bool useBadKey, bool expectMatch)
        {
            var fileName = "test.txt";
            var plainText = "hello world";
            File.WriteAllText(fileName, plainText);
            var file = new FileInfo(fileName);
            file.Encrypt(TestKey);
            var decKey = useBadKey ? TestKey.Append<byte>(33).ToArray() : TestKey;
            using (var decStr = File.OpenWrite(fileName))
            {
                file.Decrypt(decKey, decStr);
            }

            var plainAgain = File.ReadAllText(fileName);
            Assert.Equal(expectMatch, plainText == plainAgain);
            file.Delete();
            File.Delete(fileName);
        }

        [Fact]
        public void Decrypt_WithBadAuth_Error()
        {
            var fileName = "test.txt";
            var macFile = "test.txt.mac";
            var plainText = "hello world";
            File.WriteAllText(fileName, plainText);
            var file = new FileInfo(fileName);
            using (var mac = File.OpenWrite(macFile))
            {
                file.Encrypt(TestKey, mac: mac);
            }

            File.WriteAllText(macFile, "nonsense");

            using (var mac = File.OpenRead(macFile))
            using (var decStr = File.OpenWrite(fileName))
            {
                Assert.Throws<CryptographicException>(() =>
                {
                    file.Decrypt(TestKey, decStr, mac: mac);
                });
            }

            file.Delete();
            File.Delete(fileName);
            File.Delete(macFile);
        }
    }
}
