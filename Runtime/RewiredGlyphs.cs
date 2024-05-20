using System;
using System.Collections.Generic;
using UnityEngine;

namespace Rewired.UI.Hotkeys
{
    public class RewiredGlyphs : MonoBehaviour
    {
        private static RewiredGlyphs _instance;

		[SerializeField]
		private bool _dontDestroyOnLoad = true;
        [SerializeField]
        private RewiredControllerProfile[] _profiles;

        private Dictionary<Guid, RewiredControllerProfile> _dict = new Dictionary<Guid, RewiredControllerProfile>();

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
			if (_dontDestroyOnLoad)
				DontDestroyOnLoad(this);
        }

        private void OnDestroy()
        {
            if (_instance == this)
                _instance = null;
        }

        private void OnEnable()
        {
            foreach (var profile in _profiles)
            {
                if (profile == null)
                    continue;

                if (!Guid.TryParse(profile.guid, out var guid))
                {
                    Debug.LogError($"OnEnable: guid '{profile.guid}' cannot be parsed");
                    continue;
                }

                if (_dict.ContainsKey(guid))
                {
                    Debug.LogWarning($"OnEnable: profile with guid '{profile.guid}' already registered, please double check it!");
                    continue;
                }

                _dict[guid] = profile;
            }
        }

        private void OnDisable()
        {
            _dict.Clear();
        }

        public static bool TryGetAssets(Controller controller, int actionId, out RewiredControllerProfile.ElementAssets result)
        {
            if (controller == null)
            {
                result = default;
                return false;
            }

            if (!ReInput.isReady)
            {
                result = default;
                return false;
            }    

            var map = ReInput.players.GetSystemPlayer().controllers.maps.GetFirstButtonMapWithAction(controller.type, actionId, skipDisabledMaps: true);
            if (map == null)
            {
                result = default;
                return false;
            }

            return TryGetAssets(controller.type, controller.hardwareTypeGuid, map.elementIdentifierId, out result);
        }

        public static bool TryGetAssets(ControllerType type, Guid guid, int elementId, out RewiredControllerProfile.ElementAssets result)
        {
            if (_instance == null)
            {
                result = default;
                return false;
            }
            return _instance.TryGetAssetsInternal(type, guid, elementId, out result);
        }

        private bool TryGetAssetsInternal(ControllerType type, Guid guid, int elementId, out RewiredControllerProfile.ElementAssets result)
        {
            if (!_dict.TryGetValue(guid, out var profile))
            {
                result = default;
                return false;
            }
            return GetAssetsFromProfile(profile, elementId, out result);
        }

        private bool GetAssetsFromProfile(RewiredControllerProfile profile, int elementId, out RewiredControllerProfile.ElementAssets result)
        {
            if (profile == null)
            {
                result = default;
                return false;
            }

            return profile.TryGetValue(elementId, out result);
        }

#if UNITY_EDITOR
        public void EditorSetProfiles(RewiredControllerProfile[] profiles)
        {
            _profiles = profiles;
        }
#endif
    }
}
