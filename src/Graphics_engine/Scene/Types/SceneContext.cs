using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Numerics;

namespace Graphics_engine.Scenes;

public readonly struct SceneContext
{
    public float DeltaTime { get; init; }
    public KeyboardState KeyboardState { get; init; }
    public KeyboardState PreviousKeyboardState { get; init; }
    public MouseState MouseState { get; init; }
    public MouseState PreviousMouseState { get; init; }
    public int ClientWidth { get; init; }
    public int ClientHeight { get; init; }
    public Matrix4x4 ViewMatrix { get; init; }
    public Matrix4x4 ProjectionMatrix { get; init; }
}
