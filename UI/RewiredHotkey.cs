using System;
using UnityEngine;
using UnityEngine.UI;

namespace Rewired.UI.Hotkeys
{
    [AddComponentMenu("UI/Rewired Hotkey")]
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
            _player = RewiredHotkeys.systemPlayer;
        }

        private void OnDestroy()
        {
            if (_player != null)
            {
                _player.onControllerChanged -= OnControllerChanged;
                _player = null;
            }
        }

        private void OnEnable()
        {
            if (_player != null)
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

        private void Start()
        {
            OnActionChanged(_action.actionId);
        }

        private void OnControllerChanged(Controller controller)
        {
            OnUpdateWidget(controller, actionId);
        }

        private void OnActionChanged(int actionId)
        {
            var controller = _player != null ? _player.lastActiveController : null;
            OnUpdateWidget(controller, actionId);
        }

        private void OnUpdateWidget(Controller controller, int actionId)
        {
            if (RewiredGlyphs.TryGetAssets(controller, actionId, out var assets))
            {
                var sprite = assets.GetSprite(RewiredControllerProfile.ElementAssets.State.Normal);
                _icon.enabled = true;
                _icon.sprite = sprite;
            }
            else
            {
                _icon.sprite = null;
                _icon.enabled = false;
            }
        }
    }
}
