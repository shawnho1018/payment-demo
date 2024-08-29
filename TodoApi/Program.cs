using OpenTelemetry; // 引入 OpenTelemetry 相關的命名空間
using OpenTelemetry.Logs;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Exporter;
using Npgsql; // 引入 PostgreSQL 資料庫相關的命名空間
using TodoApi.Helpers; // 引入自訂的輔助類別
using TodoApi.Services; // 引入自訂的服務類別

var builder = WebApplication.CreateBuilder(args); // 建立 WebApplicationBuilder 實例

// 從配置中取得服務名稱、版本和 OpenTelemetry 端點地址
var serviceName = builder.Configuration.GetValue("BuildInfo:ServiceName", defaultValue: "todo-api");
var serviceVersion = builder.Configuration.GetValue("BuildInfo:ServiceVersion", defaultValue: "v1.0.0");
var otelEndpoint = builder.Configuration.GetValue("Otlp:Endpoint", defaultValue: "http://localhost:4317");
// var otelEndpoint = Environment.GetEnvironmentVariable("OTEL_EXPORTER_OTLP_ENDPOINT");

// 檢查 OpenTelemetry 端點地址是否為空
if (string.IsNullOrEmpty(otelEndpoint))
{
    // 如果為空，則輸出警告訊息
    Console.WriteLine("Warning: OTEL_EXPORTER_OTLP_ENDPOINT environment variable not set. Traces and metrics will not be exported.");
}
else
{
    // 如果不為空，則配置 OpenTelemetry 日誌記錄
    builder.Logging.AddOpenTelemetry(options =>
    {
        options
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault() // 建立預設的資源建構器
                .AddService(serviceName: serviceName, serviceVersion: serviceVersion)) // 添加服務名稱和版本資訊
                .AddOtlpExporter(options =>
                {
                    options.Endpoint = new Uri($"{otelEndpoint}/v1/logs"); // 設定 OpenTelemetry 日誌導出器的端點地址
                });
    });
    Console.WriteLine($"otelEndpoint: {otelEndpoint}"); // 輸出 OpenTelemetry 端點地址

    // 配置 OpenTelemetry 追蹤和指標
    builder.Services.AddOpenTelemetry()
        .ConfigureResource(resource => resource
        .AddService(serviceName: serviceName, serviceVersion: serviceVersion)) // 添加服務名稱和版本資訊
        .WithTracing(tracing => tracing
            .AddHttpClientInstrumentation(options =>
            {
                options.RecordException = true; // 記錄異常訊息
            })
            .AddAspNetCoreInstrumentation(options =>
            {
                options.RecordException = true; // 記錄異常訊息
            })
            .AddSource("NPGSQL") // 添加 Npgsql 資料來源
            .AddNpgsql() // 添加 Npgsql 追蹤
            // .AddSqlClientInstrumentation(options =>
            // {
            //     options.SetDbStatementForText = true;
            //     options.RecordException = true;
            // })
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri($"{otelEndpoint}/v1/traces"); // 設定 OpenTelemetry 追蹤導出器的端點地址
                options.Protocol = OtlpExportProtocol.Grpc; // 設定 OpenTelemetry 追蹤導出器使用的協定
            })
            .SetSampler(new AlwaysOnSampler())) // 設定採樣器為始終開啟
        .WithMetrics(metrics => metrics
            .AddRuntimeInstrumentation() // 添加執行時指標
            .AddHttpClientInstrumentation() // 添加 HttpClient 指標
            .AddProcessInstrumentation() // 添加進程指標
            .AddAspNetCoreInstrumentation() // 添加 ASP.NET Core 指標
            .AddMeter("Npgsql", "Microsoft.AspNetCore.Hosting", "Microsoft.AspNetCore.Server.Kestrel", "TodoApi.Controllers.OrderController") // 添加 Npgsql、ASP.NET Core Hosting 和 Kestrel 指標
            .AddView("http.server.request.duration",
            new ExplicitBucketHistogramConfiguration
            {
                Boundaries = new double[] { 0, 0.005, 0.01, 0.025, 0.05,
                       0.075, 0.1, 0.25, 0.5, 0.75, 1, 2.5, 5, 7.5, 10 } // 設定 HTTP 伺服器請求持續時間的直方圖邊界
            })
            .AddOtlpExporter(options =>
            {
                options.Endpoint = new Uri($"{otelEndpoint}/v1/metrics"); // 設定 OpenTelemetry 指標導出器的端點地址
                options.Protocol = OtlpExportProtocol.Grpc; // 設定 OpenTelemetry 指標導出器使用的協定
            }));
}

// 添加服務到容器
builder.Services.AddHealthChecks(); // 添加健康檢查服務
builder.Services.AddControllers(); // 添加控制器服務
builder.Services.AddScoped<OrderService>(); // 添加 OrderService 服務

// 配置日誌記錄
builder.Host.ConfigureLogging(logging =>
{
    logging.ClearProviders(); // 清除所有日誌提供程式
    logging.AddConsole(); // 添加控制台日誌提供程式
});

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer(); // 添加 API 資源管理器服務
builder.Services.AddSwaggerGen(); // 添加 Swagger/OpenAPI 生成器服務

var app = builder.Build(); // 建構 WebApplication 實例

// 配置 HTTP 請求管道
app.MapHealthChecks("/healthz"); // 將健康檢查映射到 /healthz 路徑

if (app.Environment.IsDevelopment()) // 如果是開發環境
{
    app.UseSwagger(); // 使用 Swagger 中間件
    app.UseSwaggerUI(); // 使用 Swagger UI 中間件
}

//app.UseHttpsRedirection(); // 使用 HTTPS 重定向中間件

app.UseAuthorization(); // 使用授權中間件
app.UseMiddleware<ErrorHandlerMiddleware>(); // 使用自訂的錯誤處理中間件
app.MapControllers(); // 映射控制器

app.Run(); // 運行應用程式
