using System;
using System.Reflection;
using System.Security.Policy;
using System.Diagnostics.CodeAnalysis;

namespace DapperBeerClassroom.Helpers
{
    public static class TestFrameworkHelpers
    {
        public static readonly string AssignmentIdentifierPrefix = "Assignment";
        public static readonly string UnitTestIdentifierSuffix = "Test";

        public static List<Dictionary<string, object>> GetTestSteps(
            this Type[] testTypes,
            string testCommand
        )
            => testTypes
                .Select((t, i) => t.GetAssignmentTestSteps(
                    assignmentId: i+1,
                    testCommand: testCommand
                ))
                .SelectMany(s => s)
                .ToList();

        public static Type[] GetAssignmentsInNamespace(
            this Assembly assembly,
            string nameSpace
        )
            => assembly.GetTypes()
            .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal) && t.Name.StartsWith(AssignmentIdentifierPrefix))
            .ToArray();

        public static List<Dictionary<string, object>> GetAssignmentTestSteps(
            this Type assignmentType, 
            [NotNull] string testCommand,
            int assignmentId
        )
            => assignmentType
                    .GetMethods()
                    .Where(q => q.Name.EndsWith(UnitTestIdentifierSuffix))
                    .Select((t, i) => NewExcerciseTestStep(
                        testCommand: string.Format(testCommand, t.DeclaringType.Name, t.Name),
                        testMethodName: t.Name,
                        assignmentId: assignmentId,
                        testId: i+1
                    )).ToList();

        public static Dictionary<string, object> NewGraderStep(
            List<string> tests
        )
            => new Dictionary<string, object>()
            {
                ["name"] = "Autograding Reporter Dapper Beer",
                ["uses"] = "education/autograding-grading-reporter@v1",
                ["env"] = tests.Select(t => new KeyValuePair<string, string>($"{t}_RESULTS", $@"${{{{steps.{t}.outputs.result}}}}")).ToDictionary(),
                ["with"] = new Dictionary<string, object>() {
                    ["runners"] = string.Join(",", tests)
                }
            };

        public static Dictionary<string, object> NewExcerciseTestStep(
            string testCommand,
            string testMethodName,
            int assignmentId, 
            int testId
        )
            => new Dictionary<string, object>()
            {
                ["name"] = $"{assignmentId}.{testId} {testMethodName}",
                ["id"] = $"E-{assignmentId}-{testId}",
                ["uses"] = "classroom-resources/autograding-command-grader@v1",
                ["with"] = new Dictionary<string, object>()
                {
                    ["test-name"] = $"{assignmentId}-{testId}-{testMethodName}",
                    ["command"] = testCommand,
                    ["timeout"] = 10,
                    ["max-score"] = 1
                }
            };
    }
}