using MySignTool.Models.Keys;
using System;
using System.Collections.Generic;
using System.Text;

namespace MySignTool.Models.Interfaces
{
    public interface IKey
    {
        public string Name { get; }
        public string GeneralParameter { get; set; }
        public string OpenParameter { get; set; }
        public string SecretParameter { get; set; }
        public int ReadingSize { get; }
        public int PackingSize { get; }
        public void GenerateKey();

        public string LoadKey(GeneralKeyType key);
        public bool IsKeyEmpty();

        public bool IsKeyValid(bool isSigning);
    }
}
