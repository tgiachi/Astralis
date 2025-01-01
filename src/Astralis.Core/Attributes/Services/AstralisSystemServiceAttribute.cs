using System;

namespace Astralis.Core.Attributes.Services;

[AttributeUsage(AttributeTargets.Class)]
public class AstralisSystemServiceAttribute : Attribute
{
    public int Priority { get; }

    public AstralisSystemServiceAttribute(int priority = 0)
    {
        Priority = priority;
    }
}
