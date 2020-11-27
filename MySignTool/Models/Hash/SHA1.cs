using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MySignTool.Models.Hash
{
    public static class SHA1
    {

        public static string GetHash(byte[] inputText)
        {
            using var sha1 = System.Security.Cryptography.SHA1.Create();
            byte[] inputBytes = inputText;
            byte[] hash = sha1.ComputeHash(inputBytes);
            return BitConverter.ToString(hash).Replace("-", String.Empty);
        }

        public static byte[] GetHash(string path)
        {
            using var sha1 = System.Security.Cryptography.SHA1.Create();
            using var stream = File.OpenRead(path);
            byte[] hash = sha1.ComputeHash(stream);
            return hash;
        }
    }
}
