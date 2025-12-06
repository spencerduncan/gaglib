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

    private static readonly Dictionary<GagType, IGagTransformerBase> Transformers = new()
    {
        [GagType.BallGag] = new BallGagTransformer(),
        [GagType.CowGag] = new CowGagTransformer(),
        [GagType.DogGag] = new DogGagTransformer(),
        [GagType.BarkingDogGag] = new BarkingDogGagTransformer(),
        [GagType.CatgirlGag] = new CatgirlGagTransformer(),
        [GagType.CatGag] = new CatGagTransformer(),
        [GagType.UwuGag] = new UwuGagTransformer(),
        [GagType.FurryGag] = new FurryGagTransformer(),
    };

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

        if (!Transformers.TryGetValue(gagType, out var transformer))
            throw new ArgumentOutOfRangeException(nameof(gagType), gagType, "Unknown gag type");

        var tokens = Processor.Process(message);

        return transformer switch
        {
            ITextBasedGagTransformer t => BuildOutputTextBased(tokens, t),
            IGagTransformer t => BuildOutputPhonemeBased(tokens, t),
            _ => throw new InvalidOperationException($"Unknown transformer type: {transformer.GetType()}")
        };
    }

    private static string BuildOutputPhonemeBased(IReadOnlyList<TextToken> tokens, IGagTransformer transformer)
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

    private static string BuildOutputTextBased(IReadOnlyList<TextToken> tokens, ITextBasedGagTransformer transformer)
    {
        // Collect words with original text for text-based transformation
        var words = new List<(string OriginalText, IReadOnlyList<Phoneme>? Phonemes)>();

        foreach (var token in tokens)
        {
            if (token.Type == TextTokenType.Word)
            {
                words.Add((token.OriginalText, token.Phonemes));
            }
        }

        // Transform at sentence level (handles uwu/owo insertion, emoticons, etc.)
        var transformedSentence = transformer.TransformSentenceText(words);
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
            else if (token.Type == TextTokenType.Word)
            {
                if (wordIdx < transformedWords.Length)
                {
                    result.Append(transformedWords[wordIdx]);
                    wordIdx++;
                }
            }
        }

        return result.ToString();
    }
}
