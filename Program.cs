using Server.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Server.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Text.Json.Serialization;
using Server.Services;
using Server.ChatHub;
using Server.Types;

var builder = WebApplication.CreateBuilder(args);

ConfigurationManager Configuration = builder.Configuration;

var conStr = Configuration.GetConnectionString("DefaultConnection");

builder.Services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<PhotoService>();
builder.Services.AddScoped<MessageRepository>();

builder.Services.AddDbContext<ApplicationDbContext>(options => options.UseSqlite(conStr));

builder.Services.AddControllers().AddJsonOptions(x =>
            x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
builder.Services.AddCors();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddIdentityCore<SiteUser>(opt =>
{
  opt.Password.RequireNonAlphanumeric = false;
})
.AddRoles<Role>()
.AddRoleManager<RoleManager<Role>>()
.AddSignInManager<SignInManager<SiteUser>>()
.AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
  options.TokenValidationParameters = new TokenValidationParameters
  {
    ValidateIssuerSigningKey = true,
    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["JwtKey"])),
    ValidateIssuer = false,
    ValidateAudience = false,
  };

  options.Events = new JwtBearerEvents
  {
    OnMessageReceived = context =>
    {
      var accessToken = context.Request.Query["access_token"];

      var path = context.HttpContext.Request.Path;
      if (!string.IsNullOrEmpty(accessToken) &&
                      path.StartsWithSegments("/hubs"))
      {
        context.Token = accessToken;
      }
      return Task.CompletedTask;
    }
  };
});

builder.Services.AddSignalR();

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI();

app.UseRouting();

app.UseCors(x => x.AllowAnyHeader()
    .AllowAnyMethod()
    .AllowCredentials()
    .WithOrigins("http://localhost:3000"));

app.UseAuthentication();
app.UseAuthorization();
app.UseDefaultFiles();
app.UseStaticFiles();

// app.MapControllers();
app.UseEndpoints(endpoints =>
{
  endpoints.MapControllers();
  endpoints.MapHub<MessageHub>("hubs/message");
  endpoints.MapFallbackToController("Index", "StaticWeb");
});

using (var scope = app.Services.CreateScope())
{
  var services = scope.ServiceProvider;
  var context = services.GetRequiredService<ApplicationDbContext>();

  context.Database.Migrate();
}

app.Run();
