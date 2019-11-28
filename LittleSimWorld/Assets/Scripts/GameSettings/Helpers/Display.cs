using UnityEngine;

namespace GameSettings.Helpers
{
    public static class Display
    {
        public static bool Same(this Resolution current, Resolution other)
        {
            return (current.height == other.height) && (current.width == other.width);
        }

    }
}
