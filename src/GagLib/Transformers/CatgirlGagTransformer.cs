using GagLib.Abstractions;

namespace GagLib.Transformers;

/// <summary>
/// Transforms phonemes into anime catgirl speech (nya~).
/// Syllable-based with extended vowels for that signature catgirl sound.
/// </summary>
public class CatgirlGagTransformer : IGagTransformer
{
    // Long vowels and diphthongs → extended "nyaa"
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

    // Short vowels → regular "nya"
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
    public string Name => "Catgirl Gag";

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
                syllables.Add("nyaa");
            }
            else if (ShortVowels.Contains(phoneme))
            {
                syllables.Add("nya");
            }
        }

        // No vowels? Just nya
        if (syllables.Count == 0)
            return "nya";

        // Build result with signature catgirl trailing ~
        return string.Concat(syllables) + "~";
    }

    /// <inheritdoc />
    public string TransformSentence(IReadOnlyList<IReadOnlyList<Phoneme>> phonemesByWord)
    {
        var transformed = phonemesByWord.Select(Transform);
        return string.Join(" ", transformed);
    }
}
