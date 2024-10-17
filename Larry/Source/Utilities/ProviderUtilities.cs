using CUE4Parse.Encryption.Aes;
using CUE4Parse.UE4.Versions;
using Jose;
using Larry.Source.Enums;

namespace Larry.Source.Utilities
{
    public class ProviderUtilities
    {
        private static readonly Dictionary<FVersion, string> AesKeyMap = new Dictionary<FVersion, string>
        {
            { FVersion.V6_00, "0xD99660BBE70346E5BBEC944E0921051408B41CCB753F0CFA945A0F941C333E3B" },
            { FVersion.V6_10, "0x47C3245CFAB0F785D4DB3FA8E9967F887ECD623FA51308F1BD6BDB58FCFC6583" },
            { FVersion.V7_30, "0xD23E6F3CF45A2E31081CB7D5F94C85EC50CCB1A804F8C90248F72FA3896912E4" },
            { FVersion.V7_40, "0xF2A0859F249BC9A511B3A8766420C6E943004CF0EAEE5B7CFFDB8F10953E994F" },
            { FVersion.V8_00, "0xAC7AF84B59C4BD4F916F7EFCB292B3A5897CFF7DD7A688AC8B3791A4EDF32E7B" },
            { FVersion.V8_10, "0x52C122AC39C8D56ED15834768A87D18AA26E74CA694060B9E6BCC1C39C0852FA" },
            { FVersion.V8_20, "0x5F3B1AE176BF56D5FD1AA073DC01868692ABC11B9186BB12D9235072BBAEE8E2" },
            { FVersion.V8_50, "0x67D061EFA8E049F7C62F1C460F14CD5AD7E601C13F3FB66F0FB090B72B721ACC" },
            { FVersion.V8_51, "0x67D061EFA8E049F7C62F1C460F14CD5AD7E601C13F3FB66F0FB090B72B721ACC" },
            { FVersion.V9_00, "0x38C910C99FF26B29B98FBCC8FA0FBB700DB7DADCBCDCB71C4D443A047B7280CE" },
            { FVersion.V9_10, "0x67D061EFA8E049F7C62F1C460F14CD5AD7E601C13F3FB66F0FB090B72B721ACC" },
            { FVersion.V9_20, "0xE47F0FE3C66BC50D65A92F93609710FEB580BD982017A7D3FC6DE7872197E0CA" },
            { FVersion.V9_40, "0x6BD8D67B235476950DEEFC3F28646284462653C968331F0796C155A882DABB8A" },
            { FVersion.V9_41, "0x6BD8D67B235476950DEEFC3F28646284462653C968331F0796C155A882DABB8A" },
            { FVersion.V10_30, "0xCCBBADDB24A0D16C14088AB762CA93DADFD3CB773619CBF49A05A3BCC5AD920D" },
            { FVersion.V10_40, "0x3FF229552FE0F0DC46A495F9E94766EB6B5106A136597C60E7132F413B7C016E" },
            { FVersion.V11_31, "0x6C51ABA88CA1240A0D14EB94701F6C41FD7799B102E9060D1E6C316993196FDF" },
            { FVersion.V11_50, "0x1C2E5564676BB4B7B865C13EFF75E36BB566B3D36E5140786C9E602C33823C3F" },
            { FVersion.V12_10, "0x7B155D8AA29AA7D1D4FB859521408C987C5B5D5F8A2641EE16F9BA256DF64FC8" },
            { FVersion.V12_20, "0xCBDEB191165B1D8D51759732AAFC0633159CCF993D8662FD99D56F9C3F3F7401" },
            { FVersion.V12_30, "0x1AE302653ACB82998A45DBC2122EAAF86CEFF7F8E1D63B8F0D96562843BC28E9" },
            { FVersion.V12_40, "0x701E5D1B4D13A7BE1F8B56EECDAA4EFA1F9B1BF8C12AA54E9C57A39A3590B61F" },
            { FVersion.V12_41, "0x2713E24A338C7E8BF1A50E3F1987F33BB151F04B192E89E940A623AB34F8502F" },
            { FVersion.V12_60, "0x2DACE7F864025613951B2F2267128F0F5F30A1D31692C67178439AD109CEC935" },
            { FVersion.V12_61, "0x3F3717F4F206FF21BDA8D3BF62B323556D1D2E7D9B0F7ABD572D3CFE5B569FAC" },
            { FVersion.V13_40, "0x450EC70DA26DFEEA5EC2415A0745316DB399305E8A32FBE09E57B1FCC4BD771D" },
            { FVersion.V14_30, "0x3440AB1D1B824905842BE1574F149F9FC7DBA2BB566993E597402B4715A28BD5" },
            { FVersion.V14_40, "0xAB32BAB083F7D923A33AA768BC64B64BF62488948BD49FE61D95343492252558" },
            { FVersion.V14_60, "0xCF47F08CEEDD6A0A86D3D67DD0C25924FE934676B86A3777A36B7E353EB35C09" }
        };

        /// <summary>
        /// Gets the AES key corresponding to a specific game version as a byte array.
        /// </summary>
        /// <param name="version">The game version to lookup.</param>
        /// <returns>A byte array representing the AES key.</returns>
        public static byte[] GetAesKeyAsByteArray(FVersion version)
        {
            if (AesKeyMap.TryGetValue(version, out var aesKeyHex))
            {
                return HexStringToByteArray(aesKeyHex);
            }
            throw new ArgumentException($"No AES key found for version: {version}", nameof(version));
        }

        /// <summary>
        /// Gets the AES key corresponding to a specific game version as a FAesKey object.
        /// </summary>
        /// <param name="version">The game version to lookup.</param>
        /// <returns>A FAesKey object representing the AES key.</returns>
        public static FAesKey GetAesKey(FVersion version)
        {
            var keyBytes = GetAesKeyAsByteArray(version);
            return new FAesKey(keyBytes);
        }

        /// <summary>
        /// Converts a hexadecimal string to a byte array.
        /// </summary>
        /// <param name="hex">The hexadecimal string to convert.</param>
        /// <returns>The corresponding byte array.</returns>
        private static byte[] HexStringToByteArray(string hex)
        {
            hex = hex.Replace("0x", ""); // Remove '0x' prefix if present
            int numberChars = hex.Length;
            byte[] bytes = new byte[numberChars / 2];
            for (int i = 0; i < numberChars; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }
    }
}