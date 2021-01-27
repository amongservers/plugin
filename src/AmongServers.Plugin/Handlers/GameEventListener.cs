using Impostor.Api.Events;
using Impostor.Api.Games;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace AmongServers.Plugin.Handlers
{
    public class GameEventListener : IEventListener
    {
        #region Fields
        private readonly ILogger<AmongServersPlugin> _logger;
        private HeartbeatService _heartbeat;
        #endregion

        #region Methods
        /// <summary>
        /// Adds a new pending lobby.
        /// </summary>
        /// <param name="e"></param>
        /// <returns></returns>
        [EventListener]
        public ValueTask OnGameCreated(IGameCreatedEvent e)
        {
            //TODO: created game
            _heartbeat.TryHeartbeat();
            return default;
        }

        /// <summary>
        /// Moves the lobby from pending to active. 
        /// </summary>
        /// <param name="e"></param>
        [EventListener]
        public ValueTask OnGameStarted(IGameStartedEvent e)
        {
            //TODO: update game
            _heartbeat.TryHeartbeat();
            return default;
        }

        /// <summary>
        /// Move a lobby from active back to pending.
        /// </summary>
        /// <param name="e"></param>
        [EventListener]
        public ValueTask OnGameEnded(IGameEndedEvent e)
        {
            //TODO: remove game
            _heartbeat.TryHeartbeat();
            return default;
        }

        /// <summary>
        /// Remove the lobby.
        /// </summary>
        /// <param name="e"></param>
        [EventListener]
        public ValueTask OnGameDestroyed(IGameDestroyedEvent e)
        {
            //TODO: remove game if not already removed by ending
            _heartbeat.TryHeartbeat();
            return default;
        }

        /// <summary>
        /// Increase the total player count.
        /// </summary>
        /// <param name="e"></param>
        [EventListener]
        public ValueTask OnPlayerJoined(IGamePlayerJoinedEvent e)
        {
            //TODO: update player count on game
            _heartbeat.TryHeartbeat();
            return default;
        }

        /// <summary>
        /// Decrease the total player count.
        /// </summary>
        /// <param name="e"></param>
        [EventListener]
        public ValueTask OnPlayerLeftGame(IGamePlayerLeftEvent e)
        {
            //TODO: update player count on game
            _heartbeat.TryHeartbeat();
            return default;
        }
        #endregion

        #region Constructor
        public GameEventListener(ILogger<AmongServersPlugin> logger, HeartbeatService heartbeat)
        {
            _logger = logger;
            _heartbeat = heartbeat;
        }
        #endregion
    }
}
