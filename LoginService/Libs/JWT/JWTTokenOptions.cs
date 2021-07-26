using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace LoginService.Libs.JWT
{
    public class JWTTokenOptions
    {
        /// <summary>
        /// 受众
        /// </summary>
        public string Audience { get; set; }
        /// <summary>
        /// 发行人
        /// </summary>
        public string Issuer { get; set; }
        /// <summary>
        /// 过期时间，单位秒
        /// </summary>
        public double Expires { get; set; }

        #region 对称加密
        /// <summary>
        /// 对称加密key
        /// </summary>
        public string SecurityKey { get; set; }
        #endregion

        #region 非对称加密
        public string PrivateKeyFilename { get; set; }
        public string PublicKeyFilename { get; set; }
        #endregion
    }
}
