using MySignTool.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using MySignTool.Helpers;
using System.Numerics;
using MySignTool.Helpers.Extensions;
using System.Text.Json.Serialization;

namespace MySignTool.Models.Keys
{
    public class RsaKey : IKey
    {
        private string _modulus = default;

        private string _openExponent = default;

        private string _secretExponent = default;

        private int _readingSize = default;

        private int _packingSize = default;

        private string _phi = default;

        public string GeneralParameter
        {
            get
            {
                return _modulus;
            }
            set
            {
                _modulus = value;
            }
        }

        public string Phi => _phi;
        public string OpenParameter
        {
            get
            {
                return _openExponent;
            }
            set
            {
                _openExponent = value;
            }
        }

        public string SecretParameter
        {
            get
            {
                return _secretExponent;
            }
            set
            {
                _secretExponent = value;
            }
        }

        public string Name => "RSA DS";

        public int ReadingSize => _readingSize;

        public int PackingSize => _packingSize;
        public void GenerateKey()
        {
            try
            {
                BigInteger p = NumMethodsClass.GeneratePrime(95);
                BigInteger q = NumMethodsClass.GeneratePrime(98);
                BigInteger modulus = p * q;
                BigInteger phi = (p - 1) * (q - 1);
                BigInteger pubExp = NumMethodsClass.GetPublicExponent(p.ToString(), q.ToString()); ;
                BigInteger privateExponentInt = pubExp.Inverse(phi);
                if (privateExponentInt == 0)
                {
                    throw new ApplicationException();
                }

                _modulus = modulus.ToString();
                _phi = phi.ToString();
                _openExponent = pubExp.ToString();
                _secretExponent = privateExponentInt.ToString();
                RegenerateSizeParameters();
            } 
            catch (Exception)
            {
                throw new ApplicationException("Error via generate key");
            }
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
            int keySize = NumMethodsClass.GetModuleSize(BigInteger.Parse(GeneralParameter));
            _readingSize = keySize - 1;
            _packingSize = keySize;
        }
    }
}
