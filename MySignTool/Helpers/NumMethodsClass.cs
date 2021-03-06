﻿using IronPython.Hosting;
using Microsoft.Scripting.Hosting;
using MySignTool.Helpers.Extensions;
using System;
using System.Collections.Generic;
using System.Numerics;
using System.Security.Cryptography;
using System.Text;

namespace MySignTool.Helpers
{
    public static class NumMethodsClass
    {
        private static ScriptEngine _engine;
        private static ScriptScope _scope;
        private static string _pyPath = "DsaKeyGen.py";
        private static string _funcName = "getprime";
        private static dynamic _getPrime;

        public static void LoadPyScript()
        {
            _engine = Python.CreateEngine();
            _scope = _engine.CreateScope();
            ICollection<string> searchPaths = _engine.GetSearchPaths();
            searchPaths.Add(@".\Lib");
            _engine.SetSearchPaths(searchPaths);
            _engine.ExecuteFile(_pyPath, _scope);
            _getPrime = _scope.GetVariable(_funcName);
        }

        public static BigInteger GenerateBigInteger(BigInteger min, BigInteger max)
        {
            BigInteger diff = max - min;
            int byteSize = diff.ToByteArray().Length;

            byte[] newBigIntergerChunk = new byte[byteSize + 1]; //  Последний байт всегда 0х00 для положительного числа

            RNGCryptoServiceProvider rngGenerator = new RNGCryptoServiceProvider();
            rngGenerator.GetBytes(newBigIntergerChunk);

            newBigIntergerChunk[newBigIntergerChunk.Length - 1] = 0;


            BigInteger result = new BigInteger(newBigIntergerChunk);

            BigInteger.DivRem(result, diff, out result);

            result += min;

            return result;

        }

        public static BigInteger GeneratePrime(int decimalNubmerCount)
        {
            if (decimalNubmerCount < 6)
            {
                throw new ArgumentException("Слишком маленький порядок числа");
            }

            BigInteger min = BigInteger.Pow(10, decimalNubmerCount);
            BigInteger max = BigInteger.Pow(10, decimalNubmerCount + 1);

            BigInteger p;

            while (true) // Надо ограничть макс значением
            {
                p = GenerateBigInteger(min, max);
                if (p.IsPrime())
                {
                    break;
                }
            }

            return p;
        }
        public static int GetPublicExponent(string p, string q)
        {
            BigInteger NumberP = BigInteger.Parse(p);
            BigInteger NumberQ = BigInteger.Parse(q);
            BigInteger module = NumberP * NumberQ;
            if (module > 65537)
            {
                return Convert.ToInt32(65537);
            }
            return 17;
        }

        public static BigInteger GenerateBigInteger(int countOfBits)
        {
            var manager = new RNGCryptoServiceProvider();
            byte[] bytes = new byte[countOfBits + 1];
            manager.GetBytes(bytes);
            bytes[countOfBits] = 0;
            bytes[countOfBits - 1] = 0x80;
            return new BigInteger(bytes);
        }

        public static BigInteger GeneratePrimeByPython(int countOfBits)
        {
            try
            {
                dynamic result = _getPrime(countOfBits);
                return BigInteger.Parse(Convert.ToString(result));
            }
            catch
            {
                throw new ApplicationException("An error occurred while loading the algorithm!");
            }
        }

        public static int GetModuleSize(BigInteger module)
        {
            return module.ToByteArray().Length;
        }
        public static (BigInteger d, BigInteger x, BigInteger y) GCDex(BigInteger a, BigInteger b)
        {
            BigInteger x = BigInteger.Zero;
            BigInteger y = BigInteger.Zero;

            BigInteger x1 = BigInteger.Zero;
            BigInteger x2 = BigInteger.One;
            BigInteger y1 = BigInteger.One;
            BigInteger y2 = BigInteger.Zero;

            BigInteger q = BigInteger.Zero;
            BigInteger r = BigInteger.Zero;



            while (b > BigInteger.Zero)
            {
                q = BigInteger.DivRem(a, b, out r);
                x = x2 - q * x1;
                y = y2 - q * y2;

                a = b;
                b = r;

                x2 = x1;
                x1 = x;
                y2 = y1;
                y1 = y;
            }

            return (a, x2, y2);

        }
    }
}
