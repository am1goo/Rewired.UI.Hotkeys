using System.IO;
using UnityEditor;
using UnityEngine;

namespace Rewired.UI.Hotkeys
{
    public static class RewiredEditorUtility
    {
        private const string _objectName = "Rewired Hotkeys Manager";
        private static readonly string _objectNameAndExtension = $"{_objectName}.prefab";
        private static readonly string _assetPath = $"Assets/Rewired.UI.Hotkeys/{_objectNameAndExtension}";

        public static void CreateObjectInScene()
        {
            var existedObject = FindObjectInScene();
            if (existedObject != null)
            {
                EditorUtility.DisplayDialog("Warning", $"{_objectNameAndExtension} already exists in scene", "Okay");
                Selection.activeObject = existedObject;
                return;
            }
            CreateObject();

            GameObject FindObjectInScene()
            {
                var hotkeys = Object.FindObjectOfType<RewiredHotkeys>();
                if (hotkeys != null)
                    return hotkeys.gameObject;

                var glyphs = Object.FindObjectOfType<RewiredGlyphs>();
                if (glyphs != null)
                    return glyphs.gameObject;

                var activeScene = UnityEngine.SceneManagement.SceneManager.GetActiveScene();
                var rootObjects = activeScene.GetRootGameObjects();
                foreach (var obj in rootObjects)
                {
                    if (obj.name == _objectName)
                        return obj;
                }

                return null;
            }
        }

        public static void CreateObjectAsPrefab()
        {
            var existedPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(_assetPath);
            if (existedPrefab != null)
            {
                EditorUtility.DisplayDialog("Warning", $"{_objectNameAndExtension} already exists in project at {_assetPath}", "Okay");
                Selection.activeObject = existedPrefab;
                return;
            }

            var assetFolder = Path.GetDirectoryName(_assetPath).Replace("Assets\\", "");
            AssetDatabase.CreateFolder("Assets", assetFolder);

            var obj = CreateObject();
            try
            {
                var prefab = PrefabUtility.SaveAsPrefabAsset(obj, _assetPath, out var success);
                AssetDatabase.Refresh();
                if (success)
                    Selection.activeObject = prefab;
            }
            finally
            {
                Object.DestroyImmediate(obj.gameObject);
            }
        }

        private static GameObject CreateObject()
        {
            GameObject obj = new GameObject(_objectName);
            var hotkeys = obj.AddComponent<RewiredHotkeys>();
            var glyphs = obj.AddComponent<RewiredGlyphs>();

            var profiles = FindProfilesInAssets();
            glyphs.EditorSetProfiles(profiles);
            return obj;
        }

        public static RewiredControllerProfile[] FindProfilesInAssets()
        {
            var profilesGuids = AssetDatabase.FindAssets($"t:{typeof(RewiredControllerProfile)}");
            var profiles = new RewiredControllerProfile[profilesGuids.Length];
            for (int i = 0; i < profiles.Length; ++i)
            {
                var guid = profilesGuids[i];
                var path = AssetDatabase.GUIDToAssetPath(guid);
                profiles[i] = AssetDatabase.LoadAssetAtPath<RewiredControllerProfile>(path);
            }
            return profiles;
        }

        public static RewiredHotkey AddRewiredHotkey(UnityEngine.Object context)
        {
            if (context is not GameObject go)
                return null;

            var t = go.GetComponent<RewiredHotkey>();
            if (t != null)
                return t;

            return go.AddComponent<RewiredHotkey>();
        }
    }
}
