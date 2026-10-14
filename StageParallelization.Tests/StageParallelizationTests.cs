namespace StageParallelization.Tests;

using Microsoft.Extensions.DependencyInjection;
using StageParallelization.Domain;
using StageParallelization.Domain.ParallelizationRules;

public class StageParallelizationTests
{
    private readonly StageParallelization stageParallelization;

    public StageParallelizationTests()
    {
        var services = new ServiceCollection();

        services.AddTransient<IParallelizationRule, FundParallizationRule>();
        services.AddTransient<IParallelizationRule, LogicParallizationRule>();
        services.AddTransient<IParallelizationRule, ResourceParallizationRule>();

        services.AddTransient<ParallelizationPolicy>();
        services.AddTransient<StageParallelization>();

        var serviceProvider = services.BuildServiceProvider();

        stageParallelization =
            serviceProvider.GetRequiredService<StageParallelization>();
    }
    
    [Fact]
    public void Should_Parallelize_Stages_Correctly_Based_On_Logical_Dependencies()
    {
        Stage stage1 = new Stage("Stage1");
        Stage stage2 = new Stage("Stage2");
        Stage stage3 = new Stage("Stage3");
        Stage stage4 = new Stage("Stage4");

        stage2.DependsOn(stage1);
        stage3.DependsOn(stage1);
        stage4.DependsOn(stage2);

        ParallelStagesList parallelStagesList = 
            stageParallelization.DoParallelization(new List<Stage> { stage1, stage2, stage3, stage4 });

        Assert.Equal("Stage1 | Stage2, Stage3 | Stage4", parallelStagesList.Print());
    }

    [Fact]
    public void Should_Funding_Stages_Be_Before_NonFunding_Stages()
    {
        Stage stage1 = new Stage("Stage1", isFunded: false);
        Stage stage2 = new Stage("Stage2", isFunded: false);
        Stage stage3 = new Stage("Stage3", isFunded: true);
        Stage stage4 = new Stage("Stage4", isFunded: false);

        ParallelStagesList parallelStagesList = 
            stageParallelization.DoParallelization(new List<Stage> { stage1, stage2, stage3, stage4 });

        Assert.Equal("Stage3 | Stage1, Stage2, Stage4", parallelStagesList.Print());
    }

    [Fact]
    public void Should_Resource_Based_Stages_Be_Parallelized_Correctly()
    {
        StageResource resourceA = new StageResource("ResourceA");
        StageResource resourceB = new StageResource("ResourceB");

        Stage stage1 = new Stage("Stage1", initialResources: new List<StageResource> { resourceA });
        Stage stage2 = new Stage("Stage2", initialResources: new List<StageResource> { resourceA });
        Stage stage3 = new Stage("Stage3", initialResources: new List<StageResource> { resourceB });
        Stage stage4 = new Stage("Stage4", initialResources: new List<StageResource> { resourceB });

        ParallelStagesList parallelStagesList = 
            stageParallelization.DoParallelization(new List<Stage> { stage1, stage2, stage3, stage4 });

        Assert.Equal("Stage1, Stage3 | Stage2, Stage4", parallelStagesList.Print());
    }

    [Fact]
    public void Should_Apply_All_Parallelization_Rules_Together()
    {
        StageResource resourceA = new StageResource("ResourceA");
        StageResource resourceB = new StageResource("ResourceB");

        Stage stage1 = new Stage(
            "Stage1",
            initialResources: new List<StageResource> { resourceA },
            isFunded: true);

        Stage stage2 = new Stage(
            "Stage2",
            initialResources: new List<StageResource> { resourceA },
            isFunded: true);

        Stage stage3 = new Stage(
            "Stage3",
            initialResources: new List<StageResource> { resourceB },
            isFunded: true);

        Stage stage4 = new Stage("Stage4", isFunded: false);
        Stage stage5 = new Stage("Stage5", isFunded: false);

        stage3.DependsOn(stage1);
        stage5.DependsOn(stage2);

        ParallelStagesList result = stageParallelization.DoParallelization(
            new List<Stage> { stage1, stage2, stage3, stage4, stage5 });

        Assert.Equal(
            "Stage1 | Stage2, Stage3 | Stage4, Stage5",
            result.Print());
    }

