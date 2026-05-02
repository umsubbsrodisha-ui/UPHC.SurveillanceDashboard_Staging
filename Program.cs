using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UPHC.SurveillanceDashboard.Data;
using UPHC.SurveillanceDashboard.Hubs;
using UPHC.SurveillanceDashboard.Models;
using UPHC.SurveillanceDashboard.Services;

var builder = WebApplication.CreateBuilder(args);

// =========================
// SERVICES
// =========================

builder.Services.AddRazorPages();

builder.Services.AddServerSideBlazor();

// PostgreSQL
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// Identity + Roles
builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
{
    options.SignIn.RequireConfirmedAccount = false;
})
.AddRoles<IdentityRole>()
.AddEntityFrameworkStores<AppDbContext>();

// SignalR
builder.Services.AddSignalR();

// Custom Services
builder.Services.AddScoped<NotificationService>();

var app = builder.Build();


// =========================
// PIPELINE
// =========================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.MapBlazorHub();

app.MapHub<NotificationHub>("/notificationHub");

app.MapFallbackToPage("/_Host");


// =========================
// DEV ONLY MIGRATION + SEEDING
// =========================

if (app.Environment.IsDevelopment())
{
    using var scope = app.Services.CreateScope();

    var services = scope.ServiceProvider;

    var context =
        services.GetRequiredService<AppDbContext>();

    var roleManager =
        services.GetRequiredService<RoleManager<IdentityRole>>();

    var userManager =
        services.GetRequiredService<UserManager<ApplicationUser>>();

    // =========================
    // APPLY MIGRATIONS
    // =========================

    await context.Database.MigrateAsync();


    // =========================
    // ROLES
    // =========================

    string[] roles =
    {
        "Admin",
        "Analyst",
        "UPHCUser",
        "CHCUser",
        "UHWCUser",
        "NodalOfficer",
        "AddlnCommissioner",
        "MD",
        "Commissioner",
        "JdAdmin"
    };

    foreach (var role in roles)
    {
        if (!await roleManager.RoleExistsAsync(role))
        {
            await roleManager.CreateAsync(
                new IdentityRole(role));
        }
    }


    // =========================
    // ADMIN USER
    // =========================

    var adminUser =
        await userManager.FindByNameAsync("admin");

    if (adminUser == null)
    {
        var user = new ApplicationUser
        {
            UserName = "admin",
            Email = "admin@umsu.com",
            EmailConfirmed = true
        };

        var result = await userManager.CreateAsync(
            user,
            "Admin@123!"
        );

        if (result.Succeeded)
        {
            await userManager.AddToRoleAsync(
                user,
                "Admin"
            );
        }
    }


    // =========================
    // DEV USERS
    // =========================

    var usersToSeed = new List<
        (string Username,
         string Email,
         string Password,
         string Role)>
    {
        ("analyst", "analyst@umsu.com", "Analyst@123!", "Analyst"),

        ("uphcuser", "uphc@umsu.com", "UPHC@123!", "UPHCUser"),

        ("chcuser", "chc@umsu.com", "CHC@123!", "CHCUser"),

        ("uhwcuser", "uhwc@umsu.com", "UHWC@123!", "UHWCUser"),

        ("nodal", "nodal@umsu.com", "Nodal@123!", "NodalOfficer"),

        ("addlncomm", "addln@umsu.com", "Addln@123!", "AddlnCommissioner"),

        ("md", "md@umsu.com", "MD@123!", "MD"),

        ("commissioner", "comm@umsu.com", "Comm@123!", "Commissioner"),

        ("jdadmin", "jd@umsu.com", "JD@123!", "JdAdmin")
    };

    foreach (var u in usersToSeed)
    {
        var existingUser =
            await userManager.FindByNameAsync(u.Username);

        if (existingUser == null)
        {
            int? facilityId = null;

            // Facility mapping

            if (u.Role == "CHCUser")
            {
                facilityId = 1;
            }
            else if (u.Role == "UPHCUser")
            {
                facilityId = 24;
            }

            var user = new ApplicationUser
            {
                UserName = u.Username,

                Email = u.Email,

                EmailConfirmed = true,

                FacilityId = facilityId
            };

            var result = await userManager.CreateAsync(
                user,
                u.Password
            );

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(
                    user,
                    u.Role
                );
            }
        }
    }
}

app.Run();


































