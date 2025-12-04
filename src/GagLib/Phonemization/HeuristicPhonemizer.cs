using GagLib.Abstractions;

namespace GagLib.Phonemization;

/// <summary>
/// Fallback phonemizer that uses heuristic rules for words not in dictionary.
/// Uses greedy pattern matching: longest patterns first, then single letters.
/// </summary>
public class HeuristicPhonemizer : IPhonemizer
{
    private readonly Dictionary<string, Phoneme[]> _cache = new(StringComparer.OrdinalIgnoreCase);

    // Patterns ordered by length (longest first within each group)
    private static readonly (string Pattern, Phoneme[] Phonemes)[] Patterns =
    [
        // 4-letter patterns
        ("tion", [Phoneme.SH, Phoneme.AH, Phoneme.N]),
        ("sion", [Phoneme.ZH, Phoneme.AH, Phoneme.N]),
        ("ough", [Phoneme.AO]),  // "though" - simplified

        // 3-letter patterns
        ("tch", [Phoneme.CH]),
        ("dge", [Phoneme.JH]),
        ("igh", [Phoneme.AY]),
        ("ing", [Phoneme.IH, Phoneme.NG]),
        ("air", [Phoneme.EH, Phoneme.R]),
        ("ear", [Phoneme.IH, Phoneme.R]),
        ("our", [Phoneme.AW, Phoneme.R]),
        ("oor", [Phoneme.AO, Phoneme.R]),

        // 2-letter patterns (digraphs)
        ("th", [Phoneme.TH]),
        ("sh", [Phoneme.SH]),
        ("ch", [Phoneme.CH]),
        ("ph", [Phoneme.F]),
        ("wh", [Phoneme.W]),
        ("ng", [Phoneme.NG]),
        ("ck", [Phoneme.K]),
        ("qu", [Phoneme.K, Phoneme.W]),
        ("gh", []),  // Often silent
        ("kn", [Phoneme.N]),  // "know"
        ("wr", [Phoneme.R]),  // "write"
        ("gn", [Phoneme.N]),  // "gnome"
        ("mb", [Phoneme.M]),  // "lamb" - b silent at end

        // Vowel digraphs
        ("ee", [Phoneme.IY]),
        ("ea", [Phoneme.IY]),
        ("oo", [Phoneme.UW]),
        ("ai", [Phoneme.EY]),
        ("ay", [Phoneme.EY]),
        ("oa", [Phoneme.OW]),
        ("ow", [Phoneme.OW]),
        ("ou", [Phoneme.AW]),
        ("oi", [Phoneme.OY]),
        ("oy", [Phoneme.OY]),
        ("au", [Phoneme.AO]),
        ("aw", [Phoneme.AO]),
        ("ew", [Phoneme.UW]),
        ("ie", [Phoneme.IY]),
        ("ei", [Phoneme.EY]),
    ];

    // Single letter mappings
    private static readonly Dictionary<char, Phoneme[]> SingleLetters = new()
    {
        ['a'] = [Phoneme.AE],
        ['b'] = [Phoneme.B],
        ['c'] = [Phoneme.K],
        ['d'] = [Phoneme.D],
        ['e'] = [Phoneme.EH],
        ['f'] = [Phoneme.F],
        ['g'] = [Phoneme.G],
        ['h'] = [Phoneme.HH],
        ['i'] = [Phoneme.IH],
        ['j'] = [Phoneme.JH],
        ['k'] = [Phoneme.K],
        ['l'] = [Phoneme.L],
        ['m'] = [Phoneme.M],
        ['n'] = [Phoneme.N],
        ['o'] = [Phoneme.AA],
        ['p'] = [Phoneme.P],
        ['q'] = [Phoneme.K],
        ['r'] = [Phoneme.R],
        ['s'] = [Phoneme.S],
        ['t'] = [Phoneme.T],
        ['u'] = [Phoneme.AH],
        ['v'] = [Phoneme.V],
        ['w'] = [Phoneme.W],
        ['x'] = [Phoneme.K, Phoneme.S],
        ['y'] = [Phoneme.Y],
        ['z'] = [Phoneme.Z],
    };

    /// <inheritdoc />
    public bool CanPhonemize(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return false;

        // Can attempt any word that has at least one letter
        return word.Any(char.IsLetter);
    }

    /// <inheritdoc />
    public IReadOnlyList<Phoneme> Phonemize(string word)
    {
        if (string.IsNullOrWhiteSpace(word))
            return [];

        if (_cache.TryGetValue(word, out var cached))
            return cached;

        var normalized = word.ToLowerInvariant();
        List<Phoneme> result = [];
        var position = 0;

        while (position < normalized.Length)
        {
            var c = normalized[position];

            // Skip non-letters
            if (!char.IsLetter(c))
            {
                position++;
                continue;
            }

            // Try patterns (longest first)
            var matched = false;
            foreach (var (pattern, phonemes) in Patterns)
            {
                if (position + pattern.Length <= normalized.Length &&
                    normalized.Substring(position, pattern.Length) == pattern)
                {
                    result.AddRange(phonemes);
                    position += pattern.Length;
                    matched = true;
                    break;
                }
            }

            if (matched)
                continue;

            // Fall back to single letter
            if (SingleLetters.TryGetValue(c, out var singlePhonemes))
            {
                result.AddRange(singlePhonemes);
            }
            position++;
        }

        var phonemeArray = result.ToArray();
        _cache[word] = phonemeArray;
        return phonemeArray;
    }
}
