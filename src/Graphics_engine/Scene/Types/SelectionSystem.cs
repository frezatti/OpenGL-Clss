using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Numerics;

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

        if (!TryCreateWorldRay(context, out var rayOrigin, out var rayDirection))
        {
            scene.ClearSelection();
            return;
        }

        SceneObject? clickedObject = null;
        float closestDistance = float.PositiveInfinity;

        foreach (var obj in scene.Objects)
        {
            if (!obj.Visible || !obj.Selectable)
            {
                continue;
            }

            if (!Matrix4x4.Invert(obj.Transform.ToModelMatrix(), out var inverseModel))
            {
                continue;
            }

            var localOrigin = Vector3.Transform(rayOrigin, inverseModel);
            var localDirection = Vector3.Normalize(Vector3.TransformNormal(rayDirection, inverseModel));

            if (RayIntersectsBounds(localOrigin, localDirection, obj.Mesh.Bounds, out float localDistance))
            {
                if (localDistance < closestDistance)
                {
                    closestDistance = localDistance;
                    clickedObject = obj;
                }
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

    private static bool TryCreateWorldRay(SceneContext context, out Vector3 origin, out Vector3 direction)
    {
        origin = Vector3.Zero;
        direction = Vector3.Zero;

        float mouseX = context.MouseState.X;
        float mouseY = context.MouseState.Y;

        float x = (2.0f * mouseX) / context.ClientWidth - 1.0f;
        float y = 1.0f - (2.0f * mouseY) / context.ClientHeight;

        var nearPoint = new Vector3(x, y, 0.0f);
        var farPoint = new Vector3(x, y, 1.0f);
        var viewport = new Vector4(0.0f, 0.0f, context.ClientWidth, context.ClientHeight);

        var worldNear = Unproject(nearPoint, viewport, context.ViewMatrix, context.ProjectionMatrix);
        var worldFar = Unproject(farPoint, viewport, context.ViewMatrix, context.ProjectionMatrix);

        var ray = worldFar - worldNear;

        if (ray.LengthSquared() <= float.Epsilon)
        {
            return false;
        }

        origin = worldNear;
        direction = Vector3.Normalize(ray);
        return true;
    }

    private static Vector3 Unproject(Vector3 normalizedDevicePoint, Vector4 viewport, Matrix4x4 view, Matrix4x4 projection)
    {
        var point = new Vector4(normalizedDevicePoint, 1.0f);
        var viewProjection = view * projection;

        if (!Matrix4x4.Invert(viewProjection, out var inverseViewProjection))
        {
            return Vector3.Zero;
        }

        var world = Vector4.Transform(point, inverseViewProjection);

        if (MathF.Abs(world.W) > float.Epsilon)
        {
            world /= world.W;
        }

        return new Vector3(world.X, world.Y, world.Z);
    }

    private static bool RayIntersectsBounds(Vector3 origin, Vector3 direction, Bounds3D bounds, out float distance)
    {
        distance = 0.0f;
        float tMin = 0.0f;
        float tMax = float.PositiveInfinity;

        if (!IntersectSlab(origin.X, direction.X, bounds.MinX, bounds.MaxX, ref tMin, ref tMax))
        {
            return false;
        }

        if (!IntersectSlab(origin.Y, direction.Y, bounds.MinY, bounds.MaxY, ref tMin, ref tMax))
        {
            return false;
        }

        if (!IntersectSlab(origin.Z, direction.Z, bounds.MinZ, bounds.MaxZ, ref tMin, ref tMax))
        {
            return false;
        }

        distance = tMin;
        return true;
    }

    private static bool IntersectSlab(float origin, float direction, float min, float max, ref float tMin, ref float tMax)
    {
        if (MathF.Abs(direction) < 0.000001f)
        {
            return origin >= min && origin <= max;
        }

        float inverseDirection = 1.0f / direction;
        float t1 = (min - origin) * inverseDirection;
        float t2 = (max - origin) * inverseDirection;

        if (t1 > t2)
        {
            (t1, t2) = (t2, t1);
        }

        tMin = MathF.Max(tMin, t1);
        tMax = MathF.Min(tMax, t2);

        return tMin <= tMax;
    }
}
