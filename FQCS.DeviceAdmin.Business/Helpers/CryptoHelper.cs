using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace FQCS.DeviceAdmin.Business.Helpers
{
    public static class CryptoHelper
    {
        public static string HMACSHA256(string text, string key)
        {
            byte[] textBytes = Encoding.ASCII.GetBytes(text);
            byte[] keyBytes = Encoding.ASCII.GetBytes(key);
            byte[] hashBytes;
            using (HMACSHA256 hash = new HMACSHA256(keyBytes))
                hashBytes = hash.ComputeHash(textBytes);
            return BitConverter.ToString(hashBytes);
        }
    }
}
