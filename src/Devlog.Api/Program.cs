using Microsoft.EntityFrameworkCore;
using DevLog.Infrastructure.Data;
using DevLog.Domain.Interfaces;
using DevLog.Infrastructure.Repositories;
using DevLog.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.OpenApi;


var builder = WebApplication.CreateBuilder(args);

// Read the connection string from appsettings.json
// "DefaultConnection" must match the key in appsettings.json exactly
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Add services to the container.
builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseNpgsql(connectionString));
builder.Services.AddMediatR(cfg => 
    cfg.RegisterServicesFromAssembly(
        typeof(RegisterUserCommandHandler).Assembly));
//Register jwt in program.cs 
//1. setting the default scheme(first block)
//2. Token Validation Parameters
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        //validate the server that created the token
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],

        //validate the recipient of the token
        ValidateAudience = true,
        ValidAudience = builder.Configuration["Jwt:Audience"],

        //validate the token hasn't expired
        ValidateLifetime = true,

        //validate the signature using the signing key
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Secret"]!)),

        //No Clock Skew basicasllyu means token expires exactly when it is supposed to expire
        ClockSkew = TimeSpan.Zero
    };
});
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c => 
c.SwaggerDoc("v1",new OpenApiInfo
{
    Title = "DevLog Api",
    Version = "v1",
    Description = @"RESTful API for managing logging and tracking developer activities
    **Authntication**
    1.Register with POST api/auth/Register
    2.Login using POST api/auth/Login
    3.Copy 'accessToken' from response
    4.Click on authorize and paste the token
    5.All subsequent request will include the bearer token automatically.",
    Contact = new OpenApiContact
    {
        Name = "DevLog Support",
        Email = "support@DevLog.com
    }
},
c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
{
    Name = "Authorization",
    Type = SecuritySchemeType.Http,
    Scheme = "Bearer",
    In = ParameterLocation.Header,
    Description = @"Enter the JWT token from POST api/auth/Login
    **How To Get Token**
    1. Use POST api/auth/register to create an account
    2. Use POST api/auth/login with your credentials
    3. Copy the 'accessToken from the response 
    4. Click 'authorize' and paste the token from the response 
    5. Click 'Authorize'."
}),
c.AddSecurityRequirement(new OpenApiSecurityRequirement
{
    {
        new OpenApiSecurityScheme
        {
            Reference = new OpenApiReferenceError
            {
                Type = ReferenceType.SecurityScheme,
                Id = "Bearer"
            }
        },
        Array.Empty<string>()
    }
}))
);
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IJwtService, JwtService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.MapControllers();




app.Run();

