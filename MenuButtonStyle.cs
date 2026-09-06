using UnityEngine;
using UnityEngine.UIElements;

namespace Ui.Controls
{
    /// <summary>
    /// Loads and caches MenuButton's stylesheet so every instance is styled without
    /// manual USS wiring. Resolution order: <see cref="Override"/> if set, otherwise
    /// Resources.Load at <see cref="ResourcePath"/>.
    /// </summary>
    public static class MenuButtonStyle
    {
        public static string ResourcePath = "MenuButton";
        public static StyleSheet Override;

        private static StyleSheet cached;
        private static bool warned;

        public static void Apply(VisualElement target)
        {
            var sheet = Override != null ? Override : GetCachedSheet();
            if (sheet != null && !target.styleSheets.Contains(sheet))
                target.styleSheets.Add(sheet);
        }

        public static void ResetCache()
        {
            cached = null;
            warned = false;
        }

        private static StyleSheet GetCachedSheet()
        {
            if (cached == null)
            {
                cached = Resources.Load<StyleSheet>(ResourcePath);
                if (cached == null && !warned)
                {
                    warned = true;
                    Debug.LogWarning($"MenuButton: no stylesheet found at Resources/{ResourcePath}.uss. " +
                        $"Place it there, change {nameof(MenuButtonStyle)}.{nameof(ResourcePath)}, or set {nameof(MenuButtonStyle)}.{nameof(Override)}.");
                }
            }
            return cached;
        }
    }
}
