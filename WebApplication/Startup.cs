using AutoMapper;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Cors.Internal;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using PDF_Generator.Utility;
using Swashbuckle.AspNetCore.Swagger;
using System.IO;
using System.Text;
using WebApplication.Models;
using WebApplication.Repository;
using WebApplication.Repository.Dashboard;
using WebApplication.Service;

namespace WebApplication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            //services.AddDbContext<MieleContext>(opt => opt.UseInMemoryDatabase("MieleList"));
            services.AddDbContext<MieleContext>(options => options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")));
            services.Configure<EmailSettings>(Configuration.GetSection("EmailConfigurations"));
            services.Configure<EmailSettingsTickets>(Configuration.GetSection("EmailConfigurationsTickets"));

            services.AddTransient<ICanalesRepository, CanalesRepository>();
            services.AddTransient<IQuejasRepository, QuejasRepository>();
            services.AddTransient<ITipoQuejaRepository, TipoQuejasRepository>();
            services.AddTransient<IClienteRepository, ClienteRepository>();
            services.AddTransient<IProductoRepository, ProductoRepository>();
            services.AddTransient<IPropuestasRepository, PropuestasRepository>();
            services.AddTransient<IUsersRepository, UsersRepository>();
            services.AddTransient<IEmailRepository, EmailRepository>();
            services.AddTransient<IProductLinesRepository, ProductLinesRepository>();
            services.AddTransient<IProductSubLinesRepository, ProductSubLinesRepository>();
            services.AddTransient<IProductCategoriesRepository, ProductCategoriesRepository>();
            services.AddTransient<IClientsRepository, ClientsRepository>();
            services.AddTransient<IBranchOfficesRepository, BranchOfficesRepository>();
            services.AddTransient<IDashboardRepository, DashboardRepository>();
            services.AddTransient<IProductSubLinesRepository, ProductSubLinesRepository>();
            services.AddTransient<IProductSubLineTypeRepository, ProductSubLineTypeRepository>();
            services.AddTransient<ICotizacionPDF, CotizacionPDF>();

            var context = new CustomAssemblyLoadContext();
            context.LoadUnmanagedLibrary(Path.Combine(Directory.GetCurrentDirectory(), "libwkhtmltox.dll"));
            services.AddSingleton(typeof(IConverter), new SynchronizedConverter(new PdfTools()));
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
            // Add Cors
            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));

            services.AddAutoMapper();

            services.AddMvc().AddJsonOptions(options => {
                options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });

            services.Configure<MvcOptions>(options =>
            {
                options.Filters.Add(new CorsAuthorizationFilterFactory("MyPolicy"));
            });
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "Miele API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Miele API V1");
            });

            // Enable Cors
            app.UseCors("MyPolicy");
            app.UseAuthentication();
            app.UseMvc();
        }
    }
}
