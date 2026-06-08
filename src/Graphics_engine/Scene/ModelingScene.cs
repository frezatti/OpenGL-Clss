using OpenTK.Graphics.OpenGL4;
using OpenTK.Windowing.GraphicsLibraryFramework;
using System.Numerics;

namespace Graphics_engine.Scenes;

public class ModelingScene : IScene
{
    private ObjectId? _selectedObjectId;
    private int _nextId = 1;
    private int _runtimeObjectNumber = 1;
    private readonly List<SceneObject> _objects = new();
    private readonly Dictionary<ObjectId, SceneObject> _objectsById = new();
    public IReadOnlyList<SceneObject> Objects => _objects;

    public ModelingScene()
    {
        AddObject(SceneObjectFactory.CreateCube(
            "Center Cube",
            Vector3.Zero,
            Vector3.One,
            new Vector3(0.35f, 0.55f, 0.0f),
            new Vector4(0.1f, 0.7f, 1.0f, 1.0f)
        ));

        AddObject(SceneObjectFactory.CreateGrid(
            "Grid",
            new Vector3(0.0f, -0.75f, 0.0f),
            new Vector3(2.0f, 2.0f, 1.0f),
            new Vector3(MathF.PI / 2.0f, 0.0f, 0.0f),
            new Vector4(0.35f, 0.35f, 0.35f, 1.0f)
        ));
    }

    public ObjectId AddObject(SceneObject obj)
    {
        var id = new ObjectId(_nextId++);

        obj.Id = id;

        if (string.IsNullOrWhiteSpace(obj.Name))
        {
            obj.Name = $"Object {id.Value}";
        }

        obj.DirtyFlags |= ObjectDirtyFlags.Created;

        _objects.Add(obj);
        _objectsById.Add(obj.Id, obj);

        return obj.Id;
    }

    public void ClearSelection()
    {
        if (_selectedObjectId is not null && _objectsById.TryGetValue(_selectedObjectId.Value, out var selected))
        {
            selected.Selected = false;
            selected.DirtyFlags |= ObjectDirtyFlags.Selection;
        }

        _selectedObjectId = null;
    }

    public void SelectObject(ObjectId id)
    {
        ClearSelection();

        if (_objectsById.TryGetValue(id, out var obj))
        {
            obj.Selected = true;
            obj.DirtyFlags |= ObjectDirtyFlags.Selection;
            _selectedObjectId = obj.Id;
        }
    }

    public SceneObject? GetSelectedObject()
    {
        if (_selectedObjectId is null)
        {
            return null;
        }

        return _objectsById.TryGetValue(_selectedObjectId.Value, out var selected)
            ? selected
            : null;
    }

    public bool RemoveSelectedObject()
    {
        var selected = GetSelectedObject();

        if (selected is null)
        {
            return false;
        }

        return RemoveObject(selected.Id);
    }

    public bool TryGetObject(ObjectId id, out SceneObject obj)
    {
        return _objectsById.TryGetValue(id, out obj!);
    }

    public bool RemoveObject(ObjectId id)
    {
        if (!_objectsById.TryGetValue(id, out var obj) || obj is null)
        {
            return false;
        }

        obj.DirtyFlags |= ObjectDirtyFlags.Deleted;

        if (_selectedObjectId == id)
        {
            _selectedObjectId = null;
        }

        _objects.Remove(obj);
        _objectsById.Remove(id);
        return true;
    }

    public void Update(SceneContext context)
    {
        SelectionSystem.Update(this, context);

        if (WasKeyPressed(context, Keys.Backspace))
        {
            ClearSelection();
            return;
        }

        if (WasKeyPressed(context, Keys.Delete))
        {
            RemoveSelectedObject();
            return;
        }

        HandlePrimitiveCreation(context);
        UpdateSelectedObjectTransform(context);
    }

    private void HandlePrimitiveCreation(SceneContext context)
    {
        if (WasKeyPressed(context, Keys.D1) || WasKeyPressed(context, Keys.KeyPad1))
        {
            AddPrimitive(ScenePrimitiveKind.Cube);
        }

        if (WasKeyPressed(context, Keys.D2) || WasKeyPressed(context, Keys.KeyPad2))
        {
            AddPrimitive(ScenePrimitiveKind.Pyramid);
        }

        if (WasKeyPressed(context, Keys.D3) || WasKeyPressed(context, Keys.KeyPad3))
        {
            AddPrimitive(ScenePrimitiveKind.Cylinder);
        }

        if (WasKeyPressed(context, Keys.D4) || WasKeyPressed(context, Keys.KeyPad4))
        {
            AddPrimitive(ScenePrimitiveKind.Sphere);
        }

        if (WasKeyPressed(context, Keys.D5) || WasKeyPressed(context, Keys.KeyPad5))
        {
            AddPrimitive(ScenePrimitiveKind.Box);
        }
    }

