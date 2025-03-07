using System;
using System.Reflection;
using System.Security.Policy;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

// Setting testing frameworks
namespace DapperBeerClassroom
{
    public static class DapperBeerPipelineConfig
    {
        public static void Main()
        {
            var frameworks = new List<TestFramework>()
            {
                new TestFramework() { Name = "tunit", useDotnetRun = true },
                new TestFramework() { Name = "nunit", useDotnetRun = false }
            };

            var workflow = GetWorkflowBase();
            var jobs = new Dictionary<string, Dictionary<string, object>>();

            foreach(var framework in frameworks)
                jobs.Add($"{framework.Name}-job", CreateNewGradingJob(framework));

            workflow["jobs"] = jobs;
            
            var serializer = new SerializerBuilder()
                .Build();
            var yaml = serializer.Serialize(workflow);
            
            File.WriteAllText("/workspace/.github/workflows/classroom.yml", yaml.Replace("\n\n", "\n"));
        }

        private static Type[] GetAssignmentsInNamespace(this Assembly assembly, string nameSpace)
            => assembly.GetTypes()
            .Where(t => String.Equals(t.Namespace, nameSpace, StringComparison.Ordinal) && t.Name.StartsWith("Assignment"))
            .ToArray();

        private static Dictionary<string, object> GetWorkflowBase()
            => GetYamlObject("resources/classroom_base.yml");

        private static Dictionary<string, object> GetJobTemplate()
            => GetYamlObject("resources/classroom_job_base.yml");

        private static Dictionary<string, object> CreateNewGradingJob(TestFramework framework)
        {
            var asm = System.Reflection.Assembly.Load(framework.AssemblyName);
            var testTypes = GetAssignmentsInNamespace(asm, framework.TestsNamespaceName).ToArray();

            var testSteps = testTypes
                .Select((t, i) => t.GetAssignmentTestSteps(
                    assignmentId: i+1,
                    testCommand: framework.TestCommand
                ))
                .SelectMany(s => s)
                .ToList()
                .Slice(0,1);
            
            var testIds = testSteps
                .SelectMany(v => 
                    v.Where(k => k.Key == "id")
                    .Select(k => (string) k.Value)
                )
                .ToList()
                .Slice(0,1);

            var gradingStep = NewGraderStep(testIds);
            var jobTemplate = GetJobTemplate();
            var jobSteps = (List<object>) jobTemplate["steps"];

            jobSteps.Add(new Dictionary<string, object>() {
                ["name"] = "Build",
                ["run"] = $"dotnet build {framework.Path}"
            });
            

            jobSteps.AddRange(testSteps);
            jobSteps.Add(gradingStep);
            jobTemplate["steps"] = jobSteps;

            return jobTemplate;
        }

        private static List<Dictionary<string, object>> GetAssignmentTestSteps(
            this Type assignmentType, 
            string testCommand,
            int assignmentId
        )
            => assignmentType
                    .GetMethods()
                    .Where(q => q.Name.EndsWith("Test"))
                    .Select((t, i) => NewExcerciseTestStep(
                        testCommand: string.Format(testCommand, t.DeclaringType.Name, t.Name).Replace("{", "{{").Replace("}", "}}"),
                        testMethodName: t.Name,
                        assignmentId: assignmentId,
                        testId: i+1
                    )).ToList();

        private static Dictionary<string, object> GetYamlObject(string filename)
        {
            var yaml = File.ReadAllText(filename);
            var deserializer = new DeserializerBuilder().Build();
            return deserializer.Deserialize<Dictionary<string, object>>(yaml);
        }

        private static Dictionary<string, object> NewGraderStep(List<string> tests)
            => new Dictionary<string, object>()
            {
                ["name"] = "Autograding Reporter Dapper Beer",
                ["uses"] = "education/autograding-grading-reporter@v1",
                ["env"] = tests.Select(t => new KeyValuePair<string, string>($"{t}_RESULTS", $@"${{{{steps.{t}.outputs.result}}}}")).ToDictionary(),
                ["with"] = new Dictionary<string, object>() {
                    ["runners"] = string.Join(",", tests)
                }
            };

        private static Dictionary<string, object> NewExcerciseTestStep(
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

    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
            };
    }

    public class TestFramework
    {
        public required string Name { get; set; }
        public bool useDotnetRun = false;
        public string Path
            => $"./src/DapperBeer_{this.Name}";

        public string AssemblyName 
            => $"DapperBeer_{this.Name}";
        
        public string TestsNamespaceName 
            => $"DapperBeer{this.Name.FirstCharToUpper()}.Tests";
        public string TestCommand
        {
            get {
                string command = $@"export APP_DB_SERVER=${{{{ env.APP_DB_SERVER }}}};
export APP_DB_NAME=${{{{ env.APP_DB_NAME }}}};
export APP_DB_USER=${{{{ env.APP_DB_USER }}}};
export APP_DB_PASS=${{{{ env.APP_DB_PASS }}}};";

                if(useDotnetRun)
                    command += $"\ndotnet run --project {this.Path} --treenode-filter \"/*/*/{{0}}/{{1}}\"";
                else
                    command += $"\ndotnet test {this.Path} --filter FullyQualifiedName={this.TestsNamespaceName}.{{0}}.{{1}}";

                return command;
            }
        }
    }
}
