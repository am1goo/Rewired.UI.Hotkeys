using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;
using System.Linq;
using System.IO;

namespace Rewired.UI.Hotkeys
{
    [CustomEditor(typeof(RewiredControllerProfile))]
    public class RewiredControllerProfileInspector : UnityEditor.Editor
    {
        private const string _ppRecentOpenedFolder = "REWIRED_UI_HOTKEYS_RECENT_OPENED_FOLDER";

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var profile = target as RewiredControllerProfile;
            if (profile.isEmpty)
            {
                if (GUILayout.Button("Add Keyboard Buttons"))
                {
                    var assets = CreateKeyboardButtons();
                    SetAssets(profile, assets);
                    EditorUtility.SetDirty(target);
                }
                if (GUILayout.Button("Add Mouse Buttons"))
                {
                    var assets = CreateMouseButtons();
                    SetAssets(profile, assets);
                    EditorUtility.SetDirty(target);
                }
            }
            else
            {
                if (GUILayout.Button("Import Sprites .."))
                {
                    var recentOpenedFolder = EditorPrefs.GetString(_ppRecentOpenedFolder, Application.dataPath);
                    recentOpenedFolder = EditorUtility.OpenFolderPanel("Select folder with sprites", recentOpenedFolder, string.Empty);
                    if (!string.IsNullOrWhiteSpace(recentOpenedFolder))
                    {
                        var sprites = new List<Sprite>();
                        if (FindSpritesForAssets(profile, recentOpenedFolder, sprites, out var error))
                        {
                            if (profile.EditorImportAssets(sprites, out var report, out var err))
                            {
                                EditorPrefs.SetString(_ppRecentOpenedFolder, recentOpenedFolder);
                                EditorUtility.DisplayDialog("Import completed", report, "Okay");
                                Debug.Log($"Import completed:{Environment.NewLine}{report}");
                            }
                            else
                            {
                                EditorUtility.DisplayDialog("Error", err, "Okay");
                                Debug.LogError($"Error: {err}");
                            }
                        }
                        else
                        {
                            EditorUtility.DisplayDialog("Error", error, "Okay");
                            Debug.LogError($"Error: {error}");
                        }
                    }
                }
            }
        }

        private static void SetAssets(RewiredControllerProfile profile, DefaultAssets assets)
        {
            SetAssets(profile, assets.instanceId, assets.assets);
        }

        private static void SetAssets(RewiredControllerProfile profile, Guid instanceId, IEnumerable<RewiredControllerProfile.ElementAssets> assets)
        {
            profile.EditorSetGuid(instanceId.ToString());
            profile.EditorSetAssets(assets.ToArray());
        }

