using DapperBeerClassroom.Helpers;
using System.Diagnostics.CodeAnalysis;

namespace DapperBeerClassroom.Github
{    
    public class GithubWorkflowJob
    {
        public string Name { get; set; }
        public Dictionary<string, object> Job { get; set;}
        public List<object> Steps { get; set; }

        public GithubWorkflowJob (
            string name,
            string template
        )
        {
            this.Name = name;
            this.Job = YamlHelper.GetYamlObject($"resources/{template}.yml");
            this.Steps = (List<object>) this.Job["steps"] ?? new List<object>();
        }

        public virtual Dictionary<string, object> CompileJob()
        {            
            this.Job["steps"] = this.Steps;
            return this.Job;
        }

    }
}
