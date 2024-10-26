using Larry.Source.Enums;

namespace Larry.Source.Utilities.Converters
{
    public class VersionConverter
    {
        /// <summary>
        /// Converts a version string in the format "Major.Minor" to the corresponding <see cref="FVersion"/> enum value.
        /// </summary>
        /// <param name="version">The version string to convert.</param>
        /// <returns>The corresponding <see cref="FVersion"/> enum value.</returns>
        /// <exception cref="ArgumentException">Thrown when the version string is null, empty, or invalid.</exception>
        public static FVersion ConvertToFVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
            {
                throw new ArgumentException("Version cannot be null or empty.", nameof(version));
            }

            var parts = version.Split('.');
            if (parts.Length != 2)
            {
                throw new ArgumentException("Version must be in the format 'Major.Minor'.", nameof(version));
            }

            if (!int.TryParse(parts[0], out var major) || major < 0)
            {
                throw new ArgumentException("Major version must be a non-negative integer.", nameof(version));
            }

            if (!int.TryParse(parts[1], out var minor) || minor < 0 || minor > 61)
            {
                throw new ArgumentException("Minor version must be an integer between 0 and 61.", nameof(version));
            }

            string enumName = $"V{major}_{minor}";

            if (Enum.TryParse<FVersion>(enumName, out var fVersion))
            {
                return fVersion;
            }

            throw new ArgumentException($"Invalid version: {version}.", nameof(version));
        }
    }
}
