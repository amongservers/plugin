using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AmongServers.Plugin.Coordinator.Entities
{
    /// <summary>
    /// Represents a player.
    /// </summary>
    public class PlayerEntity
    {
        /// <summary>
        /// The player name.
        /// </summary>
        [JsonPropertyName("name")]
        public string Name { get; set; }
    }
}
