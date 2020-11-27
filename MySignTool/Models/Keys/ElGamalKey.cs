using MySignTool.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using MySignTool.Helpers;
using System.Numerics;
using static MySignTool.Models.Keys.DHKey;
using MySignTool.Helpers.Extensions;

namespace MySignTool.Models.Keys
{
    class ElGamalKey : IKey
    {
        #region privateFields
        internal struct ElgamalPrivateKey
        {
            public BigInteger X;
            public BigInteger Y;
        }

        internal struct ElgamalPublicParams
        {
            public BigInteger Modulus;
            public BigInteger Generator;
        }

        private string _modulus;

        private string _generator;

        private string _openParameter;

        private string _secretParameter;

        private int _readingSize;

        private int _packingSize;

        #endregion

        public string Name => "ElGamal-DS";

        public string GeneralParameter 
        {
            set
            {
                string[] parameters = value.Split(" ");
                _modulus = parameters[0];
                _generator = parameters[1];
            }
            get
            {
                return $"{_modulus} {_generator}";
            }
        }

        public string OpenParameter
        {
            set
            {
                _openParameter = value;
            }
            get
            {
                return _openParameter;
            }
        }

        public string SecretParameter
        {
            set
            {
                _secretParameter = value;
            }
            get
            {
                return _secretParameter;
            }
        }

        public int ReadingSize => _readingSize;

        public int PackingSize => _packingSize;

        public void GenerateKey()
        {
            ElgamalPublicParams pubKey = GeneratePublicElgamalParams(100);
            _modulus = pubKey.Modulus.ToString();
            _generator = pubKey.Generator.ToString();
            ElgamalPrivateKey privateKey = GetElgamalPrivateKey(pubKey);
            _openParameter = privateKey.Y.ToString();
            _secretParameter = privateKey.X.ToString();
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

        private ElgamalPrivateKey GetElgamalPrivateKey(ElgamalPublicParams param)
        {
            ElgamalPublicParams parameters = param;
            ElgamalPrivateKey pk;
            BigInteger x;
            BigInteger y;

            x = NumMethodsClass.GenerateBigInteger(2, parameters.Modulus - 1);

            y = CalculateKeysByPowm(parameters.Generator, x, parameters.Modulus);

            pk.X = x;
            pk.Y = y;
            return pk;
        }

        private ElgamalPublicParams GeneratePublicElgamalParams(int decimalCount)
        {
            DHPublicParams dH = DHKey.Generate(decimalCount);
            ElgamalPublicParams elgamalPublicParams;
            elgamalPublicParams.Generator = dH.Generator;
            elgamalPublicParams.Modulus = dH.Modulus;
            return elgamalPublicParams;
        }

        private BigInteger CalculateKeysByPowm(BigInteger value, BigInteger exp, BigInteger mod)
        {
            return BigInteger.ModPow(value, exp, mod);
        }

        private void RegenerateSizeParameters()
        {
            int keySize = NumMethodsClass.GetModuleSize(BigInteger.Parse(_modulus));
            _readingSize = keySize - 1;
            _packingSize = keySize;
        }
    }
}
