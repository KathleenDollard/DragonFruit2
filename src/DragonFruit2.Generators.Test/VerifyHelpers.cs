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

    private class TrackingStepReport
    {
        public required string TrackedStep { get; init; }
        public bool SameNumberSteps { get; internal set; } = true;
        public bool OutputsSame { get; internal set; } = true;
        public bool WasCached { get; internal set; } = true;
        public bool AllTypesAreLegal { get; internal set; } = true;
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

        var trackingReport = new TrackingStepReport[trackedSteps1.Count];
        // Get the IncrementalGeneratorRunStep collection for each run
        var steps1 = trackedSteps1.ToArray();
        for (int i = 0; i < steps1.Length; i++)
        {
            var (trackingName, runSteps1) = steps1[i];
            var runSteps2 = trackedSteps2[trackingName];
            trackingReport[i] = ReportSteps(runSteps1, runSteps2, trackingName);
        }

        // We might need something better for reporting if issues with debugging arise
        Assert.All(trackingReport, item => Assert.True(item.OutputsSame && item.WasCached && item.AllTypesAreLegal));

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

    private static TrackingStepReport ReportSteps(ImmutableArray<IncrementalGeneratorRunStep> runSteps1,
                                                  ImmutableArray<IncrementalGeneratorRunStep> runSteps2,
                                                  string trackingName)
    {
        var report = new TrackingStepReport { TrackedStep = trackingName };
        var sameNumberSteps = runSteps1.Length == runSteps2.Length;
        for (var i = 0; i < runSteps1.Length; i++)
        {
            if (report.OutputsSame) // once its false, we stop checking
            {
                report.OutputsSame = OutputsAreTheSame(runSteps1[i], runSteps2[i]);
            }
            if (report.WasCached) // once its false, we stop checking
            {
                var wasCached = runSteps2[i].Outputs
                                    .Select(x => x.Reason)
                                    .ToHashSet()
                                    .All(x => x == IncrementalStepRunReason.Cached && x == IncrementalStepRunReason.Unchanged);

            }
            if (report.AllTypesAreLegal) // once its false, we stop checking
            {
                try
                {
                    report.AllTypesAreLegal = CheckAllTypesAreLegal(runSteps1[i], trackingName);
                }
                catch (Exception)
                {
                    report.AllTypesAreLegal = false;
                }
            }
        }
        return report;
    }

    internal static bool OutputsAreTheSame(IncrementalGeneratorRunStep runStep1, IncrementalGeneratorRunStep runStep2)
    {
        // The outputs should be equal between different runs
        List<object> outputs1 = [.. runStep1.Outputs.Select(x => x.Value)];
        List<object> outputs2 = [.. runStep2.Outputs.Select(x => x.Value)];

        return outputs1.SequenceEqual(outputs2);
    }


    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    /// Based on Andrew Lock's blog post on testing incremental generators: https://andrewlock.net/creating-a-source-generator-part-10-testing-your-incremental-generator-pipeline-outputs-are-cacheable/
    /// </remarks>
    /// <param name="runStep"></param>
    /// <param name="stepName"></param>
    static bool CheckAllTypesAreLegal(IncrementalGeneratorRunStep runStep, string stepName)
    {
        var visited = new HashSet<object>();

        // Check all of the outputs - probably overkill, but why not
        foreach (var (obj, _) in runStep.Outputs)
        {
            if (!Visit(obj)) return false;
        }
        return true;

        bool Visit(object node)
        {
            // If we've already seen this object, or it's null, stop.
            if (node is null || !visited.Add(node))
            {
                return true;
            }

            if (node is Compilation || node is ISymbol || node is SyntaxNode || node is SemanticModel)
            {
                return false;
            }

            var type = node.GetType();
            if (type.IsPrimitive || type.IsEnum || type == typeof(string))
            {
                return true;
            }

            // If the object is a collection, check each of the values
            if (node is IEnumerable collection and not string)
            {
                foreach (object element in collection)
                {
                    // recursively check each element in the collection
                    if (!Visit(element)) return false;
                }
                return true;
            }

            // Recursively check each field in the object
            foreach (FieldInfo field in type.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance))
            {
                object? fieldValue = field.GetValue(node);
                if (fieldValue is not null)
                {
                    if (!Visit(fieldValue)) return false;
                }
            }
            return true;
        }
    }
}