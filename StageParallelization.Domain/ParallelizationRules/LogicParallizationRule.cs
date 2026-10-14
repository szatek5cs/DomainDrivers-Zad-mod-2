namespace StageParallelization.Domain.ParallelizationRules;

public class LogicParallizationRule : IParallelizationRule
{
    public ParallizationConstraint Apply(List<Stage> stages)
    {
        var parallelStagesList = new ParallelStagesList();

        List<Stage> independentStages = new List<Stage>();
        List<Stage> dependentStages = new List<Stage>();
        foreach (var stage in stages)
        {
            // dodaj do dependentStages tylko jesli ma zależności, które są na liście stages
            if (stage.Dependencies.Count == 0 || stage.Dependencies.All(d => !stages.Contains(d)))
            {
                independentStages.Add(stage);
                continue;
            }
            else
            {
                dependentStages.Add(stage);
            }
        }

        if (dependentStages.Count == 0)
        {
            return ParallizationConstraint.Parallel();
        }

        if (independentStages.Count > 0)
        {
            parallelStagesList.AddParallelGroup(independentStages);
        }

        while (dependentStages.Count > 0)
        {
            List<Stage> newParallelGroup = new List<Stage>();
            foreach (var dependentStage in dependentStages)
            {
                // wybierz wszystkie zależności które są w stages
                var relevantDependencies = dependentStage.Dependencies.Where(d => stages.Contains(d)).ToList();
                
                // jeśli wszystkie dependency są w jakieś grupie parallelStagesList
                // to dodaj nową grupę równoległą
                if (parallelStagesList.Contains(relevantDependencies))
                {
                    newParallelGroup.Add(dependentStage);
                }
            }
            if (newParallelGroup.Count > 0)
            {
                parallelStagesList.AddParallelGroup(newParallelGroup);
                foreach (var stage in newParallelGroup)
                {
                    dependentStages.Remove(stage);
                }
            }
            else
            {
                throw new InvalidOperationException("Cannot resolve dependencies for remaining stages.");
            }
        }

        return ParallizationConstraint.NotParallel(parallelStagesList);
    }
}