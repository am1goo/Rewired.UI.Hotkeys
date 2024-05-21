using UnityEditor;

namespace Rewired.UI.Hotkeys
{
    public static class RewiredMenuCommands
    {
        [MenuItem(Consts.menuRoot + "/Create/Hotkeys Manager (in scene)")]
        public static void CreateObjectInScene(MenuCommand menuCommand)
        {
            RewiredEditorUtility.CreateObjectInScene();
        }

        [MenuItem("Window/Rewired/Create/Hotkeys Manager (prefab)")]
        public static void CreateObjectAsPrefab(MenuCommand menuCommand)
        {
            RewiredEditorUtility.CreateObjectAsPrefab();
        }

        [MenuItem("GameObject/UI/Rewired/Rewired Hotkey", false)]
        public static void AddRewiredHotkey(MenuCommand menuCommand)
        {
            RewiredEditorUtility.AddRewiredHotkey(menuCommand.context);
        }
    }
}
