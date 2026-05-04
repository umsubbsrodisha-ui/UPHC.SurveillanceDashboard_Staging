
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UPHC.SurveillanceDashboard.Components;
using UPHC.SurveillanceDashboard.Data;
using UPHC.SurveillanceDashboard.Hubs;
using UPHC.SurveillanceDashboard.Models;
using UPHC.SurveillanceDashboard.Services;

var builder = WebApplication.CreateBuilder(args);

// ========================================
// SERVICES
// ========================================

builder.Services.AddRazorPages();

builder.Services
    .AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddHttpClient();

// PostgreSQL
builder.Services.AddDbContextFactory<AppDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")
    ));

// ========================================
// IDENTITY
// ========================================

builder.Services
    .AddDefaultIdentity<ApplicationUser>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;

        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequiredLength = 6;
    })
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<AppDbContext>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Login";

    options.LogoutPath = "/Logout";

    options.AccessDeniedPath = "/AccessDenied";

    options.ExpireTimeSpan = TimeSpan.FromHours(12);

    options.SlidingExpiration = true;
});

// SignalR
builder.Services.AddSignalR();

// Custom Services
builder.Services.AddScoped<NotificationService>();

var app = builder.Build();

// ========================================
// PIPELINE
// ========================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");

    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAntiforgery();

app.UseAuthentication();

app.UseAuthorization();

// ========================================
// ENDPOINTS
// ========================================

app.MapRazorPages();

app.MapHub<NotificationHub>("/notificationHub");

app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

// ========================================
// LOGIN ENDPOINT
// ========================================

app.MapPost("/api/login", async (
    HttpContext context,
    SignInManager<ApplicationUser> signInManager) =>
{
    var form = await context.Request.ReadFormAsync();

    var username = form["Username"].ToString();

    var password = form["Password"].ToString();

    var returnUrl = form["ReturnUrl"].ToString();

    var result = await signInManager.PasswordSignInAsync(
        username,
        password,
        false,
        false);

    if (result.Succeeded)
    {
        if (string.IsNullOrWhiteSpace(returnUrl))
        {
            returnUrl = "/dashboard";
        }

        context.Response.Redirect(returnUrl);
    }
    else
    {
        context.Response.Redirect(
            $"/Login?error=true&returnUrl={Uri.EscapeDataString(returnUrl)}");
    }
});

// ========================================
// LOGOUT ENDPOINT
// ========================================

app.MapGet("/Logout", async (
    HttpContext context,
    SignInManager<ApplicationUser> signInManager) =>
{
    await signInManager.SignOutAsync();

    context.Response.Redirect("/Login");
});

// ========================================
// DEV MIGRATIONS + SEEDING
// ========================================

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

    await context.Database.MigrateAsync();

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
                "Admin");
        }
    }

    var usersToSeed = new List<(string Username, string Email, string Password, string Role)>
        {
            ("analyst", "analyst@umsu.com", "Analyst@123!", "Analyst"),
            ("uphcuser", "uphc@umsu.com", "UPHc@123!", "UPHCUser"),
            ("chcuser", "chc@umsu.com", "CHc@123!", "CHCUser"),
            ("uhwcuser", "uhwc@umsu.com", "UHWc@123!", "UHWCUser"),
            ("nodal", "nodal@umsu.com", "Nodal@123!", "NodalOfficer"),
            ("addlncomm", "addln@umsu.com", "Addln@123!", "AddlnCommissioner"),
            ("md", "md@umsu.com", "MD@123!", "MD"),
            ("commissioner", "comm@umsu.com", "Comm@123!", "Commissioner"),
            ("jdadmin", "jd@umsu.com", "JD@123!", "JdAdmin")
        };





    foreach (var u in usersToSeed)
    {
        var existingUser = await userManager.FindByNameAsync(u.Username);
        if (existingUser == null)
        {
            int? facilityId = null;
            if (u.Role == "CHCUser") facilityId = 1;
            else if (u.Role == "UPHCUser") facilityId = 24;
            else if (u.Role == "UHWCUser") facilityId = 27;

            var user = new ApplicationUser
            {
                UserName = u.Username,
                Email = u.Email,
                EmailConfirmed = true,
                FacilityId = facilityId
            };

            var result = await userManager.CreateAsync(user, u.Password);
            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(user, u.Role);
            }
        }
    }




}

