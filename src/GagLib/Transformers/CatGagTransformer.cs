using GagLib.Abstractions;

namespace GagLib.Transformers;

/// <summary>
/// Transforms phonemes into cat sounds (meow).
/// Syllable-based: each vowel becomes a "meow", with length based on vowel quality.
/// </summary>
public class CatGagTransformer : IGagTransformer
{
    // Long vowels and diphthongs get extended "meoww"
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

    // Short vowels get regular "meow"
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
    public string Name => "Cat Gag";

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
                syllables.Add("meoww");
            }
            else if (ShortVowels.Contains(phoneme))
            {
                syllables.Add("meow");
            }
            // Consonants don't produce syllables
        }

        // No vowels? Just give a short meow
        if (syllables.Count == 0)
            return "meow";

        return string.Concat(syllables);
    }

    /// <inheritdoc />
    public string TransformSentence(IReadOnlyList<IReadOnlyList<Phoneme>> phonemesByWord)
    {
        var transformed = phonemesByWord.Select(Transform);
        return string.Join(" ", transformed);
    }
}
