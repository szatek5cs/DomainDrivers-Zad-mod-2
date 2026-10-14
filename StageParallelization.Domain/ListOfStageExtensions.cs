namespace StageParallelization.Domain;

public static class ListOfStageExtensions
{
    public static List<ResourceConflict> GetResourceConflicts(this IEnumerable<Stage> stages)
    {
        var conflicts = new List<ResourceConflict>();
        var groupedResources = stages.SelectMany(stage => stage.Resources)
            .GroupBy(resource => resource.ResourceId);

        foreach (var group in groupedResources)
        {
            if (group.Count() > 1)
            {
                var conflictedStages = group.SelectMany(resource => 
                    stages.Where(stage => stage.Resources.Any(r => r.ResourceId == resource.ResourceId))).Distinct().ToList();
                conflicts.Add(new ResourceConflict(group.Key, conflictedStages));
            }
        }

        return conflicts;
    }
}