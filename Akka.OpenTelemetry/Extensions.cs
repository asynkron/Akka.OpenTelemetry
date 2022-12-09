namespace Asynkron.Akka;

public static class TypeExtensions
{
    public static string GetMessageTypeName(this object? message)
    {
        return message?.GetType().Name ?? "null";
    }
}