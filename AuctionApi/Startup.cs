using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Amazon.CognitoIdentityProvider;
using Amazon.Runtime;
using AuctionRepository.DbContexts;
using AuctionRepository.Services.Repos;
using AuctionRepository.Services.Utility;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace AuctionApi
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
            string details = Configuration.GetConnectionString("FinancialServices");
            services.AddControllers(setupAction =>
            {

                setupAction.ReturnHttpNotAcceptable = true;
                //setupAction.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
            }).AddNewtonsoftJson(setupAction =>
            {
                setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            }
            ).AddXmlDataContractSerializerFormatters().ConfigureApiBehaviorOptions(setupAction => {
                setupAction.InvalidModelStateResponseFactory = context =>
                {
                    var problemDetailsFactory = context.HttpContext.RequestServices
                    .GetRequiredService<ProblemDetailsFactory>();
                    var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                        context.HttpContext,
                    context.ModelState);

                    problemDetails.Detail = "See the error field for details";
                    problemDetails.Instance = context.HttpContext.Request.Path;

                    var actionExecutionContext =
                    context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;


                    if ((context.ModelState.ErrorCount > 0) &&
                    (actionExecutionContext?.ActionArguments.Count ==
                    context.ActionDescriptor.Parameters.Count))
                    {
                        problemDetails.Type = "https://courselibrary.com/modelvalidationproblem";
                        problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                        problemDetails.Title = "One or more validation errors occured";

                        return new UnprocessableEntityObjectResult(problemDetails)
                        {
                            ContentTypes = { "application/problem+json" }
                        };

                    }

                    problemDetails.Status = StatusCodes.Status400BadRequest;
                    problemDetails.Title = "One or more errors on input occured";
                    return new BadRequestObjectResult(problemDetails)
                    {
                        ContentTypes = { "application/problem+json" }
                    };



                };

            });





            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());
            services.AddScoped<IUtility, Utility>();

            services.AddScoped<IAuctionsRepository, AuctionsRepository>();





            services.AddDbContext<AuctionLibraryContext>(options => options.UseSqlServer(Configuration.GetConnectionString("FinancialServices")));



            Environment.SetEnvironmentVariable("AWS_ACCESS_KEY_ID", Configuration["AWS:AccessKey"]);
            Environment.SetEnvironmentVariable("AWS_SECRET_ACCESS_KEY", Configuration["AWS:SecretKey"]);
            Environment.SetEnvironmentVariable("AWS_REGION", Configuration["Region"]);

            var awsOptions = Configuration.GetAWSOptions();
            awsOptions.Credentials = new BasicAWSCredentials(Configuration["AWS:AccessKey"], Configuration["AWS:SecretKey"]); 

            services.AddDefaultAWSOptions(awsOptions);

            //services.AddDefaultAWSOptions(Configuration.GetAWSOptions());

            services.AddAWSService<IAmazonCognitoIdentityProvider>();

            services.AddAuthentication("Bearer")
        .AddJwtBearer(options =>
        {
            options.Audience = "18pe5conu8fkudpqg6g59e5mib";
            options.Authority = "https://cognito-idp.us-east-2.amazonaws.com/us-east-2_Yqdph91I6";
            
            
        });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseAuthentication();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGet("/", (context) => context.Response.WriteAsync("Success"));
            });
        }
    }
}
