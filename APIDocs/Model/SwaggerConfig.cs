using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace APIDocs.Model
{
    /// <summary>
    /// Swagger 配置项
    /// </summary>
    public class SwaggerConfig
    {
        public string Version { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string AuthorizationUrl { get; set; }
        public List<string> ApiScopes { get; set; }
        public List<string> XmlComments { get; set; }
        public string ClientID { get; set; }
    }
}
