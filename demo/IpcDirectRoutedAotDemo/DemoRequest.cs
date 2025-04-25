namespace IpcDirectRoutedAotDemo;

public record DemoRequest
{
    public required string Value { get; init; }
}