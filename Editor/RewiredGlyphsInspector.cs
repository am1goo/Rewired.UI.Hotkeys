using UnityEngine;
using UnityEditor;

namespace Rewired.UI.Hotkeys
{
    [CustomEditor(typeof(RewiredGlyphs))]
    public class RewiredGlyphsInspector : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();

            var glyphs = target as RewiredGlyphs;
            if (GUILayout.Button("Find Profiles"))
            {
                var profilesGuids = AssetDatabase.FindAssets($"t:{typeof(RewiredControllerProfile)}");
                var profiles = new RewiredControllerProfile[profilesGuids.Length];
                for (int i =0; i < profiles.Length; ++i)
                {
                    var guid = profilesGuids[i];
                    var path = AssetDatabase.GUIDToAssetPath(guid);
                    profiles[i] = AssetDatabase.LoadAssetAtPath<RewiredControllerProfile>(path);
                }
                glyphs.EditorSetProfiles(profiles);
                EditorUtility.SetDirty(target);
            }
        }
    }
}
