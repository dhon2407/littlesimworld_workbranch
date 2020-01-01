using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace LSW.Helpers
{
    public static class EnumFunction
    {
        public static T Next<T>(this T src) where T : struct
        {
            if (!typeof(T).IsEnum)
                throw new ArgumentException(String.Format("Argument {0} is not an Enum", typeof(T).FullName));

            T[] Arr = (T[])Enum.GetValues(src.GetType());
            int j = Array.IndexOf<T>(Arr, src) + 1;
            return (Arr.Length == j) ? Arr[0] : Arr[j];
        }
    }

    public static class UIEffects
    {
        public static void Shake(this Button button, float shakeDuration, float intensity)
        {
            Transform uiTransform = button.transform;
            var originalPosition = uiTransform.localPosition;

            StartShake(uiTransform,
                shakeDuration,
                intensity,
                originalPosition).
                    Start();
        }

        private static IEnumerator<float> StartShake(Transform transform, float shakeDuration, float intensity, Vector3 originalPosition)
        {
            while (shakeDuration > 0)
            {
                shakeDuration -= Time.deltaTime;
                transform.localPosition = originalPosition + (Random.insideUnitSphere * intensity);
                yield return 0f;
            }

            transform.localPosition = originalPosition;
        }
    }
}
