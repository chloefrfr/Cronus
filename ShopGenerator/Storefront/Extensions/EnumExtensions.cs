using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShopGenerator.Storefront.Extensions
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Gets the description of an enum value, if available.
        /// </summary>
        /// <param name="value">The enum value to retrieve the description for.</param>
        /// <returns>
        /// The description specified in the <see cref="DescriptionAttribute"/>, or the enum's name if no description is provided.
        /// </returns>
        public static string GetDescription(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value.ToString());
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value.ToString();
        }
    }
}
