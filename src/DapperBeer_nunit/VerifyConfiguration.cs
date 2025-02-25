using System.Runtime.CompilerServices;

namespace DapperBeer;

using VerifyTests;

using static VerifySettings;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        Verifier.UseProjectRelativeDirectory("Snapshots");
    }
}