app.Run();





























//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.Data;
//using Microsoft.EntityFrameworkCore;
//using UPHC.SurveillanceDashboard.Components;
//using UPHC.SurveillanceDashboard.Data;
//using UPHC.SurveillanceDashboard.Hubs;
//using UPHC.SurveillanceDashboard.Models;
//using UPHC.SurveillanceDashboard.Services;

//var builder = WebApplication.CreateBuilder(args);

//// =========================
//// SERVICES
//// =========================

//builder.Services.AddRazorPages();

//// ✅ Modern .NET 8/9/10 Blazor
//builder.Services.AddRazorComponents().AddInteractiveServerComponents();

//builder.Services.AddHttpClient();  

//// PostgreSQL
//builder.Services.AddDbContextFactory<AppDbContext>(options =>
//    options.UseNpgsql(
//        builder.Configuration.GetConnectionString("DefaultConnection")
//    ));

//// Identity + Roles
//builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
//{
//    options.SignIn.RequireConfirmedAccount = false;
//})
//.AddRoles<IdentityRole>()
//.AddEntityFrameworkStores<AppDbContext>();
////.AddDefaultUI();  // ← ADDED

//// ✅ ADD THIS - Tell Identity to use your custom login page
//builder.Services.ConfigureApplicationCookie(options =>
//{
//    options.LoginPath = "/login";


//});

//// SignalR
//builder.Services.AddSignalR();

//// Custom Services
//builder.Services.AddScoped<NotificationService>();

//var app = builder.Build();

//// =========================
//// PIPELINE
//// =========================

//if (!app.Environment.IsDevelopment())
//{
//    app.UseExceptionHandler("/Error");
//    app.UseHsts();
//}

//app.UseHttpsRedirection();
//app.UseStaticFiles();
//app.UseRouting();

//app.UseAntiforgery();  // ← ADDED

//app.UseAuthentication();
//app.UseAuthorization();

//// =========================
//// ENDPOINTS
//// =========================

//app.MapRazorPages();
//app.MapHub<NotificationHub>("/notificationHub");
//app.MapRazorComponents<App>()
//    .AddInteractiveServerRenderMode();

//// =========================
//// DEV ONLY MIGRATION + SEEDING
//// =========================

//if (app.Environment.IsDevelopment())
//{
//    using var scope = app.Services.CreateScope();
//    var services = scope.ServiceProvider;
//    var context = services.GetRequiredService<AppDbContext>();
//    var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
//    var userManager = services.GetRequiredService<UserManager<ApplicationUser>>();

//    await context.Database.MigrateAsync();

//    string[] roles =
//    {
//        "Admin", "Analyst", "UPHCUser", "CHCUser", "UHWCUser",
//        "NodalOfficer", "AddlnCommissioner", "MD", "Commissioner", "JdAdmin"
//    };

//    foreach (var role in roles)
//    {
//        if (!await roleManager.RoleExistsAsync(role))
//        {
//            await roleManager.CreateAsync(new IdentityRole(role));
//        }
//    }

//    var adminUser = await userManager.FindByNameAsync("admin");
//    if (adminUser == null)
//    {
//        var user = new ApplicationUser
//        {
//            UserName = "admin",
//            Email = "admin@umsu.com",
//            EmailConfirmed = true
//        };

//        var result = await userManager.CreateAsync(user, "Admin@123!");
//        if (result.Succeeded)
//        {
//            await userManager.AddToRoleAsync(user, "Admin");
//        }
//    }