    private void AddPrimitive(ScenePrimitiveKind kind)
    {
        int number = _runtimeObjectNumber++;
        float x = ((number - 1) % 5 - 2) * 1.25f;
        float z = -((number - 1) / 5) * 1.25f;
        var position = new Vector3(x, 0.0f, z);
        SceneObject obj = kind switch
        {
            ScenePrimitiveKind.Cube => SceneObjectFactory.CreateCube(
                $"Cube {number}",
                position,
                Vector3.One,
                Vector3.Zero,
                new Vector4(0.1f, 0.7f, 1.0f, 1.0f)
            ),

            ScenePrimitiveKind.Pyramid => SceneObjectFactory.CreatePyramid(
                $"Pyramid {number}",
                position,
                Vector3.One,
                Vector3.Zero,
                new Vector4(1.0f, 0.55f, 0.15f, 1.0f)
            ),

            ScenePrimitiveKind.Cylinder => SceneObjectFactory.CreateCylinder(
                $"Cylinder {number}",
                position,
                Vector3.One,
                Vector3.Zero,
                new Vector4(0.35f, 0.95f, 0.45f, 1.0f)
            ),

            ScenePrimitiveKind.Sphere => SceneObjectFactory.CreateSphere(
                $"Sphere {number}",
                position,
                Vector3.One,
                Vector3.Zero,
                new Vector4(0.85f, 0.35f, 1.0f, 1.0f)
            ),

            ScenePrimitiveKind.Box => SceneObjectFactory.CreateCube(
                $"Stretched Box {number}",
                position,
                new Vector3(1.6f, 0.65f, 0.8f),
                Vector3.Zero,
                new Vector4(0.95f, 0.85f, 0.25f, 1.0f)
            ),

            _ => throw new ArgumentOutOfRangeException(nameof(kind), kind, null)
        };

        var id = AddObject(obj);
        SelectObject(id);
    }

    private void UpdateSelectedObjectTransform(SceneContext context)
    {
        var obj = GetSelectedObject();

        if (obj is null)
        {
            return;
        }

        bool changed = false;
        float moveSpeed = 1.0f * context.DeltaTime;
        float rotationSpeed = 1.5f * context.DeltaTime;
        float scaleSpeed = 1.0f * context.DeltaTime;

        var position = obj.Transform.Position;
        var rotation = obj.Transform.Rotation;
        var scale = obj.Transform.Scale;

        if (context.KeyboardState.IsKeyDown(Keys.Left))
        {
            position.X -= moveSpeed;
            changed = true;
        }

        if (context.KeyboardState.IsKeyDown(Keys.Right))
        {
            position.X += moveSpeed;
            changed = true;
        }

        if (context.KeyboardState.IsKeyDown(Keys.Up))
        {
            position.Y += moveSpeed;
            changed = true;
        }

        if (context.KeyboardState.IsKeyDown(Keys.Down))
        {
            position.Y -= moveSpeed;
            changed = true;
        }

        if (context.KeyboardState.IsKeyDown(Keys.W) || context.KeyboardState.IsKeyDown(Keys.PageDown))
        {
            position.Z -= moveSpeed;
            changed = true;
        }

        if (context.KeyboardState.IsKeyDown(Keys.S) || context.KeyboardState.IsKeyDown(Keys.PageUp))
        {
            position.Z += moveSpeed;
            changed = true;
        }

        if (context.KeyboardState.IsKeyDown(Keys.Q))
        {
            rotation.Z += rotationSpeed;
            changed = true;
        }

        if (context.KeyboardState.IsKeyDown(Keys.E))
        {
            rotation.Z -= rotationSpeed;
            changed = true;
        }

        if (context.KeyboardState.IsKeyDown(Keys.R))
        {
            rotation.X += rotationSpeed;
            changed = true;
        }

        if (context.KeyboardState.IsKeyDown(Keys.F))
        {
            rotation.X -= rotationSpeed;
            changed = true;
        }

        if (context.KeyboardState.IsKeyDown(Keys.T))
        {
            rotation.Y += rotationSpeed;
            changed = true;
        }

        if (context.KeyboardState.IsKeyDown(Keys.G))
        {
            rotation.Y -= rotationSpeed;
            changed = true;
        }

        if (context.KeyboardState.IsKeyDown(Keys.Equal))
        {
            scale += Vector3.One * scaleSpeed;
            changed = true;
        }

        if (context.KeyboardState.IsKeyDown(Keys.Minus))
        {
            scale -= Vector3.One * scaleSpeed;
            scale.X = MathF.Max(scale.X, 0.05f);
            scale.Y = MathF.Max(scale.Y, 0.05f);
            scale.Z = MathF.Max(scale.Z, 0.05f);
            changed = true;
        }

        if (!changed)
        {
            return;
        }

        obj.Transform.Position = position;
        obj.Transform.Rotation = rotation;
        obj.Transform.Scale = scale;
        obj.DirtyFlags |= ObjectDirtyFlags.Transform;
    }

    private static bool WasKeyPressed(SceneContext context, Keys key)
    {
        return context.KeyboardState.IsKeyDown(key) &&
               !context.PreviousKeyboardState.IsKeyDown(key);
    }

    private enum ScenePrimitiveKind
    {
        Cube,
        Pyramid,
        Cylinder,
        Sphere,
        Box
    }
}
