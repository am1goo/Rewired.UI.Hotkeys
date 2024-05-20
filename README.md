# Rewired.UI.Hotkey
An expansions to Guavaman's Rewired Unity Asset that provides a easy-to-use API for showing input icons to the player at runtime based on their input method and bindings.

## Features
- full supports of Rewired's glyphs in UI (one `RewiredHotkey` component for all needs)
- binded to Rewired's actions (you don't miss any of actions)
- fast integration of keyboard and mouse glyphs (just create `Rewired->Controller Profile` scriptable object and make one click `Add Keyboard Buttons`)

## How to use
- place `RewiredGlyphs` and `RewiredHotkeys` on the same (or not) prefab as `RewiredInput` script placed
- create controller profiles for each device what you need and link these profiles to `RewiredGlyphs`
- add `RewiredHotkey` component into your UI and select specific action what you want to bind

## Installation
##### via Unity Package Manager
The latest version can be installed via [package manager](https://docs.unity3d.com/Manual/upm-ui-giturl.html) using following git URL: \
`https://github.com/am1goo/Rewired.UI.Hotkeys.git#1.0.0`

## Requirements
- Unity Engine 2019.x
- Rewired Advanced Input System [Asset Store](https://assetstore.unity.com/packages/tools/utilities/rewired-21676)

## Tested in
- Unity 2019.4.x
- Unity 2020.3.x
  
## Using in
[Sin Slayers](https://www.gog.com/en/game/sin_slayers) - RPG with roguelike elements set in a dark fantasy world, where your choices determine how challenging the fights and enemies will be.

## Contribute
Contribution in any form is very welcome. Bugs, feature requests or feedback can be reported in form of Issues.
