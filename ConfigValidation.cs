using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace SourceLabelMaker
{
    internal class ConfigValidation
    {
        internal static bool ValidateAlignment(string alignment)
        {
            if (string.IsNullOrEmpty(alignment)) return false;
            return (
                (alignment.Equals("TopLeft", StringComparison.OrdinalIgnoreCase)) ||
                (alignment.Equals("TopCenter", StringComparison.OrdinalIgnoreCase)) ||
                (alignment.Equals("TopRight", StringComparison.OrdinalIgnoreCase)) ||

                (alignment.Equals("MiddleLeft", StringComparison.OrdinalIgnoreCase)) ||
                (alignment.Equals("MiddleCenter", StringComparison.OrdinalIgnoreCase)) ||
                (alignment.Equals("MiddleRight", StringComparison.OrdinalIgnoreCase)) ||

                (alignment.Equals("BottomLeft", StringComparison.OrdinalIgnoreCase)) ||
                (alignment.Equals("BottomCenter", StringComparison.OrdinalIgnoreCase)) ||
                (alignment.Equals("BottomRight", StringComparison.OrdinalIgnoreCase))
            );
        }

        internal static bool ValidateColor(string color)
        {
            if (string.IsNullOrEmpty(color)) return false;
            string pattern = @"^0x00([0-9A-Fa-f]{6})$";
            Match regexMatch = Regex.Match(color, pattern);
            return regexMatch.Success;
        }

        internal static bool ValidateBackgroundMode(string backgroundMode)
        {
            if (string.IsNullOrEmpty(backgroundMode)) return false;
            return (
                backgroundMode.Equals("Transparent", StringComparison.OrdinalIgnoreCase) ||
                backgroundMode.Equals("Opaque", StringComparison.OrdinalIgnoreCase) ||
                backgroundMode.Equals("OpaqueFullWidth", StringComparison.OrdinalIgnoreCase)
            );
        }

        internal static bool ValidateFaceName(string faceName)
        {
            if (string.IsNullOrEmpty(faceName)) return false;
            using(InstalledFontCollection fontCollection = new InstalledFontCollection())
            {
                return fontCollection.Families
                    .Any(family => family.Name.Equals(faceName, StringComparison.OrdinalIgnoreCase));
            }
        }

        internal static bool ValidateFontSize(string fontSize)
        {
            if(string.IsNullOrEmpty(fontSize)) return false;
            if(int.TryParse(fontSize, out int fontSizeInt)) 
            {
                return fontSizeInt > 0;
            }
            return false;
        }

        internal static bool ValidateProportionalMode(string proportionalMode)
        {
            if (string.IsNullOrEmpty(proportionalMode)) return false;
            return (
                proportionalMode.Equals("NonProportional", StringComparison.OrdinalIgnoreCase) ||
                proportionalMode.Equals("Proportional", StringComparison.OrdinalIgnoreCase)
            );
        }

        internal static bool ValidateScrollSize(string size) => int.TryParse(size, out int _);

        internal static bool ValidateRegex(string text, string regexPattern)
        {
            if (string.IsNullOrEmpty(text)) return false;
            Match regexMatch = Regex.Match(text, regexPattern);
            return regexMatch.Success;
        }

        internal static bool ValidateBlinking(string blinking)
        {
            if(string.IsNullOrEmpty(blinking)) return false;
            string pattern = @"^-?\d+,-?\d+$";
            Match regexMatch = Regex.Match(blinking, pattern);
            return regexMatch.Success;
        }

        internal static bool ValidateBlendingType(string blendingType)
        {
            if(string.IsNullOrEmpty(blendingType)) return false;
            return (
                blendingType.Equals("Default", StringComparison.OrdinalIgnoreCase) ||
                blendingType.Equals("Solid", StringComparison.OrdinalIgnoreCase) ||
                blendingType.Equals("Alpha", StringComparison.OrdinalIgnoreCase) ||
                blendingType.Equals("DstColorKey", StringComparison.OrdinalIgnoreCase)
            );
        }
    }
}
