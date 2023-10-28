using System.Diagnostics;
using System.Net.Mime;
using System.Reflection;
using System.Security.Claims;
using SocialStoriesBackend.DbContext;
using SocialStoriesBackend.Middleware;
using SocialStoriesBackend.Attributes;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;
using SocialStoriesBackend.Entities;
using SocialStoriesBackend.Services;
using Swashbuckle.AspNetCore.Filters;
using Amazon.Runtime;
using Amazon.S3;

namespace SocialStoriesBackend;

public class Startup
{
    public Startup(IConfiguration configuration, IWebHostEnvironment environment)
    {
        Configuration = configuration;
        Environment = environment;
    }

    private IConfiguration Configuration { get; }
    private IWebHostEnvironment Environment { get; }

    public void ConfigureServices(IServiceCollection services)
    {
        // For AWS 
        services.Configure<Settings.AWS>(Configuration.GetSection("AWS"));
        var awsConfig = Configuration.GetSection("AWS").Get<Settings.AWS>();
        Debug.Assert(awsConfig != null);
        
        var awsOptions = Configuration.GetAWSOptions();
        // Bad practice
        awsOptions.Credentials = new BasicAWSCredentials(awsConfig.AccessKey, awsConfig.SecretKey);
        
        services.AddDefaultAWSOptions(awsOptions);
        services.AddAWSService<IAmazonS3>();
        
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            // Allow custom schema names
            options.CustomSchemaIds(type => type.GetCustomAttributes<SwaggerSchemaIdAttribute>().SingleOrDefault()?.SchemaId ?? SchemaHelper.DefaultSchemaIdSelector(type));
            
            // Support JWT in Swagger
            options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = HeaderNames.Authorization,
                Description = @"JWT Authorization header using the Bearer scheme.
                                Enter 'Bearer' [space] and then your token in the text input below.
                                Example: 'Bearer 12345abcdef'",
                Scheme = JwtBearerDefaults.AuthenticationScheme
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtBearerDefaults.AuthenticationScheme
                        },
                    },
                    new List<string>()
                }
            });
            
            
            // Showcase any XML comments in Swagger
            options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml"), true);
            
            // Enable swagger response headers
            options.OperationFilter<AddResponseHeadersFilter>();

            // Add (Auth) to action summary
            options.OperationFilter<AppendAuthorizeToSummaryOperationFilter>();

            // Adds request headers to actions that have the [SwaggerRequestHeader] attribute
            // options.OperationFilter<RequestHeaderFilter>();
            
            options.ExampleFilters();
        });

        services.AddSwaggerExamplesFromAssemblies(Assembly.GetExecutingAssembly());

        // Cors
        services.AddCors(options => options.AddPolicy("Any",
        builder => {
            builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .WithOrigins("http://localhost:3000") // add react frontend
            .AllowAnyHeader();
        }));
        
        // Policies
        services.AddAuthorization(options => {
            options.AddPolicy(Authorization.AdminPolicy, policy => {
                policy.RequireRole(Authorization.AdminRole);
            });
            
            options.AddPolicy(Authorization.UserPolicy, policy => {
                policy.RequireRole(Authorization.AdminRole, Authorization.UserRole);
            });
        });
        
        // IHttpFactory
        services.AddHttpClient();
        
        // Identity
        services.AddIdentity<User, IdentityRole<Guid>>()
          .AddEntityFrameworkStores<DbContextService>()
          .AddRoles<IdentityRole<Guid>>()
          .AddRoleManager<RoleManager<IdentityRole<Guid>>>()
          .AddDefaultTokenProviders();
            
        //Console.WriteLine(Environment.IsDevelopment());
        
        // Database
        if (Environment.IsDevelopment()) { // Environment.IsDevelopment() --> testing and experimenting with MySQL, I am pretty sure I am wrong with doing this
            var mySqlConnectionString = Configuration["Database:ConnectionString"];

            if (Helpers.InDocker()) {
                var splitConnectionString = mySqlConnectionString.Split(';').ToList();
                var ipParameterEnumerable = splitConnectionString.Where(str => str.Contains("Server"));
                if (ipParameterEnumerable is not null)
                {
                    var ipParameterString = ipParameterEnumerable.First();
                    var splitIpParameterString = ipParameterString.Split('=');
                    var ipParameterIndex = splitConnectionString.IndexOf(ipParameterString);
                    if (ipParameterIndex == -1)
                        throw new Exception("IP Parameter index is invalid!");
                    
                    splitConnectionString[ipParameterIndex] = ipParameterString.Replace(splitIpParameterString[1], "host.docker.internal");
                }
                mySqlConnectionString = string.Join(';', splitConnectionString);
            }
            

            
            services.AddDbContext<DbContextService>(options => options.UseMySQL(mySqlConnectionString));
        }
        else
        {
            services.AddDbContext<DbContextService>(options => options.UseInMemoryDatabase(databaseName: "social_stories")); 
        }
        
        
        var identityOptions = new IdentityOptions();
        
        // Authorization and Authentication
        services.AddAuthentication(options => {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddCookie()
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateActor = true,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = Authorization.SecurityKey,
                ValidIssuer = Authorization.Issuer,
                ValidAudience = Authorization.Audience,
                NameClaimType = identityOptions.ClaimsIdentity.UserIdClaimType,
                RoleClaimType = ClaimTypes.Role
            };
        });
        
        // Setup mapping of Entities to DTOs
        services.AddAutoMapper(Assembly.GetExecutingAssembly());
            
        // controllers
        var mvcBuilder = services.AddControllers();
        mvcBuilder.ConfigureApiBehaviorOptions(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                // Ensure InvalidModel returns JSON
                var result = new ValidationFailedResult(context.ModelState);
                result.ContentTypes.Add(MediaTypeNames.Application.Json);
                return result;
            };
        });
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider)
    {
        if (env.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();

            using var scope = app.ApplicationServices.CreateScope();
            using var context = scope.ServiceProvider.GetService<DbContextService>();

            context?.Database.EnsureCreatedAsync().GetAwaiter().GetResult();
        }
        
        app.UseCors("Any");

        app.UseMiddleware<InternalServerErrorMiddleware>();
        app.UseMiddleware<NotFoundMiddleware>();
        app.UseMiddleware<NotAuthorizedMiddleware>();

        app.UseRouting();
        
        // Enable Authorization and Authentication
        app.UseAuthentication();
        app.UseAuthorization();

        //Enable middleware to serve generated Swagger as a JSON endpoint.
		app.UseSwagger();

        //Enable middleware to serve swagger-ui(HTML, JS, CSS, etc.), specifying them Swagger JSON endpoint.
        app.UseSwaggerUI(c => { c.SwaggerEndpoint("/swagger/v1/swagger.json", "social_stories/v1"); });

        app.UseEndpoints(endpoints =>
        {
	        endpoints.MapControllers();
        });
	}
}