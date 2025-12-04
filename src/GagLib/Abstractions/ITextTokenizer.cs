namespace GagLib.Abstractions;

/// <summary>
/// Tokenizes text into words and preserved content (punctuation, emoji, Discord embeds).
/// </summary>
public interface ITextTokenizer
{
    /// <summary>
    /// Tokenizes the input text into a sequence of tokens.
    /// </summary>
    /// <param name="text">The text to tokenize.</param>
    /// <returns>A list of tokens representing words and preserved content.</returns>
    IReadOnlyList<TextToken> Tokenize(string text);
}
