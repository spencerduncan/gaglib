namespace GagLib.Abstractions;

/// <summary>
/// A gag transformer that operates on original text rather than phonemes.
/// Used for transformations that require word substitution, spelling changes, or text manipulation.
/// </summary>
public interface ITextBasedGagTransformer : IGagTransformerBase
{
    /// <summary>
    /// Transforms a word using the original text.
    /// </summary>
    /// <param name="originalText">The original word text.</param>
    /// <param name="phonemes">The phonemes for the word (may be null or empty).</param>
    /// <returns>The transformed word.</returns>
    string TransformWord(string originalText, IReadOnlyList<Phoneme>? phonemes);

    /// <summary>
    /// Transforms a full sentence using original text.
    /// </summary>
    /// <param name="words">Original words with their phonemes.</param>
    /// <returns>The transformed sentence.</returns>
    string TransformSentenceText(IReadOnlyList<(string OriginalText, IReadOnlyList<Phoneme>? Phonemes)> words);
}
