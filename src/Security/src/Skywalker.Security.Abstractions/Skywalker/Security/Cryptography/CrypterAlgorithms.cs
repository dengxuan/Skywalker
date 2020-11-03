using System;
using System.Collections.Generic;
using System.Text;

namespace Skywalker.Extensions.Security.Cryptography
{
    public enum CrypterAlgorithms
    {
        /* 对称算法 */
        AES,
        DES,
        TripleDES,
        RC2,
        Rijndeal,

        /* 非对称算法 */
        RSA,
        DSA,
        ECDsa,
    }
}
