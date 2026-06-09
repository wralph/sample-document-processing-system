using DocumentProcessor.Web.Components;
using DocumentProcessor.Web.Data;
using DocumentProcessor.Web.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorComponents().AddInteractiveServerComponents();

// Configure database connection
var dbInfo = new DatabaseInfoService();
string connectionString = string.Empty;
try
{
    var secretsService = new SecretsService();
    string secretJson;

    // First: Try to get secret with "target" in name (Postgres)
    try
    {
        secretJson = await secretsService.GetSecretAsync("arn:aws:secretsmanager:us-east-1:179438214058:secret:atx-db-modernization-atx-db-modernization-1-target-spRD4D");
        if (!string.IsNullOrWhiteSpace(secretJson))
        {
            var username = secretsService.GetFieldFromSecret(secretJson, "username");
            var password = secretsService.GetFieldFromSecret(secretJson, "password");
            var host = secretsService.GetFieldFromSecret(secretJson, "host");
            var port = secretsService.GetFieldFromSecret(secretJson, "port");
            var dbname = "postgres";
            connectionString = $"Host={host};Port={port};Database={dbname};Username={username};Password={password};SSL Mode=Require;Trust Server Certificate=true";
            dbInfo.DatabaseType = "PostgreSQL"; dbInfo.SecretName = "arn:aws:secretsmanager:us-east-1:179438214058:secret:atx-db-modernization-atx-db-modernization-1-target-spRD4D"; dbInfo.HostAddress = $"{host}:{port}";
        }
        else throw new Exception("Secret was empty");
    }
    catch
    {
        throw;
    }
}
catch (Exception ex)
{
    Console.WriteLine($"Warning: Could not load connection string from AWS Secrets Manager: {ex.Message}");
    Console.WriteLine("Falling back to appsettings.json connection string");
    connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? "Host=localhost;Port=5432;Database=postgres;Username=postgres;Password=";
    dbInfo.DatabaseType = "PostgreSQL (Local)"; dbInfo.SecretName = "appsettings.json"; dbInfo.HostAddress = "localhost";
}

builder.Services.AddDbContext<AppDbContext>(o => o.UseNpgsql(connectionString));
builder.Services.AddSingleton(dbInfo);
builder.Services.AddScoped<FileStorageService>();
builder.Services.AddScoped<AIService>();
builder.Services.AddScoped<DocumentProcessingService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await scope.ServiceProvider.GetRequiredService<AppDbContext>().Database.EnsureCreatedAsync();
}

if (app.Environment.IsDevelopment())
    app.UseDeveloperExceptionPage();
else
    app.UseExceptionHandler("/Error");

app.UseHttpsRedirection();
app.UseAntiforgery();
app.UseStaticFiles();
app.MapRazorComponents<App>().AddInteractiveServerRenderMode();

app.Run();
