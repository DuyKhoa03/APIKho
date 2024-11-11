using APIQLKho.Models;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

// C?u hình d?ch v? xác th?c và thêm chính sách "ManagerOnly"
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";
        options.AccessDeniedPath = "/User/AccessDenied";
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ManagerOnly", policy =>
        policy.RequireClaim("Role", "1")); // Role = 1 ??i di?n cho qu?n lý
});

// C?u hình d?ch v? JSON cho các controller
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    options.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;
});


// C?u hình chu?i k?t n?i ??n c? s? d? li?u
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<QlkhohangContext>(options =>
    options.UseSqlServer(connectionString));

// C?u hình Swagger
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "My API",
        Description = "An ASP.NET Core Web API for managing resources",
        TermsOfService = new Uri("https://example.com/terms"),
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "youremail@example.com",
            Url = new Uri("https://example.com/contact"),
        },
        License = new OpenApiLicense
        {
            Name = "Use under LICX",
            Url = new Uri("https://example.com/license"),
        }
    });
});

var app = builder.Build();

// C?u hình ?? s? d?ng Swagger và Swagger UI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "My APIQLKho v1");
        // c.RoutePrefix = string.Empty; // ??t Swagger UI là trang chính khi truy c?p vào root c?a ?ng d?ng
    });
}

// Configure the HTTP request pipeline.

app.UseHttpsRedirection();

// C?u hình ?? s? d?ng xác th?c và phân quy?n
app.UseAuthentication(); // Thêm dòng này ?? s? d?ng xác th?c
app.UseAuthorization();

app.MapControllers();

app.Run();
