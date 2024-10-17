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
            string enumName = $"V{parts[0]}_{parts[1]}";

            if (Enum.TryParse<FVersion>(enumName, out var fVersion))
            {
                return fVersion;
            }

            throw new ArgumentException($"Invalid version: {version}.", nameof(version));
        }
    }
}
