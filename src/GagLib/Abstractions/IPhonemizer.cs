namespace GagLib.Abstractions;

/// <summary>
/// Converts text into phoneme sequences.
/// </summary>
public interface IPhonemizer
{
    /// <summary>
    /// Converts a single word to its phoneme representation.
    /// </summary>
    /// <param name="word">The word to phonemize.</param>
    /// <returns>List of phonemes, or empty if word cannot be phonemized.</returns>
    IReadOnlyList<Phoneme> Phonemize(string word);

    /// <summary>
    /// Converts a sentence/text into phoneme sequences, one per word.
    /// </summary>
    /// <param name="text">The text to phonemize.</param>
    /// <returns>List of phoneme lists, one per word.</returns>
    IReadOnlyList<IReadOnlyList<Phoneme>> PhonemizeSentence(string text);

    /// <summary>
    /// Checks if a word can be phonemized by this implementation.
    /// </summary>
    /// <param name="word">The word to check.</param>
    /// <returns>True if the word can be phonemized.</returns>
    bool CanPhonemize(string word);
}
