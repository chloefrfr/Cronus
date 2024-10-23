using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Versions;
using Larry.Source.Enums;
using Larry.Source.Utilities.Converters;



namespace Larry.Source.Utilities.Managers
{
    public class FileProviderManager
    {
        public DefaultFileProvider FileProvider { get; private set; }
        private Config config;

        public FileProviderManager(Config config)
        {
            this.config = config;
        }

        /// <summary>
        /// Initializes the FileProvider with the specified game directory.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                FileProvider = new DefaultFileProvider(config.GameDirectory, SearchOption.TopDirectoryOnly, true, new VersionContainer(EGame.GAME_UE4_LATEST));
                FileProvider.Initialize();
                FileProvider.LoadLocalization();

                var version = VersionConverter.ConvertToFVersion(config.CurrentVersion);
                var aesKey = ProviderUtilities.GetAesKey(version);

                var aesKeys = new List<KeyValuePair<FGuid, FAesKey>>
                {
                    new KeyValuePair<FGuid, FAesKey>(new FGuid(), new FAesKey(aesKey.Key))
                };

                await FileProvider.SubmitKeysAsync(aesKeys);

                var keysCount = FileProvider.Keys.Count;

               Logger.Information($"FileProvider initialized successfully with {keysCount} {(keysCount == 1 ? "key" : "keys")}. Version: {config.CurrentVersion}");

                Parallel.ForEach(FileProvider.MountedVfs, vfs =>
                {
                    Logger.Information($"Successfully mounted file '{vfs.Name}'");
                });
            }
            catch (FileNotFoundException ex)
            {
                Logger.Error($"File not found: {ex.FileName}");
            }
            catch (UnauthorizedAccessException ex)
            {
                Logger.Error($"Access denied: {ex.Message}");
            }
            catch (Exception ex)
            {
                Logger.Error($"Unexpected error initializing FileProvider: {ex.Message}");
            }
        }

        /// <summary>
        /// Loads assets from the specified path within the FileProvider.
        /// </summary>
        /// <param name="path">The path to search for assets.</param>
        /// <returns>A list of asset names found in the specified path.</returns>
        public List<string> LoadAssetsFromPath(string path)
        {
            if (FileProvider == null)
            {
                Logger.Error("FileProvider is not initialized.");
                return new List<string>();
            }

            if (string.IsNullOrWhiteSpace(path))
            {
                Logger.Warning("Provided path is null or empty.");
                return new List<string>();
            }

            try
            {
                // Best intending ever!!
                var assetFiles = FileProvider.Files
                    .AsParallel()
                    .Where(file => file.Key.StartsWith(path, StringComparison.OrdinalIgnoreCase)
                                    && file.Key.EndsWith(".uasset", StringComparison.OrdinalIgnoreCase));
                return assetFiles.Select(file => file.Key).ToList();
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading assets from path '{path}': {ex.Message}");
                return new List<string>();
            }
        }

        /// <summary>
        /// Loads all cosmetics from a predefined path and logs the number of cosmetics loaded and the time taken.
        /// </summary>
        /// <returns>A list of cosmetic asset paths.</returns>
        public async Task<List<string>> LoadAllCosmeticsAsync()
        {
            if (FileProvider == null)
            {
                Logger.Error("FileProvider is not initialized.");
                return new List<string>();
            }
            const string cosmeticsPath = "FortniteGame/Content/Athena/Items/Cosmetics";
            var cosmetics = new List<string>();

            try
            {
                Stopwatch stopwatch = Stopwatch.StartNew();

                await Task.Run(() =>
                {
                    foreach (var file in FileProvider.Files)
                    {
                        string key = file.Key;

                        if (key.StartsWith(cosmeticsPath, StringComparison.OrdinalIgnoreCase) &&
                            key.EndsWith(".uasset", StringComparison.OrdinalIgnoreCase))
                        {
                            cosmetics.Add(key);
                        }
                    }
                });

                stopwatch.Stop();
                Logger.Information($"Loaded {cosmetics.Count} cosmetics in {stopwatch.ElapsedMilliseconds} ms from path '{cosmeticsPath}'.");

                return cosmetics;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading cosmetics from path '{cosmeticsPath}': {ex.Message}");
                return new List<string>();
            }
        }

    }
}
