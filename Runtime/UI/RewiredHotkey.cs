using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.Hotkeys
{
    [AddComponentMenu("Rewired/Rewired Hotkey")]
    public class RewiredHotkey : MonoBehaviour
    {
        [SerializeField]
        private Image _icon;
        [SerializeField]
        private HotkeyAction _action;

        private RewiredHotkeys.Player _player;
        public RewiredHotkeys.Player player { get => _player; set => SetPlayer(value); }

        public int actionId
        {
            get => _action.actionId;
            set
            {
                if (_action.actionId == value)
                    return;

                _action.actionId = value;
                OnActionChanged(value);
            }
        }

        private void OnValidate()
        {
            _icon = GetComponentInChildren<Image>();
        }

        private void Awake()
        {
            OnControllerChanged(null);

            RewiredGlyphs.onOverrideChanged += OnGlyphsOverrideChanged;
        }

        private IEnumerator Start()
        {
            yield return new WaitUntil(() => RewiredHotkeys.isReady);

            OnActionChanged(actionId);

            if (_player != null)
                yield break;

            var systemPlayer = RewiredHotkeys.systemPlayer;
            SetPlayer(systemPlayer);
        }

        private void OnDestroy()
        {
            RewiredGlyphs.onOverrideChanged -= OnGlyphsOverrideChanged;

            if (_player != null)
            {
                _player.onControllerChanged -= OnControllerChanged;
                _player = null;
            }
        }

        private void OnEnable()
        {
            if (_player == null)
            {
                var systemPlayer = RewiredHotkeys.systemPlayer;
                SetPlayer(systemPlayer);
            }
            else
            {
                OnControllerChanged(_player.lastActiveController);
                _player.onControllerChanged -= OnControllerChanged;
                _player.onControllerChanged += OnControllerChanged;
            }
        }

        private void OnDisable()
        {
            if (_player != null)
                _player.onControllerChanged -= OnControllerChanged;
        }

        public void SetPlayer(RewiredHotkeys.Player player)
        {
            if (_player == player)
                return;

            if (_player != null)
            {
                _player.onControllerChanged -= OnControllerChanged;
                _player = null;
            }

            _player = player;

            if (_player != null)
            {
                OnControllerChanged(_player.lastActiveController);
                _player.onControllerChanged += OnControllerChanged;
            }
        }

        private void OnControllerChanged(Controller controller)
        {
            OnUpdateWidget(controller, actionId);
        }

        private void OnGlyphsOverrideChanged(ControllerType controllerType)
        {
            var controller = _player != null ? _player.lastActiveController : null;
            if (controller == null || controller.type != controllerType)
                return;

            OnUpdateWidget(controller, actionId);
        }

        private void OnActionChanged(int actionId)
        {
            var controller = _player != null ? _player.lastActiveController : null;
            OnUpdateWidget(controller, actionId);
        }

        private void OnUpdateWidget(Controller controller, int actionId)
        {
            if (_player == null)
            {
                _icon.sprite = null;
                _icon.enabled = false;
                return;
            }

            if (!RewiredGlyphs.TryGetAssets(_player.rewiredPlayer, controller, actionId, out var assets))
            {
                _icon.sprite = null;
                _icon.enabled = false;
                return;
            }

            var sprite = assets.graphicAssets.GetSprite(RewiredControllerProfile.ElementAssets.State.Normal);
            _icon.enabled = true;
            _icon.sprite = sprite;
        }

        [Serializable]
        public class HotkeyAction
        {
            [RewiredActionIdProperty()]
            [SerializeField]
            private int _actionId = -1;
            public int actionId
            {
                get => _actionId;
                set => _actionId = value;
            }
        }
    }
}
