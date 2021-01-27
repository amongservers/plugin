using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AmongServers.Plugin.Coordinator.Entities
{
    /// <summary>
    /// Represents a heartbeat request entity.
    /// </summary>
    public class HeartbeatEntity
    {
        /// <summary>
        /// The server endpoint.
        /// </summary>
        [JsonPropertyName("endpoint")]
        public string Endpoint { get; set; }

        /// <summary>
        /// The server name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }

        /// <summary>
        /// The running games, only includes pending, starting and running lobbies.
        /// </summary>
        [JsonPropertyName("games")]
        public GameEntity[] Games { get; set; }
    }
}
