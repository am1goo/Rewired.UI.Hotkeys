using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.UI
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
        public struct ElementAssets
        {
            [SerializeField]
            private string _name;
            public string name => _name;
            [SerializeField]
            private int _elementId;
            public int elementId => _elementId;
            [SerializeField]
            private Texture2D _normal;
            public Texture2D normal => _normal;
            [SerializeField]
            private Texture2D _pressed;
            public Texture2D pressed => _pressed;

#if UNITY_EDITOR
            public ElementAssets(string name, int elementId)
            {
                _name = name;
                _elementId = elementId;
                _normal = null;
                _pressed = null;
            }
#endif

            public Texture2D GetTexture(State state)
            {
                switch (state)
                {
                    case State.Normal: return _normal;
                    case State.Pressed: return _pressed;
                    default: return null;
                }
            }

            public Sprite GetSprite(State state, int pixelPerUnit = 100)
            {
                Texture2D tex = GetTexture(state);
                if (tex == null)
                    return null;

                Rect rect = new Rect(0, 0, tex.width, tex.height);
                Vector2 pivot = new Vector2(0.5f, 0.5f);
                return Sprite.Create(tex, rect, pivot, pixelPerUnit);
            }

            public static int SortByElementId(ElementAssets a, ElementAssets b)
            {
                return a.elementId.CompareTo(b.elementId);
            }

            public enum State : byte
            {
                Normal = 0,
                Pressed = 1,
            }
        }
    }
}
