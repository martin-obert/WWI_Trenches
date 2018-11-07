using System.IO;
using UnityEditor;
using UnityEngine;

namespace Assets.ObjAnimations
{
    public static class ScriptableObjectUtility
    {
        /// <summary>
        //	This makes it easy to create, name and place unique new ScriptableObject asset files.
        /// </summary>
        public static void CreateAsset<T>(this T asset, string path, string name) where T : ScriptableObject
        {
            if (path.StartsWith(Application.dataPath))
            {
                path = "Assets" + path.Substring(Application.dataPath.Length);
            }

            
            string assetPathAndName = AssetDatabase.GenerateUniqueAssetPath("Assets/Resources/New_" + name.Replace(" ", "_") + ".asset");

            AssetDatabase.CreateAsset(asset, assetPathAndName);

            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            EditorUtility.FocusProjectWindow();
            Selection.activeObject = asset;

        }
    }
}