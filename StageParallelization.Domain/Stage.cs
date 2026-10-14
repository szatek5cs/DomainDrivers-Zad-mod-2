namespace StageParallelization.Domain;

public class Stage
{
    private List<Stage> dependencies = new(); 

    private List<StageResource> resources = new();

    public Stage(string stageName, bool isFunded = false)
    {
        StageId = Guid.NewGuid();
        StageName = stageName;
        IsFunded = isFunded;
    }

    public Stage(string stageName, List<StageResource> initialResources, bool isFunded = false)
    {
        StageId = Guid.NewGuid();
        StageName = stageName;
        resources.AddRange(initialResources);
        IsFunded = isFunded;
    }

    public bool IsFunded { get; private set; }
    
    public Guid StageId { get; private set; }
    
    public string StageName { get; private set; } 

    public IReadOnlyList<Stage> Dependencies => dependencies;

    public IReadOnlyList<StageResource> Resources => resources;

    public void DependsOn(Stage stage)
    {
        dependencies.Add(stage);
    }

    public void AddResource(StageResource resource)
    {
        resources.Add(resource);
    }
}