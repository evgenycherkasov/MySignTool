using System;
using System.Collections.Generic;
using System.Text;

namespace MySignTool.Models.Interfaces
{
    public interface ISignature
    {
        public string Name { get; }
        public bool VerifySignature(byte[] hash, byte[] signature, IKey Key);

        public byte[] Sign(IKey key, byte[] hash);
    }
}
