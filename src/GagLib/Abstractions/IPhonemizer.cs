namespace GagLib.Abstractions;

/// <summary>
/// Converts words into phoneme sequences.
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
    /// Checks if a word can be phonemized by this implementation.
    /// </summary>
    /// <param name="word">The word to check.</param>
    /// <returns>True if the word can be phonemized.</returns>
    bool CanPhonemize(string word);
}
