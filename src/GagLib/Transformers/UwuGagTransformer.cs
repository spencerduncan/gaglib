using GagLib.Abstractions;

namespace GagLib.Transformers;

/// <summary>
/// Transforms phonemes into Uwu language patterns.
/// Based on "The Uwu Language" conlang phonology rules.
/// Attribution: https://docs.google.com/document/d/1ZV1U0S8qC6yJEi6grFO_Vq5A15lRVCLYeq_udWsC-9Y
///
/// Uwu words are built from syllables like uwu, owo, uwo, ovu, etc.
/// This transformer generates syllable patterns based on input vowel count.
/// </summary>
public class UwuGagTransformer : IGagTransformer
{
    private readonly Random _random;

    /// <summary>
    /// Creates an Uwu transformer with the default random source.
    /// </summary>
    public UwuGagTransformer() : this(Random.Shared) { }

    /// <summary>
    /// Creates an Uwu transformer with a specified random source (for testing).
    /// </summary>
    public UwuGagTransformer(Random random)
    {
        _random = random;
    }

    private static readonly HashSet<Phoneme> Vowels =
    [
        Phoneme.AA, Phoneme.AE, Phoneme.AH, Phoneme.AO, Phoneme.AW,
        Phoneme.AY, Phoneme.EH, Phoneme.ER, Phoneme.EY, Phoneme.IH,
        Phoneme.IY, Phoneme.OW, Phoneme.OY, Phoneme.UH, Phoneme.UW
    ];

    // U-like vowels (high/back) → u
    private static readonly HashSet<Phoneme> UVowels =
    [
        Phoneme.UW, Phoneme.UH, Phoneme.IY, Phoneme.IH
    ];

    // O-like vowels (mid/back) → o
    private static readonly HashSet<Phoneme> OVowels =
    [
        Phoneme.OW, Phoneme.AO, Phoneme.OY, Phoneme.AH, Phoneme.ER
    ];

    // A-like vowels (low/front) → a (used for nya patterns)
    private static readonly HashSet<Phoneme> AVowels =
    [
        Phoneme.AA, Phoneme.AE, Phoneme.AY, Phoneme.AW, Phoneme.EH, Phoneme.EY
    ];

    // Uwu-style syllable patterns (VwV or VvV) with tonal diacritics for flavor
    private static readonly string[] UPatterns = ["uwu", "uwo", "üwu", "ùwu", "úwu", "ûwu", "ŭwu", "uvu", "üvü"];
    private static readonly string[] OPatterns = ["owo", "owu", "öwo", "òwo", "ówo", "ôwo", "ŏwo", "ovu", "övö"];
    private static readonly string[] APatterns = ["awa", "awu", "awo", "nya", "nyáa", "nyanya"];

    /// <inheritdoc />
    public string Name => "Uwu Gag";

    /// <inheritdoc />
    public string Transform(IReadOnlyList<Phoneme> phonemes)
    {
        if (phonemes.Count == 0)
            return string.Empty;

        // Count syllables by counting vowels
        var syllableVowels = new List<Phoneme>();
        foreach (var phoneme in phonemes)
        {
            if (Vowels.Contains(phoneme))
            {
                syllableVowels.Add(phoneme);
            }
        }

        // No vowels? Return single uwu
        if (syllableVowels.Count == 0)
            return "uwu";

        // Generate uwu-style syllables based on input vowels
        var result = new System.Text.StringBuilder();

        for (var i = 0; i < syllableVowels.Count; i++)
        {
            var vowel = syllableVowels[i];

            // Pick pattern based on vowel type
            string[] patterns;
            if (UVowels.Contains(vowel))
                patterns = UPatterns;
            else if (OVowels.Contains(vowel))
                patterns = OPatterns;
            else
                patterns = APatterns;

            var pattern = patterns[_random.Next(patterns.Length)];

            // For multi-syllable words, sometimes just use a simple connector
            if (i > 0)
            {
                // Add connector between syllables
                if (_random.NextDouble() < 0.3)
                {
                    result.Append('w');
                }
            }

            result.Append(pattern);
        }

        return result.ToString();
    }

    /// <inheritdoc />
    public string TransformSentence(IReadOnlyList<IReadOnlyList<Phoneme>> phonemesByWord)
    {
        var transformed = phonemesByWord.Select(Transform).ToList();
        var sentence = string.Join(" ", transformed);

        // Add uwu/owo suffix randomly (about 25% chance) or for short sentences
        if (transformed.Count <= 2 || _random.NextDouble() < 0.25)
        {
            sentence += _random.NextDouble() < 0.5 ? " uwu" : " owo";
        }

        return sentence;
    }
}
