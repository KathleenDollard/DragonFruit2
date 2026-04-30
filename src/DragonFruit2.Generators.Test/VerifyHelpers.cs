using DragonFruit2.Generators.Metadata;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using System.Collections;
using System.Collections.Immutable;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace DragonFruit2.Generators.Test;

public static class VerifyHelpers
{


    [ModuleInitializer]
    public static void Init()
    {
        VerifySourceGenerators.Initialize();
    }

    public static GeneratorDriver GetGeneratorDriver<T>(Compilation compilation)
        where T : IIncrementalGenerator, new()
    {

        var generator = new T().AsSourceGenerator();

        var settings = new GeneratorDriverOptions(
            disabledOutputs: IncrementalGeneratorOutputKind.None,
            trackIncrementalGeneratorSteps: true);

        GeneratorDriver driver = CSharpGeneratorDriver.Create([generator], driverOptions: settings);

        driver = driver.RunGenerators(compilation);

        return driver;
    }


    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// I started with Andrew Lock's blog post on testing incremental generators: 
    /// /https://andrewlock.net/creating-a-source-generator-part-10-testing-your-incremental-generator-pipeline-outputs-are-cacheable/
    /// but with multiple steps and multi-threaded execution debugging was a pain, so I changed to the approach of 
    /// creating a report and verifying the report (to get all issues) rather than the assert as you go Andrew used.
    /// </remarks>
    /// <param name="runResult1"></param>
    /// <param name="runResult2"></param>
    /// <param name="trackingNames"></param>
    internal static void AssertRunResultsEqual(GeneratorDriverRunResult runResult1,
                                               GeneratorDriverRunResult runResult2,
                                               string[] trackingNames)
    {
        // We're given all the tracking names, but not all the
        // stages will necessarily execute, so extract all the 
        // output steps, and filter to ones we know about
        var trackedSteps1 = GetTrackedSteps(runResult1, trackingNames);
        var trackedSteps2 = GetTrackedSteps(runResult2, trackingNames);

        Assert.NotEmpty(trackedSteps1);
        Assert.Equal(trackedSteps1.Count, trackedSteps2.Count);
        Assert.Equal(trackedSteps1.Keys, trackedSteps2.Keys);

        // Get the IncrementalGeneratorRunStep collection for each run
        foreach (var (trackingName, runSteps1) in trackedSteps1)
        {
            // Assert that both runs produced the same outputs
            var runSteps2 = trackedSteps2[trackingName];
            AssertStepsEqual(runSteps1, runSteps2, trackingName);
        }

        return;

        // Local function that extracts the tracked steps
        static Dictionary<string, ImmutableArray<IncrementalGeneratorRunStep>> GetTrackedSteps(
            GeneratorDriverRunResult runResult, string[] trackingNames)
            => runResult
                    .Results[0] // We're only running a single generator, so this is safe
                    .TrackedSteps // Get the pipeline outputs
                    .Where(step => trackingNames.Contains(step.Key)) // filter to known steps
                    .ToDictionary(x => x.Key, x => x.Value); // Convert to a dictionary
    }


    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Based on Andrew Lock's blog post on testing incremental generators: https://andrewlock.net/creating-a-source-generator-part-10-testing-your-incremental-generator-pipeline-outputs-are-cacheable/
    /// </remarks>
    /// <param name="runSteps1"></param>
    /// <param name="runSteps2"></param>
    /// <param name="stepName"></param>
    internal static void AssertStepsEqual(ImmutableArray<IncrementalGeneratorRunStep> runSteps1,
                                          ImmutableArray<IncrementalGeneratorRunStep> runSteps2,
                                          string stepName)
    {
        Assert.Equal(runSteps1.Length, runSteps2.Length);

        for (var i = 0; i < runSteps1.Length; i++)
        {
            var runStep1 = runSteps1[i];
            var runStep2 = runSteps2[i];

            // The outputs should be equal between different runs
            List<object> outputs1 = [.. runStep1.Outputs.Select(x => x.Value)];
            List<object> outputs2 = [.. runStep2.Outputs.Select(x => x.Value)];

            //// This is a hack because comparing the outputs as objects is not working
            //for (int j = 0; j < outputs1.Count(); j++)
            //{

            //    switch ((outputs1[j], outputs2[j]))
            //    {
            //        case (CommandNode node1, CommandNode node2):
            //            Assert.Equivalent(node1, node2);
            //            break;
            //        case (CommandInfo commandInfo1, CommandInfo commandInfo2):
            //            Assert.Equivalent(commandInfo1, commandInfo2);
            //            break;
            //        case (CliInfo cliInfo1, CliInfo cliInfo2):
            //            Assert.Equivalent(cliInfo1, cliInfo2);
            //            break;
            //        case (CliInfoGroup cliInfoGroup1, CliInfoGroup cliInfoGroup2):
            //            Assert.Equivalent(cliInfoGroup1, cliInfoGroup2);
            //            break;
            //        default:
            //            throw new InvalidOperationException($"Unexpected step output type: {outputs1.GetType()}");
            //    }
            //}

            // outputs should be the same since they are cached
            Assert.Equal(outputs1, outputs2);

            // Hashsets remove duplicates
            var step2Reasons = runStep2.Outputs
                                    .Select(x => x.Reason)
                                    .ToHashSet();

            // Therefore, on the second run the results should always be cached or unchanged!
            // - Unchanged is when the _input_ has changed, but the output hasn't
            // - Cached is when the the input has not changed, so the cached output is used 
            var cacheFailures = step2Reasons.Where(x => x != IncrementalStepRunReason.Cached && x != IncrementalStepRunReason.Unchanged);
            Assert.Empty(cacheFailures);

            // Make sure we're not using anything we shouldn't
            AssertObjectGraph(runStep1, stepName);
        }
    }

    static void AssertOutputsEqual<T>(IEnumerable<T> outputs)
    {

    }

    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Based on Andrew Lock's blog post on testing incremental generators: https://andrewlock.net/creating-a-source-generator-part-10-testing-your-incremental-generator-pipeline-outputs-are-cacheable/
    /// </remarks>
    /// <param name="runStep"></param>
    /// <param name="stepName"></param>
    static void AssertObjectGraph(IncrementalGeneratorRunStep runStep, string stepName)
    {
        // Including the stepName in error messages to make it easy to isolate issues
        var because = $"{stepName} shouldn't contain banned symbols";
        var visited = new HashSet<object>();

        // Check all of the outputs - probably overkill, but why not
        foreach (var (obj, _) in runStep.Outputs)
        {
            Visit(obj);
        }

        void Visit(object node)
        {
            // If we've already seen this object, or it's null, stop.
            if (node is null || !visited.Add(node))
            {
                return;
            }

            // Make sure it's not a banned type
            Assert.IsNotType<Compilation>(node, false);
            Assert.IsNotType<ISymbol>(node, false);
            Assert.IsNotType<SyntaxNode>(node, false);
            Assert.IsNotType<SemanticModel>(node, false);


            // Examine the object
            Type type = node.GetType();
            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
            {
                return;
            }

            // If the object is a collection, check each of the values
            if (node is IEnumerable collection and not string)
            {
                foreach (object element in collection)
                {
                    // recursively check each element in the collection
                    Visit(element);
                }

                return;
            }

            // Recursively check each field in the object
            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                object? fieldValue = field.GetValue(node);
                if (fieldValue is not null)
                {
                    Visit(fieldValue);
                }
            }
        }
    }



}