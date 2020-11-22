using System;
using System.Collections.Generic;
using System.Text;

namespace MySignTool.Models.Interfaces
{
    public interface ISignature
    {
        public string Name { get; }
        public bool VerifySignature(string hash, string signature, IKey Key);

        public string Sign(IKey key, string hash);
    }
}
