using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using TTS.Domain.Identity;
using TTS.Domain.Shared;
using TTS.Repository;
using TTS.Repository.Implementation;
using TTS.Repository.Interface;
using TTS.Service.Implementation;
using TTS.Service.Interface;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration.AddEnvironmentVariables();

builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));

// Add services to the container.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options
    .UseSqlServer(connectionString, sqlOptions =>
    {
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 10,
            maxRetryDelay: TimeSpan.FromSeconds(10),
            errorNumbersToAdd: null);
    }));

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

builder.Services.AddTransient<IEmailService, EmailService>();

builder.Services.AddTransient<IProjectsService, ProjectsService>();
builder.Services.AddTransient<IUserService, UserService>();
builder.Services.AddTransient<IActivitesService, ActivitiesService>();
builder.Services.AddTransient<IAttachmentService, AttachmentService>();
builder.Services.AddTransient<ICommentsService, CommentsService>();
builder.Services.AddTransient<IAdminService, AdminService>();


builder.Services.AddDefaultIdentity<TTSApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddControllersWithViews().AddNewtonsoftJson(options =>
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore);

double expiryMinutes = Convert.ToDouble(builder.Configuration["Authentication:ExpiryMinutes"]);
builder.Services.ConfigureApplicationCookie(options =>
{
    options.ExpireTimeSpan = TimeSpan.FromMinutes(expiryMinutes);
    options.SlidingExpiration = true;
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

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

using (var scope = app.Services.CreateScope())
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
    var roles = new[] { "Admin", "Client", "Consultant" };

    foreach(var role in roles)
    {
        if(!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(new IdentityRole(role));
        }
    }
}

using (var scope = app.Services.CreateScope())
{
    var userManager = scope.ServiceProvider.GetRequiredService<UserManager<TTSApplicationUser>>();

    string email = "admin@admin.com";
    string password = "Test123!";

    if(await userManager.FindByEmailAsync(email) == null)
    {
        var user = new TTSApplicationUser();
        user.CreatedOn = DateTime.Now;
        user.UserName = email;
        user.Email = email; 
        user.EmailConfirmed = true;

        await userManager.CreateAsync(user, password);

        await userManager.AddToRoleAsync(user, "Admin");
    }

}

app.Run();
