using AmongServers.Plugin.Coordinator.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AmongServers.Plugin.Coordinator
{
    /// <summary>
    /// Provides functionality to request data from the API.
    /// </summary>
    public class ApiClient
    {
        private HttpClient _client;

        /// <summary>
        /// The timeout for requests.
        /// </summary>
        public TimeSpan Timeout {
            get {
                return _client.Timeout;
            }
            set {
                _client.Timeout = value;
            }
        }

        /// <summary>
        /// Gets the banner(s) from the REST API, should either contain the project link or a link to update if the provided version is out of date.
        /// </summary>
        /// <param name="entity">The heartbeat entity.</param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task HeartbeatAsync(HeartbeatEntity entity, CancellationToken cancellationToken = default)
        {
            HttpResponseMessage responseMessage = await _client.PostAsync($"heartbeat", new StringContent(JsonSerializer.Serialize(entity), Encoding.UTF8), cancellationToken);

            if (responseMessage.IsSuccessStatusCode) {
                responseMessage.Dispose();
            } else {
                throw new Exception($"The service returned an error: {responseMessage.StatusCode}");
            }

            throw new Exception("The service is unavailable, try again later");
        }

        /// <summary>
        /// Creates an API client.
        /// </summary>
        /// <param name="apiUrl">The API client.</param>
        public ApiClient(string apiUrl)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(apiUrl);
        }
    }
}
