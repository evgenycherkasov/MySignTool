using MySignTool.Helpers;
using MySignTool.Helpers.Extensions;
using MySignTool.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MySignTool.Models.SignAlgorithms
{
    class ElGamalDigitalSignClass : ISignature
    {
        public string Name => "ElGamal-DS";

        public byte[] Sign(IKey Key, byte[] hash)
        {
            byte[] hashBytes = hash;
            List<byte> result = new List<byte>(hashBytes.Length);
            if (!Key.IsKeyValid(isSigning: true))
            {
                throw new ApplicationException("Key is not valid");
            }

            try
            {
                int readSize = Key.ReadingSize;
                int writeSize = Key.PackingSize;

                BigInteger k;
                string[] generalParameters = Key.GeneralParameter.Split(" ");
                BigInteger modulus = BigInteger.Parse(generalParameters[0]);
                BigInteger generator = BigInteger.Parse(generalParameters[1]);
                BigInteger secret = BigInteger.Parse(Key.SecretParameter);
                do
                {
                    k = NumMethodsClass.GenerateBigInteger(2, modulus - 3);
                } while (BigInteger.GreatestCommonDivisor(k, modulus - 1) != 1);

                BigInteger r = BigInteger.ModPow(generator, k, modulus);

                byte[] packedBlock = new byte[writeSize];

                byte[] rBlock = r.ToByteArray(false);
                Buffer.BlockCopy(rBlock, 0, packedBlock, 0, rBlock.Length);
                result.AddRange(packedBlock);

                for (int currentByte = 0; currentByte < hashBytes.Length; currentByte += readSize)
                {
                    int byteCopyCount = Math.Min(readSize, hashBytes.Length - currentByte);
                    byte[] currentBlock = new byte[byteCopyCount + 1]; // Добавлен 0х00 чтобы число было положительным 

                    Buffer.BlockCopy(hashBytes, currentByte, currentBlock, 0, byteCopyCount);
                    BigInteger h = new BigInteger(currentBlock);

                    BigInteger temp;
                    BigInteger.DivRem(secret * r, modulus - 1, out temp);


                    BigInteger u;
                    BigInteger.DivRem(h - temp + (modulus - 1), modulus - 1, out u);

                    BigInteger s;
                    BigInteger.DivRem(k.Inverse(modulus - 1) * u, modulus - 1, out s);

                    byte[] cipherBlock = s.ToByteArray(false);
                    packedBlock = new byte[writeSize];
                    Buffer.BlockCopy(cipherBlock, 0, packedBlock, 0, cipherBlock.Length);
                    result.AddRange(packedBlock);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }

            return result.ToArray();
        }

        public bool VerifySignature(byte[] hash, byte[] signature, IKey Key)
        {
            if (!Key.IsKeyValid(isSigning: true))
            {
                throw new ApplicationException("Key is not valid");
            }
            try
            {
                int offsetDigest = 0;
                int readSize = Key.PackingSize;
                int writeSize = Key.ReadingSize;
                byte[] hashBytes = hash;
                BigInteger publicParam = BigInteger.Parse(Key.OpenParameter);

                string[] generalParameters = Key.GeneralParameter.Split(" ");

                BigInteger modulus = BigInteger.Parse(generalParameters[0]);
                BigInteger generator = BigInteger.Parse(generalParameters[1]);

                byte[] rBlock = new byte[readSize + 1];
                Buffer.BlockCopy(signature, 0, rBlock, 0, readSize);
                BigInteger r = new BigInteger(rBlock);

                BigInteger y_r = BigInteger.ModPow(publicParam, r, modulus);

                for (int currentByte = readSize; currentByte < signature.Length; currentByte += readSize)
                {
                    int byteCopyCount = Math.Min(readSize, signature.Length - currentByte);
                    byte[] currentBlock = new byte[byteCopyCount + 1]; // Добавлен 0х00 чтобы число было положительным 

                    Buffer.BlockCopy(signature, currentByte, currentBlock, 0, byteCopyCount);
                    BigInteger s = new BigInteger(currentBlock);

                    BigInteger r_s = BigInteger.ModPow(r, s, modulus);

                    BigInteger left;
                    BigInteger.DivRem(y_r * r_s, modulus, out left);

                    byteCopyCount = Math.Min(writeSize, hashBytes.Length - offsetDigest);
                    currentBlock = new byte[byteCopyCount + 1];
                    Buffer.BlockCopy(hashBytes, offsetDigest, currentBlock, 0, byteCopyCount);
                    offsetDigest += writeSize;
                    BigInteger h = new BigInteger(currentBlock);
                    BigInteger right = BigInteger.ModPow(generator, h, modulus);
                    if (left != right)
                    {
                        return false;
                    }
                }
            }
            catch (Exception _)
            {
                return false;
            }
            return true;
        }
    }
}
