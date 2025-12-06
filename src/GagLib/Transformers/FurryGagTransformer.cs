using System.Text;
using System.Text.RegularExpressions;
using GagLib.Abstractions;

namespace GagLib.Transformers;

/// <summary>
/// Transforms text into furry/OwO speak style.
/// Based on common furry text transformation patterns.
/// </summary>
public partial class FurryGagTransformer : ITextBasedGagTransformer
{
    private readonly Random _random;

    /// <summary>
    /// Creates a furry transformer with the default random source.
    /// </summary>
    public FurryGagTransformer() : this(Random.Shared) { }

    /// <summary>
    /// Creates a furry transformer with a specified random source (for testing).
    /// </summary>
    public FurryGagTransformer(Random random)
    {
        _random = random;
    }

    /// <inheritdoc />
    public string Name => "Furry Gag";

    // Word substitutions (case-insensitive matching, preserve case in output)
    private static readonly Dictionary<string, string> WordSubstitutions = new(StringComparer.OrdinalIgnoreCase)
    {
        ["you"] = "chu",
        ["your"] = "ur",
        ["you're"] = "chu'we",
        ["the"] = "teh",
        ["this"] = "dis",
        ["love"] = "wuv",
        ["for"] = "fur",
        ["not"] = "knot",
        ["with"] = "wif",
        ["what"] = "wat",
        ["hi"] = "hai",
        ["bye"] = "bai",
        ["hello"] = "hewwo",
        ["cute"] = "kyoot",
        ["please"] = "pwease",
        ["pretty"] = "pwetty",
        ["little"] = "wittle",
        ["look"] = "wook",
        ["really"] = "weawwy",
        ["feel"] = "feew",
        ["feelings"] = "feewings",
    };

    // Emoticons/suffixes to append to words (no spaces - spaces break word mapping)
    private static readonly string[] WordSuffixes = ["~", "~uwu", "~owo", "~:3", "~X3", "~>:3", "~^w^"];

    /// <inheritdoc />
    public string TransformWord(string originalText, IReadOnlyList<Phoneme>? phonemes)
    {
        if (string.IsNullOrEmpty(originalText))
            return originalText;

        // Check for direct word substitution first
        if (WordSubstitutions.TryGetValue(originalText, out var substitution))
        {
            return MatchCase(originalText, substitution);
        }

        // Apply text transformations
        var result = ApplyTextTransformations(originalText);
        return result;
    }

    /// <inheritdoc />
    public string TransformSentenceText(IReadOnlyList<(string OriginalText, IReadOnlyList<Phoneme>? Phonemes)> words)
    {
        var transformedWords = new List<string>();

        for (var i = 0; i < words.Count; i++)
        {
            var (originalText, phonemes) = words[i];
            var transformed = TransformWord(originalText, phonemes);

            // Randomly append suffix to word (about 12% chance)
            if (_random.NextDouble() < 0.12)
            {
                transformed += WordSuffixes[_random.Next(WordSuffixes.Length)];
            }

            transformedWords.Add(transformed);
        }

        return string.Join(" ", transformedWords);
    }

    private string ApplyTextTransformations(string text)
    {
        var result = text;

        // N before vowel → NY (but not if already ny)
        result = NBeforeVowelRegex().Replace(result, match =>
        {
            var n = match.Groups[1].Value;
            var vowel = match.Groups[2].Value;
            // Preserve case of n
            var ny = char.IsUpper(n[0]) ? "Ny" : "ny";
            return ny + vowel;
        });

        // R → W (when followed by vowel or at end of word)
        result = RToWRegex().Replace(result, match =>
        {
            var r = match.Groups[1].Value;
            var rest = match.Groups[2].Value;
            var w = char.IsUpper(r[0]) ? "W" : "w";
            return w + rest;
        });

        // L → W (when followed by vowel)
        result = LToWRegex().Replace(result, match =>
        {
            var l = match.Groups[1].Value;
            var vowel = match.Groups[2].Value;
            var w = char.IsUpper(l[0]) ? "W" : "w";
            return w + vowel;
        });

        // TH → D or F (common in baby/furry talk)
        result = ThToFRegex().Replace(result, match =>
        {
            var th = match.Value;
            var f = char.IsUpper(th[0]) ? "F" : "f";
            return f;
        });

        return result;
    }

    private static string MatchCase(string original, string replacement)
    {
        if (string.IsNullOrEmpty(original) || string.IsNullOrEmpty(replacement))
            return replacement;

        // All uppercase
        if (original.All(char.IsUpper))
            return replacement.ToUpperInvariant();

        // First letter uppercase (Title case)
        if (char.IsUpper(original[0]))
            return char.ToUpperInvariant(replacement[0]) + replacement[1..];

        // All lowercase
        return replacement.ToLowerInvariant();
    }

    // Regex patterns using source generators for performance
    [GeneratedRegex(@"([Nn])([aeiouAEIOU])", RegexOptions.None)]
    private static partial Regex NBeforeVowelRegex();

    [GeneratedRegex(@"([Rr])([aeiouAEIOUwW]?)", RegexOptions.None)]
    private static partial Regex RToWRegex();

    [GeneratedRegex(@"([Ll])([aeiouAEIOU])", RegexOptions.None)]
    private static partial Regex LToWRegex();

    [GeneratedRegex(@"[Tt]h(?=[aeiouAEIOU])", RegexOptions.None)]
    private static partial Regex ThToFRegex();
}
