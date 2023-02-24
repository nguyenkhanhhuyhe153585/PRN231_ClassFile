using ClassFileBackEnd.Authen;
using ClassFileBackEnd.Mapper;
using ClassFileBackEnd.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors();
builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);
builder.Services.AddDbContext<ClassfileContext>(opt => opt.UseSqlServer(
    builder.Configuration.GetConnectionString("DB")
    )
);

// Thêm vào để config cho JWT
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    var configbuilder = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
    IConfigurationRoot configuration = configbuilder.Build();
    var keyRaw = configuration["JWT:Key"];
    var issuerRaw = configuration["JWT:Issuer"];
    var audienceRaw = configuration["JWT:Audience"];
    var Key = Encoding.UTF8.GetBytes(configuration["JWT:Key"]);
    opt.RequireHttpsMetadata = false;
    opt.SaveToken = true;
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = configuration["JWT:Issuer"],
        ValidAudience = configuration["JWT:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Key)

    };
});
// Lấy từ stoflw https://bit.ly/3lB8UaU
builder.Services.AddAuthorization(opt =>
{
    var defaultAuthorizationPolicyBuilder = new AuthorizationPolicyBuilder(
                JwtBearerDefaults.AuthenticationScheme);
    defaultAuthorizationPolicyBuilder =
        defaultAuthorizationPolicyBuilder.RequireAuthenticatedUser().RequireClaim("Sub");
    opt.AddPolicy("TeacherRequired",
        policy => policy.RequireClaim("Role", "TC"));
    opt.DefaultPolicy = defaultAuthorizationPolicyBuilder.Build();

});

builder.Services.AddScoped<JWTManagerRepository>();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthorization();
app.UseAuthentication(); // This need to be added	
app.MapControllers();
app.UseCors(opt => opt.AllowAnyHeader()
                .AllowAnyMethod()
                .SetIsOriginAllowed((host) => true)
                .AllowCredentials());
app.UseCors("TeacherRequired");

app.Run();