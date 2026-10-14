namespace StageParallelization.Domain;

public record ResourceConflict(Guid ResourceId, List<Stage> ConflictedStages);