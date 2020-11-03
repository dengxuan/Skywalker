using System;
using System.Collections.Generic;
using System.Text;

namespace Skywalker.Extensions.Security.Cryptography.Abstractions
{
    public interface ICrypter
    {
        byte[] Encrypt(byte[] bytes);

        byte[] Decrypt(byte[] bytes);
    }
}