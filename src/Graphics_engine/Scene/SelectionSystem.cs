using OpenTK.Windowing.GraphicsLibraryFramework;
namespace Graphics_engine.Scenes;

public static class SelectionSystem
{
    public static void Update(ModelingScene scene, SceneContext context)
    {
        bool leftPressedThisFrame =
            context.MouseState.IsButtonDown(MouseButton.Left) &&
            !context.PreviousMouseState.IsButtonDown(MouseButton.Left);

        if (!leftPressedThisFrame)
        {
            return;
        }

        float worldX = (context.MouseState.X * 2f) / context.ClientWidth - 1f;
        float worldY = 1f - (context.MouseState.Y * 2f) / context.ClientHeight;

        SceneObject? clickedObject = null;

        for (int i = scene.Objects.Count - 1; i >= 0; i--)
        {
            var obj = scene.Objects[i];

            if (!obj.Visible)
            {
                continue;
            }

            float localX = (worldX - obj.Transform.Position.X) / obj.Transform.Scale.X;
            float localY = (worldY - obj.Transform.Position.Y) / obj.Transform.Scale.Y;

            if (obj.Mesh.Bounds.Contains(localX, localY))
            {
                clickedObject = obj;
                break;
            }
        }

        if (clickedObject is null)
        {
            scene.ClearSelection();
        }
        else
        {
            scene.SelectObject(clickedObject.Id);
        }
    }
}
