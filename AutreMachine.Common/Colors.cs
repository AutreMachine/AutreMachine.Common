using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.CommonTools
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
    }
}
