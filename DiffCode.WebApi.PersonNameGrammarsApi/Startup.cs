using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

using DiffCode.WebApi.PersonNameGrammarsApi.Data;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

using Newtonsoft.Json;






namespace DiffCode.WebApi.PersonNameGrammarsApi
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }






    public IConfiguration Configuration { get; }

    
    
    
    
    
    
    
    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {


      services.AddDbContext<GrammarsContext>(opt =>
      {
        opt.UseSqlServer(Configuration.GetConnectionString("GrammarsConn")).UseLazyLoadingProxies();
      });




      services
        .AddControllers()
        .AddNewtonsoftJson(opts =>
        {
          opts.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
          opts.SerializerSettings.MetadataPropertyHandling = MetadataPropertyHandling.ReadAhead;
          opts.SerializerSettings.MaxDepth = 3;
          opts.UseMemberCasing();
        });


      services.AddMvc();




      services.AddSwaggerGen(c=> 
      {
        c.SwaggerDoc("v1", new OpenApiInfo
        {
          Version = "v1",
          Title = "PersonNameGrammars API",
          Description = "",
          Contact = new OpenApiContact
          {
            Name = "Gregory Melekhov",
            Email = "gmelehov@gmail.com"
          }
        });

        var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
        var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
        c.IncludeXmlComments(xmlPath);
      });





      services.AddCors(options => options.AddPolicy("CorsPolicy", builder =>
      {
        builder
        .AllowAnyMethod()
        .AllowAnyHeader()
        .AllowAnyOrigin()
        //.WithOrigins("*/*")
        //.WithOrigins("http://192.168.0.14:10124", "http://192.168.0.14:10125", "http://localhost:10202", "https://localhost:44329")
        //.AllowCredentials()
        ;

      }));





    }

    
    
    
    
    
    
    
    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      };
      app.UseHttpsRedirection();


      app.UseStaticFiles();
      app.UseCors("CorsPolicy");



      app.UseSwagger();
      app.UseSwaggerUI(c =>
      {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "PersonNameGrammars API V1");
        c.InjectStylesheet("/css/swagger-ui/custom.css");
      });







      app.UseRouting();
      app.UseAuthorization();
      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();
      });
    }
  }
}
