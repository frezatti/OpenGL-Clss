using OpenTK.Windowing.GraphicsLibraryFramework;
namespace Graphics_engine.Scenes;

public readonly struct SceneContext
{
    public float DeltaTime { get; init; }
    public KeyboardState KeyboardState { get; init; }
    public MouseState MouseState { get; init; }
    public MouseState PreviousMouseState { get; init; }
    public int ClientWidth { get; init; }
    public int ClientHeight { get; init; }
}
