using Impostor.Api.Games;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AmongServers.Plugin
{
    /// <summary>
    /// Defines host to communicate with the API.
    /// </summary>
    public class APIClient
    {
        #region Fields
        private readonly HttpClient _client;
        private const string _endpoint = "https://localhost:5001";
        private readonly string _name;
        private readonly int _port;
        #endregion

        #region Methods
        /// <summary>
        /// Builds the J
        /// </summary>
        /// <returns></returns>
        public Task<HttpResponseMessage> SendHeartbeatAsync(IGame game = null)
        {
            if (game == null)
                return _client.PostAsync("heartbeat", new StringContent($"{{\"port\":{_port},\"name\":\"{_name}\"}}", Encoding.UTF8, "application/json"));
            else
                return _client.PostAsync("heartbeat", new StringContent($"{{\"port\":{_port},\"name\":\"{_name}\",\"code\":\"{game.Code}\",\"players\":{game.PlayerCount},\"state\":\"{game.GameState.ToString()}\"}}", Encoding.UTF8, "application/json"));
        }
        #endregion

        #region Constructor
        public APIClient(string name, int port)
        {
            _client = new HttpClient();
            _client.BaseAddress = new Uri(_endpoint);
            _name = name;
            _port = port;
        }
        #endregion
    }
}
