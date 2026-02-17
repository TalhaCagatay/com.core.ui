# com.core.ui

A Unity package that provides a consistent way to show **popups** (stack-based) and **screens** (single full-screen at a time). It works with **com.core.initializer** and uses **com.core.dotween** for popup animations.

## Features

- **Popups** – Stack of modal or non-modal dialogs. Push a popup type; it is shown on top. Pop to close the top one. First time a type is pushed, it is instantiated from Resources and cached.
- **Screens** – Full-screen panels (e.g. main menu, loading). Only one screen is active; showing a new screen hides the current one. Same get-or-create caching by type.
- **UIController** – Implements `IController`; get it via `ControllerHandler.GetController<UIController>()`. Manages popup stack, screen switching, and a background GameObject that sits behind the top popup.
- **BasePopup** – Abstract base with DoTween scale animations (configurable duration and ease). Override `PlayShowAnimation` / `PlayHideAnimation` for custom behavior.
- **BaseScreen** – Abstract base that enables/disables the GameObject and raises show/hide events (no animation by default).

## Dependencies

- **Unity** (6000.3 or compatible)
- **com.core.initializer** (0.1.0) – controller registration and access
- **com.core.dotween** (0.1.0) – popup animations
- **com.cysharp.unitask** (2.5.10)

UniTask is used for async show/hide; it is typically pulled in via com.core.initializer.

## Installation

Add the package via Git (Package Manager → Add package from git URL) or add to your project’s `manifest.json`:

```json
"com.core.ui": "https://github.com/TalhaCagatay/com.core.ui.git#0.1.0"
```

Ensure **com.core.initializer** and **com.core.dotween** are installed and that the **UIParent** prefab and your popup/screen prefabs are in the correct Resources paths (see below).

## Resources Layout

All UI prefabs must live under a **Resources** folder so they can be loaded by type name.

| Asset | Path |
|-------|------|
| UIParent | `Resources/UIPrefabs/UIParent` (prefab with `UIParent` component) |
| Popups | `Resources/UIPrefabs/Popups/<TypeName>.prefab` (e.g. `ConfirmPopup.prefab`) |
| Screens | `Resources/UIPrefabs/Screens/<TypeName>.prefab` (e.g. `MainMenuScreen.prefab`) |

- **Prefab name must match the script/type name.** A popup script named `ConfirmPopup` must have a prefab named `ConfirmPopup` (or `ConfirmPopup.prefab`).
- **UIParent** has a component that exposes three references: **ScreenParent**, **PopupParent**, and **BackgroundGO**. Popups are instantiated under `PopupParent`, screens under `ScreenParent`. The background is shown/hidden and ordered behind the top popup automatically.

## API Overview

### Getting UIController

Requires com.core.initializer. After controllers are initialized:

```csharp
var ui = ControllerHandler.GetController<UIController>();
```

### Popups (stack)

| Method | Description |
|--------|-------------|
| `PushPopupAsync<TPopup>()` | Shows the popup of type `TPopup` on top of the stack. Returns when show animation completes. Creates and caches instance on first use. |
| `PushPopup<TPopup>()` | Fire-and-forget version of `PushPopupAsync`. |
| `PopPopupAsync()` | Closes the top popup (hide animation). |
| `PopPopup()` | Fire-and-forget version of `PopPopupAsync`. |
| `PeekPopup()` | Returns the current top popup, or `null` if the stack is empty. |

### Screens (single active)

| Method | Description |
|--------|-------------|
| `ShowScreenAsync<TScreen>()` | Shows the screen of type `TScreen`. Hides the current screen if different. Returns when shown. |
| `ShowScreen<TScreen>()` | Fire-and-forget version of `ShowScreenAsync`. |
| `HideScreenAsync<TScreen>()` | Hides the screen of type `TScreen` (must have been shown at least once). |
| `HideScreen<TScreen>()` | Fire-and-forget version of `HideScreenAsync`. |

### Example usage

```csharp
using com.core.initializer;
using com.core.ui;
using Cysharp.Threading.Tasks;
using UnityEngine;

public class MenuLogic : MonoBehaviour
{
    private async void Start()
    {
        await ControllerHandler.InitializationCompleted.Task;

        var ui = ControllerHandler.GetController<UIController>();

        // Show main menu screen
        await ui.ShowScreenAsync<MainMenuScreen>();

        // Open a popup on top
        var popup = await ui.PushPopupAsync<ConfirmPopup>();
        // ... configure popup, wait for user choice ...

        await ui.PopPopupAsync();

        // Switch to game screen
        await ui.ShowScreenAsync<GameScreen>();
    }
}
```

## Creating a Popup

1. Create a script that inherits from `BasePopup`.
2. Create a prefab with the **exact same name** as the script and place it under `Resources/UIPrefabs/Popups/`.
3. Attach your script to the prefab.

You can override `PlayShowAnimation` and `PlayHideAnimation` for custom DoTween (or other) animations. Base implementation uses scale with configurable duration and ease.

## Creating a Screen

1. Create a script that inherits from `BaseScreen`.
2. Create a prefab with the **exact same name** as the script and place it under `Resources/UIPrefabs/Screens/`.
3. Attach your script to the prefab.

Screens are simply shown/hidden (GameObject active state); override `ShowAsync`/`HideAsync` if you need transitions.

## IUI Interface

Both `BasePopup` and `BaseScreen` implement `IUI`:

- **ShowAsync()** / **HideAsync()** – Async show/hide.
- **Showed** / **Hided** – Events when the UI is shown or hidden.

## Limitations

- Popup and screen prefabs must be under the specified Resources paths.
- Prefab name must match the type name of the script.
- UIParent prefab must exist at `Resources/UIPrefabs/UIParent` (in a project that has a Resources folder visible to the com.core.ui assembly, e.g. in the main application).

## License

See the repository or package for license information.
