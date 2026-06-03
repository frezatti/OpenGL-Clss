using OpenTK.Mathematics;

namespace Graphics_engine.Scenes;

public class SpeedScene : IScene
{
    private const int NeedleIndex = 3;

    private const float NeedleMinRotation = -110.0f * MathF.PI / 180.0f;
    private const float NeedleMaxRotation = 110.0f * MathF.PI / 180.0f;

    private float _needleRotation = NeedleMaxRotation;

    private const float NeedleUpSpeed = 2.8f;
    private const float NeedleDownSpeed = 1.6f;

    private float _speed = 0.0f;
    private const float SpeedIncreaseRate = 0.8f;
    private const float SpeedDecreaseRate = 1.6f;
    private const float NaturalDecayRate = 0.35f;

    public RenderItem[] RenderItems { get; private set; }

    public SpeedScene()
    {
        RenderItems = MeshLoader.LoadExample();
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

        _needleRotation = MathHelper.Lerp(
            NeedleMaxRotation,
            NeedleMinRotation,
            _speed
        );


        var needle = RenderItems[NeedleIndex];
        needle.Transfom.Rotation = _needleRotation;
    }
}
