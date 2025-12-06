namespace GagLib.Abstractions;

/// <summary>
/// Base interface for all gag transformers.
/// </summary>
public interface IGagTransformerBase
{
    /// <summary>
    /// Gets the display name of this gag type.
    /// </summary>
    string Name { get; }
}