//    var usersToSeed = new List<(string Username, string Email, string Password, string Role)>
//    {
//        ("analyst", "analyst@umsu.com", "Analyst@123!", "Analyst"),
//        ("uphcuser", "uphc@umsu.com", "UPHc@123!", "UPHCUser"),
//        ("chcuser", "chc@umsu.com", "CHc@123!", "CHCUser"),
//        ("uhwcuser", "uhwc@umsu.com", "UHWc@123!", "UHWCUser"),
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
//            if (u.Role == "CHCUser") facilityId = 1;
//            else if (u.Role == "UPHCUser") facilityId = 24;
//            else if (u.Role == "UHWCUser") facilityId = 27;

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


////app.MapPost("/api/login", async (
////    SignInManager<ApplicationUser> signInManager,
////    LoginRequest request) =>
////{
////    var result = await signInManager.PasswordSignInAsync(
////        request.Username, request.Password, false, false);
////    return result.Succeeded ? Results.Ok() : Results.Unauthorized();
////});

//app.MapPost("/api/login", async (
//    HttpContext context,
//    SignInManager<ApplicationUser> signInManager) =>
//{
//    var form = await context.Request.ReadFormAsync();
//    var result = await signInManager.PasswordSignInAsync(
//        form["Username"], form["Password"], false, false);

//    if (result.Succeeded)
//    {
//        context.Response.Redirect("/dashboard");
//    }
//    else
//    {
//        context.Response.Redirect("/login?error=true");
//    }
//});

//app.Run();
//public record LoginRequest(string Username, string Password);






































//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using UPHC.SurveillanceDashboard.Components;
//using UPHC.SurveillanceDashboard.Data;
//using UPHC.SurveillanceDashboard.Hubs;
//using UPHC.SurveillanceDashboard.Models;
//using UPHC.SurveillanceDashboard.Services;

//var builder = WebApplication.CreateBuilder(args);

//// =========================
//// SERVICES
//// =========================

//builder.Services.AddRazorPages();

//// ✅ Modern .NET 8/9/10 Blazor
//builder.Services.AddRazorComponents()
//    .AddInteractiveServerComponents();

//// PostgreSQL
//builder.Services.AddDbContextFactory<AppDbContext>(options =>
//    options.UseNpgsql(
//        builder.Configuration.GetConnectionString("DefaultConnection")
//    ));

//// Identity + Roles
//builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
//{
//    options.SignIn.RequireConfirmedAccount = false;
//})
//.AddRoles<IdentityRole>()
//.AddEntityFrameworkStores<AppDbContext>();

//// SignalR
//builder.Services.AddSignalR();

//// Custom Services
//builder.Services.AddScoped<NotificationService>();

//var app = builder.Build();


//// =========================
//// PIPELINE
//// =========================

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


//// =========================
//// ENDPOINTS
//// =========================

//app.MapRazorPages();

//app.MapHub<NotificationHub>("/notificationHub");

//// ✅ Modern Blazor App Mapping
//app.MapRazorComponents<App>()
//    .AddInteractiveServerRenderMode();


//// =========================
//// DEV ONLY MIGRATION + SEEDING
//// =========================

//if (app.Environment.IsDevelopment())
//{
//    using var scope = app.Services.CreateScope();

//    var services = scope.ServiceProvider;

//    var context =
//        services.GetRequiredService<AppDbContext>();

//    var roleManager =
//        services.GetRequiredService<RoleManager<IdentityRole>>();

//    var userManager =
//        services.GetRequiredService<UserManager<ApplicationUser>>();

//    // =========================
//    // APPLY MIGRATIONS
//    // =========================

//    await context.Database.MigrateAsync();


//    // =========================
//    // ROLES
//    // =========================

//    string[] roles =
//    {
//        "Admin",
//        "Analyst",
//        "UPHCUser",
//        "CHCUser",
//        "UHWCUser",
//        "NodalOfficer",
//        "AddlnCommissioner",
//        "MD",
//        "Commissioner",
//        "JdAdmin"
//    };

