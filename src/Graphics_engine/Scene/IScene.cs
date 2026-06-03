namespace Graphics_engine.Scenes;

public interface IScene
{
    IReadOnlyList<SceneObject> Objects { get; }

    void Update(SceneContext context);
}

