using System.Runtime.CompilerServices;

namespace DapperBeerNunit;

using VerifyTests;

using static VerifySettings;

public static class ModuleInitializer
{
    [ModuleInitializer]
    public static void Init()
    {
        Verifier.UseProjectRelativeDirectory("../DapperBeer/Snapshots");
    }
}