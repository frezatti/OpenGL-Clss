using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Numerics;

namespace Graphics_engine.Scenes;

public static class SelectionSystem
{
    public static void Update(ModelingScene scene, SceneContext context)
    {
        if (!context.MouseState.IsButtonDown(MouseButton.Left))
        {
            return;
        }

        if (!TryCreateWorldRay(context, out var rayOrigin, out var rayDirection))
        {
            scene.ClearSelection();
            return;
        }

        SceneObject? clickedObject = null;
        float closestDistanceSquared = float.PositiveInfinity;

        foreach (var obj in scene.Objects)
        {
            if (!obj.Visible || !obj.Selectable)
            {
                continue;
            }

            var model = obj.Transform.ToModelMatrix();

            if (!Matrix4x4.Invert(model, out var inverseModel))
            {
                continue;
            }

            var localOrigin = Vector3.Transform(rayOrigin, inverseModel);
            var localDirection = Vector3.Normalize(Vector3.TransformNormal(rayDirection, inverseModel));

            if (!RayIntersectsBounds(localOrigin, localDirection, obj.Mesh.Bounds, out float localDistance))
            {
                continue;
            }

            var localHitPoint = localOrigin + localDirection * localDistance;
            var worldHitPoint = Vector3.Transform(localHitPoint, model);
            float worldDistanceSquared = Vector3.DistanceSquared(rayOrigin, worldHitPoint);

            if (worldDistanceSquared < closestDistanceSquared)
            {
                closestDistanceSquared = worldDistanceSquared;
                clickedObject = obj;
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

        if (context.ClientWidth <= 0 || context.ClientHeight <= 0)
        {
            return false;
        }

        float mouseX = context.MouseState.X;
        float mouseY = context.MouseState.Y;

        float ndcX = (2.0f * mouseX) / context.ClientWidth - 1.0f;
        float ndcY = 1.0f - (2.0f * mouseY) / context.ClientHeight;

        if (!Matrix4x4.Invert(context.ViewMatrix, out var inverseView))
        {
            return false;
        }

        origin = new Vector3(inverseView.M41, inverseView.M42, inverseView.M43);

        // Matrix4x4.CreatePerspectiveFieldOfView uses a 0..1 depth range:
        // z = 0 is the near plane and z = 1 is the far plane.
        if (!TryUnproject(new Vector3(ndcX, ndcY, 0.0f), context.ViewMatrix, context.ProjectionMatrix, out var worldNear))
        {
            return false;
        }

        if (!TryUnproject(new Vector3(ndcX, ndcY, 1.0f), context.ViewMatrix, context.ProjectionMatrix, out var worldFar))
        {
            return false;
        }

        var ray = worldNear - origin;

        if (ray.LengthSquared() <= float.Epsilon)
        {
            ray = worldFar - origin;
        }

        if (ray.LengthSquared() <= float.Epsilon)
        {
            return false;
        }

        direction = Vector3.Normalize(ray);
        return true;
    }

    private static bool TryUnproject(
        Vector3 normalizedDevicePoint,
        Matrix4x4 view,
        Matrix4x4 projection,
        out Vector3 worldPoint)
    {
        worldPoint = Vector3.Zero;

        var viewProjection = view * projection;

        if (!Matrix4x4.Invert(viewProjection, out var inverseViewProjection))
        {
            return false;
        }

        var point = new Vector4(normalizedDevicePoint, 1.0f);
        var world = Vector4.Transform(point, inverseViewProjection);

        if (MathF.Abs(world.W) <= float.Epsilon)
        {
            return false;
        }

        world /= world.W;
        worldPoint = new Vector3(world.X, world.Y, world.Z);
        return true;
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
