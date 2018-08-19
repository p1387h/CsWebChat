using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace CsWebChat.WpfClient.LoginModule.Services
{
    class PasswordHashService
    {
        public string HashSecureString(SecureString input)
        {
            return this.HashSecureString(input, new SHA1Managed());
        }

        public string HashSecureString(SecureString input, HashAlgorithm hasher)
        {
            var bstr = Marshal.SecureStringToBSTR(input);
            var length = Marshal.ReadInt32(bstr, -4);
            var bytes = new byte[length];

            var bytesPin = GCHandle.Alloc(bytes, GCHandleType.Pinned);
            try
            {
                Marshal.Copy(bstr, bytes, 0, length);
                Marshal.ZeroFreeBSTR(bstr);

                var hash = hasher.ComputeHash(bytes);
                var hashBase64 = Convert.ToBase64String(hash);
                return hashBase64;
            }
            finally
            {
                for (var i = 0; i < bytes.Length; i++)
                {
                    bytes[i] = 0;
                }

                bytesPin.Free();
            }
        }
    }
}
