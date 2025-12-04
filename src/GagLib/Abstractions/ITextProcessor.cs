namespace GagLib.Abstractions;

/// <summary>
/// Processes text by tokenizing and phonemizing words while preserving other content.
/// </summary>
public interface ITextProcessor
{
    /// <summary>
    /// Processes the input text, phonemizing words and preserving other content.
    /// </summary>
    /// <param name="text">The text to process.</param>
    /// <returns>A list of tokens with words phonemized and other content preserved.</returns>
    IReadOnlyList<TextToken> Process(string text);
}