//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using UPHC.SurveillanceDashboard.Data;
//using UPHC.SurveillanceDashboard.Hubs;
//using UPHC.SurveillanceDashboard.Models;
//using UPHC.SurveillanceDashboard.Services;

//var builder = WebApplication.CreateBuilder(args);

//// Add services
//builder.Services.AddRazorPages();
//builder.Services.AddServerSideBlazor();

//// PostgreSQL
//builder.Services.AddDbContextFactory<AppDbContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

//// Identity with Roles
//builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
//{
//    options.SignIn.RequireConfirmedAccount = false;
//})
//.AddRoles<IdentityRole>()
//.AddEntityFrameworkStores<AppDbContext>();

//// SignalR + Services
//builder.Services.AddSignalR();
//builder.Services.AddScoped<NotificationService>();

//var app = builder.Build();

//// Configure pipeline
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();
//app.UseAuthentication();
//app.UseAuthorization();

//app.MapRazorPages();
//app.MapBlazorHub();
//app.MapHub<NotificationHub>("/notificationHub");
//app.MapFallbackToPage("/_Host");


////  SEEDING BLOCK (DEV FRIENDLY)
//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;

//    var context = services.GetRequiredService<AppDbContext>();
//    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
//    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

//    // Apply Migrations (creates DB + Facility seed)
//    context.Database.Migrate();

//    // Roles
//    string[] roles = {
//        "Admin", "Analyst", "UPHCUser", "CHCUser",
//        "UHWCuser", "NodalOfficer", "AddlnCommissioner",
//        "MD", "Commissioner", "JdAdmin"
//    };

//    foreach (var role in roles)
//    {
//        if (!await roleManager.RoleExistsAsync(role))
//        {
//            await roleManager.CreateAsync(new IdentityRole(role));
//        }
//    }

//    // Admin User
//    var adminUser = await userManager.FindByNameAsync("admin");

//    if (adminUser == null)
//    {
//        var user = new ApplicationUser
//        {
//            UserName = "admin",
//            Email = "umsubbsrodisha@gmail.com",
//            EmailConfirmed = true,
//            FacilityId = null
//        };

//        var result = await userManager.CreateAsync(user, "Admin@123!");

//        if (result.Succeeded)
//        {
//            await userManager.AddToRoleAsync(user, "Admin");
//        }
//    }

//    // Other Role Users (DEV USE)
//    var usersToSeed = new List<(string Username, string Email, string Password, string Role)>
//    {
//        ("analyst", "analyst@umsu.com", "Analyst@123!", "Analyst"),
//        ("uphcuser", "uphc@umsu.com", "UPHC@123!", "UPHCUser"),
//        ("chcuser", "chc@umsu.com", "CHC@123!", "CHCUser"),
//        ("uhwcuser", "uhwc@umsu.com", "UHWC@123!", "UHWCuser"),
//        ("nodal", "nodal@umsu.com", "Nodal@123!", "NodalOfficer"),
//        ("addlncomm", "addln@umsu.com", "Addln@123!", "AddlnCommissioner"),
//        ("md", "md@umsu.com", "MD@123!", "MD"),
//        ("commissioner", "comm@umsu.com", "Comm@123!", "Commissioner"),
//        ("jdadmin", "jd@umsu.com", "JD@123!", "JdAdmin")
//    };

//    foreach (var u in usersToSeed)
//    {
//        var existingUser = await userManager.FindByNameAsync(u.Username);

//        if (existingUser == null)
//        {
//            int? facilityId = null;

//            // 🔴 Assign Facility based on role
//            if (u.Role == "CHCUser")
//                facilityId = 1;

//            else if (u.Role == "UPHCUser")
//                facilityId = 24;

//            var user = new ApplicationUser
//            {
//                UserName = u.Username,
//                Email = u.Email,
//                EmailConfirmed = true,
//                FacilityId = facilityId
//            };

//            var result = await userManager.CreateAsync(user, u.Password);

//            if (result.Succeeded)
//            {
//                await userManager.AddToRoleAsync(user, u.Role);
//            }
//        }
//    }
//}

//app.Run();















































//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
////using UPHC.SurveillanceDashboard.Data;
////using UPHC.SurveillanceDashboard.Hubs;
////using UPHC.SurveillanceDashboard.Services;
//using UPHC.SurveillanceDashboard.Models;


//using UPHC.SurveillanceDashboard.Components;

