using MySignTool.Models.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;
using MySignTool.Converters;
using System.Threading.Tasks;
namespace MySignTool.Models.Keys
{
    public static class KeyWorkerClass
    {

        public static void WriteKeyToFile(IKey Key, string path)
        {
            #region SavePrivateKey

            GeneralKeyType privateKey = new GeneralKeyType
            {
                Type = "Private",
                GeneralParameter = Key.GeneralParameter,
                Param = Key.SecretParameter
            };
            string keyString = JsonSerializer.Serialize<GeneralKeyType>(privateKey);
            string result = Base64ConverterClass.Base64Encode(keyString);
            File.WriteAllText(path + @"\privatekey", result);

            #endregion

            #region SavePublicKey

            GeneralKeyType publicKey = new GeneralKeyType
            {
                Type = "Public",
                GeneralParameter = Key.GeneralParameter,
                Param = Key.OpenParameter
            };
            keyString = JsonSerializer.Serialize(publicKey);
            result = Base64ConverterClass.Base64Encode(keyString);
            File.WriteAllText(path + @"\publickey", result);

            #endregion
        }

        public static GeneralKeyType LoadKeyFromFile(string path)
        {
            try
            {
                string content = File.ReadAllText(path);
                string decodedKey = Base64ConverterClass.Base64Decode(content);
                GeneralKeyType key = JsonSerializer.Deserialize<GeneralKeyType>(decodedKey);
                return key;
            }
            catch
            {
                throw new ApplicationException("Key is not valid!");
            }
        }
    }
}
