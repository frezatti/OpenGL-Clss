using OpenTK.Windowing.Desktop;
using Graphics_engine;

var gameSettings = GameWindowSettings.Default;


var nativeSetings = new NativeWindowSettings()
{
    ClientSize = new OpenTK.Mathematics.Vector2i(800, 600),
    Title = "My OpenGL Engine"
};

using (var window = new Window(gameSettings, nativeSetings))
{
    window.Run();
}
