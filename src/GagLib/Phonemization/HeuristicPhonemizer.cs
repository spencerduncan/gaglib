using GagLib.Abstractions;

namespace GagLib.Phonemization;

/// <summary>
/// Fallback phonemizer that uses heuristic rules for words not in dictionary.
/// </summary>
public class HeuristicPhonemizer : IPhonemizer
{
    /// <inheritdoc />
    public bool CanPhonemize(string word)
    {
        // Heuristic can attempt any word
        return !string.IsNullOrWhiteSpace(word);
    }

    /// <inheritdoc />
    public IReadOnlyList<Phoneme> Phonemize(string word)
    {
        // TODO: Implement letter-to-phoneme heuristics
        // Basic approach: map common letter patterns to phonemes
        return Array.Empty<Phoneme>();
    }

    /// <inheritdoc />
    public IReadOnlyList<IReadOnlyList<Phoneme>> PhonemizeSentence(string text)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Select(Phonemize).ToList().AsReadOnly();
    }
}
