using GagLib.Abstractions;
using GagLib.Phonemization;
using GagLib.TextProcessing;
using GagLib.Transformers;

namespace GagLib;

/// <summary>
/// Main entry point for GagLib speech transformation.
/// </summary>
public static class Gag
{
    private static readonly TextProcessor Processor = TextProcessor.CreateDefault();
    private static readonly BallGagTransformer BallGag = new();

    /// <summary>
    /// Transforms a message as if spoken through a gag.
    /// Preserves punctuation, emoji, and Discord embeds.
    /// </summary>
    /// <param name="gagType">The type of gag to apply.</param>
    /// <param name="message">The message to transform.</param>
    /// <returns>The gagged message with preserved formatting.</returns>
    public static string Transform(GagType gagType, string message)
    {
        if (string.IsNullOrEmpty(message))
            return message;

        var transformer = GetTransformer(gagType);
        var tokens = Processor.Process(message);

        return BuildOutput(tokens, transformer);
    }

    private static IGagTransformer GetTransformer(GagType gagType)
    {
        return gagType switch
        {
            GagType.BallGag => BallGag,
            _ => throw new ArgumentOutOfRangeException(nameof(gagType), gagType, "Unknown gag type")
        };
    }

    private static string BuildOutput(IReadOnlyList<TextToken> tokens, IGagTransformer transformer)
    {
        var result = new System.Text.StringBuilder();

        foreach (var token in tokens)
        {
            if (token.Type == TextTokenType.Preserved)
            {
                result.Append(token.OriginalText);
            }
            else if (token.Phonemes != null && token.Phonemes.Count > 0)
            {
                result.Append(transformer.Transform(token.Phonemes));
            }
            else
            {
                // Word with no phonemes - keep original
                result.Append(token.OriginalText);
            }
        }

        return result.ToString();
    }
}
