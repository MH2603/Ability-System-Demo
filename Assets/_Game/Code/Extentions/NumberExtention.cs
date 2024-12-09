using UnityEngine;

namespace MH.Extentions
{
    public static class NumberExtention
    {
        public static bool IsInt(float number)
        {
            return Mathf.Approximately(number % 1, 0);
        }

        public static int ToMillisecond(float number)
        {
            return (int)(number * 1000);
        }
    }
}