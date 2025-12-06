using GagLib.Abstractions;

namespace GagLib.Transformers;

/// <summary>
/// Transforms phonemes into cow sounds (moo).
/// Syllable-based: each vowel becomes a "moo", with length based on vowel quality.
/// </summary>
public class CowGagTransformer : IGagTransformer
{
    // Long vowels and diphthongs get extended "mooo"
    private static readonly HashSet<Phoneme> LongVowels =
    [
        Phoneme.IY,  // bee
        Phoneme.UW,  // boot
        Phoneme.OW,  // boat
        Phoneme.EY,  // bay
        Phoneme.AY,  // buy
        Phoneme.AW,  // bout
        Phoneme.OY,  // boy
        Phoneme.AO,  // bought
    ];

    // Short vowels get regular "moo"
    private static readonly HashSet<Phoneme> ShortVowels =
    [
        Phoneme.IH,  // bit
        Phoneme.EH,  // bet
        Phoneme.AE,  // bat
        Phoneme.AH,  // but
        Phoneme.UH,  // book
        Phoneme.AA,  // bot
        Phoneme.ER,  // bird
    ];

    /// <inheritdoc />
    public string Name => "Cow Gag";

    /// <inheritdoc />
    public string Transform(IReadOnlyList<Phoneme> phonemes)
    {
        if (phonemes.Count == 0)
            return string.Empty;

        var syllables = new List<string>();

        foreach (var phoneme in phonemes)
        {
            if (LongVowels.Contains(phoneme))
            {
                syllables.Add("mooo");
            }
            else if (ShortVowels.Contains(phoneme))
            {
                syllables.Add("moo");
            }
            // Consonants don't produce syllables
        }

        // No vowels? Just give a short moo
        if (syllables.Count == 0)
            return "moo";

        // Extend final syllable for emphasis
        if (syllables.Count > 0)
        {
            var last = syllables[^1];
            syllables[^1] = last + "o";
        }

        return string.Concat(syllables);
    }

    /// <inheritdoc />
    public string TransformSentence(IReadOnlyList<IReadOnlyList<Phoneme>> phonemesByWord)
    {
        var transformed = phonemesByWord.Select(Transform);
        return string.Join(" ", transformed);
    }
}
