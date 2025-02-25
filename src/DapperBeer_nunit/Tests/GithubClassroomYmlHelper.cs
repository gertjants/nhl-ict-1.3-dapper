using System.Reflection;

namespace DapperBeer.Tests;

public class GithubClassroomYmlHelper
{
    // [Test]
    public void GenerateClassroomYml()
    {
        //Type[] testClassesTypes = [typeof(Assignments1Tests), typeof(Assignments2Tests), typeof(Assignments3Tests)];
        Type[] testClassesTypes = [];

        var ids = new List<string>();
        int exerciseIndex = 1;
        for (int i = 0; i < testClassesTypes.Length; i++)
        {
            // Get methods that have the [Test] attribute
            var testMethods = testClassesTypes[i]
                .GetMethods(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttributes(typeof(TestAttribute), false).Any());

            int assignmentNumber = i+1;
            
            
            // - name: Assignment 1-1
            //     id: E-1-1
            //     uses: classroom-resources/autograding-command-grader@v1
            //     with:
            //     test-name: Assignment 1-1 Test Name
            //     command: dotnet run --project DapperBeer --treenode-filter "/*/*/Tests/Assignments1Tests/*"
            //     timeout: 10
            //     max-score: 1
            
            int exerciseNumber = 1;
            foreach (var method in testMethods)
            {
                string id = $"E-{assignmentNumber}-{exerciseNumber}";
                ids.Add(id);
                string autograderTest = 
                $"""
                  - name: {assignmentNumber}.{exerciseNumber} {method.Name}
                    id: {id}
                    uses: classroom-resources/autograding-command-grader@v1
                    with:
                      test-name: {assignmentNumber}-{exerciseNumber}-{method.Name}
                      command: dotnet run --project DapperBeer --treenode-filter "/*/*/Assignments{assignmentNumber}Tests/{method.Name}"
                      timeout: 10
                      max-score: 1
                """;
            
                Console.WriteLine(autograderTest);

                exerciseNumber++;
                exerciseIndex++;
            }
        }
        
        //id lists
        string runners = string.Join(", ", ids);

        IEnumerable<string> idEnvs =
            ids.Select(id => id + "_RESULTS: \"${{steps." +id + ".outputs.result}}\"").ToList();

        string env = string.Join("\n", idEnvs);
        
        string autogradingReport =
            $"""
             - name: Autograding Reporter Dapper Beer
               uses: education/autograding-grading-reporter@v1
               env: 
                 {env}
               with:
                 runners: {runners}
             """;

        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine("---------------");
        Console.WriteLine();
        Console.WriteLine();
        Console.WriteLine(autogradingReport);
        
        // - name: Autograding Reporter Dapper Beer
        // uses: education/autograding-grading-reporter@v1
        // env:
        // A-DOTNET-TEST1_RESULTS: "${{steps.a-dotnet-test1.outputs.result}}"
        // A-DOTNET-TEST2_RESULTS: "${{steps.a-dotnet-test2.outputs.result}}"
        // A-DOTNET-TEST3_RESULTS: "${{steps.a-dotnet-test3.outputs.result}}"
        // with:
        // runners: a-dotnet-test1, a-dotnet-test2, a-dotnet-test3


    }
}