    [Fact]
    public void Should_Not_Lose_Stages_When_Applying_Multiple_Rules()
    {
        StageResource resourceX = new StageResource("ResourceX");

        Stage stageA = new Stage(
            "A",
            initialResources: new List<StageResource> { resourceX },
            isFunded: true);

        Stage stageB = new Stage(
            "B",
            initialResources: new List<StageResource> { resourceX },
            isFunded: true);

        Stage stageC = new Stage(
            "C",
            isFunded: false);

        ParallelStagesList result = stageParallelization.DoParallelization(
            new List<Stage> { stageA, stageB, stageC });

        Assert.Equal("A | B | C", result.Print());
    }

    [Fact]
    public void Should_Apply_Funding_And_Resource_Rules_Together()
    {
        StageResource resourceX = new StageResource("ResourceX");

        Stage stageA = new Stage(
            "A",
            initialResources: new List<StageResource> { resourceX },
            isFunded: true);

        Stage stageB = new Stage(
            "B",
            initialResources: new List<StageResource> { resourceX },
            isFunded: false);

        Stage stageC = new Stage(
            "C",
            isFunded: false);

        ParallelStagesList result = stageParallelization.DoParallelization(
            new List<Stage> { stageA, stageB, stageC });

        Assert.Equal("A | B, C", result.Print());
    }

    [Fact]
    public void Should_Respect_Dependencies_And_Resource_Conflicts_Together()
    {
        StageResource resourceX = new StageResource("ResourceX");

        Stage stageA = new Stage("A");

        Stage stageB = new Stage(
            "B",
            initialResources: new List<StageResource> { resourceX });

        Stage stageC = new Stage(
            "C",
            initialResources: new List<StageResource> { resourceX });

        stageB.DependsOn(stageA);
        stageC.DependsOn(stageA);

        ParallelStagesList result = stageParallelization.DoParallelization(
            new List<Stage> { stageA, stageB, stageC });

        Assert.Equal("A | B | C", result.Print());
    }

    [Fact]
    public void Should_Apply_All_Parallelization_Rules_Together_For_Three_Stages()
    {
        StageResource resourceA = new StageResource("ResourceA");
        StageResource resourceB = new StageResource("ResourceB");

        Stage stage1 = new Stage(
            "Stage1",
            initialResources: new List<StageResource> { resourceA },
            isFunded: true);

        Stage stage2 = new Stage(
            "Stage2",
            initialResources: new List<StageResource> { resourceA },
            isFunded: true);

        Stage stage3 = new Stage(
            "Stage3",
            initialResources: new List<StageResource> { resourceB },
            isFunded: false);

        stage3.DependsOn(stage1);

        ParallelStagesList result = stageParallelization.DoParallelization(
            new List<Stage> { stage1, stage2, stage3 });

        Assert.Equal("Stage1 | Stage2 | Stage3", result.Print());
    }

    [Fact]
    public void Should_Apply_All_Parallelization_Rules_Together_For_Four_Stages()
    {
        StageResource resourceA = new StageResource("ResourceA");
        StageResource resourceB = new StageResource("ResourceB");

        Stage stage1 = new Stage("Stage1",
            initialResources: new List<StageResource> { resourceA },
            isFunded: true);

        Stage stage2 = new Stage("Stage2",
            initialResources: new List<StageResource> { resourceA },
            isFunded: true);

        Stage stage3 = new Stage("Stage3",
            initialResources: new List<StageResource> { resourceB },
            isFunded: true);

        Stage stage4 = new Stage("Stage4", isFunded: false);

        stage3.DependsOn(stage1);

        ParallelStagesList result = stageParallelization.DoParallelization(
            new List<Stage> { stage1, stage2, stage3, stage4 });

        Assert.Equal("Stage1 | Stage2, Stage3 | Stage4", result.Print());
    }
}
