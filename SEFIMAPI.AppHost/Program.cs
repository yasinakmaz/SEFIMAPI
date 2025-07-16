using Microsoft.Extensions.Configuration;

Console.WriteLine("Aspire Ortam Başlatılıyor...");

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
    .Build();

var environment = config["Environment"] ?? "Production";
var isDevelopment = environment == "Development";

var httpsPort = config.GetValue<int>("AspireSettings:HttpsPort");
var dashboardPort = config.GetValue<int>("AspireSettings:DashboardPort");
var otlpUrl = config["AspireSettings:DashboardOtlpUrl"];
var otlpHttpUrl = config["AspireSettings:DashboardHttpUrl"];
var allowUnsecured = config.GetValue<bool>("AspireSettings:AllowUnsecuredTransport");

if (!isDevelopment)
{
    Environment.SetEnvironmentVariable("ASPNETCORE_ENVIRONMENT", "Production");
    // Burada tüm ağdan dinlemek için 0.0.0.0 kullanıyoruz
    Environment.SetEnvironmentVariable("ASPNETCORE_URLS", $"https://0.0.0.0:{dashboardPort};https://0.0.0.0:{httpsPort}");
    Environment.SetEnvironmentVariable("ASPNETCORE_HTTPS_PORT", httpsPort.ToString());
    Environment.SetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_ENDPOINT_URL", otlpUrl);
    Environment.SetEnvironmentVariable("ASPIRE_DASHBOARD_OTLP_HTTP_ENDPOINT_URL", otlpHttpUrl);
    Environment.SetEnvironmentVariable("ASPIRE_ALLOW_UNSECURED_TRANSPORT", allowUnsecured.ToString());

    Console.WriteLine("Üretim ortamı değişkenleri yapılandırmadan ayarlandı");
    Console.WriteLine($"Dashboard URL: https://0.0.0.0:{dashboardPort}");
    Console.WriteLine($"API URL: https://0.0.0.0:{httpsPort}");
}

var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.SEFIMAPI>("sefimapi")
    .WithHttpEndpoint(port: httpsPort, name: "api");

Console.WriteLine($"SEFIMAPI servisi {httpsPort} portunda yapılandırıldı");

var app = builder.Build();

if (!isDevelopment)
{
    Console.WriteLine("=== ÜRETİM MODU BAŞARILIYLA BAŞLATILDI ===");
    Console.WriteLine($"Ana API Servisi: https://<sunucu-ip>:{httpsPort}");
    Console.WriteLine($"Dashboard: https://<sunucu-ip>:{dashboardPort}");
    Console.WriteLine("MAUI uygulamanız API endpoint'ine bağlanabilir");
    Console.WriteLine("Sistem hazır - kapatmak için Ctrl+C");
    Console.WriteLine("==========================================");
}

app.Run();
