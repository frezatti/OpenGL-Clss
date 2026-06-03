namespace Graphics_engine.Scenes;

public interface IScene
{
    RenderItem[] RenderItems { get; }

    void Update(SceneContext context);
}

