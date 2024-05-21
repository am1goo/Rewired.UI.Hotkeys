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
        private static readonly Dictionary<string, Type> _types = new Dictionary<string, Type>();
        private static Assembly _assembly;

        private bool _initialized;
        private Type _type;
        private int _selectedIndex;
        private string[] _options;
        private Dictionary<int, string> _stringsByValue;
        private Dictionary<string, int> _valuesByString;

        private const string _noneStr = "None";
        private const int _noneValue = -1;

        private void Initialize(SerializedProperty property, Type type)
        {
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
            {
                Initialize(property, _type);
                _initialized = true;
            }

            var selectedIndex = EditorGUI.Popup(position, _selectedIndex, _options);
            if (selectedIndex == _selectedIndex)
                return;

            SetIndexOf(property, selectedIndex);
            _selectedIndex = selectedIndex;
        }

        private static Type Lookup(string lookupType)
        {
            if (_types.TryGetValue(lookupType, out var exist))
                return exist;

            if (_assembly == null)
            {
                var assemblies = AppDomain.CurrentDomain.GetAssemblies();
                _assembly = Array.Find(assemblies, x => x.FullName == "Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null");
            }

            var type = _assembly.GetType(lookupType);
            _types.Add(lookupType, type);
            return type;
        }
    }
}
