using DapperBeerClassroom.Helpers;

namespace DapperBeerClassroom.Models
{
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
                if(useDotnetRun)
                    return $"\ndotnet run --project {this.Path} --treenode-filter \"/*/*/{{0}}/{{1}}\"";
                else
                    return $"\ndotnet test {this.Path} --filter FullyQualifiedName={this.TestsNamespaceName}.{{0}}.{{1}}";
            }
        }
    }
}