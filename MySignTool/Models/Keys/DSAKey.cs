using MySignTool.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using MySignTool.Helpers;
using MySignTool.Helpers.Extensions;

namespace MySignTool.Models.Keys
{
    public class DSAKey : IKey
    {
        private string _groupParams;
        private string _paramY;
        private string _paramX;

        private int _readingSize;
        private int _packingSize;
        public string Name => "DSA";

        public string GeneralParameter
        {
            get
            {
                return _groupParams;
            }
            set
            {
                _groupParams = value;
            }
        }
        public string OpenParameter
        {
            get
            {
                return _paramY;
            }
            set
            {
                _paramY = value;
            }
        }
        public string SecretParameter
        {
            get
            {
                return _paramX;
            }
            set
            {
                _paramX = value;
            }
        }
        public int ReadingSize => _readingSize;

        public int PackingSize => _packingSize;

        public void GenerateKey()
        {
            BigInteger q = NumMethodsClass.GeneratePrimeByPython(160);
            BigInteger p;
            BigInteger b;
            do
            {
                b = NumMethodsClass.GenerateBigInteger((1024 - 160) / 8);
                p = b * q + 1;
            } while (!p.IsPrime()); 

            BigInteger g = BigInteger.Zero;

            for (BigInteger i = 2; i < p - 1; i++)
            {
                g = BigInteger.ModPow(i, b, p);
                if (g > 1)
                {
                    break;
                }
            }

            if (g == BigInteger.Zero)
            {
                throw new ApplicationException("Try generate again, error via generating G");
            }

            BigInteger x = NumMethodsClass.GenerateBigInteger(2, q - 1);
            BigInteger y = BigInteger.ModPow(g, x, p);
            GeneralParameter = $"{p.ToString()} {q.ToString()} {g.ToString()}";
            OpenParameter = y.ToString();
            SecretParameter = x.ToString();
            RegenerateSizeParameters();
        }

        public bool IsKeyEmpty()
        {
            return String.IsNullOrEmpty(GeneralParameter);
        }

        public bool IsKeyValid(bool isSigning)
        {
            switch (isSigning)
            {
                case true:
                    return !(String.IsNullOrEmpty(GeneralParameter) || String.IsNullOrEmpty(SecretParameter));
                case false:
                    return !(String.IsNullOrEmpty(GeneralParameter) || String.IsNullOrEmpty(OpenParameter));
            }
        }
        public string LoadKey(GeneralKeyType key)
        {
            try
            {
                switch (key.Type)
                {
                    case "Public":
                        OpenParameter = key.Param;
                        GeneralParameter = key.GeneralParameter;
                        break;
                    case "Private":
                        SecretParameter = key.Param;
                        GeneralParameter = key.GeneralParameter;
                        break;
                    default:
                        throw new ApplicationException();
                }
                RegenerateSizeParameters();
                return key.Type;
            }
            catch
            {
                throw new ApplicationException("Failed to load the key, perhaps the key does not match the selected algorithm");
            }
        }

        private void RegenerateSizeParameters()
        {
            string modulus = GeneralParameter.Split(" ")[0];
            int keySize = NumMethodsClass.GetModuleSize(BigInteger.Parse(modulus));
            _readingSize = keySize - 1;
            _packingSize = keySize;
        }
    }
}
