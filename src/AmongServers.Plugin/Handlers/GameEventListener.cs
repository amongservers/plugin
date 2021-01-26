using Impostor.Api.Events;
using Impostor.Api.Games;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AmongServers.Plugin.Handlers
{
    public class GameEventListener : IEventListener
    {
        #region Fields
        private readonly ILogger<AmongServersPlugin> _logger;
        private readonly APIClient _client;
        private string _name;
        #endregion

        #region Methods
        /// <summary>
        /// Adds a new pending lobby.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [EventListener]
        public async ValueTask OnGameCreated(IGameCreatedEvent e)
        {
            var response = await _client.SendHeartbeatAsync(e.Game).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                _logger.LogError($"Failed to inform server of new lobby: {response.StatusCode}");
        }

        /// <summary>
        /// Moves the lobby from pending to active. 
        /// </summary>
        /// <param name="e"></param>
        [EventListener]
        public async ValueTask OnGameStarted(IGameStartedEvent e)
        {
            var response = await _client.SendHeartbeatAsync(e.Game).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                _logger.LogError($"Failed to inform server of game start: {response.StatusCode}");
        }

        /// <summary>
        /// Move a lobby from active back to pending.
        /// </summary>
        /// <param name="e"></param>
        [EventListener]
        public async ValueTask OnGameEnded(IGameEndedEvent e)
        {
            var response = await _client.SendHeartbeatAsync(e.Game).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                _logger.LogError($"Failed to inform server of game end: {response.StatusCode}");
        }

        /// <summary>
        /// Remove the lobby.
        /// </summary>
        /// <param name="e"></param>
        [EventListener]
        public async ValueTask OnGameDestroyed(IGameDestroyedEvent e)
        {
            var response = await _client.SendHeartbeatAsync(e.Game).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                _logger.LogError($"Failed to inform server of game destory: {response.StatusCode}");
        }

        /// <summary>
        /// Increase the total player count.
        /// </summary>
        /// <param name="e"></param>
        [EventListener]
        public async ValueTask OnPlayerJoined(IGamePlayerJoinedEvent e)
        {
            var response = await _client.SendHeartbeatAsync(e.Game).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                _logger.LogError($"Failed to inform server of player joined: {response.StatusCode}");
        }

        /// <summary>
        /// Decrease the total player count.
        /// </summary>
        /// <param name="e"></param>
        [EventListener]
        public async ValueTask OnPlayerLeftGame(IGamePlayerLeftEvent e)
        {
            var response = await _client.SendHeartbeatAsync(e.Game).ConfigureAwait(false);
            if (!response.IsSuccessStatusCode)
                _logger.LogError($"Failed to inform server of player exit: {response.StatusCode}");
        }
        #endregion

        #region Constructor
        public GameEventListener(ILogger<AmongServersPlugin> logger, APIClient client)
        {
            _logger = logger;
            _client = client;
        }
        #endregion
    }
}
