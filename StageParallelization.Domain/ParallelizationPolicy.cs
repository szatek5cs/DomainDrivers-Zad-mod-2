namespace StageParallelization.Domain;

public class ParallelizationPolicy(IEnumerable<IParallelizationRule> parallelizationRules)
{
    private readonly IReadOnlyList<IParallelizationRule> _parallelizationRules = parallelizationRules.ToList();

    public ParallelStagesList BuildSequenceFromRules(List<Stage> stages)
    {
        ParallelStagesList sequence = new ParallelStagesList();

        List<Stage> stagesToProcess = [.. stages];

        if (_parallelizationRules.Count == 0)
        {
            sequence.AddParallelGroup(stagesToProcess);
            return sequence;
        }

        while (stagesToProcess.Count > 0)
        {
            List<ParallelStagesList> rulesSequences = GetRulesSequences(stagesToProcess);

            // znajdź wspólną część każdej pierwszej grupy
            List<Stage> commonStages = GetCommonStagesFromFirstGroup(rulesSequences); 

            // część wspólna to nowa grupa równoległa
            if (commonStages.Count > 0)
            {
                sequence.AddParallelGroup(commonStages);
                foreach (var stage in commonStages)
                {
                    stagesToProcess.Remove(stage);
                }
            }
            else
            {
                throw new InvalidOperationException("Parallelization conflict: no common stages found.");
            }
        }

        return sequence;
    }

    private List<Stage> GetCommonStagesFromFirstGroup(List<ParallelStagesList> rulesSequences)
    {
        List<Stage> commonStages = new List<Stage>();
        for (int i = 0; i < rulesSequences.Count; i++)
        {
            if (rulesSequences[i].ParallelStagesGroups.Count > 0)
            {
                if (commonStages.Count == 0)
                {
                    commonStages.AddRange(rulesSequences[i].ParallelStagesGroups[0]);
                }
                else
                {
                    commonStages = commonStages.Intersect(rulesSequences[i].ParallelStagesGroups[0]).ToList();
                }
            }
        }
        return commonStages;
    }

    private List<ParallelStagesList> GetRulesSequences(List<Stage> stagesToProcess)
    {
        var rulesSequences = new List<ParallelStagesList>(_parallelizationRules.Count);
        
        for (int i = 0; i < _parallelizationRules.Count; i++)
        {
            rulesSequences.Add(new ParallelStagesList());
            var rule = _parallelizationRules[i];

            ParallizationConstraint constraint = rule.Apply(stagesToProcess);
            if (!constraint.CouldBeParallel)
            {
                rulesSequences[i] = constraint.Sequence!;
            }
            else
            {
                rulesSequences[i].AddParallelGroup(stagesToProcess);
            }
        }

        return rulesSequences;
    }
}