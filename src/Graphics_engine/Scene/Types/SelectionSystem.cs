using OpenTK.Windowing.GraphicsLibraryFramework;
namespace Graphics_engine.Scenes;

// Temporary selection bridge.
// This maps mouse position directly to normalized X/Y and uses localZ = 0.
// It does not account for camera projection, depth, or object rotation.
// Replace with ray picking after matrix convention and camera controls are stable.
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

            if (!obj.Visible || !obj.Selectable)
            {
                continue;
            }

            float localX = (worldX - obj.Transform.Position.X) / obj.Transform.Scale.X;
            float localY = (worldY - obj.Transform.Position.Y) / obj.Transform.Scale.Y;
            float localZ = 0.0f;

            if (obj.Mesh.Bounds.Contains(localX, localY, localZ))
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
