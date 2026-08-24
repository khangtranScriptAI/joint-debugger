# Joint Position Debugger

A passive Android **joint-position visualizer** built in C# + ImGui. It draws
filled circle markers for 13 body joints over any other app so you can sanity
check the output of a 3D animation / pose system in isolation.

> **This is a debug visualizer only.** It does not inject, hook, modify, or
> otherwise interact with the app beneath the overlay. The overlay window is
> `FLAG_NOT_TOUCHABLE` — taps fall straight through to whatever is underneath.

---

## What it draws

For every joint in the spec:

```
Head, Neck, LeftWrist, Hip, Root, RightFoot, LeftFoot, RightWrist,
LeftHand, LeftShoulder, RightShoulder, LeftElbow, RightElbow
```

the pipeline is:

```
Entity → Joint 3D → W2S.WorldToScreen → Validation → Swap Left/Right
        → Vector2[] joints → check X > 0 && Y > 0 → Joint Marker → Label (optional)
```

and **only** filled circles (radius `3.5f`, color `LimeGreen`) plus a soft
outline (radius `4.5f`, alpha `80/255`). No lines, no bones, no skeleton, no
bounding boxes — by design and by spec.

---

## Project layout

```
JointDebugger/
├── JointDebugger.csproj
├── AndroidManifest.xml
├── MainActivity.cs                # permission flow + service launcher
├── Config/Config.cs               # all user-tunable knobs + Reset()
├── Data/
│   ├── Entity.cs                  # 13-joint skeleton container
│   └── JointType.cs               # the 13 joint names (exact spec set)
├── Math/
│   ├── Vector2.cs                 # screen-space
│   ├── Vector3.cs                 # world-space
│   ├── Vector4.cs                 # RGBA color
│   ├── CameraMatrix.cs            # 4x4 column-major
│   └── W2S.cs                     # WorldToScreen(camera, pos, w, h) -> Vector2?
├── Services/
│   ├── EntityProvider.cs          # TODO: replace with real game source
│   ├── CameraProvider.cs          # TODO: replace with real game source
│   └── OverlayService.cs          # SurfaceView + EGL + render loop
├── ImGui/
│   ├── EGLSetup.cs                # EGL 1.x + GLES 3.0 bootstrap
│   ├── ImGuiController.cs         # ImGui context + per-frame NewFrame/Render
│   └── InputBridge.cs             # no-op (overlay is non-touchable)
├── Rendering/
│   ├── DrawSkeleton.cs            # ← THE function (markers + labels only)
│   └── OverlayRenderer.cs         # config window + debug table
└── Util/
    └── ColorUtil.cs               # ColorToUint32 (IM_COL32 packing)
```

---

## Prerequisites

- .NET 8 SDK (`dotnet --version` ≥ `8.0.0`)
- Android SDK with API 26+ platform installed
- An Android NDK (only required if you later switch to a native ImGui backend
  via a `.so`; the pure-C# path needs no NDK)
- A device or emulator running **Android 8.0 (API 26) or newer**

NuGet packages (restored automatically):

| Package      | Version | Why                                      |
|--------------|---------|------------------------------------------|
| `ImGui.NET`  | 1.87.0  | C# binding for Dear ImGui                |
| `OpenTK`     | 4.7.7   | (Optional) math types if you swap to it  |

---

## Build

```bash
cd JointDebugger
dotnet restore
dotnet build -c Release
```

To install on a connected device with adb:

```bash
dotnet build -c Release
adb install -r bin/Release/net8.0-android/com.jointdebugger.app-Signed.apk
```

> If your project is set up under Visual Studio's Android pipeline, you can
> also right-click → **Publish Android App** and pick **Ad-Hoc** signing.

---

## Run

1. Launch **Joint Position Debugger** from the launcher.
2. On first run it will jump straight to the system overlay-permission page
   (`Settings → Apps → Special access → Display over other apps`).
3. Grant the permission and come back — the app starts the overlay service
   automatically. You'll see a small config window with the sliders/toggles
   and a debug table listing each joint's screen-space `(X, Y)`.
4. The joint markers are drawn via `ImGui.GetForegroundDrawList()` so they
   always render on top of any in-game content.
5. To turn everything off, use the **Reset Settings** button or stop the
   service from the system notification.

---

## Wiring real game data

The two provider classes are intentionally simple to swap:

```csharp
// Services/EntityProvider.cs
public Entity Acquire()
{
    // TODO: replace with real game memory read / hook.
    return BuildSampleEntity();
}
```

```csharp
// Services/CameraProvider.cs
public CameraMatrix Acquire()
{
    // TODO: replace with real game view-projection matrix read.
    return BuildSampleMatrix();
}
```

The `Entity` is a `Dictionary<JointType, Vector3>` and the `CameraMatrix` is
a column-major `float[16]`. The render loop calls both providers every frame.

---

## Spec compliance checklist

- [x] `SYSTEM_ALERT_WINDOW` permission flow with `ACTION_MANAGE_OVERLAY_PERMISSION`
- [x] Overlay window uses `TYPE_APPLICATION_OVERLAY` + `FLAG_NOT_TOUCHABLE`
- [x] ImGui.NET, OpenGL ES 3.0 via EGL
- [x] `ImGui.GetForegroundDrawList()` is the drawlist used for the markers
- [x] All 13 joints from the spec are present in `JointType` and `AllJoints`
- [x] `W2S.WorldToScreen(cameraMatrix, pos, w, h) -> Vector2?` with Y-flip
- [x] Validation `X > 0 && Y > 0`
- [x] Left/Right swap for Shoulder, Elbow, Wrist when `Left.X > Right.X`
- [x] Marker = filled circle, LimeGreen, radius `3.5f`
- [x] Outline = filled circle, radius `4.5f`, alpha `80/255`
- [x] Label = white text, alpha `200/255` (when `Config.ShowBoneLabels`)
- [x] `ColorToUint32` uses IM_COL32 bit packing
- [x] No `AddLine` / `AddRect` / Skeleton / Bone / BBox anywhere in the render path
- [x] Config exposes all 9 user-tunable settings + `Reset()`
- [x] Debug table window lists every joint's screen X / Y
