using SwizlyPeasy.Common.Extensions;
using SwizlyPeasy.Common.HealthChecks;
using SwizlyPeasy.Common.Middlewares;
using SwizlyPeasy.Consul.ClientConfig;
using SwizlyPeasy.Consul.ServiceRegistration;
using SwizlyPeasy.Demo.API.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// swizly peasy consul & health checks
builder.Services.ConfigureConsulClient(builder.Configuration);
builder.Services.RegisterService(builder.Configuration);
builder.Services.AddSwizlyPeasyHealthChecks();
builder.Services.SetAuthenticationAndAuthorization();

var app = builder.Build();
app.UseMiddleware<ExceptionsHandlerMiddleware>();
app.Use404AsException();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
//--------- Swizly Peasy MiddleWares ----------
// swizly peasy health checks middleware
app.UseSwizlyPeasyHealthChecks();
// mapping the headers as claims
app.UseMiddleware<HeaderToClaimsMiddleware>();
//---------------------------------------------

app.UseAuthorization();
app.MapControllers();

app.Run();