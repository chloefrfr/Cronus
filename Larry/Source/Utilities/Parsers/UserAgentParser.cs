using Larry.Source.Classes.Agent;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Larry.Source.Utilities.Parsers
{
    public static class UserAgentParser
    {
        /// <summary>
        /// Parses the user agent string and extracts season information.
        /// </summary>
        /// <param name="userAgent">The user agent string to parse.</param>
        /// <returns>A <see cref="SeasonInfo"/> object containing season details, or null if parsing fails.</returns>
        public static SeasonInfo Parse(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent))
            {
                Logger.Error("User agent string is null or empty.");
                return null;
            }

            var buildId = GetBuildID(userAgent);
            var buildString = GetBuildString(userAgent);

            if ((buildId != null || buildString != null) && double.TryParse(buildId ?? buildString, out double build))
            {
                return HandleValidBuild(new UserAgentInfo
                {
                    BuildId = buildId ?? string.Empty,
                    BuildString = buildString ?? string.Empty
                });
            }

            Logger.Error("Failed to parse Build ID or Build String.");
            return null;
        }

        /// <summary>
        /// Gets the Build ID from the user agent string.
        /// </summary>
        /// <param name="userAgent">The user agent string to parse.</param>
        /// <returns>The given Build ID or null if not found.</returns>
        private static string GetBuildID(string userAgent)
        {
            var parts = userAgent.Split('-');
            if (parts.Length > 3)
            {
                return parts[3].Split(',')[0].Split(' ')[0];
            }
            return (parts.Length > 1) ? parts[1].Split('+')[0].Split(' ')[0] : null;
        }

        /// <summary>
        /// Gets the Build String from the user agent string.
        /// </summary>
        /// <param name="userAgent">The user agent string to parse.</param>
        /// <returns>The given Build String or null if not found.</returns>
        private static string GetBuildString(string userAgent)
        {
            var parts = userAgent.Split("Release-");
            return parts.Length > 1 ? parts[1].Split('-')[0] : null;
        }

        /// <param name="userAgentInfo">An object containing the user agent's build information.</param>
        /// <returns>A <see cref="SeasonInfo"/> object populated with the season details.</returns>
        private static SeasonInfo HandleValidBuild(UserAgentInfo userAgentInfo)
        {
            double.TryParse(userAgentInfo.BuildId, out double netcl);
            int build = ParseBuildString(userAgentInfo.BuildString);

            string buildUpdate = string.Empty;
            var buildUpdateParts = userAgentInfo.BuildString.Split('-');

            if (buildUpdateParts.Length > 1)
            {
                buildUpdate = buildUpdateParts[1].Split('+')[0]; 
            }

            int season = build;
            string lobby = string.Empty;
            string background = string.Empty;

            if (double.IsNaN(netcl))
            {
                lobby = "LobbySeason0";
                season = 0;
                build = 0;
            }
            else if (netcl < 3724489)
            {
                lobby = "Season0";
                season = 0;
                build = 0;
            }
            else if (netcl <= 3790078)
            {
                lobby = "LobbySeason1";
                season = 1;
                build = 1;
            }
            else if (buildUpdate == userAgentInfo.BuildId || buildUpdate == "Cert")
            {
                season = 2;
                build = 2;
                lobby = "LobbyWinterDecor";
            }
            else if (season == 6)
            {
                background = "fortnitemares";
            }
            else if (season == 10)
            {
                background = "seasonx";
            }
            else
            {
                lobby = $"Lobby{season}";
                background = $"season{season}";
            }

            return new SeasonInfo
            {
                Season = season,
                Build = build,
                Netcl = netcl.ToString(),
                Lobby = lobby,
                BuildUpdate = buildUpdate,
                Background = background
            };
        }

        /// <summary>
        /// Parses the Build String to extract the build number.
        /// </summary>
        /// <param name="buildString">The build string to parse.</param>
        /// <returns>The parsed build number.</returns>
        private static int ParseBuildString(string buildString)
        {
            return (int)(Math.Floor(double.TryParse(buildString, out double build) ? build : 0));
        }
    }
}
