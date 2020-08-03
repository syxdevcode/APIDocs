using APIDocs.Helper;
using APIDocs.JsonConvert;
using APIDocs.Model;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using System.IO;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;

namespace APIDocs
{
    public class Startup
    {
        public Startup(IWebHostEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true)
                .AddJsonFile("swagger.config.json", optional: true)
                .AddEnvironmentVariables();
            Configuration = builder.Build();

            CurrentEnvironment = env;
        }

        public IConfiguration Configuration { get; }

        private IWebHostEnvironment CurrentEnvironment { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddSingleton(typeof(SpireDocHelper));

            services.AddControllers().AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    options.SerializerSettings.DateFormatString = "yyyy-MM-dd HH:mm:ss";
                })
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Encoder = JavaScriptEncoder.Create(UnicodeRanges.All);
                    options.JsonSerializerOptions.Converters.Add(new SystemTextJsonConvert.DateTimeConverter());
                    options.JsonSerializerOptions.Converters.Add(new SystemTextJsonConvert.DateTimeNullableConverter());
                }).SetCompatibilityVersion(CompatibilityVersion.Version_3_0); ;

            //模型绑定 特性验证，自定义返回格式
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.InvalidModelStateResponseFactory = actionContext =>
                {
                    //获取验证失败的模型字段
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .Select(e => e.Value.Errors.First().ErrorMessage)
                        .ToList();
                    var str = string.Join("|", errors);
                    //设置返回内容
                    var result = new
                    {
                        Code = 400,
                        Msg = str
                    };
                    return new BadRequestObjectResult(result);
                };
            });

            #region swagger配置

            //添加配置信息
            var swaggerConfig = Configuration.GetSection("SwaggerConfig").Get<SwaggerConfig>();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc(swaggerConfig.Version, new OpenApiInfo
                {
                    Version = swaggerConfig.Version,
                    Title = swaggerConfig.Title,
                    Description = swaggerConfig.Description,
                    //TermsOfService = "None",
                    Contact = new OpenApiContact { Name = "test", Email = "test@163.com" },
                });
                // enable openApi Annotations
                c.EnableAnnotations();

                //Set the comments path for the swagger json and ui.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                foreach (var item in swaggerConfig.XmlComments)
                {
                    var xmlPath = Path.Combine(basePath, item);
                    if (File.Exists(xmlPath))
                        c.IncludeXmlComments(xmlPath);
                }
            });

            #endregion swagger配置
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                #region swagger

                app.UseSwagger(c =>
                {
                    //c.SerializeAsV2 = true;
                });
                // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "生成接口word文档 V1"));

                #endregion swagger

                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}