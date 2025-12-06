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
    private static readonly CowGagTransformer CowGag = new();
    private static readonly DogGagTransformer DogGag = new();
    private static readonly BarkingDogGagTransformer BarkingDogGag = new();
    private static readonly CatgirlGagTransformer CatgirlGag = new();
    private static readonly CatGagTransformer CatGag = new();

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
            GagType.CowGag => CowGag,
            GagType.DogGag => DogGag,
            GagType.BarkingDogGag => BarkingDogGag,
            GagType.CatgirlGag => CatgirlGag,
            GagType.CatGag => CatGag,
            _ => throw new ArgumentOutOfRangeException(nameof(gagType), gagType, "Unknown gag type")
        };
    }

    private static string BuildOutput(IReadOnlyList<TextToken> tokens, IGagTransformer transformer)
    {
        // Collect word phonemes for sentence-level transformation
        var wordPhonemes = new List<IReadOnlyList<Phoneme>>();
        var wordIndices = new List<int>();

        for (var i = 0; i < tokens.Count; i++)
        {
            var token = tokens[i];
            if (token.Type == TextTokenType.Word && token.Phonemes != null && token.Phonemes.Count > 0)
            {
                wordPhonemes.Add(token.Phonemes);
                wordIndices.Add(i);
            }
        }

        // Transform all words at sentence level (allows transformer to add sentence-end effects)
        var transformedSentence = transformer.TransformSentence(wordPhonemes);
        var transformedWords = transformedSentence.Split(' ');

        // Rebuild output interleaving transformed words with preserved tokens
        var result = new System.Text.StringBuilder();
        var wordIdx = 0;

        foreach (var token in tokens)
        {
            if (token.Type == TextTokenType.Preserved)
            {
                result.Append(token.OriginalText);
            }
            else if (token.Phonemes != null && token.Phonemes.Count > 0)
            {
                // Use pre-transformed word
                if (wordIdx < transformedWords.Length)
                {
                    result.Append(transformedWords[wordIdx]);
                    wordIdx++;
                }
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
