namespace Graphics_engine.Scenes;

public class ModelingScene : IScene
{
    private int _next_id = 1;
    private readonly List<SceneObject> _objects = new();
    private readonly Dictionary<ObjectId, SceneObject> _object_dictonary = new();
    public IReadOnlyList<SceneObject> Objects => _objects;

    public ModelingScene()
    {
        foreach (var item in MeshLoader.LoadExample())
        {
            AddObject(item);
        }
    }

    public ObjectId AddObject(SceneObject obj)
    {
        var id = new ObjectId(_next_id++);

        obj.Id = id;

        if (string.IsNullOrWhiteSpace(obj.Name))
        {
            obj.Name = $"Object {id.Value}";
        }

        _objects.Add(obj);
        _object_dictonary.Add(obj.Id, obj);

        return obj.Id;
    }

    public bool TryGetObject(ObjectId id, out SceneObject obj)
    {
        return _object_dictonary.TryGetValue(id, out obj!);
    }

    public bool RemoveObject(ObjectId id)
    {
        if (_object_dictonary.TryGetValue(id, out var obj) || obj is null) return false;

        _objects.Remove(obj);
        _object_dictonary.Remove(id);
        return true;

    }


    public void Update(SceneContext context)
    {
    }
}
