using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.UI.Hotkeys
{
    [CreateAssetMenu(fileName = "RewiredControllerProfile", menuName = "Rewired/Controller Profile", order = 0)]
    public class RewiredControllerProfile : ScriptableObject
    {
        [SerializeField]
        private string _guid;
        public string guid => _guid;
        [SerializeField]
        private ElementAssets[] _assets;

        public bool isEmpty => _assets == null || _assets.Length == 0;

        public bool TryGetValue(int elementId, out ElementAssets result)
        {
            foreach (var asset in _assets)
            {
                if (asset.elementId != elementId)
                    continue;

                result = asset;
                return true;
            }

            result = default;
            return false;
        }

#if UNITY_EDITOR
        public void EditorSetGuid(string guid)
        {
            _guid = guid;
        }

        public void EditorSetAssets(ElementAssets[] assets)
        {
            _assets = assets;
        }
#endif

        [Serializable]
        public class ElementAssets
        {
            [SerializeField]
            private string _name;
            public string name => _name;
            [SerializeField]
            private int _elementId;
            public int elementId => _elementId;
            [SerializeField]
            private GraphicAssets _graphicAssets;
            public GraphicAssets graphicAssets => _graphicAssets;

#if UNITY_EDITOR
            public ElementAssets(string name, int elementId)
            {
                _name = name;
                _elementId = elementId;
                _graphicAssets = new GraphicAssets();
            }
#endif
            public static int SortByElementId(ElementAssets a, ElementAssets b)
            {
                return a.elementId.CompareTo(b.elementId);
            }

            public enum State : byte
            {
                Normal  = 0,
                Pressed = 1,
            }
        }

        [Serializable]
        public class GraphicAssets
        { 
            [SerializeField]
            private Sprite _normal;
            public Sprite normal => _normal;
            [SerializeField]
            private Sprite _pressed;
            public Sprite pressed => _pressed;

            public Sprite GetSprite(ElementAssets.State state)
            {
                switch (state)
                {
                    case ElementAssets.State.Normal:
                        return _normal;
                    case ElementAssets.State.Pressed:
                        return _pressed ?? _normal;
                    default:
                        return null;
                }
            }
        }
    }
}
