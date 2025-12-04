using GagLib.Abstractions;

namespace GagLib.Transformers;

/// <summary>
/// Transforms phonemes into muffled sounds as if speaking through a ball gag.
/// </summary>
public class BallGagTransformer : IGagTransformer
{
    /// <inheritdoc />
    public string Name => "Ball Gag";

    /// <inheritdoc />
    public string Transform(IReadOnlyList<Phoneme> phonemes)
    {
        if (phonemes.Count == 0)
        {
            return string.Empty;
        }

        // TODO: Implement phoneme-to-muffled-sound mapping
        // Basic approach:
        // - Vowels → "mmm", "mmmh", "nngh"
        // - Nasals (M, N, NG) → partially pass through
        // - Stops (P, B, T, D, K, G) → "mph", "mm"
        // - Fricatives → "ff", "hh"
        return "mmph";
    }

    /// <inheritdoc />
    public string TransformSentence(IReadOnlyList<IReadOnlyList<Phoneme>> phonemesByWord)
    {
        var transformed = phonemesByWord.Select(Transform);
        return string.Join(" ", transformed);
    }
}
