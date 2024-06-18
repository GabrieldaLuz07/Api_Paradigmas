using ApiTF.BaseDados.Models;
using ApiTF.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System.IO;
using System;
using System.Reflection;
using ApiTF.BaseDados;
using ApiTF.Services.DTOs;
using ApiTF.Services.Mappings;
using ApiTF.Services.Validate;
using FluentValidation.AspNetCore;
using FluentValidation;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddDbContext<TfDbContext>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<PromotionService>();
builder.Services.AddScoped<SaleService>();
builder.Services.AddScoped<StockLogService>();
builder.Services.AddTransient<IValidator<ProductDTO>, ProductValidate>();
builder.Services.AddTransient<IValidator<ProductUpdateDTO>, ProductUpdateValidate>();
builder.Services.AddTransient<IValidator<PromotionDTO>, PromotionValidate>();
builder.Services.AddTransient<IValidator<SaleDTO>, SaleValidate>();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile));
builder.Services.AddFluentValidationClientsideAdapters();
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.Preserve;
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});
builder.Services.AddEndpointsApiExplorer();
builder.Logging.AddFile("Logs/ApiWebDB-{Date}.log");
builder.Services.AddSwaggerGen();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "API Trabalho Final",
        Description = "Gerenciador de produtos, vendas e promoções",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Example Contact",
            Url = new Uri("https://example.com/contact")
        },
        License = new OpenApiLicense
        {
            Name = "Example License",
            Url = new Uri("https://example.com/license")
        }
    });

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.MapControllers();
app.Run();
