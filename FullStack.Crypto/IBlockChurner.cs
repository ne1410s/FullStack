using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FullStack.Crypto
{
    public interface IBlockChurner : IDisposable
    {
        byte[] ChurnBlock(
            byte[] sourceBuffer,
            byte[] tagBuffer,
            byte[] counter,
            bool encryptOrAuthlessDecrypt = true);
    }
}
