using FluentValidation;
using FluentValidation.AspNetCore;
using TransportPlatform.Accounting.Application.Handlers;
using TransportPlatform.Accounting.Domain.Exceptions;
using TransportPlatform.Accounting.Infrastructure;
using TransportPlatform.Infrastructure.Common.Auth;
using TransportPlatform.Infrastructure.Common.Observability;

var builder = WebApplication.CreateBuilder(args);

builder.Host.AddTransportLogging(builder.Configuration, "transport-accounting");

builder.Services.AddAccountingInfrastructure(builder.Configuration);
builder.Services.AddTransportAuth(builder.Configuration);
builder.Services.AddTransportObservability(builder.Configuration, "transport-accounting");

// â”€â”€ Application handlers â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
builder.Services.AddScoped<CreateCustomerHandler>();
builder.Services.AddScoped<RegisterMyProfileHandler>();
builder.Services.AddScoped<UpdateMyProfileHandler>();
builder.Services.AddScoped<RegisterUserHandler>();
builder.Services.AddScoped<LoginHandler>();
builder.Services.AddScoped<CreateEmployeeHandler>();
builder.Services.AddScoped<GetLedgerHandler>();
builder.Services.AddScoped<GetCustomersHandler>();
builder.Services.AddScoped<GetEmployeesHandler>();

// â”€â”€ Validation â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();

// â”€â”€ API â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€â”€
builder.Services.AddControllers();
builder.Services.AddApiVersioning(o =>
{
    o.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    o.AssumeDefaultVersionWhenUnspecified = true;
    o.ReportApiVersions = true;
}).AddApiExplorer(o =>
{
    o.GroupNameFormat = "'v'VVV";
    o.SubstituteApiVersionInUrl = true;
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "TransportPlatform Accounting API", Version = "v1" });
    var xml = Path.Combine(AppContext.BaseDirectory, "TransportPlatform.Accounting.Api.xml");
    c.IncludeXmlComments(xml);
});
builder.Services.AddHealthChecks();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    await app.Services.InitialiseDatabaseAsync();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler(errApp => errApp.Run(async ctx =>
{
    var ex = ctx.Features.Get<Microsoft.AspNetCore.Diagnostics.IExceptionHandlerFeature>()?.Error;
    ctx.Response.StatusCode = ex switch
    {
        AccountingDomainException => StatusCodes.Status400BadRequest,
        CustomerNotFoundException or EmployeeNotFoundException or LedgerEntryNotFoundException
            => StatusCodes.Status404NotFound,
        _ => StatusCodes.Status500InternalServerError
    };
    ctx.Response.ContentType = "application/json";
    await ctx.Response.WriteAsJsonAsync(new { message = ex?.Message ?? "An unexpected error occurred." });
}));

app.UseAuthentication();
app.UseTransportUserContext();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
