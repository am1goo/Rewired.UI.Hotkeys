using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace Rewired.UI.Hotkeys
{
    [CustomPropertyDrawer(typeof(RewiredActionIdPropertyAttribute))]
    public class RewiredActionIdPropertyDrawer : PropertyDrawer
    {
        private static readonly string[] _optionsNotInitialized = new string[] { "None" };
        private static readonly Dictionary<string, Type> _types = new Dictionary<string, Type>();

        private bool _initialized;
        private Type _type;
        private int _selectedIndex;
        private string[] _options;
        private Dictionary<int, string> _stringsByValue;
        private Dictionary<string, int> _valuesByString;

        private const string _noneStr = "None";
        private const int _noneValue = -1;

        private bool Initialize(SerializedProperty property, Type type)
        {
            if (type == null)
                return false;

            var stringsByValue = new Dictionary<int, string>();
            var valuesByString = new Dictionary<string, int>();
            var options = new List<string>();

            stringsByValue.Add(_noneValue, _noneStr);
            valuesByString.Add(_noneStr, _noneValue);
            options.Add(_noneStr);

            var fis = type.GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var fi in fis)
            {
                var attr = fi.GetCustomAttribute<Dev.ActionIdFieldInfoAttribute>();
                if (attr == null)
                    continue;

                var str = $"{attr.categoryName}/{attr.friendlyName}";

                var value = (int)fi.GetValue(null);
                stringsByValue.Add(value, str);
                valuesByString.Add(str, value);
                options.Add(str);
            }

            _stringsByValue = stringsByValue;
            _valuesByString = valuesByString;
            _options = options.ToArray();
            _selectedIndex = GetIndexOf(property);
            return true;
        }

        private int GetIndexOf(SerializedProperty property)
        {
            var value = property.intValue;
            var str = _stringsByValue[value];
            return Array.IndexOf(_options, str);
        }

        private void SetIndexOf(SerializedProperty property, int i)
        {
            var str = _options[i];
            if (!_valuesByString.TryGetValue(str, out var value))
                value = _noneValue;

            property.intValue = value;
        }

        public override bool CanCacheInspectorGUI(SerializedProperty property)
        {
            return false;
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var attr = attribute as RewiredActionIdPropertyAttribute;
            if (_type == null)
                _type = Lookup(attr.type);

            if (!_initialized)
                _initialized = Initialize(property, _type);

            if (_initialized)
            {
                var selectedIndex = EditorGUI.Popup(position, _selectedIndex, _options);
                if (selectedIndex == _selectedIndex)
                    return;

                SetIndexOf(property, selectedIndex);
                _selectedIndex = selectedIndex;
            }
            else
            {
                var prevEnabled = GUI.enabled;
                GUI.enabled = false;
                EditorGUI.Popup(position, 0, _optionsNotInitialized);
                GUI.enabled = prevEnabled;

                EditorGUILayout.HelpBox("Export 'RewiredConsts.cs' file into project via Rewired Input Manager (Tools -> Export)", MessageType.Warning);
            }
        }

        private static Type Lookup(string lookupType)
        {
            if (_types.TryGetValue(lookupType, out var exist))
                return exist;

            var found = default(Type);
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            foreach (var assembly in assemblies)
            {
                found = assembly.GetType(lookupType);
                if (found != null)
                    break;
            }

            _types.Add(lookupType, found);
            return found;
        }
    }
}
