using GagLib.Abstractions;

namespace GagLib.TextProcessing;

/// <summary>
/// Processes text by tokenizing and phonemizing words while preserving other content.
/// </summary>
public class TextProcessor : ITextProcessor
{
    private readonly ITextTokenizer _tokenizer;
    private readonly IPhonemizer _phonemizer;

    /// <summary>
    /// Initializes a new instance of the <see cref="TextProcessor"/> class.
    /// </summary>
    /// <param name="tokenizer">The tokenizer to use.</param>
    /// <param name="phonemizer">The phonemizer to use for words.</param>
    public TextProcessor(ITextTokenizer tokenizer, IPhonemizer phonemizer)
    {
        _tokenizer = tokenizer;
        _phonemizer = phonemizer;
    }

    /// <summary>
    /// Creates a TextProcessor with the default tokenizer and composite phonemizer.
    /// </summary>
    /// <returns>A configured TextProcessor.</returns>
    public static TextProcessor CreateDefault()
    {
        var tokenizer = new TextTokenizer();
        var phonemizer = Phonemization.CompositePhonemizer.CreateDefault();
        return new TextProcessor(tokenizer, phonemizer);
    }

    /// <inheritdoc />
    public IReadOnlyList<TextToken> Process(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Array.Empty<TextToken>();

        var tokens = _tokenizer.Tokenize(text);

        foreach (var token in tokens)
        {
            if (token.Type == TextTokenType.Word)
            {
                token.Phonemes = _phonemizer.Phonemize(token.OriginalText);
            }
        }

        return tokens;
    }
}
