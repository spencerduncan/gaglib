using GagLib.Abstractions;

namespace GagLib.Phonemization;

/// <summary>
/// Composite phonemizer that tries multiple strategies in order.
/// Typically: CMU dictionary first, then heuristic fallback.
/// </summary>
public class CompositePhonemizer : IPhonemizer
{
    private readonly IReadOnlyList<IPhonemizer> _phonemizers;

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

    /// <inheritdoc />
    public IReadOnlyList<IReadOnlyList<Phoneme>> PhonemizeSentence(string text)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Select(Phonemize).ToList().AsReadOnly();
    }
}
