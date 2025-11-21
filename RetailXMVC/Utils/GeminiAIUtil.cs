using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace RetailXMVC.Utils
{
    public static class GeminiAIUtil
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public static async Task<string> AnalyzeFinancialData(string prompt, IConfiguration configuration)
        {
            try
            {
                var apiKey = configuration["GeminiAI:ApiKey"];
                var model = configuration["GeminiAI:Model"] ?? "gemini-2.0-flash";

                if (string.IsNullOrEmpty(apiKey))
                {
                    return "⚠️ Chưa cấu hình API Key cho Gemini AI trong appsettings.json";
                }

                var requestUrl = $"https://generativelanguage.googleapis.com/v1beta/models/{model}:generateContent?key={apiKey}";

                var requestBody = new
                {
                    contents = new[]
                    {
                        new
                        {
                            parts = new[]
                            {
                                new { text = prompt }
                            }
                        }
                    },
                    generationConfig = new
                    {
                        temperature = 0.7,
                        maxOutputTokens = 2048,
                        topK = 40,
                        topP = 0.95
                    }
                };

                var json = JsonConvert.SerializeObject(requestBody);
                var content = new StringContent(json, Encoding.UTF8, "application/json");

                var response = await _httpClient.PostAsync(requestUrl, content);

                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"❌ Gemini API Error: {errorContent}");
                    return "⚠️ Không thể kết nối với Gemini AI. Vui lòng kiểm tra API Key hoặc quota.";
                }

                var responseBody = await response.Content.ReadAsStringAsync();
                var result = JObject.Parse(responseBody);

                var aiResponse = result["candidates"]?[0]?["content"]?["parts"]?[0]?["text"]?.ToString();

                return aiResponse ?? "⚠️ AI không trả về phân tích.";
            }
            catch (HttpRequestException ex)
            {
                Console.WriteLine($"❌ HTTP Error: {ex.Message}");
                return "⚠️ Lỗi kết nối mạng. Vui lòng kiểm tra Internet.";
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error calling Gemini: {ex.Message}");
                return $"⚠️ Lỗi không xác định: {ex.Message}";
            }
        }
    }
}