namespace Astralis.Core.Server.Attributes.Scripts;

[AttributeUsage(AttributeTargets.Property)]
public class ScriptConfigVariableAttribute : Attribute
{
    public string Name { get; }

    public ScriptConfigVariableAttribute(string name)
    {
        Name = name;
    }
}
