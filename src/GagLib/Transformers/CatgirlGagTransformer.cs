using GagLib.Abstractions;

namespace GagLib.Transformers;

/// <summary>
/// Transforms phonemes into anime catgirl speech (nya~).
/// Based on "nyaccent" rules - only transforms specific patterns while keeping words mostly intelligible.
/// </summary>
public class CatgirlGagTransformer : IGagTransformer
{
    private readonly Random _random;

    /// <summary>
    /// Creates a catgirl transformer with the default random source.
    /// </summary>
    public CatgirlGagTransformer() : this(Random.Shared) { }

    /// <summary>
    /// Creates a catgirl transformer with a specified random source (for testing).
    /// </summary>
    public CatgirlGagTransformer(Random random)
    {
        _random = random;
    }

    private static readonly HashSet<Phoneme> Vowels =
    [
        Phoneme.AA, Phoneme.AE, Phoneme.AH, Phoneme.AO, Phoneme.AW,
        Phoneme.AY, Phoneme.EH, Phoneme.ER, Phoneme.EY, Phoneme.IH,
        Phoneme.IY, Phoneme.OW, Phoneme.OY, Phoneme.UH, Phoneme.UW
    ];

    // A-type vowels that trigger "nya" (like "a" in magic, cat)
    private static readonly HashSet<Phoneme> ATypeVowels =
    [
        Phoneme.AE,  // cat, magic
        Phoneme.AA,  // father
    ];

    // U-type vowels that trigger "mew/nyu" (like universe, you)
    private static readonly HashSet<Phoneme> UTypeVowels =
    [
        Phoneme.UW,  // boot, universe
        Phoneme.UH,  // book
        Phoneme.IY,  // me, see (sounds like "mee" → "mew")
    ];

    // Phoneme to text mapping
    private static readonly Dictionary<Phoneme, string> PhonemeText = new()
    {
        // Vowels
        [Phoneme.AA] = "ah", [Phoneme.AE] = "a", [Phoneme.AH] = "uh",
        [Phoneme.AO] = "aw", [Phoneme.AW] = "ow", [Phoneme.AY] = "ai",
        [Phoneme.EH] = "eh", [Phoneme.ER] = "er", [Phoneme.EY] = "ay",
        [Phoneme.IH] = "ih", [Phoneme.IY] = "ee", [Phoneme.OW] = "oh",
        [Phoneme.OY] = "oi", [Phoneme.UH] = "oo", [Phoneme.UW] = "oo",
        // Consonants
        [Phoneme.B] = "b", [Phoneme.CH] = "ch", [Phoneme.D] = "d",
        [Phoneme.DH] = "th", [Phoneme.F] = "f", [Phoneme.G] = "g",
        [Phoneme.HH] = "h", [Phoneme.JH] = "j", [Phoneme.K] = "k",
        [Phoneme.L] = "l", [Phoneme.M] = "m", [Phoneme.N] = "n",
        [Phoneme.NG] = "ng", [Phoneme.P] = "p", [Phoneme.R] = "r",
        [Phoneme.S] = "s", [Phoneme.SH] = "sh", [Phoneme.T] = "t",
        [Phoneme.TH] = "th", [Phoneme.V] = "v", [Phoneme.W] = "w",
        [Phoneme.Y] = "y", [Phoneme.Z] = "z", [Phoneme.ZH] = "zh",
    };

    /// <inheritdoc />
    public string Name => "Catgirl Gag";

    /// <inheritdoc />
    public string Transform(IReadOnlyList<Phoneme> phonemes)
    {
        if (phonemes.Count == 0)
            return string.Empty;

        var result = new System.Text.StringBuilder();
        var i = 0;

        while (i < phonemes.Count)
        {
            var phoneme = phonemes[i];

            // Rule 1: M/H + A-type vowel at start → "nya" + rest (magic → nyagic)
            if (i == 0 && (phoneme == Phoneme.M || phoneme == Phoneme.HH) &&
                i + 1 < phonemes.Count && ATypeVowels.Contains(phonemes[i + 1]))
            {
                result.Append("nya");
                i += 2; // Skip both M/H and the vowel
                continue;
            }

            // Rule 2: Word starts with A-type vowel → "nya" prefix (apple → nyapple)
            if (i == 0 && ATypeVowels.Contains(phoneme))
            {
                result.Append("nya");
                i++;
                continue;
            }

            // Rule 3: Y + U-type vowel → "mew" (you → mew, universe prefix)
            if (phoneme == Phoneme.Y && i + 1 < phonemes.Count && UTypeVowels.Contains(phonemes[i + 1]))
            {
                result.Append("mew");
                i += 2;
                continue;
            }

            // Rule 4: Standalone U-type at start → "nyu" (ooh → nyu)
            if (i == 0 && UTypeVowels.Contains(phoneme))
            {
                result.Append("nyu");
                i++;
                continue;
            }

            // Rule 5: N + vowel → "ny" + vowel sound (nothing → nyothing)
            if (phoneme == Phoneme.N && i + 1 < phonemes.Count && Vowels.Contains(phonemes[i + 1]))
            {
                var nextVowel = phonemes[i + 1];
                result.Append("ny");
                if (PhonemeText.TryGetValue(nextVowel, out var vowelSound))
                {
                    result.Append(vowelSound);
                }
                i += 2;
                continue;
            }

            // Default: keep the phoneme as-is
            if (PhonemeText.TryGetValue(phoneme, out var text))
            {
                result.Append(text);
            }
            i++;
        }

        return result.ToString();
    }

    /// <inheritdoc />
    public string TransformSentence(IReadOnlyList<IReadOnlyList<Phoneme>> phonemesByWord)
    {
        var transformed = phonemesByWord.Select(Transform).ToList();
        var sentence = string.Join(" ", transformed);

        // Add trailing "~" randomly (about 30% chance) or if sentence is short
        if (transformed.Count <= 2 || _random.NextDouble() < 0.3)
        {
            sentence += "~";
        }

        return sentence;
    }
}
