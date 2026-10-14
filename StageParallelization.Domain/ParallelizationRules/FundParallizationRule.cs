namespace StageParallelization.Domain.ParallelizationRules;

public class FundParallizationRule : IParallelizationRule
{
    public ParallizationConstraint Apply(List<Stage> stages)
    {
        // czy wszystkie etapy są finansowane lub wszystkie są niefinansowane
        bool couldBeParallel = stages.All(stage => stage.IsFunded) || stages.All(stage => !stage.IsFunded);

        if (couldBeParallel)
        {
            return ParallizationConstraint.Parallel();
        }
        else
        {
            var before = stages.Where(stage => stage.IsFunded).ToList();
            var after = stages.Where(stage => !stage.IsFunded).ToList();
            
            var sequence = new ParallelStagesList();
            sequence.AddParallelGroup(before);
            sequence.AddParallelGroup(after);

            return ParallizationConstraint.NotParallel(sequence);
        }
    }
}