namespace Astralis.Core.Server.Events.Variables;

public record AddVariableBuilderEvent(string VariableName, Func<object> Builder);
