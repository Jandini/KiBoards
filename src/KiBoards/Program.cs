// Created with JandaBox http://github.com/Jandini/JandaBox
using AutoMapper;
using Serilog;
using System.Reflection;
using KiBoards.Services;

var builder = WebApplication.CreateBuilder(args);

// Read configuration from environment variables
builder.Configuration.AddEnvironmentVariables();

// Create serilog logger
var logger = new LoggerConfiguration()
    .ReadFrom.Configuration(builder.Configuration)
    .CreateLogger()
    .ForContext<Program>();

// Log service name and version
var assembly = Assembly.GetExecutingAssembly();
var version = assembly.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;
var name = assembly.GetName().Name;
logger.Information("Starting {name:l} {version:l}", name, version);

// Use serilog for web hosting
builder.Host.UseSerilog(logger);

// Add services to the container
builder.Services.AddHealth();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Add controllers
builder.Services.AddControllers()
    // Suppress ProblemDetails schema
    .ConfigureApiBehaviorOptions(o => o.SuppressMapClientErrors = true);


// Add http client
builder.Services.AddHttpClient();


// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(
    // Get rid of Dto postfix 
    options => options.CustomSchemaIds((type) => type.Name.EndsWith("Dto")
        ? type.Name[..^3]
        : type.Name));

// Add kibana services
builder.Services.AddKibana(builder.Configuration);

var app = builder.Build();

#if (DEBUG)
// Assert mapper configuration 
app.Services.GetRequiredService<IMapper>().ConfigurationProvider.AssertConfigurationIsValid();

// Log all environment variables in DEBUG mode only
var variables = Environment.GetEnvironmentVariables();
foreach (var key in variables.Keys.Cast<string>().Order())
    logger.ForContext(typeof(Environment)).Debug("{key:l}={value:l}", key, variables[key]);

#endif

// Configure the HTTP request pipeline
if (!app.Environment.IsProduction())
{
    logger.Warning("Adding swagger");
    app.UseSwagger();
    app.UseSwaggerUI(options => options.DocumentTitle = name);
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
