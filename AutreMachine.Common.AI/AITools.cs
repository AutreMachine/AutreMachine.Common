using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutreMachine.Common.AI
{
    /// <summary>
    /// Set of tools to manipulate vectors
    /// </summary>
    public class AITools
    {
        public static float? CosineSimilarity(float[]? array1, float[]? array2)
        {
            if (array1 == null || array2 == null)
                return null;

            if (array1.Length != array2.Length)
                return null;

            return Dot(array1, array2); // /(Magnitude(array1)*Magnitude(array2));
        }



        #region Vector methods
        private static float? Dot(float[] array1, float[] array2)
        {
            if (array1.Length != array2.Length)
                return null;
            float dot = 0;
            for (int i = 0; i < array1.Length; i++)
            {
                dot += array1[i] * array2[i];
            }

            return dot;
        }

        private static float Magnitude(float[] array)
        {
            double magn = 0;
            for (int i = 0; i < array.Length; i++)
            {
                magn += Math.Pow(array[i], 2);
            }
            magn = Math.Sqrt(magn);
            return (float)magn;
        }


        #endregion
    }
}
