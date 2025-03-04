using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common
{
    public class Colors
    {
        /// <summary>
        /// Extract Luminance from RGB
        /// </summary>
        /// <param name="rgb"></param>
        /// <returns></returns>
        public static double LuminanceFromRGB(string rgb)
        {
            // (0.2126 * R + 0.7152 * G + 0.0722 * B)
            rgb = rgb.Replace("#", "").Trim();
            var r = Convert.ToInt32(rgb.Substring(0, 2), 16);
            var g = Convert.ToInt32(rgb.Substring(2, 2), 16);
            var b = Convert.ToInt32(rgb.Substring(4, 2), 16);

            var lumi = 0.2126 * ((float)r / 255.0f) + 0.7152 * ((float)g / 255.0f) + 0.0722 * ((float)b / 255.0f);

            return lumi;
        }

        /// <summary>
        /// Create gradient between 2 colors
        /// </summary>
        /// <param name="fromRGB"></param>
        /// <param name="toRGB"></param>
        /// <param name="percentage"></param>
        /// <returns></returns>
        public static string Gradient(string fromRGB, string toRGB, float percentage)
        {
            if (percentage < 0.0f)
                percentage = 0.0f;
            if (percentage > 1.0f)
                percentage = 1.0f;
            
            fromRGB = fromRGB.Replace("#", "").Trim();
            if (fromRGB.Length != 6)
                return string.Empty;
            var r = Convert.ToInt32(fromRGB.Substring(0, 2), 16);
            var g = Convert.ToInt32(fromRGB.Substring(2, 2), 16);
            var b = Convert.ToInt32(fromRGB.Substring(4, 2), 16);

            toRGB = toRGB.Replace("#", "").Trim();
            if (toRGB.Length != 6)
                return string.Empty;
            var r1 = Convert.ToInt32(toRGB.Substring(0, 2), 16);
            var g1 = Convert.ToInt32(toRGB.Substring(2, 2), 16);
            var b1 = Convert.ToInt32(toRGB.Substring(4, 2), 16);

            float stepR = r1 - r;
            float stepG = g1 - g;
            float stepB = b1 - b;

            var r2 = r + (int)(stepR * percentage);
            var g2 = g + (int)(stepG * percentage);
            var b2 = b + (int)(stepB * percentage);

            var colorstring = String.Format("#{0:X2}{1:X2}{2:X2}", r2, g2, b2);
            return colorstring;
        }
    }
}
