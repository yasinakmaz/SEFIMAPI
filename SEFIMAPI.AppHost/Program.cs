// Process'in en baþýnda, herhangi bir Aspire kodu çalýþmadan önce
// environment variable'larý set edelim
Console.WriteLine("Aspire Environment Initialization baþlatýlýyor...");

// Environment detection - bu çok erken yapýlmalý
var isDevelopment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development";

if (!isDevelopment)
{
    // Environment variable'larý process baþlangýcýnda set et
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");
    Environment.SetEnvironmentVariable("ASPNETCORE_URLS", "https://localhost:18888");
    Environment.SetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL", "http://localhost:19999");
    Environment.SetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL", "http://localhost:19998");
    Environment.SetEnvironmentVariable("ASPIRE_ALLOW_UNSECURED_TRANSPORT", "true");

    Console.WriteLine("Production environment variables configured");
    Console.WriteLine("Dashboard minimal configuration applied");
}

// Þimdi Aspire builder'ý initialize edebiliriz
// Bu noktada environment variable'lar hazýr
var builder = DistributedApplication.CreateBuilder(args);

// API servisinizi normal þekilde tanýmlayýn
var apiService = builder.AddProject<Projects.SEFIMAPI>("sefimapi")
    .WithHttpEndpoint(port: 5001, name: "api");

Console.WriteLine("SEFIMAPI servisi port 5001'de configuration edildi");

var app = builder.Build();

// Status information
if (!isDevelopment)
{
    Console.WriteLine("=== Production Mode Baþarýyla Baþlatýldý ===");
    Console.WriteLine("Primary API Service: https://localhost:5001");
    Console.WriteLine("Dashboard (background): https://localhost:18888");
    Console.WriteLine("MAUI uygulamanýz API endpoint'ine baðlanabilir");
    Console.WriteLine("Sistem hazýr - kapatmak için Ctrl+C");
    Console.WriteLine("==========================================");
}

app.Run();