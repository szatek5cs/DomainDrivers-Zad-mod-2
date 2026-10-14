namespace StageParallelization.Domain.ParallelizationRules;

public class ResourceParallizationRule : IParallelizationRule
{
    public ParallizationConstraint Apply(List<Stage> stages)
    {
        // czy są etapy które korzystają z tych samych zasobów
        var resourceConflicts = stages.GetResourceConflicts();

        if (resourceConflicts.Any())
        {
            var sequence = BuildGroups(stages, resourceConflicts);

            return ParallizationConstraint.NotParallel(sequence);
        }
        else
        {
            return ParallizationConstraint.Parallel();
        }
    }

    public ParallelStagesList BuildGroups(
        IEnumerable<Stage> stages,
        IEnumerable<ResourceConflict> conflicts)
    {
        var allStages = stages
            .DistinctBy(s => s.StageId)
            .ToList();

        var conflictList = conflicts.ToList();
        var groups = new List<List<Stage>>();

        foreach (var stage in allStages)
        {
            var group = groups.FirstOrDefault(g =>
                !conflictList.Any(conflict =>
                    conflict.ConflictedStages.Any(s =>
                        s.StageId == stage.StageId)
                    && conflict.ConflictedStages.Any(s =>
                        g.Any(groupStage =>
                            groupStage.StageId == s.StageId))));

            if (group is null)
            {
                group = new List<Stage>();
                groups.Add(group);
            }

            group.Add(stage);
        }

        var result = new ParallelStagesList();

        foreach (var group in groups)
        {
            result.AddParallelGroup(group);
        }

        return result;
    }
}