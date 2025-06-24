using The_circle.Domain.Models;

namespace The_circle.Application;

public sealed class Unit
{
    public static readonly Unit Value = new Unit();
    private Unit() { }
}
