using MySignTool.Helpers;
using MySignTool.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Text;

namespace MySignTool.Models.Keys
{
    public static class DHKey
    {
        public struct DHPublicParams
        {
            public BigInteger Modulus;
            public BigInteger Generator;
        }
        public static DHPublicParams Generate(int decimalNubmerCount)
        {
            BigInteger min = BigInteger.Pow(10, decimalNubmerCount);
            BigInteger max = BigInteger.Pow(10, decimalNubmerCount + 1);

            BigInteger p;
            BigInteger q;

            while (true)
            {
                q = NumMethodsClass.GenerateBigInteger(min, max);
                if (q.IsPrime())
                {
                    p = (q << 1) + BigInteger.One;

                    if (p.IsPrime())
                    {
                        break;
                    }
                }
            }


            BigInteger pMinusOne = p - 1;
            BigInteger g;

            do
            {
                g = NumMethodsClass.GenerateBigInteger(2, pMinusOne);
            } while (BigInteger.ModPow(g, 2, p) == BigInteger.One
                  || BigInteger.ModPow(g, q, p) == BigInteger.One);

            DHPublicParams dHPublic;
            dHPublic.Modulus = p;
            dHPublic.Generator = g;
            return dHPublic;
        }

        public static BigInteger CalculateKeysByPowm(BigInteger value, BigInteger exp, BigInteger mod)
        {
            return BigInteger.ModPow(value, exp, mod);
        }
    }
}
