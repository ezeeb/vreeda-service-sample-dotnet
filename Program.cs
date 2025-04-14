using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using MongoDB.Driver;
using VreedaServiceSampleDotNet.Api;
using VreedaServiceSampleDotNet.Models;
using VreedaServiceSampleDotNet.Services;
using Polly;
using Polly.Extensions.Http;

var builder = WebApplication.CreateBuilder(args);

// load app settings
builder.Configuration
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
    .AddJsonFile("appsettings.Local.json", optional: true, reloadOnChange: true) // Load local file
    .AddEnvironmentVariables();

// bind app settings
var appSettings = new AppSettings();
builder.Configuration.Bind(appSettings);
builder.Services.AddSingleton(appSettings);

// create ad b2c http client
builder.Services.AddHttpClient("AdB2cClient");

// create vreeda api http client
builder.Services.AddHttpClient("VreedaApiClient")
    .AddPolicyHandler(GetRetryPolicy())
    .AddPolicyHandler(GetCircuitBreakerPolicy());

// Helper method for Retry-Policy
static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .OrResult(msg => msg.StatusCode == System.Net.HttpStatusCode.TooManyRequests)
        .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)));
}

// Helper method for Circuit-Breaker-Policy
static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
{
    return HttpPolicyExtensions
        .HandleTransientHttpError()
        .CircuitBreakerAsync(5, TimeSpan.FromSeconds(30));
}

// create vreeda api client
builder.Services.AddScoped<IVreedaApiClient, VreedaApiClient>();

// add frontend
builder.Services.AddSpaStaticFiles(options =>
    options.RootPath = "Frontend/build");

// enable cors
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "origins",
        policy => { policy.WithOrigins(appSettings.HostUrl); });
});

// configure MongoDB client
builder.Services.AddSingleton<IMongoClient>(_ => new MongoClient(appSettings.MongoDBOptions.ConnectionString));

// register ServiceState service
builder.Services.AddScoped<IServiceState, ServiceStateMongoDb>(sp =>
{
    var mongoClient = sp.GetRequiredService<IMongoClient>();
    var dbName = appSettings.MongoDBOptions.DbName;
    var logger = sp.GetRequiredService<ILogger<ServiceStateMongoDb>>();
    return new ServiceStateMongoDb(mongoClient, dbName, logger);
});

// Add In-Memory Cache for session storage
builder.Services.AddDistributedMemoryCache();

// Configure session
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Session timeout
    options.Cookie.HttpOnly = true; // Cookie accessible only via HTTP
    options.Cookie.IsEssential = true; // Required for GDPR compliance
});

var app = builder.Build();

app.UseCors("origins");
app.UseRouting();
app.UseForwardedHeaders();
app.UseSession();

if (!app.Environment.IsDevelopment())
{
    app.UseSpaStaticFiles();
}

app.MapApiEndpoints();

app.UseWhen(
    context => !context.Request.Path.StartsWithSegments("/api"),
    then => then.UseSpa(spa =>
    {
        spa.Options.SourcePath = "Frontend";

        if (app.Environment.IsDevelopment())
        {
            spa.UseReactDevelopmentServer(npmScript: "start");
        }
    }));

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}

app.Run();

// ReSharper disable once ClassNeverInstantiated.Global
public partial class Program { }