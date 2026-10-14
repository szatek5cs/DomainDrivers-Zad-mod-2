namespace StageParallelization.Domain;

public interface IParallelizationRule
{
    ParallizationConstraint Apply(List<Stage> stages);
}