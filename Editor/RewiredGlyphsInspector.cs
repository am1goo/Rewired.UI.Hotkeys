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
                var profiles = RewiredEditorUtility.FindProfilesInAssets();
                glyphs.EditorSetProfiles(profiles);
                EditorUtility.SetDirty(target);
            }
        }
    }
}
