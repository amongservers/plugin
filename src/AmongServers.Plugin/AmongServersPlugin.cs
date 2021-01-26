using AmongServers.Plugin.Handlers;
using Impostor.Api.Events.Managers;
using Impostor.Api.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace AmongServers.Plugin
{
    [ImpostorPlugin(
        package: "AmongServers.Plugin",
        name: "Among Servers",
        author: "Among Servers",
        version: "1.0.0")]
    public class AmongServersPlugin : PluginBase
    {
        #region Fields
        private readonly ILogger<AmongServersPlugin> _logger;
        private readonly IEventManager _eventManager;
        private readonly IConfiguration _configuration;
        private APIClient _client;
        private IDisposable _unregister;
        #endregion

        public override ValueTask EnableAsync()
        {
            _logger.LogInformation("Example is being enabled.");

            // Try to get the name from the config file
            string serverName = _configuration.GetSection("Server")["Name"];
            if (string.IsNullOrEmpty(serverName))
                serverName = "Awesome Among Us Server!";
            int port = int.Parse(_configuration.GetSection("Server")["PublicPort"]);

            // Create client.
            _client = new APIClient(serverName, port);

            // Register events
            _unregister = _eventManager.RegisterListener(new GameEventListener(_logger, _client));

            // Startup the servers heartbeat every 30 seconds.
            Thread t = new Thread(RunHeartbeat);
            t.Start();

            return default;
        }

        public override ValueTask DisableAsync()
        {
            _logger.LogInformation("Example is being disabled.");
            // Add the line below!
            _unregister.Dispose();
            return default;
        }

        /// <summary>
        /// Runs the servers heartbeat.
        /// </summary>
        /// <returns></returns>
        private async void RunHeartbeat()
        {
            _logger?.LogInformation("Starting Among Servers Heatbeat...");
            while (_unregister != null)
            {
                try
                {
                    await _client.SendHeartbeatAsync().ConfigureAwait(false);
                    await Task.Delay(5000);
                }
                catch (Exception ex)
                {
                    _logger?.LogCritical($"Failed to send heartbeat: {ex.Message}");
                }
            };
            _logger?.LogInformation("Stopping Among Servers Heatbeat...");
        }

        #region Constructor
        public AmongServersPlugin(ILogger<AmongServersPlugin> logger, IEventManager eventManager, IConfiguration configuration)
        {
            _logger = logger;
            _eventManager = eventManager;
            _configuration = configuration;
        }
        #endregion
    }
}
