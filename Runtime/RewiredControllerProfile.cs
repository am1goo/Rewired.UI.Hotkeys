using System;
using System.Collections.Generic;
using System.Linq;
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

        public bool EditorImportAssets(List<Sprite> sprites, out string report, out string error)
        {
            sprites.Sort(SortBeSpriteName);

            var pairs = new Dictionary<string, string>();
            foreach (var asset in _assets)
            {
                var found = sprites.FindAll(x => OnFindAll(x, asset));

                var normal = found.Find(x => OnFindSpriteByState(x, ElementAssets.State.Normal));
                var pressed = found.Find(x => OnFindSpriteByState(x, ElementAssets.State.Pressed));

                if (normal != null && asset.graphicAssets.normal == null && !pairs.ContainsKey(normal.name))
                {
                    asset.graphicAssets.EditorSetSprite(ElementAssets.State.Normal, normal);
                    pairs.Add(normal.name, asset.name);
                }

                if (pressed != null && asset.graphicAssets.pressed == null && !pairs.ContainsKey(pressed.name))
                {
                    asset.graphicAssets.EditorSetSprite(ElementAssets.State.Pressed, pressed);
                    pairs.Add(pressed.name, asset.name);
                }
            }

            if (pairs.Count > 0)
            {
                var spritesInfo = string.Join(Environment.NewLine, pairs.Select(x => $"{x.Key} -> {x.Value}"));
                report = $"Total sprites: {pairs.Count}{Environment.NewLine}{spritesInfo}";
                error = default;
                return true;
            }
            else
            {
                report = default;
                error = "No sprites found";
                return false;
            }
        }

        private static bool OnFindAll(Sprite sprite, ElementAssets asset)
        {
            var lowerSpriteName = sprite.name.ToLowerInvariant();
            var lowerAssetName = asset.name.ToLowerInvariant();
            return lowerSpriteName.StartsWith($"{lowerAssetName}_") || lowerSpriteName.Contains($"_{lowerAssetName}_") || lowerSpriteName.EndsWith($"_{lowerAssetName}");
        }

        private static bool OnFindSpriteByState(Sprite sprite, ElementAssets.State state)
        {
            var lowerSpriteName = sprite.name.ToLowerInvariant();
            switch (state)
            {
                case ElementAssets.State.Normal:
                    return !lowerSpriteName.Contains("pressed");
                case ElementAssets.State.Pressed:
                    return lowerSpriteName.Contains("pressed");
                default:
                    return false;
            }
        }

        private static int SortBeSpriteName(Sprite a, Sprite b)
        {
            return a.name.Length.CompareTo(b.name.Length);
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

            public void EditorSetSprite(State state, Sprite sprite)
            {
                _graphicAssets.EditorSetSprite(state, sprite);
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

#if UNITY_EDITOR
            public void EditorSetSprite(ElementAssets.State state, Sprite sprite)
            {
                switch (state)
                {
                    case ElementAssets.State.Normal:
                        _normal = sprite;
                        break;
                    case ElementAssets.State.Pressed:
                        _pressed = sprite;
                        break;
                }
            }
#endif
        }
    }
}
