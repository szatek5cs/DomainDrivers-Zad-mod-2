namespace StageParallelization.Domain;

public class StageResource
{
    public StageResource(string resourceName)
    {
        ResourceId = Guid.NewGuid();
        ResourceName = resourceName;
    }

    public Guid ResourceId { get; private set; }

    public string ResourceName { get; private set; }
}