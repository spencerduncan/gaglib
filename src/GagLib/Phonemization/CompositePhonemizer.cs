using GagLib.Abstractions;

namespace GagLib.Phonemization;

/// <summary>
/// Composite phonemizer that tries multiple strategies in order.
/// Default chain: Dictionary lookup → Trie-based splitting → Heuristic fallback.
/// </summary>
public class CompositePhonemizer : IPhonemizer
{
    private readonly IReadOnlyList<IPhonemizer> _phonemizers;

    /// <summary>
    /// Creates a CompositePhonemizer with the default strategy chain:
    /// Dictionary lookup → Trie-based word splitting → Heuristic fallback.
    /// </summary>
    /// <returns>A configured CompositePhonemizer.</returns>
    public static CompositePhonemizer CreateDefault()
    {
        var dictionary = new CmuDictionaryPhonemizer();
        var splitting = new TrieSplittingPhonemizer(dictionary);
        var heuristic = new HeuristicPhonemizer();

        return new CompositePhonemizer(dictionary, splitting, heuristic);
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositePhonemizer"/> class.
    /// </summary>
    /// <param name="phonemizers">The phonemizers to try in order.</param>
    public CompositePhonemizer(params IPhonemizer[] phonemizers)
    {
        _phonemizers = phonemizers.ToList().AsReadOnly();
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="CompositePhonemizer"/> class.
    /// </summary>
    /// <param name="phonemizers">The phonemizers to try in order.</param>
    public CompositePhonemizer(IEnumerable<IPhonemizer> phonemizers)
    {
        _phonemizers = phonemizers.ToList().AsReadOnly();
    }

    /// <inheritdoc />
    public bool CanPhonemize(string word)
    {
        return _phonemizers.Any(p => p.CanPhonemize(word));
    }

    /// <inheritdoc />
    public IReadOnlyList<Phoneme> Phonemize(string word)
    {
        foreach (var phonemizer in _phonemizers)
        {
            if (phonemizer.CanPhonemize(word))
            {
                var result = phonemizer.Phonemize(word);
                if (result.Count > 0)
                {
                    return result;
                }
            }
        }
        return Array.Empty<Phoneme>();
    }
}