//    foreach (var role in roles)
//    {
//        if (!await roleManager.RoleExistsAsync(role))
//        {
//            await roleManager.CreateAsync(
//                new IdentityRole(role));
//        }
//    }


//    // =========================
//    // ADMIN USER
//    // =========================

//    var adminUser =
//        await userManager.FindByNameAsync("admin");

//    if (adminUser == null)
//    {
//        var user = new ApplicationUser
//        {
//            UserName = "admin",
//            Email = "admin@umsu.com",
//            EmailConfirmed = true
//        };

//        var result = await userManager.CreateAsync(
//            user,
//            "Admin@123!"
//        );

//        if (result.Succeeded)
//        {
//            await userManager.AddToRoleAsync(
//                user,
//                "Admin"
//            );
//        }
//    }


//    // =========================
//    // DEV USERS
//    // =========================

//    var usersToSeed = new List<
//        (string Username,
//         string Email,
//         string Password,
//         string Role)>
//    {
//        ("analyst", "analyst@umsu.com", "Analyst@123!", "Analyst"),

//        ("uphcuser", "uphc@umsu.com", "UPHC@123!", "UPHCUser"),

//        ("chcuser", "chc@umsu.com", "CHC@123!", "CHCUser"),

//        ("uhwcuser", "uhwc@umsu.com", "UHWC@123!", "UHWCUser"),

//        ("nodal", "nodal@umsu.com", "Nodal@123!", "NodalOfficer"),

//        ("addlncomm", "addln@umsu.com", "Addln@123!", "AddlnCommissioner"),

//        ("md", "md@umsu.com", "MD@123!", "MD"),

//        ("commissioner", "comm@umsu.com", "Comm@123!", "Commissioner"),

//        ("jdadmin", "jd@umsu.com", "JD@123!", "JdAdmin")
//    };

//    foreach (var u in usersToSeed)
//    {
//        var existingUser =
//            await userManager.FindByNameAsync(u.Username);

//        if (existingUser == null)
//        {
//            int? facilityId = null;

//            // Facility mapping

//            if (u.Role == "CHCUser")
//            {
//                facilityId = 1;
//            }
//            else if (u.Role == "UPHCUser")
//            {
//                facilityId = 24;
//            }
//            else if (u.Role == "UHWCUser")
//            {
//                facilityId = 27;
//            }

//            var user = new ApplicationUser
//            {
//                UserName = u.Username,

//                Email = u.Email,

//                EmailConfirmed = true,

//                FacilityId = facilityId
//            };

//            var result = await userManager.CreateAsync(
//                user,
//                u.Password
//            );

//            if (result.Succeeded)
//            {
//                await userManager.AddToRoleAsync(
//                    user,
//                    u.Role
//                );
//            }
//        }
//    }
//}

//app.Run();











































//using Microsoft.AspNetCore.Identity;
//using Microsoft.EntityFrameworkCore;
//using UPHC.SurveillanceDashboard.Data;
//using UPHC.SurveillanceDashboard.Hubs;
//using UPHC.SurveillanceDashboard.Models;
//using UPHC.SurveillanceDashboard.Services;

//var builder = WebApplication.CreateBuilder(args);

//// =========================
//// SERVICES
//// =========================

//builder.Services.AddRazorPages();

//builder.Services.AddServerSideBlazor();

//// PostgreSQL
//builder.Services.AddDbContextFactory<AppDbContext>(options =>
//    options.UseNpgsql(
//        builder.Configuration.GetConnectionString("DefaultConnection")
//    ));

//// Identity + Roles
//builder.Services.AddDefaultIdentity<ApplicationUser>(options =>
//{
//    options.SignIn.RequireConfirmedAccount = false;
//})
//.AddRoles<IdentityRole>()
//.AddEntityFrameworkStores<AppDbContext>();

