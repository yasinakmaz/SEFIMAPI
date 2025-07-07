// Process'in en ba��nda, herhangi bir Aspire kodu �al��madan �nce
// environment variable'lar� set edelim
Console.WriteLine("Aspire Environment Initialization ba�lat�l�yor...");

// Environment detection - bu �ok erken yap�lmal�
var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

if (!isDevelopment)
{
    // Environment variable'lar� process ba�lang�c�nda set et
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");
    Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "https://localhost:18888");
    Environment.SetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL", "http://localhost:19999");
    Environment.SetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL", "http://localhost:19998");
    Environment.SetEnvironmentVariable("ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true");

    Console.WriteLine("Production environment variables configured");
    Console.WriteLine("Dashboard minimal configuration applied");
}

// �imdi Aspire builder'� initialize edebiliriz
// Bu noktada environment variable'lar haz�r
var builder = DistributedApplication.CreateBuilder(args);

// API servisinizi normal �ekilde tan�mlay�n
var apiService = builder.AddProject<Projects.SEFIMAPI>("sefimapi")
    .WithHttpEndpoint(port: 5001, name: "api");

Console.WriteLine("SEFIMAPI servisi port 5001'de configuration edildi");

var app = builder.Build();

// Status information
if (!isDevelopment)
{
    Console.WriteLine("=== Production Mode Ba�ar�yla Ba�lat�ld� ===");
    Console.WriteLine("Primary API Service: https://localhost:5001");
    Console.WriteLine("Dashboard (background): https://localhost:18888");
    Console.WriteLine("MAUI uygulaman�z API endpoint'ine ba�lanabilir");
    Console.WriteLine("Sistem haz�r - kapatmak i�in Ctrl+C");
    Console.WriteLine("==========================================");
}

app.Run();