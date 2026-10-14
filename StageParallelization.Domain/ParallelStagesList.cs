namespace StageParallelization.Domain;

public class ParallelStagesList
{
    private List<List<Stage>> parallelStagesGroups = new();

    public void AddParallelGroup(List<Stage> stages)
    {
        parallelStagesGroups.Add(stages);
    }

    public override string ToString()
    {
        return Print();
    }

    public string Print()
    {
        return string.Join(" | ", parallelStagesGroups.Select(
            group => string.Join(", ", group.Select(stage => stage.StageName))));
    }

    internal bool Contains(IReadOnlyList<Stage> stages)
    {
        foreach (var stage in stages)
        {
            bool found = false;
            foreach (var group in parallelStagesGroups)
            {
                if (group.Contains(stage))
                {
                    found = true;
                    break;
                }
            }
            if (!found)
                return false;
        }
        return true;
    }

    internal static ParallelStagesList FromList(List<List<Stage>> updatedParallelStagesGroups)
    {
        var parallelStagesList = new ParallelStagesList();
        foreach (var group in updatedParallelStagesGroups)
        {
            parallelStagesList.AddParallelGroup(group);
        }
        return parallelStagesList;
    }

    public IReadOnlyList<List<Stage>> ParallelStagesGroups => parallelStagesGroups;
}