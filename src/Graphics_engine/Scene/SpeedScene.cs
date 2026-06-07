using OpenTK.Mathematics;

namespace Graphics_engine.Scenes;

public class SpeedScene : IScene
{
    private const int PointerIndex = 3;

    private const float PointerMinRotation = -110.0f * MathF.PI / 180.0f;
    private const float PointerMaxRotation = 110.0f * MathF.PI / 180.0f;

    private float _pointerRotation = PointerMaxRotation;

    private float _speed = 0.0f;
    private const float SpeedIncreaseRate = 0.8f;
    private const float SpeedDecreaseRate = 1.6f;
    private const float NaturalDecayRate = 0.35f;

    private readonly List<SceneObject> _objects = new();
    public IReadOnlyList<SceneObject> Objects => _objects;

    public SpeedScene()
    {
        _objects = MeshLoader.LoadExample().ToList();
    }

    public void Update(SceneContext context)
    {
        var deltaTime = context.DeltaTime;
        bool leftMouseHeld = context.MouseState.IsButtonDown(
            OpenTK.Windowing.GraphicsLibraryFramework.MouseButton.Left
        );

        bool clickedRightHalf = context.MouseState.X >= context.ClientWidth / 2.0f;
        bool clickedLeftHalf = context.MouseState.X < context.ClientWidth / 2.0f;

        if (leftMouseHeld && clickedRightHalf)
        {
            _speed += SpeedIncreaseRate * deltaTime;
        }
        else if (leftMouseHeld && clickedLeftHalf)
        {
            _speed -= SpeedDecreaseRate * deltaTime;
        }
        else
        {
            _speed -= NaturalDecayRate * deltaTime;
        }

        _speed = Math.Clamp(_speed, 0.0f, 1.0f);

        _pointerRotation = MathHelper.Lerp(
            PointerMaxRotation,
            PointerMinRotation,
            _speed
        );

        var pointer = _objects[PointerIndex];
        pointer.Transform.Rotation = new System.Numerics.Vector3(0.0f, 0.0f, _pointerRotation);
    }
}
