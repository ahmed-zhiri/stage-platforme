using Microsoft.EntityFrameworkCore;
using SIGSTO.Data;
using SIGSTO.Models;
using SIGSTO.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite("Data Source=sigsto.db"));

builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(60);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddScoped<CVScoringService>();
builder.Services.AddScoped<EmailService>();

var app = builder.Build();

// Apply migrations and seed data
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();

    if (!db.Gestionnaires.Any())
    {
        db.Gestionnaires.Add(new GestionnaireDesStages
        {
            Nom = "Admin",
            Prenom = "ONEE",
            Email = "gestionnaire@onee.ma",
            Password = BCryptHash("gestionnaire123"),
            EmailVerifie = true,
            Role = RoleUtilisateur.Gestionnaire
        });
        db.SaveChanges();
    }

    if (!db.Encadrants.Any())
    {
        db.Encadrants.Add(new Encadrant
        {
            Nom = "El horre",
            Prenom = "Abdellilah",
            Email = "encadrant@onee.ma",
            Password = BCryptHash("encadrant123"),
            EmailVerifie = true,
            Role = RoleUtilisateur.Encadrant,
            Departement = "Informatique"
        });
        db.SaveChanges();
    }
}

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}

app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}");

app.Run();

// Simple hash function (not BCrypt, just SHA256 for this version)
static string BCryptHash(string password)
{
    using var sha = System.Security.Cryptography.SHA256.Create();
    var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
    return Convert.ToBase64String(bytes);
}
