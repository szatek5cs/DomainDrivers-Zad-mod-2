namespace StageParallelization.Domain;

public class StageParallelization(ParallelizationPolicy policy)
{
    public ParallelStagesList DoParallelization(List<Stage> stages)
    {
        return policy.BuildSequenceFromRules(stages);
    }
}