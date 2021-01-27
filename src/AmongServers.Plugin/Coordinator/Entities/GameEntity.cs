using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AmongServers.Plugin.Coordinator.Entities
{
    /// <summary>
    /// Represents a game (lobby) entity.
    /// </summary>
    public class GameEntity
    {
        /// <summary>
        /// The game state.
        /// </summary>
        [JsonPropertyName("state")]
        public string State { get; set; }

        /// <summary>
        /// The game map.
        /// </summary>
        [JsonPropertyName("map")]
        public string Map { get; set; }

        /// <summary>
        /// The maximum number of players.
        /// </summary>
        [JsonPropertyName("maxPlayers")]
        public int MaxPlayers { get; set; }

        /// <summary>
        /// The number of players currently playing/waiting.
        /// </summary>
        [JsonPropertyName("countPlayers")]
        public int CountPlayers { get; set; }

        /// <summary>
        /// The number of imposters the game will/currently have.
        /// </summary>
        [JsonPropertyName("numImposters")]
        public int NumImposters { get; set; }

        /// <summary>
        /// If the game is public.
        /// </summary>
        [JsonPropertyName("isPublic")]
        public bool IsPublic { get; set; }

        /// <summary>
        /// The game host, this will be null if the game is private.
        /// </summary>
        [JsonPropertyName("hostPlayer")]
        public PlayerEntity Host { get; set; }

        /// <summary>
        /// The players, this array will be empty if the game is private.
        /// </summary>
        [JsonPropertyName("players")]
        public PlayerEntity[] Players { get; set; }
    }
}
