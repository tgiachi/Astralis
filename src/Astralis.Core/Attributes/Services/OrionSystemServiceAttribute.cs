using System;

namespace Astralis.Core.Attributes.Services;

[AttributeUsage(AttributeTargets.Class)]
public class OrionSystemServiceAttribute : Attribute
{
    public int Priority { get; }

    public OrionSystemServiceAttribute(int priority = 0)
    {
        Priority = priority;
    }
}
