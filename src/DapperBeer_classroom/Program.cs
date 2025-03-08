using DapperBeerClassroom.Models;
using DapperBeerClassroom.Helpers;
using DapperBeerClassroom.Github;

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

            var workflow = new GithubWorkflow(
                name: "classroom",
                template: "classroom_base"
            );
            
            foreach(var framework in frameworks)
                workflow.Jobs.Add($"{framework.Name}-job", (new GithubTestGraderJob(framework)).CompileJob());

            workflow.GenerateWorkflow();
        }

    }
}
