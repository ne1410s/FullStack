using System;
using System.IO;
using Xunit;

namespace FullStack.Crypto.Tests
{
    public class UnitTest1
    {
        private byte[] salt = { 3, 2, 1 };
        private byte[] pass = { 12, 4, 5, 3, 2, 4, 77 };

        [Fact]
        public void Test1()
        {
            var fi = new FileInfo(@"c:\temp\1.flv.gcm.enc");
            var churnParams = FileChurnParams.KeepSource | FileChurnParams.RedoTarget;// | FileChurnParams.SubjectAuthentication;
            churnParams |= FileChurnParams.IsDecrypt;
            fi.Churn(salt, pass, churnParams);
        }

        [Fact]
        public void Test2()
        {
            var x = new byte[] { 255 };
            ByteExtensions.Increment(ref x, bigEndian: false);
            
            Assert.Equal(2, x.Length);
            Assert.Equal(0, x[0]);
            Assert.Equal(1, x[1]);
        }

        [Fact]
        public void Test3()
        {
            var x = new byte[] { 255 };
            ByteExtensions.Increment(ref x, bigEndian: true);

            Assert.Equal(2, x.Length);
            Assert.Equal(1, x[0]);
            Assert.Equal(0, x[1]);
        }
    }
}
