// See https://aka.ms/new-console-template for more information
using StageParallelization.Domain;
using Microsoft.Extensions.DependencyInjection;
using StageParallelization.Domain.ParallelizationRules;

Console.WriteLine("Hello, World!");

Stage stage1 = new Stage("Stage1");
Stage stage2 = new Stage("Stage2");
Stage stage3 = new Stage("Stage3");
Stage stage4 = new Stage("Stage4");

stage2.DependsOn(stage1);
stage3.DependsOn(stage1);
stage4.DependsOn(stage2);

var services = new ServiceCollection();

services.AddTransient<IParallelizationRule, FundParallizationRule>();
services.AddTransient<IParallelizationRule, LogicParallizationRule>();
services.AddTransient<IParallelizationRule, ResourceParallizationRule>();

services.AddTransient<ParallelizationPolicy>();
services.AddTransient<StageParallelization.Domain.StageParallelization>();

var serviceProvider = services.BuildServiceProvider();

var stageParallelization =
    serviceProvider.GetRequiredService<StageParallelization.Domain.StageParallelization>();

ParallelStagesList parallelStagesList = 
    stageParallelization.DoParallelization(new List<Stage> { stage1, stage2, stage3, stage4 });

Console.WriteLine("There is from program: " + parallelStagesList.Print());
Console.WriteLine("Should be:             Stage1 | Stage2, Stage3 | Stage4");
Console.ReadLine();
