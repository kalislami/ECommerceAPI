using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using ECommerceApi.DTOs;
using ECommerceApi.Helpers;
using Microsoft.Extensions.Options;

namespace ECommerceApi.Services
{
    public class MidtransService
    {
        private readonly HttpClient _httpClient;
        private readonly MidtransSettings _midtransSettings;

        public MidtransService(HttpClient httpClient, IOptions<MidtransSettings> options)
        {
            _httpClient = httpClient;
            _midtransSettings = options.Value;

            _httpClient.BaseAddress = new Uri(_midtransSettings.BaseAddress);
            var byteKey = Encoding.UTF8.GetBytes(_midtransSettings.ServerKey + ":");
            _httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Basic", Convert.ToBase64String(byteKey));
        }

        public async Task<PaymentResponse?> CreateTransactionAsync(PaymentRequest request)
        {
            var payload = new
            {
                payment_type = "gopay",
                transaction_details = new
                {
                    order_id = request.OrderId,
                    gross_amount = request.GrossAmount
                },
                customer_details = new
                {
                    email = request.CustomerEmail
                }
            };

            try
            {
                var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync("charge", content);
                var responseJson = await response.Content.ReadAsStringAsync();
                if (!response.IsSuccessStatusCode)
                {
                    Console.WriteLine("errornya:");
                    Console.WriteLine(responseJson);
                    return null;
                }

                using var doc = JsonDocument.Parse(responseJson);
                var root = doc.RootElement;

                string redirectUrl = "";
                if (root.TryGetProperty("actions", out var actionsArray) && actionsArray.GetArrayLength() > 0)
                {
                    foreach (var action in actionsArray.EnumerateArray())
                    {
                        if (action.GetProperty("name").GetString() == "deeplink-redirect")
                        {
                            redirectUrl = action.GetProperty("url").GetString() ?? "";
                            break;
                        }
                    }
                }

                return new PaymentResponse
                {
                    Message = root.GetProperty("status_message").GetString() ?? "",
                    RedirectUrl = redirectUrl
                };
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception when calling Midtrans: {ex.Message}");
                return null;
            }

        }
    }
}