namespace GagLib.Abstractions;

/// <summary>
/// Transforms phoneme sequences into gagged/muffled text output.
/// </summary>
public interface IGagTransformer
{
    /// <summary>
    /// Gets the display name of this gag type.
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Transforms a sequence of phonemes into gagged text.
    /// </summary>
    /// <param name="phonemes">The phonemes to transform.</param>
    /// <returns>The gagged text representation.</returns>
    string Transform(IReadOnlyList<Phoneme> phonemes);

    /// <summary>
    /// Transforms a full sentence (multiple words) into gagged text.
    /// </summary>
    /// <param name="phonemesByWord">Phonemes grouped by word.</param>
    /// <returns>The gagged text with word boundaries preserved.</returns>
    string TransformSentence(IReadOnlyList<IReadOnlyList<Phoneme>> phonemesByWord);
}
