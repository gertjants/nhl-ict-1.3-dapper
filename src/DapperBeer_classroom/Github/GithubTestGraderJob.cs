using DapperBeerClassroom.Helpers;
using DapperBeerClassroom.Models;
using System.Diagnostics.CodeAnalysis;

namespace DapperBeerClassroom.Github
{
    public class GithubTestGraderJob : GithubWorkflowJob
    {
        public List<Dictionary<string, object>> Tests { get; set;}
        public List<string> TestIds
            => this.Tests
                .SelectMany(v => 
                     v.Where(k => k.Key == "id")
                    .Select(k => (string) k.Value)
                )
                .ToList();

        public TestFramework framework { get; private set; }

        public GithubTestGraderJob(
            [NotNull] TestFramework framework,
            [NotNull] string template="classroom_job_base"
        ) : base(
            name: framework.Name,
            template: template 
        )
        {
            this.framework = framework;
            this.Tests = new List<Dictionary<string,object>>();
            this.Steps.Add(new Dictionary<string, object>() {
                ["name"] = "Build",
                ["run"] = $"dotnet build {framework.Path}"
            });
        }

        public bool GetTestSteps()
        {
            var assembly = System.Reflection.Assembly.Load(framework.AssemblyName);
            var assignments = assembly
                .GetAssignmentsInNamespace(framework.TestsNamespaceName)
                .ToArray();

            var tests = assignments
                .GetTestSteps(framework.TestCommand);

            this.Tests.AddRange(tests);

            return (tests.Any());
        }

        public override Dictionary<string, object> CompileJob()
        {     
            if(GetTestSteps())
            {
                // Adding tests
                this.Steps.AddRange(this.Tests);
                // Adding Grading step
                this.Steps.Add(TestFrameworkHelpers.NewGraderStep(this.TestIds));
            }

            return base.CompileJob();
        }
    }
}