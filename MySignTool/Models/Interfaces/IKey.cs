using System;
using System.Collections.Generic;
using System.Text;

namespace MySignTool.Models.Interfaces
{
    public interface IKey
    {
        public string Name { get; }
        public string GeneralParameter { get; }
        public string OpenParameter { get; }
        public string SecretParameter { get; }
        public int ReadingSize { get; }
        public int PackingSize { get; }
        public void GenerateKey();
    }
}
