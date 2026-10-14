namespace StageParallelization.Domain;

public class ParallizationConstraint
{
    private ParallizationConstraint() { }

    public ParallelStagesList? Sequence { get; init; } = null;

    public bool CouldBeParallel { get; init; }

    public static ParallizationConstraint Parallel()
    {
        return new ParallizationConstraint { CouldBeParallel = true };
    }

    public static ParallizationConstraint NotParallel(ParallelStagesList sequence)
    {
        return new ParallizationConstraint { CouldBeParallel = false, Sequence = sequence };
    }
}