//// SignalR
//builder.Services.AddSignalR();

//// Custom Services
//builder.Services.AddScoped<NotificationService>();

//var app = builder.Build();


//// =========================
//// PIPELINE
//// =========================

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


//// =========================
//// DEV ONLY MIGRATION + SEEDING
//// =========================

//if (app.Environment.IsDevelopment())
//{
//    using var scope = app.Services.CreateScope();

//    var services = scope.ServiceProvider;

//    var context =
//        services.GetRequiredService<AppDbContext>();

//    var roleManager =
//        services.GetRequiredService<RoleManager<IdentityRole>>();

//    var userManager =
//        services.GetRequiredService<UserManager<ApplicationUser>>();

//    // =========================
//    // APPLY MIGRATIONS
//    // =========================

//    await context.Database.MigrateAsync();


//    // =========================
//    // ROLES
//    // =========================

//    string[] roles =
//    {
//        "Admin",
//        "Analyst",
//        "UPHCUser",
//        "CHCUser",
//        "UHWCUser",
//        "NodalOfficer",
//        "AddlnCommissioner",
//        "MD",
//        "Commissioner",
//        "JdAdmin"
//    };

//    foreach (var role in roles)
//    {
//        if (!await roleManager.RoleExistsAsync(role))
//        {
//            await roleManager.CreateAsync(
//                new IdentityRole(role));
//        }
//    }


//    // =========================
//    // ADMIN USER
//    // =========================

//    var adminUser =
//        await userManager.FindByNameAsync("admin");

//    if (adminUser == null)
//    {
//        var user = new ApplicationUser
//        {
//            UserName = "admin",
//            Email = "admin@umsu.com",
//            EmailConfirmed = true
//        };

//        var result = await userManager.CreateAsync(
//            user,
//            "Admin@123!"
//        );

//        if (result.Succeeded)
//        {
//            await userManager.AddToRoleAsync(
//                user,
//                "Admin"
//            );
//        }
//    }


//    // =========================
//    // DEV USERS
//    // =========================

//    var usersToSeed = new List<
//        (string Username,
//         string Email,
//         string Password,
//         string Role)>
//    {
//        ("analyst", "analyst@umsu.com", "Analyst@123!", "Analyst"),

//        ("uphcuser", "uphc@umsu.com", "UPHC@123!", "UPHCUser"),

//        ("chcuser", "chc@umsu.com", "CHC@123!", "CHCUser"),

//        ("uhwcuser", "uhwc@umsu.com", "UHWC@123!", "UHWCUser"),

//        ("nodal", "nodal@umsu.com", "Nodal@123!", "NodalOfficer"),

//        ("addlncomm", "addln@umsu.com", "Addln@123!", "AddlnCommissioner"),

//        ("md", "md@umsu.com", "MD@123!", "MD"),

//        ("commissioner", "comm@umsu.com", "Comm@123!", "Commissioner"),

//        ("jdadmin", "jd@umsu.com", "JD@123!", "JdAdmin")
//    };

//    foreach (var u in usersToSeed)
//    {
//        var existingUser =
//            await userManager.FindByNameAsync(u.Username);

//        if (existingUser == null)
//        {
//            int? facilityId = null;

//            // Facility mapping

//            if (u.Role == "CHCUser")
//            {
//                facilityId = 1;
//            }
//            else if (u.Role == "UPHCUser")
//            {
//                facilityId = 24;
//            }

//            var user = new ApplicationUser
//            {
//                UserName = u.Username,

//                Email = u.Email,

//                EmailConfirmed = true,

//                FacilityId = facilityId
//            };

//            var result = await userManager.CreateAsync(
//                user,
//                u.Password
//            );

//            if (result.Succeeded)
//            {
//                await userManager.AddToRoleAsync(
//                    user,
//                    u.Role
//                );
//            }
//        }
//    }
//}

//app.Run();


































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
