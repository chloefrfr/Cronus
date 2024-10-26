using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using CUE4Parse.Encryption.Aes;
using CUE4Parse.FileProvider;
using CUE4Parse.UE4.Assets.Exports;
using CUE4Parse.UE4.Assets.Objects;
using CUE4Parse.UE4.Objects.Core.Misc;
using CUE4Parse.UE4.Objects.GameplayTags;
using CUE4Parse.UE4.Versions;
using CUE4Parse.Utils;
using Discord;
using Larry.Source.Classes.MCP;
using Larry.Source.Database.Entities;
using Larry.Source.Enums;
using Larry.Source.Utilities.Converters;



namespace Larry.Source.Utilities.Managers
{
    public class FileProviderManager
    {
        public DefaultFileProvider FileProvider { get; private set; }
        private Config config;
        private readonly CacheManager<List<Variants>> _variantsCacheManager = new(TimeSpan.FromMinutes(10));
        private readonly CacheManager<List<string>> _assetsCacheManager = new(TimeSpan.FromMinutes(10));
        private readonly CacheManager<List<string>> _cosmeticsCacheManager = new(TimeSpan.FromMinutes(10));

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

            if (_assetsCacheManager.TryGetValue(path, out var cachedAssets))
            {
                return cachedAssets;
            }

            try
            {
                var assetFiles = FileProvider.Files
                    .AsParallel()
                    .Where(file => file.Key.StartsWith(path, StringComparison.OrdinalIgnoreCase)
                                   && file.Key.EndsWith(".uasset", StringComparison.OrdinalIgnoreCase));
                var assetList = assetFiles.Select(file => file.Key).ToList();

                _assetsCacheManager.Set(path, assetList);

                return assetList;
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

            if (_cosmeticsCacheManager.TryGetValue(cosmeticsPath, out var cachedCosmetics))
            {
                return cachedCosmetics;
            }

            var cosmetics = new List<string>();
            var stopwatch = Stopwatch.StartNew();

            try
            {
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

                _cosmeticsCacheManager.Set(cosmeticsPath, cosmetics);

                stopwatch.Stop(); 
                Logger.Information($"Loaded {cosmetics.Count} cosmetics in {stopwatch.ElapsedMilliseconds} ms.");

                return cosmetics;
            }
            catch (Exception ex)
            {
                Logger.Error($"Error loading cosmetics from path '{cosmeticsPath}': {ex.Message}");
                return new List<string>();
            }
        }


        /// <summary>
        /// Asynchronously gets a list of cosmetic variants based on the specified template ID.
        /// </summary>
        /// <param name="templateId">The template ID of the cosmetic item to retrieve variants for.</param>
        /// <returns>A task that represents the asynchronous operation. The task result contains a list of <see cref="Variants"/> associated with the template ID.</returns>
        public async Task<List<Variants>> GetVariantsAsync(string templateId)
        {
            try
            {
                if (_variantsCacheManager.TryGetValue(templateId, out var cachedVariants))
                {
                    return cachedVariants;
                }

                var filteredCosmeticsTask = Task.Run(() => LoadAllCosmeticsAsync());
                var filteredCosmetics = await filteredCosmeticsTask;

                var foundItem = filteredCosmetics.FirstOrDefault();
                if (foundItem is null) return new List<Variants>();

                var itemObjTask = FileProvider.LoadObjectAsync(foundItem.Split('.')[0]);
                var itemObj = await itemObjTask;
                if (itemObj is null) return new List<Variants>();

                if (!itemObj.TryGetValue(out UObject[] itemVariants, "ItemVariants"))
                    return new List<Variants>();

                var variantTypes = new[]
                {
                    "PartOptions", "MaterialOptions", "DynamicOptions", "GenericTagOptions",
                    "LoadoutAugmentations", "ParticleOptions", "MeshOptions"
                };

                var variantsList = itemVariants.AsParallel().SelectMany(variant =>
                {
                    var variantResults = new List<Variants>();

                    if (variant.TryGetValue(out FGameplayTag variantChannel, "VariantChannelTag") && variantChannel != null)
                    {
                        var parsedChannelName = Constants.ParseChannelName(variantChannel.TagName.PlainText);

                        if (variant.ExportType == "FortCosmeticNumericalVariant")
                        {
                            variantResults.Add(new Variants
                            {
                                channel = parsedChannelName,
                                active = "",
                                owned = new List<string>()
                            });
                        }

                        variantResults.AddRange(variantTypes
                            .Select(type => variant.TryGetValue(out FStructFallback[] options, type) ? options : null)
                            .Where(options => options != null)
                            .Select(options => Constants.CreateVariantFromOptions(options, parsedChannelName)));
                    }

                    return variantResults;
                }).ToList();

                _variantsCacheManager.Set(templateId, variantsList);

                return variantsList;
            }
            catch (Exception ex)
            {
                Logger.Error($"An error occurred: {ex.Message}");
                return new List<Variants>();
            }
        }
    }
}
