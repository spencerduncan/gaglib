namespace GagLib.Abstractions;

/// <summary>
/// The type of a text token.
/// </summary>
public enum TextTokenType
{
    /// <summary>
    /// A word that should be phonemized.
    /// </summary>
    Word,

    /// <summary>
    /// Content that should be preserved as-is (punctuation, emoji, Discord embeds).
    /// </summary>
    Preserved
}

/// <summary>
/// Represents a token extracted from text, either a word to phonemize or content to preserve.
/// </summary>
public class TextToken
{
    /// <summary>
    /// Gets the type of this token.
    /// </summary>
    public TextTokenType Type { get; }

    /// <summary>
    /// Gets the original text of this token.
    /// </summary>
    public string OriginalText { get; }

    /// <summary>
    /// Gets or sets the phonemes for this token. Only populated for Word tokens after processing.
    /// </summary>
    public IReadOnlyList<Phoneme>? Phonemes { get; set; }

    private TextToken(TextTokenType type, string text)
    {
        Type = type;
        OriginalText = text;
    }

    /// <summary>
    /// Creates a word token that will be phonemized.
    /// </summary>
    /// <param name="text">The word text.</param>
    /// <returns>A new word token.</returns>
    public static TextToken Word(string text) => new(TextTokenType.Word, text);

    /// <summary>
    /// Creates a preserved token that will be kept as-is.
    /// </summary>
    /// <param name="text">The text to preserve.</param>
    /// <returns>A new preserved token.</returns>
    public static TextToken Preserved(string text) => new(TextTokenType.Preserved, text);
}
