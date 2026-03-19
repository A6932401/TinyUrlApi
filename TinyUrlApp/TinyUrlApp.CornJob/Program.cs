using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWebApplication()
    .ConfigureServices(services =>
    {
        services.AddHttpClient("TinyUrlApi", client =>
        {
            client.BaseAddress = new Uri(
                Environment.GetEnvironmentVariable("ApiBaseUrl")
                    ?? throw new InvalidOperationException("ApiBaseUrl is not configured.")
            );
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });
    })
    .Build();

host.Run();