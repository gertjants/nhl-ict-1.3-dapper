using DapperBeerClassroom.Helpers;
using YamlDotNet.Serialization;
using System.Diagnostics.CodeAnalysis;

namespace DapperBeerClassroom.Github
{    
    public class GithubWorkflow
    {
        public string Name { get; set; }
        public Dictionary<string, object> Jobs { get; set; }
        private Dictionary<string, object> Workflow { get; set;}

        public GithubWorkflow(
            [NotNull] string name,
            [NotNull] string template
        )
        {
            this.Name = name;
            this.Workflow = YamlHelper.GetYamlObject($"resources/{template}.yml");
            this.Jobs = new Dictionary<string, object>();
        }

        public void GenerateWorkflow()
        {
            this.Workflow["jobs"] = this.Jobs;
            YamlHelper.WriteYamlFile(this.Name, this.Workflow);
        }
    }
}