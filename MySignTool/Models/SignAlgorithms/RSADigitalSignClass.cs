using MySignTool.Models.Interfaces;
using MySignTool.Models.Keys;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using MySignTool.Converters;
namespace MySignTool.Models.SignAlgorithms
{
    class RSADigitalSignClass : ISignature
    {
        public string Name => "RSA DS";

        public string Sign(IKey Key, string hash)
        {
            byte[] hashBytes = Encoding.Default.GetBytes(hash);
            byte[] signature = CalculateRSA(hashBytes, Key as RsaKey);
            return Encoding.Default.GetString(signature);
        }

        public bool VerifySignature(string hash, string signature, IKey Key)
        {
            try
            {
                byte[] hashBytes = Encoding.Default.GetBytes(hash);
                byte[] signatureBytes = Encoding.Default.GetBytes(signature);
                byte[] result = CalculateRSA(signatureBytes, Key as RsaKey, false);
                string resultHash = Encoding.Default.GetString(result);
                if (resultHash == hash)
                {
                    return true;
                }
                return false;
            }
            catch (ApplicationException e)
            {
                throw e;
            }
        }

        private byte[] CalculateRSA(byte[] input, RsaKey Key, bool isSigning = true)
        {
            List<byte> result = new List<byte>(input.Length);

            int readingSize = isSigning ? Key.ReadingSize : Key.PackingSize;
            int packingSize = isSigning ? Key.PackingSize : Key.ReadingSize;

            for (int i = 0; i < input.Length; i += readingSize)
            {
                int countOfBytes = Math.Min(readingSize, input.Length - i); // calculate count of bytes in block

                byte[] block = new byte[countOfBytes + 1];

                Buffer.BlockCopy(input, i, block, 0, countOfBytes);

                byte[] tempResult = ModifyMessage(block, Key, isSigning).ToByteArray();

                byte[] packingUnit = new byte[packingSize];

                Buffer.BlockCopy(tempResult, 0, packingUnit, 0, Math.Min(packingUnit.Length, tempResult.Length)); //copy to packing block from result of ciphering

                result.AddRange(packingUnit);
            }

            return result.ToArray();
        }

        private BigInteger ModifyMessage(byte[] text, RsaKey key, bool isSigning)
        {
            BigInteger IntText = new BigInteger(text);

            BigInteger exp;
            if (isSigning)
            {
                exp = BigInteger.Parse(key.SecretParameter);
            } 
            else
            {
                exp = BigInteger.Parse(key.OpenParameter);
            }

            BigInteger module = BigInteger.Parse(key.GeneralParameter);

            BigInteger result = BigInteger.ModPow(IntText, exp, module);

            return result;
        }
    }
}
