using GagLib.Abstractions;

namespace GagLib.Transformers;

/// <summary>
/// Transforms speech into cartoon dog style ("ruh-roh!").
/// Replaces initial consonants with R, prepends R to vowel-initial syllables.
/// </summary>
public class DogGagTransformer : IGagTransformer
{
    private static readonly HashSet<Phoneme> Vowels =
    [
        Phoneme.AA, Phoneme.AE, Phoneme.AH, Phoneme.AO, Phoneme.EH,
        Phoneme.ER, Phoneme.IH, Phoneme.IY, Phoneme.UH, Phoneme.UW,
        Phoneme.AW, Phoneme.AY, Phoneme.EY, Phoneme.OW, Phoneme.OY
    ];

    // Map phonemes to readable text approximations
    private static readonly Dictionary<Phoneme, string> PhonemeText = new()
    {
        [Phoneme.AA] = "ah",
        [Phoneme.AE] = "a",
        [Phoneme.AH] = "uh",
        [Phoneme.AO] = "aw",
        [Phoneme.EH] = "eh",
        [Phoneme.ER] = "er",
        [Phoneme.IH] = "ih",
        [Phoneme.IY] = "ee",
        [Phoneme.UH] = "oo",
        [Phoneme.UW] = "oo",
        [Phoneme.AW] = "ow",
        [Phoneme.AY] = "y",
        [Phoneme.EY] = "ay",
        [Phoneme.OW] = "oh",
        [Phoneme.OY] = "oy",
        [Phoneme.B] = "b",
        [Phoneme.CH] = "ch",
        [Phoneme.D] = "d",
        [Phoneme.DH] = "d",
        [Phoneme.F] = "f",
        [Phoneme.G] = "g",
        [Phoneme.HH] = "h",
        [Phoneme.JH] = "j",
        [Phoneme.K] = "k",
        [Phoneme.L] = "l",
        [Phoneme.M] = "m",
        [Phoneme.N] = "n",
        [Phoneme.NG] = "ng",
        [Phoneme.P] = "p",
        [Phoneme.R] = "r",
        [Phoneme.S] = "s",
        [Phoneme.SH] = "sh",
        [Phoneme.T] = "t",
        [Phoneme.TH] = "th",
        [Phoneme.V] = "v",
        [Phoneme.W] = "w",
        [Phoneme.Y] = "y",
        [Phoneme.Z] = "z",
        [Phoneme.ZH] = "zh",
    };

    /// <inheritdoc />
    public string Name => "Dog Gag";

    /// <inheritdoc />
    public string Transform(IReadOnlyList<Phoneme> phonemes)
    {
        if (phonemes.Count == 0)
            return string.Empty;

        var result = new System.Text.StringBuilder();
        var isFirstConsonantCluster = true;
        var addedR = false;

        foreach (var phoneme in phonemes)
        {
            var isVowel = Vowels.Contains(phoneme);

            if (isFirstConsonantCluster && !isVowel)
            {
                // Skip initial consonants, we'll replace with R
                continue;
            }

            if (isFirstConsonantCluster && isVowel)
            {
                // First vowel - add R before it
                result.Append('r');
                addedR = true;
                isFirstConsonantCluster = false;
            }

            if (PhonemeText.TryGetValue(phoneme, out var text))
            {
                result.Append(text);
            }
        }

        // Edge case: word was all consonants
        if (!addedR && result.Length == 0)
        {
            return "r";
        }

        return result.ToString();
    }

    /// <inheritdoc />
    public string TransformSentence(IReadOnlyList<IReadOnlyList<Phoneme>> phonemesByWord)
    {
        var transformed = phonemesByWord.Select(Transform);
        return string.Join(" ", transformed);
    }
}
