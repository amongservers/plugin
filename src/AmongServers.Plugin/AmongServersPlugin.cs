using AmongServers.Plugin.Handlers;
using Impostor.Api.Events.Managers;
using Impostor.Api.Plugins;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.ComponentModel;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace AmongServers.Plugin
{
    [ImpostorPlugin(
        package: "AmongServers.Plugin",
        name: "Among Servers",
        author: "Among Servers",
        version: "0.1.3")]
    public class AmongServersPlugin : PluginBase
    {
        #region Fields
        private readonly ILogger<AmongServersPlugin> _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventManager _eventManager;
        private readonly IConfiguration _configuration;
        private HeartbeatService _heartbeat;
        private IDisposable _unregister;
        #endregion

        public override async ValueTask EnableAsync()
        {
            _logger.LogInformation("Enabling AmongServers plugin");

            var serverConfig = _configuration.GetSection("Server");

            if (serverConfig.GetSection("EnableAmongServers").Exists() && bool.TryParse(serverConfig["EnableAmongServers"], out bool result) && !result)
            {
                _logger.LogInformation("AmongServers plugin is disabled via config.");
                return;
            }

            // get the server name
            string serverName = serverConfig.GetSection("Name").Exists() ? serverConfig["Name"] : null;

            // get the endpoint data
            IPAddress publicAddress = null;

            if (serverConfig.GetSection("PublicIp").Exists()) {
                if (!IPAddress.TryParse(serverConfig["PublicIp"], out publicAddress))
                    _logger?.LogWarning("The public IP format is invalid, falling back to auto-IP detection");
            }

            int publicPort = int.Parse(_configuration.GetSection("Server")["PublicPort"]);

            // create the heartbeat service
            _heartbeat = ActivatorUtilities.CreateInstance<HeartbeatService>(_serviceProvider);

            // configure details
            if (!string.IsNullOrEmpty(serverName))
                _heartbeat.ServerName = serverName;

            _heartbeat.ServerEndpoint = new IPEndPoint(publicAddress ?? IPAddress.Any, publicPort);

            // Register events
            _unregister = _eventManager.RegisterListener(new GameEventListener(_logger, _heartbeat));

            // start the heartbeat
            await _heartbeat.StartAsync();
        }

        public override async ValueTask DisableAsync()
        {
            _logger.LogInformation("Disabling AmongServers plugin");

            // dispose the event listener
            _unregister.Dispose();

            // stop heartbeat
            await _heartbeat.DisposeAsync();
        }

        #region Constructor
        public AmongServersPlugin(ILogger<AmongServersPlugin> logger, IEventManager eventManager, IConfiguration configuration, IServiceProvider serviceProvider)
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _eventManager = eventManager;
            _configuration = configuration;
        }
        #endregion
    }
}
