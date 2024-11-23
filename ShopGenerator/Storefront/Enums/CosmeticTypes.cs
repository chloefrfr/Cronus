using System;
using System.Collections.Generic;

namespace ShopGenerator.Storefront.Enums
{
    public enum CosmeticTypes
    {
        AthenaCharacter,
        AthenaBackpack,
        AthenaPickaxe,
        AthenaGlider,
        AthenaDance,
        AthenaToy,
        AthenaEmoji,
        AthenaItemWrap,
        AthenaMusicPack,
        AthenaPet,
        AthenaPetCarrier,
        AthenaLoadingScreen,
        AthenaSkyDiveContrail,
    }

    public static class CosmeticTypesExtensions
    {
        private static readonly Dictionary<CosmeticTypes, string> CosmeticTypeNames = new()
        {
            { CosmeticTypes.AthenaCharacter, "AthenaCharacter" },
            { CosmeticTypes.AthenaBackpack, "AthenaBackpack" },
            { CosmeticTypes.AthenaPickaxe, "AthenaPickaxe" },
            { CosmeticTypes.AthenaGlider, "AthenaGlider" },
            { CosmeticTypes.AthenaDance, "AthenaDance" },
            { CosmeticTypes.AthenaToy, "AthenaToy" },
            { CosmeticTypes.AthenaEmoji, "AthenaEmoji" },
            { CosmeticTypes.AthenaItemWrap, "AthenaItemWrap" },
            { CosmeticTypes.AthenaMusicPack, "AthenaMusicPack" },
            { CosmeticTypes.AthenaPet, "AthenaPet" },
            { CosmeticTypes.AthenaPetCarrier, "AthenaPetCarrier" },
            { CosmeticTypes.AthenaLoadingScreen, "AthenaLoadingScreen" },
            { CosmeticTypes.AthenaSkyDiveContrail, "AthenaSkyDiveContrail" },
        };

        public static string GetStringValue(this CosmeticTypes type)
        {
            return CosmeticTypeNames[type];
        }
    }
}
