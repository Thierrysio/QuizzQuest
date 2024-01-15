
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.Maui.Controls.Compatibility;

using Forge.OpenAI;
using System.Net.Http.Headers;



namespace QuizzQuest.OpensAIS
{
    public class OpenAIService
    {
        string apiKey = "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";
        private readonly HttpClient _httpClient;

        public OpenAIService()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://api.openai.com/v1/") // URL de base de l'API OpenAI
            };

            // Remplacez par votre clé API d'OpenAI
            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "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");
            _httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
        }

        public async Task<string> AskQuestionAsync(string prompt)
        {
            var requestData = new
            {
                model = "text-davinci-003", // Modèle GPT à utiliser
                prompt = prompt,
                temperature = 0.5,
                max_tokens = 1500,
                top_p = 1,
                frequency_penalty = 0,
                presence_penalty = 0
            };

            string jsonRequest = JsonSerializer.Serialize(requestData);
            var content = new StringContent(jsonRequest, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync("completions", content);

            if (response.IsSuccessStatusCode)
            {
                var jsonResponse = await response.Content.ReadAsStringAsync();
                var responseObject = JsonSerializer.Deserialize<OpenAIResponse>(jsonResponse);
                return responseObject?.choices?[0].text.Trim();
            }

            return "Erreur lors de la récupération de la réponse.";
        }

    }
}
