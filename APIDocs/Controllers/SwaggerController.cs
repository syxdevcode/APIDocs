using APIDocs.Helper;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.IO;

namespace APIDocs.Controllers
{
    [ApiController]
    [Route("api/[controller]/[action]")]
    [Produces("application/json")]
    [SwaggerTag("Swagger")]
    public class SwaggerController : ControllerBase
    {
        private readonly IWebHostEnvironment _environment;
        private readonly SpireDocHelper _spireDocHelper;
        private readonly ISwaggerProvider _swaggerGenerator;

        public SwaggerController(IWebHostEnvironment environment, SpireDocHelper spireDocHelper, ISwaggerProvider swaggerGenerator)
        {
            _environment = environment;
            _spireDocHelper = spireDocHelper;
            _swaggerGenerator = swaggerGenerator;//从ioc容器中获取swagger生成文档的对象
        }

        /// <summary>
        /// Swagger 文档导出
        /// </summary>
        /// <param name="type">.docx</param>
        /// <param name="version">v1</param>
        /// <returns></returns>
        [HttpGet]
        public FileResult ExportApiWord(string type, string version)
        {
            var model = _swaggerGenerator.GetSwagger(version);
            if (model == null)
            {
                throw new Exception("Swagger Json cannot be equal to null！");
            }
            var html = T4Helper.GeneritorSwaggerHtml($"{_environment.ContentRootPath}\\Razor\\SwaggerDoc.cshtml", model);
            
            Stream data = _spireDocHelper.SwaggerHtmlConvers(html, type, out string memi);

            return File(data, memi, $"Titan.Blog.WebAPP API文档 {version}{type}");
        }
    }
}