        private static bool FindSpritesForAssets(RewiredControllerProfile profile, string folderPath, List<Sprite> result, out string error)
        {
            var di = new DirectoryInfo(folderPath);
            if (!di.Exists)
            {
                error = $"Folder {di.FullName} not exists";
                return false;
            }

            var diPath = Path.GetDirectoryName(di.FullName);
            var dataPath = Path.GetDirectoryName(Application.dataPath);
            if (!diPath.StartsWith(dataPath))
            {
                error = $"Folder {di.FullName} should be inside of Assets project folder";
                return false;
            }

            foreach (var fi in di.GetFiles())
            {
                switch (fi.Extension)
                {
                    case ".meta":
                        continue;
                }

                var filePath = fi.FullName;
                var pathInAssets = filePath.Substring(dataPath.Length, filePath.Length - dataPath.Length).TrimStart(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
                var sprite = AssetDatabase.LoadAssetAtPath<Sprite>(pathInAssets);
                result.Add(sprite);
            }

            error = default;
            return true;
        }

        private static DefaultAssets CreateKeyboardButtons()
        {
            var keyboardType = typeof(Keyboard);

            var constsType = typeof(Consts);
            var instanceIdField = constsType.GetField("hardwareTypeGuid_universalKeyboard", BindingFlags.NonPublic | BindingFlags.Static);
            var instanceId = (Guid)instanceIdField.GetValue(null);

            var keyIndexToKeyboardKeyCodeProp = keyboardType.GetProperty("keyIndexToKeyboardKeyCode", BindingFlags.NonPublic | BindingFlags.Static);
            var keyIndexToKeyboardKeyCode = (KeyboardKeyCode[])keyIndexToKeyboardKeyCodeProp.GetValue(null);

            var list = new List<RewiredControllerProfile.ElementAssets>();
            foreach (var keyCode in Enum.GetValues(typeof(KeyboardKeyCode)))
            {
                var elementId = Array.IndexOf(keyIndexToKeyboardKeyCode, keyCode);
                if (elementId < 0)
                {
                    Debug.LogError($"CreateKeyboardButtons: elementId not found for {keyCode}");
                    continue;
                }

                var name = keyCode.ToString();
                list.Add(new RewiredControllerProfile.ElementAssets(name, elementId));
            }
            list.Sort(RewiredControllerProfile.ElementAssets.SortByElementId);
            return new DefaultAssets(instanceId, list);
        }

        private static DefaultAssets CreateMouseButtons()
        {
            var constsType = typeof(Consts);
            var instanceIdField = constsType.GetField("hardwareTypeGuid_universalMouse", BindingFlags.NonPublic | BindingFlags.Static);
            var instanceId = (Guid)instanceIdField.GetValue(null);

            var list = new List<RewiredControllerProfile.ElementAssets>()
            {
                new RewiredControllerProfile.ElementAssets("Mouse Horizontal", 0),
                new RewiredControllerProfile.ElementAssets("Mouse Wheel", 1),
                new RewiredControllerProfile.ElementAssets("Mouse Wheel", 2),
                new RewiredControllerProfile.ElementAssets("Left Mouse Button", 3),
                new RewiredControllerProfile.ElementAssets("Right Mouse Button", 4),
                new RewiredControllerProfile.ElementAssets("Mouse Button 3", 5),
                new RewiredControllerProfile.ElementAssets("Mouse Button 4", 6),
                new RewiredControllerProfile.ElementAssets("Mouse Button 5", 7),
                new RewiredControllerProfile.ElementAssets("Mouse Wheel Horizontal", 10),
            };
            return new DefaultAssets(instanceId, list);
        }

        private static Guid InputTools_FormatHardwareIdentifierString(string text)
        {
            var assembly = typeof(Utils.UnityTools).Assembly;
            var inputToolsType = assembly.GetType("Rewired.Utils.InputTools");

            var formatHardwareIdentifierStringMethod = inputToolsType.GetMethod("FormatHardwareIdentifierString", BindingFlags.Public | BindingFlags.Static);
            var formatHardwareIdentifierString = formatHardwareIdentifierStringMethod.Invoke(null, new object[] { text });

            var guidString = (string)formatHardwareIdentifierString;
            var guid = Guid.Parse(guidString);
            return guid;
        }

        private static Guid MiscTools_CreateGuidHashSHA1(string text)
        {
            var assembly = typeof(Utils.UnityTools).Assembly;
            var miscToolsType = assembly.GetType("Rewired.Utils.MiscTools");

            var createGuidHashSHA1Method = miscToolsType.GetMethod("CreateGuidHashSHA1", BindingFlags.Public | BindingFlags.Static);
            var createGuidHashSHA1 = createGuidHashSHA1Method.Invoke(null, new object[] { text });

            var guid = (Guid)createGuidHashSHA1;
            return guid;
        }

        private struct DefaultAssets
        {
            private Guid _instanceId;
            public Guid instanceId => _instanceId;

            private IEnumerable<RewiredControllerProfile.ElementAssets> _assets;
            public IEnumerable<RewiredControllerProfile.ElementAssets> assets => _assets;

            public DefaultAssets(Guid instanceId, IEnumerable<RewiredControllerProfile.ElementAssets> assets)
            {
                _instanceId = instanceId;
                _assets = assets;
            }
        }
    }
}
