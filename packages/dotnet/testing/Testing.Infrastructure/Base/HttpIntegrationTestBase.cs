using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using Waggle.Common.Models;
using Xunit;

namespace Waggle.Testing.Infrastructure.Base
{
    public abstract class HttpIntegrationTestBase : IAsyncLifetime
    {
        protected readonly HttpClient Client;
        protected readonly IServiceProvider Services;

        protected static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            Converters = { new JsonStringEnumConverter(allowIntegerValues: true) },
            ReadCommentHandling = JsonCommentHandling.Skip
        };

        protected HttpIntegrationTestBase(IServiceProvider services, HttpClient client)
        {
            Services = services;
            Client = client;
        }

        public virtual Task InitializeAsync() => Task.CompletedTask;
        public virtual Task DisposeAsync() => Task.CompletedTask;

        #region Endpoint Helpers

        protected virtual string GetEndpoint(string path) => $"/api/{path.TrimStart('/')}";

        #endregion

        #region Core HTTP Helpers

        protected async Task<ApiResponse<T>> SendRequestAsync<T>(HttpRequestMessage request, bool expectSuccess = true)
        {
            var response = await Client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (expectSuccess && !response.IsSuccessStatusCode)
                response.EnsureSuccessStatusCode();

            return JsonSerializer.Deserialize<ApiResponse<T>>(json, JsonOptions)
                   ?? throw new InvalidOperationException("Failed to deserialize response");
        }

        protected async Task<ApiResponse> SendRequestAsync(HttpRequestMessage request, bool expectSuccess = true)
        {
            var response = await Client.SendAsync(request);
            var json = await response.Content.ReadAsStringAsync();

            if (expectSuccess && !response.IsSuccessStatusCode)
                response.EnsureSuccessStatusCode();

            return JsonSerializer.Deserialize<ApiResponse>(json, JsonOptions)
                   ?? throw new InvalidOperationException("Failed to deserialize response");
        }

        #endregion

        #region HTTP Verb Helpers

        protected Task<ApiResponse<TDto>> GetAsync<TDto>(string endpoint, bool expectSuccess = true)
            => SendRequestAsync<TDto>(
                new HttpRequestMessage(HttpMethod.Get, GetEndpoint(endpoint)),
                expectSuccess);

        protected Task<ApiResponse> GetAsync(string endpoint, bool expectSuccess = true)
            => SendRequestAsync(
                new HttpRequestMessage(HttpMethod.Get, GetEndpoint(endpoint)),
                expectSuccess);

        protected Task<ApiResponse<TDto>> PostAsync<TDto, TCreateDto>(string endpoint, TCreateDto dto, bool expectSuccess = true)
            => SendRequestAsync<TDto>(
                new HttpRequestMessage(HttpMethod.Post, GetEndpoint(endpoint))
                {
                    Content = JsonContent.Create(dto, options: JsonOptions)
                },
                expectSuccess);

        protected Task<ApiResponse> PostAsync<TCreateDto>(string endpoint, TCreateDto dto, bool expectSuccess = true)
            => SendRequestAsync(
                new HttpRequestMessage(HttpMethod.Post, GetEndpoint(endpoint))
                {
                    Content = JsonContent.Create(dto, options: JsonOptions)
                },
                expectSuccess);

        protected Task<ApiResponse<TDto>> PutAsync<TDto, TUpdateDto>(string endpoint, TUpdateDto dto, bool expectSuccess = true)
            => SendRequestAsync<TDto>(
                new HttpRequestMessage(HttpMethod.Put, GetEndpoint(endpoint))
                {
                    Content = JsonContent.Create(dto, options: JsonOptions)
                },
                expectSuccess);

        protected Task<ApiResponse> PutAsync<TUpdateDto>(string endpoint, TUpdateDto dto, bool expectSuccess = true)
            => SendRequestAsync(
                new HttpRequestMessage(HttpMethod.Put, GetEndpoint(endpoint))
                {
                    Content = JsonContent.Create(dto, options: JsonOptions)
                },
                expectSuccess);

        protected Task<ApiResponse<TDto>> PatchAsync<TDto, TPatchDto>(string endpoint, TPatchDto dto, bool expectSuccess = true)
            => SendRequestAsync<TDto>(
                new HttpRequestMessage(HttpMethod.Patch, GetEndpoint(endpoint))
                {
                    Content = JsonContent.Create(dto, options: JsonOptions)
                },
                expectSuccess);

        protected Task<ApiResponse> PatchAsync<TPatchDto>(string endpoint, TPatchDto dto, bool expectSuccess = true)
            => SendRequestAsync(
                new HttpRequestMessage(HttpMethod.Patch, GetEndpoint(endpoint))
                {
                    Content = JsonContent.Create(dto, options: JsonOptions)
                },
                expectSuccess);

        protected Task<ApiResponse<TDto>> DeleteAsync<TDto>(string endpoint, bool expectSuccess = true)
            => SendRequestAsync<TDto>(
                new HttpRequestMessage(HttpMethod.Delete, GetEndpoint(endpoint)),
                expectSuccess);

        protected Task<ApiResponse> DeleteAsync(string endpoint, bool expectSuccess = true)
            => SendRequestAsync(
                new HttpRequestMessage(HttpMethod.Delete, GetEndpoint(endpoint)),
                expectSuccess);

        #endregion
    }
}