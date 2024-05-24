using System;
using UnityEngine;

namespace Rewired.UI.Hotkeys
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RewiredActionIdPropertyAttribute : PropertyAttribute
    {
        public const string defaultType = "RewiredConsts.Action";

        private string _type;
        public string type => _type;

        public RewiredActionIdPropertyAttribute(string type = defaultType)
        {
            _type = type;
        }
    }
}
