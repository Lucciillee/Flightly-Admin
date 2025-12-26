using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using ProjectWebApp.Data;
using ProjectWebApp.Model;

var builder = WebApplication.CreateBuilder(args); //This line builds the web application

var identityConnectionString = builder.Configuration.GetConnectionString("IdentityContextConnection")
    ?? throw new InvalidOperationException("Connection string 'IdentityContextConnection' not found.");// Retrieves the connection string for the Identity database from configuration,Reads the Identity database connection string

builder.Services.AddDbContext<IdentityContext>(options =>
    options.UseSqlServer(identityConnectionString));// Sets up the Identity database context with SQL Server,Use SQL Server for user authentication data

builder.Services.AddDefaultIdentity<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<IdentityContext>();// Configures Identity with default settings and role support, without this the login system won't work

// Add services to the container.
builder.Services.AddControllersWithViews(); // Adds support for MVC controllers with views, enable pages and screens like dashboard

builder.Services.AddDbContext<FlightlyDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("FlightlyDB"))); // Sets up the main application database context with SQL Server, Set up Flightly’s main database
builder.Services.AddScoped<EmailService>();// Registers the EmailService for dependency injection, Whenever an email needs to be sent, create an EmailService for it.

var app = builder.Build();

using (var scope = app.Services.CreateScope())//The system initializes required administrative roles automatically at runtime.
{
    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

    string[] roles = { "Admin", "Sub-Admin" };//Create Admin roles automatically

    foreach (var roleName in roles)
    {
        if (!await roleManager.RoleExistsAsync(roleName))
        {
            await roleManager.CreateAsync(new IdentityRole(roleName));
        }
    }
}



// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");// Sets up a global exception handler for production environment
}

app.UseStaticFiles();// Enables serving static files like CSS, JS, images, Protect the website

app.UseRouting();// Enables routing middleware, necessary for URL routing, Protect the website

app.UseAuthentication();// Enables authentication middleware, necessary for login functionality, Protect the website
app.UseAuthorization();// Enables authorization middleware, necessary for role-based access control, Protect the website

// Redirect to Identity Login page
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/")
    {
        context.Response.Redirect("/Identity/Account/Login");// Redirect root URL to the login page
        return;
    }

    await next();
});

// Razor Pages for Identity
app.MapRazorPages();//Enables Identity pages (Login, Register, Forgot Password)

// Your MVC routes (dashboard, etc.),Enables MVC routes (Admin dashboards, reports, etc.
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();// Starts the web application
