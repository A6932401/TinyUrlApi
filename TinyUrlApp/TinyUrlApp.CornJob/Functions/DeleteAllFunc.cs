using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using TinyUrlApp.CornJob.Model;

namespace TinyUrlApp.CornJob.Functions;

public class DeleteAllFunc
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<DeleteAllFunc> _logger;

    public DeleteAllFunc(
        IHttpClientFactory httpClientFactory,
        ILogger<DeleteAllFunc> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    // Runs every 1 hour → "0 0 * * * *"
    [Function("DeleteAllLinksFunction")]
    public async Task Run([TimerTrigger("0 0 * * * *")] TimerInfo timerInfo)
    {
        _logger.LogInformation("DeleteAllLinksFunction triggered at: {Time}", DateTime.UtcNow);

        var result = new CronJobResult
        {
            TriggeredAt = DateTime.UtcNow
        };

        try
        {
            var client = _httpClientFactory.CreateClient("TinyUrlApi");

            var response = await client.DeleteAsync("/api/link/All");

            result.StatusCode = (int)response.StatusCode;
            result.IsSuccess = response.IsSuccessStatusCode;

            if (response.IsSuccessStatusCode)
            {
                _logger.LogInformation(
                    "All links deleted successfully. Status: {StatusCode} at {Time}",
                    response.StatusCode, DateTime.UtcNow);
            }
            else
            {
                var errorBody = await response.Content.ReadAsStringAsync();
                result.ErrorMessage = errorBody;

                _logger.LogWarning(
                    "Delete failed. Status: {StatusCode}, Body: {Body}",
                    response.StatusCode, errorBody);
            }
        }
        catch (Exception ex)
        {
            result.IsSuccess = false;
            result.ErrorMessage = ex.Message;

            _logger.LogError(ex,
                "Exception in DeleteAllLinksFunction at: {Time}", DateTime.UtcNow);

            throw; // rethrow so Azure marks this run as failed
        }
        finally
        {
            _logger.LogInformation(
                "CronJob Result → Success: {IsSuccess}, StatusCode: {StatusCode}, Error: {Error}",
                result.IsSuccess, result.StatusCode, result.ErrorMessage ?? "none");
        }
    }
}