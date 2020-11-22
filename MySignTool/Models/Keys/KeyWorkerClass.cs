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
        internal struct Key {
            public string GeneralParameter;
            public string Param;
        }

        public static void WriteKeyToFile(IKey Key, string path)
        {
            #region SavePrivateKey

            Key privateKey = default;
            privateKey.GeneralParameter = Key.GeneralParameter;
            privateKey.Param = Key.SecretParameter;
            string keyString = JsonSerializer.Serialize<Key>(privateKey);
            string result = Base64ConverterClass.Base64Encode(keyString);
            File.WriteAllText(Path.GetDirectoryName(path) + @"\privatekey", result);

            #endregion

            #region SavePublicKey

            Key publicKey = default;
            publicKey.GeneralParameter = Key.GeneralParameter;
            publicKey.Param = Key.OpenParameter;
            keyString = JsonSerializer.Serialize<Key>(publicKey);
            result = Base64ConverterClass.Base64Encode(keyString);
            File.WriteAllText(Path.GetDirectoryName(path) + @"\publickey", result);

            #endregion
        }

        public static async void LoadKeyFromFile(string path)
        {
            try
            {
                using FileStream fs = new FileStream(path, FileMode.Open);
                Key key = await JsonSerializer.DeserializeAsync<Key>(fs);
            }
            catch
            {
                throw new ApplicationException("Ключ не валиден!");
            }
        }
    }
}
