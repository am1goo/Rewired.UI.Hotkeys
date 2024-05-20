using Rewired;
using System;
using UnityEngine;

[Serializable]
public class HotkeyAction
{
    [SerializeField]
#if REWIRED_UI_HOTKEYS_USE_CONSTS
    [ActionIdProperty(typeof(RewiredConsts.Action))]
#endif
    private int _actionId = -1;
    public int actionId
    {
        get => _actionId;
        set => _actionId = value;
    }
}
