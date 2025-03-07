namespace DapperBeerTunit;

public record TUnitParallelLimit : TUnit.Core.Interfaces.IParallelLimit
{
    public int Limit => 2;
}