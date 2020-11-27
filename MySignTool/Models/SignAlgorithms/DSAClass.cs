using MySignTool.Models.Interfaces;
using MySignTool.Models.Keys;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;
using MySignTool.Helpers;
using MySignTool.Helpers.Extensions;

namespace MySignTool.Models.SignAlgorithms
{
    public class DSAClass : ISignature
    {
        public string Name => "DSA";

        public byte[] Sign(IKey Key, byte[] hash)
        {
            byte[] hashBytes = hash;
            if (Key.IsKeyValid(isSigning: true))
            {
                List<byte> result = new List<byte>();

                string[] generalList = Key.GeneralParameter.Split(" ");

                BigInteger p = BigInteger.Parse(generalList[0]);
                BigInteger q = BigInteger.Parse(generalList[1]);
                BigInteger g = BigInteger.Parse(generalList[2]);
                BigInteger x = BigInteger.Parse(Key.SecretParameter);

                BigInteger k = BigInteger.Zero;
                BigInteger r = BigInteger.Zero;

                do
                {
                    k = NumMethodsClass.GenerateBigInteger(1, q - 1);
                    BigInteger temp = BigInteger.ModPow(g, k, p);
                    BigInteger.DivRem(temp, q, out r);
                } while (r == 0);

                int countOfBytes = hashBytes.Length; // calculate count of bytes in block

                byte[] block = new byte[countOfBytes + 1];

                Buffer.BlockCopy(hashBytes, 0, block, 0, countOfBytes);

                BigInteger s = BigInteger.Zero;

                BigInteger.DivRem(k.Inverse(q) * (new BigInteger(block) + x * r), q, out s);

                if (s == BigInteger.Zero)
                {
                    throw new ApplicationException("Generate keys again. Invalid keys");
                }

                byte[] tempResult = s.ToByteArray();

                byte[] packingUnit = new byte[hashBytes.Length];

                Buffer.BlockCopy(tempResult, 0, packingUnit, 0, Math.Min(tempResult.Length, hashBytes.Length)); //copy to packing block from result of ciphering

                result.AddRange(packingUnit);
                
                result.AddRange(r.ToByteArray());

                return result.ToArray();
            }
            else
            {
                throw new ApplicationException("Key is not valid");
            }
        }

        public bool VerifySignature(byte[] hash, byte[] signature, IKey Key)
        {
            if (!Key.IsKeyValid(isSigning: true))
            {
                throw new ApplicationException("Key is not valid");
            }
            try
            {
                byte[] hashBytes = hash;
                byte[] signatureBytes = signature;
                bool firstBlock = true;

                string[] generalList = Key.GeneralParameter.Split(" ");

                BigInteger p = BigInteger.Parse(generalList[0]);
                BigInteger q = BigInteger.Parse(generalList[1]);
                BigInteger g = BigInteger.Parse(generalList[2]);
                BigInteger y = BigInteger.Parse(Key.OpenParameter);

                BigInteger u1 = BigInteger.Zero;
                BigInteger u2 = BigInteger.Zero;
                BigInteger w = BigInteger.Zero;
                BigInteger hashInt = new BigInteger(hashBytes);

                for (int i = 0; i < hashBytes.Length * 2; i += hashBytes.Length)
                {
                    int countOfBytes = hashBytes.Length; // calculate count of bytes in block

                    byte[] block = new byte[countOfBytes + 1];

                    Buffer.BlockCopy(signatureBytes, i, block, 0, countOfBytes);

                    if (firstBlock)
                    {
                        BigInteger sInt = new BigInteger(block);
                        w = sInt.Inverse(q);
                        BigInteger.DivRem(hashInt * w, q, out u1);
                    }
                    else
                    {
                        BigInteger r = new BigInteger(block);
                        BigInteger.DivRem(r * w, q, out u2);

                        BigInteger fArg = BigInteger.ModPow(g, u1, p);
                        BigInteger sArg = BigInteger.ModPow(y, u2, p);
                        BigInteger tempResult;
                        BigInteger.DivRem(fArg * sArg, p, out tempResult);

                        BigInteger u = BigInteger.Zero;
                        BigInteger.DivRem(tempResult, q, out u);
                        if (u == r)
                        {
                            return true;
                        }
                        else
                        {
                            return false;
                        }
                    }
                    firstBlock = false;
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
