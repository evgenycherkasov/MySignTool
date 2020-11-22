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

        public string GeneralParameter => _modulus;

        [JsonIgnore]
        public string Phi => _phi;
        public string OpenParameter => _openExponent;

        public string SecretParameter => _secretExponent;

        public string Name => "RSA DS";

        [JsonIgnore]
        public int ReadingSize => _readingSize;

        [JsonIgnore]
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
                int keySize = NumMethodsClass.GetModuleSize(modulus);
                _readingSize = keySize - 1;
                _packingSize = keySize;
            } 
            catch (Exception)
            {
                throw new ApplicationException("Возникла ошибка при генерации ключа");
            }
        }
    }
}
