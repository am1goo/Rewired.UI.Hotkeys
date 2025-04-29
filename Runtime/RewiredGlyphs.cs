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

        private readonly Dictionary<Guid, RewiredControllerProfile> _profilesByGuid = new Dictionary<Guid, RewiredControllerProfile>();
        private readonly Dictionary<ControllerType, Guid> _overridesByControllerType = new Dictionary<ControllerType, Guid>();

        public delegate void OnOverrideChangedDelegate(ControllerType controllerType);
        public static OnOverrideChangedDelegate onOverrideChanged;

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

                if (_profilesByGuid.ContainsKey(guid))
                {
                    Debug.LogWarning($"OnEnable: profile with guid '{profile.guid}' already registered, please double check it!");
                    continue;
                }

                _profilesByGuid[guid] = profile;
            }
        }

        private void OnDisable()
        {
            _profilesByGuid.Clear();
        }

        public static bool TryGetAssets(Player player, Controller controller, int actionId, out RewiredControllerProfile.ElementAssets result)
        {
            if (player == null)
            {
                result = default;
                return false;
            }

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

            var map = player.controllers.maps.GetFirstElementMapWithAction(controller.type, controller.id, actionId, skipDisabledMaps: true);
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
            if (_overridesByControllerType.TryGetValue(type, out var overrideGuid))
            {
                guid = overrideGuid;
            }

            if (!_profilesByGuid.TryGetValue(guid, out var profile))
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

        public static bool SetOverrideByControllerType(ControllerType type, Guid guid)
        {
            if (_instance == null)
                return false;

            return _instance.SetOverrideByControllerTypeInternal(type, guid);
        }

        private bool SetOverrideByControllerTypeInternal(ControllerType type, Guid guid)
        { 
            if (guid == Guid.Empty)
            {
                if (_overridesByControllerType.ContainsKey(type))
                {
                    _overridesByControllerType.Remove(type);
                    onOverrideChanged?.Invoke(type);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                _overridesByControllerType[type] = guid;
                onOverrideChanged?.Invoke(type);
                return true;
            }
        }

#if UNITY_EDITOR
        public void EditorSetProfiles(RewiredControllerProfile[] profiles)
        {
            _profiles = profiles;
        }
#endif
    }
}
