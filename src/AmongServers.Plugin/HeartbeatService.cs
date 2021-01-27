using AmongServers.Plugin.Coordinator;
using AmongServers.Plugin.Coordinator.Entities;
using Impostor.Api.Games;
using Impostor.Api.Innersloth;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace AmongServers.Plugin
{
    /// <summary>
    /// The heartbeat service, responsible for updating the API about the server.
    /// </summary>
    public class HeartbeatService : IAsyncDisposable
    {
        private readonly CancellationTokenSource _cancellationSource = new CancellationTokenSource();
        private readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private readonly ApiClient _client;
        private readonly ILogger _logger;
        private DateTimeOffset _nextAutomaticHeartbeat = DateTimeOffset.UtcNow;
        private Task _heartbeatTask;
        private ConcurrentDictionary<IGame, object> _games = new ConcurrentDictionary<IGame, object>();

        /// <summary>
        /// The server name.
        /// </summary>
        public string ServerName { get; set; } = "My Awesome Impostor Server";

        /// <summary>
        /// The server endpoint.
        /// </summary>
        public IPEndPoint ServerEndpoint { get; set; }

        /// <summary>
        /// Attach a game to the heartbeat.
        /// </summary>
        /// <param name="game">The game.</param>
        public void AttachGame(IGame game)
        {
            _games[game] = null;
        }

        /// <summary>
        /// Detach a game from the heartbeat.
        /// </summary>
        /// <param name="game">The game.</param>
        public void DetachGame(IGame game)
        {
            _games.TryRemove(game, out _);
        }

        /// <summary>
        /// Dispose the heartbeat service.
        /// </summary>
        /// <returns></returns>
        public ValueTask DisposeAsync()
        {
            _cancellationSource.Cancel();
            return default;
        }

        /// <summary>
        /// Start sending heartbeats.
        /// </summary>
        public ValueTask StartAsync()
        {
            _ = RunHeartbeatAsync();
            return default;
        }

        private async Task HeartbeatAsync()
        {
            try {
                await _client.HeartbeatAsync(new Coordinator.Entities.HeartbeatEntity() {
                    Name = ServerName,
                    Endpoint = ServerEndpoint.ToString(),
                    Games = _games.Keys.Where(g => g.GameState == GameStates.Started || g.GameState == GameStates.Starting || g.GameState == GameStates.NotStarted)
                        .Select(s => new GameEntity() {
                            IsPublic = s.IsPublic,
                            Map = s.Options.Map.ToString().ToLower(),
                            State = s.GameState.ToString().ToLower(),
                            NumImposters = s.Options.NumImpostors,
                            MaxPlayers = s.Options.MaxPlayers,
                            CountPlayers = s.PlayerCount,
                            HostPlayer = s.IsPublic && s.Host != null ? new PlayerEntity() {
                                Name = s.Host.Character.PlayerInfo.PlayerName
                            } : null,
                            Players = s.IsPublic && s.Players != null ? s.Players.Select(p => new PlayerEntity() {
                                Name = p.Character.PlayerInfo.PlayerName
                            }).ToArray() : Array.Empty<PlayerEntity>()
                        }).ToArray()
                }, _cancellationSource.Token);
            } catch (Exception ex) {
                _logger?.LogDebug(ex, "Failed to send the heartbeat");
            } finally {
                _semaphore.Release();
            }
        }

        private bool TryHeartbeatInternal()
        {
            if (!_semaphore.Wait(0))
                return false;

            // execute a heartbeat
            try {
                _heartbeatTask = HeartbeatAsync();
            } catch(Exception ex) {
                _logger?.LogDebug(ex, "Failed to send the heartbeat");
            }

            return true;
        }

        /// <summary>
        /// Try and request a heartbeat is sent, this might be ignored if another heartbeat is in progress or an automatic heartbeat will be sent soon anyway.
        /// </summary>
        /// <returns>If the heartbeat was sent otherwise another heartbeat is in progress.</returns>
        public bool TryHeartbeat()
        {
            // if the next heartbeat is less than 5s away don't send
            if (_nextAutomaticHeartbeat - DateTimeOffset.UtcNow < TimeSpan.FromSeconds(5)) {
                return false;
            }

            return TryHeartbeatInternal();
        }

        /// <summary>
        /// Runs the servers heartbeat, this will send a request to the master server server every 30s.
        /// </summary>
        /// <param name="cancellationToken">The cancellation token.</param>
        /// <returns></returns>
        private async Task RunHeartbeatAsync()
        {
            while(!_cancellationSource.IsCancellationRequested) {
                TryHeartbeatInternal();

                try {
                    _nextAutomaticHeartbeat = DateTimeOffset.UtcNow + TimeSpan.FromSeconds(30);
                    await Task.Delay(TimeSpan.FromSeconds(30), _cancellationSource.Token);
                } catch(OperationCanceledException) {
                    return;
                }
            }
        }

        public HeartbeatService(ILogger<HeartbeatService> logger) {
            _client = new ApiClient(Constants.ApiUrl);
            _logger = logger;
        }
    }
}
