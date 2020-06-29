using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using _24hplusdotnetcore.BatchJob;
using _24hplusdotnetcore.Middleware;
using _24hplusdotnetcore.ModelDtos;
using _24hplusdotnetcore.Models;
using _24hplusdotnetcore.Services;
using _24hplusdotnetcore.Services.CRM;
using _24hplusdotnetcore.Services.GCC;
using _24hplusdotnetcore.Services.MC;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Refit;

namespace _24hplusdotnetcore
{
    public class Startup
    {
        readonly string AllowSpecificOrigins = "_allowSpecificOrigins";
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<MongoDbConnection>(Configuration.GetSection(nameof(MongoDbConnection)));

            services.AddCors(options =>
            {
                options.AddPolicy(name: AllowSpecificOrigins,
                              builder =>
                              {
                                  builder.WithOrigins("*")
                                         .AllowAnyHeader()
                                         .AllowAnyMethod();
                              });
            });

            services.AddControllers();
            services.AddDirectoryBrowser();

            #region "Adding Singleton"
            services.AddSingleton<IMongoDbConnection>(sp => sp.GetRequiredService<IOptions<MongoDbConnection>>().Value);
            services.AddSingleton<DemoService>();
            services.AddSingleton<UserServices>();
            services.AddSingleton<RoleServices>();
            services.AddSingleton<CustomerServices>();
            services.AddSingleton<ProductCategoryServices>();
            // services.AddSingleton<CipherServices>();
            services.AddSingleton<AuthServices>();
            services.AddSingleton<AuthRefreshServices>();
            services.AddSingleton<UserLoginServices>();
            services.AddSingleton<DocumentCategoryServices>();
            services.AddSingleton<UserRoleServices>();
            services.AddSingleton<MobileVersionServices>();
            services.AddSingleton<LoaiCVServices>();
            services.AddSingleton<ProductServices>();
            services.AddSingleton<FileUploadServices>();
            services.AddSingleton<PaymentServices>();
            services.AddSingleton<CheckInfoServices>();
            services.AddSingleton<NotificationServices>();
            services.AddSingleton<Services.MC.MCService>();
            services.AddSingleton<ConfigServices>();
            services.AddSingleton<CRMServices>();
            services.AddSingleton<DataCRMProcessingServices>();
            services.AddSingleton<MCService>();
            services.AddSingleton<DataMCProcessingServices>();

            // GCC Service
            services.AddSingleton<GCCService>();
            services.AddSingleton<GCCProductService>();
            services.AddSingleton<GCCMotoProgramService>();
            services.AddSingleton<GCCMotoService>();

            //Add batchjob
            services.AddSingleton<IHostedService, AddNewCustomerFromCRM>();
            services.AddSingleton<IHostedService, PushCustomerToCRM>();
            //services.AddSingleton<IHostedService, PushDataToMC>();
            #endregion
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = Configuration["Jwt:Issuer"],
                    ValidAudience = Configuration["Jwt:Issuer"],
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Jwt:Key"]))
                };
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "JWT"
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        new List<string>()
                    }
                });
            });
            services.AddMvcCore().AddNewtonsoftJson();
            ConfigureMCRestClient(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "My API V1");
            });
            app.UseStaticFiles(new StaticFileOptions
            {
                FileProvider = new PhysicalFileProvider(
            Path.Combine(Directory.GetCurrentDirectory(), "mobileapp")),
                RequestPath = "/mobileapp"
            });
            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseCors(AllowSpecificOrigins);

            app.UseAuthorization();

            app.RequestAPIMiddleware();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }

        private void ConfigureMCRestClient(IServiceCollection services)
        {
            services.AddHttpClient("restLoginService")
                .ConfigureHttpClient((serviceProvider, conf) =>
                {
                    conf.BaseAddress = new Uri("https://uat-mfs-v2.mcredit.com.vn:8043");
                    conf.DefaultRequestHeaders.Add("x-security", Common.Config.CredMC_Security_Key);
                });

            services.AddRefitClient<IRestMCService>()
                .ConfigureHttpClient((serviceProvider, conf) =>
                {
                    conf.BaseAddress = new Uri("https://uat-mfs-v2.mcredit.com.vn:8043");
                    conf.DefaultRequestHeaders.Add("x-security", Common.Config.CredMC_Security_Key);
                })
                .ConfigurePrimaryHttpMessageHandler((serviceProvider) =>
                {
                    var httpFactory = serviceProvider.GetService<IHttpClientFactory>();
                    var restMCService = RestService.For<IRestLoginService>(httpFactory.CreateClient("restLoginService"));
                    Func<Task<string>> getToken = async () =>
                    {
                        try
                        {
                            LoginResponseModel result = await restMCService.GetTokenAsync(new LoginRequestModel
                            {
                                Username = Common.Config.CredMC_Username,
                                Password = Common.Config.CredMC_Password,
                                NotificationId = "notificationId.mekongcredit.3rd",
                                Imei = "imei.mekongcredit.3rd",
                                OsType = "ANDROID"
                            });
                            return result.Token;
                        }
                        catch (Exception ex)
                        {
                            return string.Empty;
                        }
                    };
                    return new RestHttpClientHandler(getToken);
                });
        }
    }

    class RestHttpClientHandler : HttpClientHandler
    {
        private readonly Func<Task<string>> _getToken;
        public RestHttpClientHandler(Func<Task<string>> getToken)
        {
            if (getToken == null) throw new ArgumentNullException("getToken");
            _getToken = getToken;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            var auth = request.Headers.Authorization;

            if (auth != null && auth.Scheme == "Bearer")
            {
                var token = await _getToken().ConfigureAwait(false);
                request.Headers.Authorization = new AuthenticationHeaderValue(auth.Scheme, token);
            }

            return await base.SendAsync(request, cancellationToken);
        }

    }
}
