using GagLib.Abstractions;

namespace GagLib.Phonemization;

/// <summary>
/// Phonemizer that uses the CMU Pronouncing Dictionary for lookups.
/// </summary>
public class CmuDictionaryPhonemizer : IPhonemizer
{
    private readonly Dictionary<string, List<Phoneme>> _dictionary = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="CmuDictionaryPhonemizer"/> class.
    /// </summary>
    public CmuDictionaryPhonemizer()
    {
        // TODO: Load CMU dictionary from embedded resource
    }

    /// <inheritdoc />
    public bool CanPhonemize(string word)
    {
        return _dictionary.ContainsKey(NormalizeWord(word));
    }

    /// <inheritdoc />
    public IReadOnlyList<Phoneme> Phonemize(string word)
    {
        var normalized = NormalizeWord(word);
        return _dictionary.TryGetValue(normalized, out var phonemes)
            ? phonemes.AsReadOnly()
            : Array.Empty<Phoneme>();
    }

    /// <inheritdoc />
    public IReadOnlyList<IReadOnlyList<Phoneme>> PhonemizeSentence(string text)
    {
        var words = TokenizeText(text);
        return words.Select(Phonemize).ToList().AsReadOnly();
    }

    private static string NormalizeWord(string word)
    {
        // TODO: Strip punctuation, normalize case
        return word.Trim().ToUpperInvariant();
    }

    private static IEnumerable<string> TokenizeText(string text)
    {
        // TODO: Proper tokenization
        return text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    }
}
