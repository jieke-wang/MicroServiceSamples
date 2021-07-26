using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

using Newtonsoft.Json;

namespace LoginService.Libs.JWT
{
    public class RSAHelper
    {
        private static ConcurrentDictionary<string, RSAParameters> RSAParametersCache = new ConcurrentDictionary<string, RSAParameters>();

        public string PrivateKeyFilename { get; set; } = "key.json";
        public string PublicKeyFilename { get; set; } = "key.public.json";

        /// <summary>
        /// 从本地文件中读取用来签发 Token 的 RSA Key
        /// </summary>
        /// <param name="filePath">存放密钥的文件夹路径</param>
        /// <param name="withPrivate"></param>
        /// <param name="keyParameters"></param>
        /// <returns></returns>
        public bool TryGetKeyParameters(string filePath, bool withPrivate, out RSAParameters keyParameters)
        {
            string filename = withPrivate ? PrivateKeyFilename : PublicKeyFilename;
            string fileTotalPath = Path.Combine(filePath, filename);
            keyParameters = default;

            if (RSAParametersCache.TryGetValue(fileTotalPath, out keyParameters))
                return true;

            if (File.Exists(fileTotalPath) == false)
            {
                return false;
            }
            else
            {
                //keyParameters = JsonConvert.DeserializeObject<RSAParameters>(File.ReadAllText(fileTotalPath));
                keyParameters = RSAParametersCache.GetOrAdd(fileTotalPath, fileName => JsonConvert.DeserializeObject<RSAParameters>(File.ReadAllText(fileTotalPath)));
                return true;
            }
        }

        /// <summary>
        /// 生成并保存 RSA 公钥与私钥
        /// </summary>
        /// <param name="filePath">存放密钥的文件夹路径</param>
        /// <returns></returns>
        public RSAParameters GenerateAndSaveKey(string filePath, bool withPrivate = true)
        {
            RSAParameters publicKeys, privateKeys;
            using (var rsa = new RSACryptoServiceProvider(2048))//即时生成
            {
                try
                {
                    privateKeys = rsa.ExportParameters(true);
                    publicKeys = rsa.ExportParameters(false);
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
            File.WriteAllText(Path.Combine(filePath, PrivateKeyFilename), JsonConvert.SerializeObject(privateKeys));
            File.WriteAllText(Path.Combine(filePath, PublicKeyFilename), JsonConvert.SerializeObject(publicKeys));
            return withPrivate ? privateKeys : publicKeys;
        }

        public bool GetRSAParameters(string filePath, bool withPrivate, out RSAParameters keyParameters)
        {
            if (TryGetKeyParameters(filePath, withPrivate, out keyParameters)) return true;
            keyParameters = GenerateAndSaveKey(filePath, withPrivate);
            return true;
        }
    }
}
