using System;
using UnityEngine;

namespace Rewired.UI.Hotkeys
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property)]
    public class RewiredActionIdPropertyAttribute : PropertyAttribute
    {
        private string _type;
        public string type => _type;

        public RewiredActionIdPropertyAttribute(string type)
        {
            _type = type;
        }
    }
}
