using GagLib.Abstractions;

namespace GagLib.Transformers;

/// <summary>
/// Transforms phonemes into muffled sounds as if speaking through a ball gag.
///
/// Ball gag physics:
/// - Mouth forced open and blocked by ball
/// - Tongue movement restricted
/// - Lips can't close properly
/// - Air can still pass through nose
///
/// Result: nasals pass through, everything else becomes muffled "mmph" sounds.
/// </summary>
public class BallGagTransformer : IGagTransformer
{
    // Phoneme categories and their muffled outputs
    private static readonly Dictionary<Phoneme, string> PhonemeMap = new()
    {
        // Nasals - pass through relatively well (air through nose)
        [Phoneme.M] = "m",
        [Phoneme.N] = "n",
        [Phoneme.NG] = "ng",

        // Vowels - become nasal moans
        [Phoneme.IY] = "nn",   // bee
        [Phoneme.IH] = "nh",   // bit
        [Phoneme.EY] = "nnh",  // bay
        [Phoneme.EH] = "eh",   // bet
        [Phoneme.AE] = "aa",   // bat
        [Phoneme.AA] = "aah",  // bot
        [Phoneme.AO] = "aw",   // bought
        [Phoneme.OW] = "oh",   // boat
        [Phoneme.UH] = "uh",   // book
        [Phoneme.UW] = "oo",   // boot
        [Phoneme.AH] = "uh",   // but
        [Phoneme.ER] = "rr",   // bird
        [Phoneme.AY] = "ah",   // buy
        [Phoneme.AW] = "aw",   // bout
        [Phoneme.OY] = "oy",   // boy

        // Voiced stops - can't form properly, become nasal
        [Phoneme.B] = "mm",
        [Phoneme.D] = "nn",
        [Phoneme.G] = "ngh",

        // Voiceless stops - air escapes, muffled pop
        [Phoneme.P] = "mph",
        [Phoneme.T] = "th",
        [Phoneme.K] = "kh",

        // Voiced fricatives - vibration muffled
        [Phoneme.V] = "mm",
        [Phoneme.Z] = "nn",
        [Phoneme.ZH] = "zh",   // measure
        [Phoneme.DH] = "dh",   // this

        // Voiceless fricatives - some air escapes
        [Phoneme.F] = "ff",
        [Phoneme.S] = "th",
        [Phoneme.SH] = "sh",
        [Phoneme.TH] = "th",   // think
        [Phoneme.HH] = "hh",

        // Affricates
        [Phoneme.CH] = "tsh",
        [Phoneme.JH] = "zh",

        // Approximants - heavily muffled
        [Phoneme.L] = "ll",
        [Phoneme.R] = "rr",
        [Phoneme.W] = "ww",
        [Phoneme.Y] = "yy",
    };

    /// <inheritdoc />
    public string Name => "Ball Gag";

    /// <inheritdoc />
    public string Transform(IReadOnlyList<Phoneme> phonemes)
    {
        if (phonemes.Count == 0)
            return string.Empty;

        var result = new System.Text.StringBuilder();

        foreach (var phoneme in phonemes)
        {
            if (PhonemeMap.TryGetValue(phoneme, out var muffled))
            {
                result.Append(muffled);
            }
        }

        // Clean up repeated characters for more natural output
        return SimplifyOutput(result.ToString());
    }

    /// <inheritdoc />
    public string TransformSentence(IReadOnlyList<IReadOnlyList<Phoneme>> phonemesByWord)
    {
        var transformed = phonemesByWord.Select(Transform);
        return string.Join(" ", transformed);
    }

    /// <summary>
    /// Simplifies output by collapsing excessive repetition.
    /// "mmmmmmm" → "mmm", "nnnnnn" → "nnn"
    /// </summary>
    private static string SimplifyOutput(string input)
    {
        if (string.IsNullOrEmpty(input))
            return input;

        var result = new System.Text.StringBuilder();
        var i = 0;

        while (i < input.Length)
        {
            var c = input[i];
            var count = 1;

            // Count consecutive same characters
            while (i + count < input.Length && input[i + count] == c)
            {
                count++;
            }

            // Cap repetition at 3
            var outputCount = Math.Min(count, 3);
            result.Append(c, outputCount);

            i += count;
        }

        return result.ToString();
    }
}
