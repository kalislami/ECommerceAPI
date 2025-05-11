using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using ECommerceApi.Data;
using ECommerceApi.Helpers;
using ECommerceApi.Services;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var builder = WebApplication.CreateBuilder(args);

// ====== Konfigurasi JWT Settings dari appsettings.json ======
builder.Services.Configure<JwtSettings>(builder.Configuration.GetSection("JwtSettings"));
var jwtSettings = builder.Configuration.GetSection("JwtSettings")?.Get<JwtSettings>() ?? throw new ArgumentNullException("JwtSettings configuration is missing");
var key = Encoding.UTF8.GetBytes(jwtSettings.SecretKey);

// ====== Konfigurasi DbContext untuk DB ======
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// ====== Tambahkan Authentication JWT ======
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(key)
    };
});

// ====== Tambahkan Controller & Swagger ======
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "ECommerce API", Version = "v1" });

    // Tambahkan konfigurasi untuk bearer token
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Masukkan token seperti ini: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});


builder.Services.AddScoped<JwtHelper>();
builder.Services.AddScoped<FirebaseStorageService>();

// Tambahkan MidtransService dengan konfigurasi HttpClient
builder.Services.Configure<MidtransSettings>(builder.Configuration.GetSection("Midtrans"));
builder.Services.AddHttpClient<MidtransService>();

builder.Services.AddControllers();

// Inisialisasi Firebase Admin SDK
FirebaseApp.Create(new AppOptions()
{
    Credential = GoogleCredential.FromFile("firebase-adminsdk.json")
});

var app = builder.Build();

// ====== Jalankan auto-migration saat startup ======
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate(); // Auto apply migration
}

// ====== Middleware ======
app.UseSwagger();
app.UseSwaggerUI();

app.UseAuthentication(); // Harus sebelum Authorization
app.UseAuthorization();

app.MapControllers();

app.Run();
