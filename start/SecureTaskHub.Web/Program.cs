using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using SecureTaskHub.Domain.Interfaces;
using SecureTaskHub.Infrastructure.Data;
using SecureTaskHub.Infrastructure.Repositories;
using SecureTaskHub.Web.Services;
// Task 3 — add these when you create Requirements/ and AuthHandlers/ folders:
// using SecureTaskHub.Web.AuthHandlers;
// using SecureTaskHub.Web.Requirements;
// Task 6 — add this when you create the Factories/ folder:
// using SecureTaskHub.Web.Factories;

var builder = WebApplication.CreateBuilder(args);

// =============================================================================
// WEB APPLICATION with COOKIE-BASED AUTHENTICATION
// =============================================================================
// This web app uses cookie authentication (different from the API's JWT tokens)
// - Users log in via forms
// - Authentication cookie is stored in browser
// - Cookie is sent with each request to maintain session
// =============================================================================

// Add Database Context
builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    options.UseSqlite(
        builder.Configuration.GetConnectionString("DefaultConnection")
        ?? "Data Source=SecureTaskHubDb.sqlite"
    );
});

// Add ASP.NET Identity with Cookie Authentication
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // TODO Secure: Same weak password policy as API (for consistency in demo)
    options.Password.RequireDigit = false; // Should be true
    options.Password.RequireLowercase = false; // Should be true
    options.Password.RequireUppercase = false; // Should be true
    options.Password.RequireNonAlphanumeric = false; // Should be true
    options.Password.RequiredLength = 6; // Should be 12+

    // TODO Secure: Enable lockout
    options.Lockout.AllowedForNewUsers = false; // Should be true

    // TODO Secure: Require email confirmation
    options.SignIn.RequireConfirmedEmail = false;

    // Token lifespan for password reset, email confirmation, etc.
    options.Tokens.PasswordResetTokenProvider = TokenOptions.DefaultProvider;
    options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultProvider;
})
.AddEntityFrameworkStores<ApplicationDbContext>()
.AddDefaultTokenProviders()
.AddDefaultUI(); // Provides default Identity UI pages

// Configure cookie authentication settings
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Identity/Account/Login";
    options.LogoutPath = "/Identity/Account/Logout";
    options.AccessDeniedPath = "/Identity/Account/AccessDenied";

    // TODO Secure: Tighten cookie security settings
    options.Cookie.HttpOnly = true; // Good: Prevents JavaScript access
    options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Good: HTTPS only
    options.ExpireTimeSpan = TimeSpan.FromDays(1); // TODO Secure: Consider shorter expiration
    options.SlidingExpiration = true; // Renews cookie on activity
});

// Add Repository
builder.Services.AddScoped<ITaskItemRepository, TaskItemRepository>();

// Add Email Sender service for password reset
builder.Services.AddTransient<IEmailSender, EmailSender>();

// Add Razor Pages
builder.Services.AddRazorPages(options =>
{
    // Require authentication by default for all pages
    options.Conventions.AuthorizeFolder("/");

    // Allow anonymous access to specific pages
    options.Conventions.AllowAnonymousToPage("/Index");
    // Task 6 (Part D): Comment out this line when testing RequireITDepartment on Privacy.
    // AllowAnonymous conventions override [Authorize] attributes — the test will not work without removing this.
    // Restore this line after testing and remove [Authorize] from PrivacyModel.
    options.Conventions.AllowAnonymousToPage("/Privacy");
    options.Conventions.AllowAnonymousToFolder("/Identity");

    // TODO Secure: Properly restrict admin pages
    // Currently relying on code-level checks, but should use:
    // options.Conventions.AuthorizeFolder("/Admin", "RequireAdminRole");
});

// Add Authorization
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAdminRole", policy =>
        policy.RequireRole("Admin"));
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}
else
{
    // Auto-apply migrations in development
    // TODO Secure: Remove in production
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();

app.UseRouting();

// =============================================================================
// MIDDLEWARE ORDER IS CRITICAL!
// =============================================================================
// 1. Authentication: Reads authentication cookie
// 2. Authorization: Enforces [Authorize] attributes and policies
app.UseAuthorization();

app.MapStaticAssets();
app.MapRazorPages()
   .WithStaticAssets();

app.Run();