//var builder = WebApplication.CreateBuilder(args);

//// Add services to the container.
//builder.Services.AddRazorComponents()
//    .AddInteractiveServerComponents();

//var app = builder.Build();

//// Configure the HTTP request pipeline.
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error", createScopeForErrors: true);
//    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
//    app.UseHsts();
//}
//app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
//app.UseHttpsRedirection();

//app.UseAntiforgery();

//app.MapStaticAssets();
//app.MapRazorComponents<App>()
//    .AddInteractiveServerRenderMode();

//app.Run();


//-----------------------updated---------------

//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using UPHC.SurveillanceDashboard.Data;
//using UPHC.SurveillanceDashboard.Hubs;
//using UPHC.SurveillanceDashboard.Models;
//using UPHC.SurveillanceDashboard.Services;

//var builder = WebApplication.CreateBuilder(args);

//// Add services
//builder.Services.AddRazorPages();
//builder.Services.AddServerSideBlazor();


//// PostgreSQL 13 + Npgsql 10.0.1
//builder.Services.AddDbContextFactory<AppDbContext>(options =>
//    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


////In case of sql server
////builder.Services.AddDbContextFactory<AppDbContext>(options =>
////    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

//// Identity with Roles with Postgres
//builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
//{
//    options.SignIn.RequireConfirmedAccount = false;
//})
//.AddRoles<IdentityRole>()
//.AddEntityFrameworkStores<AppDbContext>();

////builder.Services.AddDefaultIdentity<IdentityUser>(options =>
////{
////    options.SignIn.RequireConfirmedAccount = false;
////    options.Password.RequireDigit = false;
////    options.Password.RequiredLength = 6;
////})
////.AddRoles<IdentityRole>()
////.AddEntityFrameworkStores<AppDbContext>();

//// SignalR + Services
//builder.Services.AddSignalR();
//builder.Services.AddScoped<NotificationService>();

//var app = builder.Build();

//// Configure pipeline
//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();
//app.UseAuthentication();
//app.UseAuthorization();

//app.MapRazorPages();
//app.MapBlazorHub();
//app.MapHub<NotificationHub>("/notificationHub");
//app.MapFallbackToPage("/_Host");

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;

//    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
//    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

//    // 1. Ensure Roles exist
//    string[] roles = { "Admin", "Analyst", "UPHCUser", "CHCUser", "UHWCuser", "NodalOfficer","AddlnCommissioner","MD","Commissiner","JdAdmin" };

//    foreach (var role in roles)
//    {
//        if (!await roleManager.RoleExistsAsync(role))
//        {
//            await roleManager.CreateAsync(new IdentityRole(role));
//        }
//    }

//    // 2. Seed Admin User
//    string adminUsername = "admin";
//    string adminEmail = "umsubbsrodisha@gmail.com";
//    string adminPassword = "Admin@123!";



//    var adminUser = await userManager.FindByNameAsync(adminUsername);

//    if (adminUser == null)
//    {
//        var user = new ApplicationUser
//        {
//            UserName = adminUsername,   //login
//            Email = adminEmail,         //backend only
//            EmailConfirmed = true,
//            FacilityId = null
//        };

//        var result = await userManager.CreateAsync(user, adminPassword);

//        if (result.Succeeded)
//        {
//            await userManager.AddToRoleAsync(user, "Admin");
//        }
//    }
//}


//app.Run();




















// SEED DATABASE + ROLES (runs once)

//using (var scope = app.Services.CreateScope())
//{
//    var services = scope.ServiceProvider;
//    //var dbFactory = services.GetRequiredService<IDbContextFactory<AppDbContext>>();

//    //using var context = dbFactory.CreateDbContext();


//    //// Seed Roles
//    string[] roleNames = { "UPHCUser", "Analyst","Admin","CHCUser" };
//    //foreach (var roleName in roleNames)
//    //{
//    //    if (!await context.Roles.AnyAsync(r => r.Name == roleName))
//    //    {
//    //        await services.GetRequiredService<RoleManager<IdentityRole>>().CreateAsync(new IdentityRole(roleName));
//    //    }
//    //}

//    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();

//    foreach (var roleName in roleNames)
//    {
//        if (!await roleManager.RoleExistsAsync(roleName))
//        {
//            await roleManager.CreateAsync(new IdentityRole(roleName));
//        }
//    }
//}
