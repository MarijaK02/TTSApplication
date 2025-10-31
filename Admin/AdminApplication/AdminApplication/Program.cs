using AdminApplication.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<MainAppSettings>(builder.Configuration.GetSection("MainApp"));

builder.Services.AddRazorPages();

builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession();
builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient("MainAppClient", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["MainApp:ApiBaseUrl"]);
});


var key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"]);

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = builder.Configuration["Jwt:Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
        policy.RequireClaim("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", "Admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

//app.Use(async (context, next) =>
//{
//    var token = context.Session.GetString("AdminJwt");

//    if (string.IsNullOrEmpty(token))
//    {
//        var mainAppUrl = app.Configuration["MainApp:BaseUrl"] ?? "https://localhost:44315/";
//        context.Response.Redirect($"{mainAppUrl}Home/Index");
//        return;
//    }

//    var handler = new JwtSecurityTokenHandler();
//    try
//    {
//        var jwtToken = handler.ReadJwtToken(token);
//        if (jwtToken.ValidTo < DateTime.UtcNow)
//        {
//            context.Session.Remove("AdminJwt");
//            var mainAppUrl = app.Configuration["MainApp:BaseUrl"] ?? "https://localhost:44315/";
//            context.Response.Redirect($"{mainAppUrl}Home/Index");
//            return;
//        }
//    }
//    catch
//    {
//        context.Session.Remove("AdminJwt");
//        var mainAppUrl = app.Configuration["MainApp:BaseUrl"] ?? "https://localhost:44315/";
//        context.Response.Redirect($"{mainAppUrl}Home/Index");
//        return;
//    }

//    await next();
//});

app.Use(async (context, next) =>
{
    var token = context.Session.GetString("AdminJwt");
    if (!string.IsNullOrEmpty(token))
    {
        context.Request.Headers["Authorization"] = "Bearer " + token;
    }
    await next();
});

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
