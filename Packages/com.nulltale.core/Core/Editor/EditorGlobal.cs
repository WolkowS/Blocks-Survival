using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace CoreLib
{
    public static class EditorGlobal
    {
        [PostProcessScene(2)]
        public static void RemoveEditorAttributes()
        {
            foreach (var type in TypeCache.GetTypesWithAttribute<EditorOnlyAttribute>())
            {
                foreach (var toDestroy in Object.FindObjectsOfType(type))
                    Object.Destroy(toDestroy);
            }
        }

        [MenuItem("CONTEXT/Component/Show or Hide", false, priority:0)]
        public static void ShowHide(MenuCommand command)
        {
            var cmp = (Component)command.context;
            var show = cmp.hideFlags.HasFlag(HideFlags.HideInInspector);
            if (show)
                cmp.hideFlags &= ~HideFlags.HideInInspector;
            else
                cmp.hideFlags |= HideFlags.HideInInspector;

            //Undo.RegisterCompleteObjectUndo(cmp, $"{(show ? "Show" : "Hide")} {cmp}");
        }
        
        [MenuItem("CONTEXT/Renderer/Material", false, priority:0)]
        public static void MaterialInstance(MenuCommand command)
        {
            const string k_MaterialSuffix = " _";
            var renderer = (Renderer)command.context;

            var mat = new Material(renderer.sharedMaterial);
            if (mat.name.EndsWith(k_MaterialSuffix) == false)
                mat.name += k_MaterialSuffix;

            renderer.sharedMaterial = mat;
            Debug.Log($"Material instance was created on <color=yellow>{renderer}</color>", renderer);
        }
        
        [MenuItem("CONTEXT/Renderer/Save Material", false, priority:0)]
        public static void SaveMaterialFromRenderer(MenuCommand command)
        {
            var renderer = (Renderer)command.context;
            var mat      = new Material(renderer.sharedMaterial);

            var path = $"Assets/{renderer.gameObject.name}.mat";
            for (var n = 0; AssetDatabase.AssetPathToGUID(path).IsNullOrEmpty() == false; n++)
                path = $"Assets/{renderer.gameObject.name}_{n}.mat";

            renderer.sharedMaterial = mat;
            AssetDatabase.CreateAsset(mat, path);
            Debug.Log($"Material instance for {renderer}, has been saved in <color=yellow>{path}</color>", mat);
        }
        
        [MenuItem("CONTEXT/Material/Save Material", false, priority:0)]
        public static void SaveMaterialFromMaterial(MenuCommand command)
        {
            var material = (Material)command.context;
            var mat      = new Material(material);

            var path = $"Assets/{material.name}.mat";
            for (var n = 0; AssetDatabase.AssetPathToGUID(path).IsNullOrEmpty() == false; n++)
                path = $"Assets/{material.name}_{n}.mat";

            AssetDatabase.CreateAsset(mat, path);
            Debug.Log($"Material instance has been saved in <color=yellow>{path}</color>", mat);
        }
    }
}