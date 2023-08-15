#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace CoreLib.Artistic
{
    public static class MenuItems
    {
        [Shortcut("Guidelines/Toggle Composition", KeyCode.J, ShortcutModifiers.Shift)]
        public static void Composition()
        {
            foreach (var gl in TryGetGuidelines())            
                gl.ToggleComposition();
        }
        
        [Shortcut("Guidelines/Toggle Flip", KeyCode.H, ShortcutModifiers.Shift)]
        public static void Flip()
        {
            foreach (var gl in TryGetGuidelines())
                gl.ToggleFlip();
        }
        
        [Shortcut("Guidelines/Toggle Grayscale", KeyCode.G, ShortcutModifiers.Shift)]
        public static void Grayscale()
        {
            foreach (var gl in TryGetGuidelines())
                gl.ToggleGrayscale();
        }
        
        // =======================================================================
        private static IEnumerable<ArtisticFeature> TryGetGuidelines()
        {
            // if (ArtisticFeature.Instances.Count == 0)
            //     Debug.Log($"Can't find {nameof(ArtisticFeature)}");

            foreach (var af in Extensions.FindAssets<ArtisticFeature>())
                yield return af;
            
            //foreach (var af in ArtisticFeature.Instances)
            //    yield return af;
        }
    }
}
#endif