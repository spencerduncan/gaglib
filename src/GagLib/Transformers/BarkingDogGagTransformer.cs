using GagLib.Abstractions;

namespace GagLib.Transformers;

/// <summary>
/// Transforms phonemes into dog barking sounds (ruff, woof, arf, bark, grr).
/// Syllable-based with varied sounds based on phoneme patterns.
/// </summary>
public class BarkingDogGagTransformer : IGagTransformer
{
    // Long vowels and diphthongs → bigger sounds (woof, bark)
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

    // Short vowels → shorter sounds (ruff, arf)
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

    // Growly consonants that trigger "grr" suffix
    private static readonly HashSet<Phoneme> GrowlyConsonants =
    [
        Phoneme.R,
        Phoneme.G,
        Phoneme.K,
    ];

    private readonly string[] _shortSounds = ["ruff", "arf"];
    private readonly string[] _longSounds = ["woof", "bark"];

    /// <inheritdoc />
    public string Name => "Barking Dog Gag";

    /// <inheritdoc />
    public string Transform(IReadOnlyList<Phoneme> phonemes)
    {
        if (phonemes.Count == 0)
            return string.Empty;

        var sounds = new List<string>();
        var shortIndex = 0;
        var longIndex = 0;
        var hasGrowl = false;

        foreach (var phoneme in phonemes)
        {
            if (LongVowels.Contains(phoneme))
            {
                sounds.Add(_longSounds[longIndex % _longSounds.Length]);
                longIndex++;
            }
            else if (ShortVowels.Contains(phoneme))
            {
                sounds.Add(_shortSounds[shortIndex % _shortSounds.Length]);
                shortIndex++;
            }
            else if (GrowlyConsonants.Contains(phoneme))
            {
                hasGrowl = true;
            }
        }

        // No vowels? Just bark
        if (sounds.Count == 0)
            return "ruff";

        // Add growl if word had growly consonants
        if (hasGrowl && sounds.Count > 0)
        {
            sounds.Add("grr");
        }

        return string.Join("", sounds);
    }

    /// <inheritdoc />
    public string TransformSentence(IReadOnlyList<IReadOnlyList<Phoneme>> phonemesByWord)
    {
        var transformed = phonemesByWord.Select(Transform);
        return string.Join(" ", transformed);
    }
}
