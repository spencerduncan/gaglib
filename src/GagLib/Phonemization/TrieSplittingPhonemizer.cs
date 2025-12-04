using GagLib.Abstractions;

namespace GagLib.Phonemization;

/// <summary>
/// Phonemizer that splits unknown words into known dictionary words using a trie.
/// Assumes the word has already been checked against the dictionary directly.
/// </summary>
public class TrieSplittingPhonemizer : IPhonemizer
{
    private readonly CmuDictionaryPhonemizer _dictionary;
    private readonly WordTrie _trie;
    private readonly Dictionary<string, SplitResult> _cache = new(StringComparer.OrdinalIgnoreCase);

    private const int MaxWordLength = 50;

    /// <summary>
    /// Initializes a new instance of the <see cref="TrieSplittingPhonemizer"/> class.
    /// </summary>
    /// <param name="dictionary">The dictionary phonemizer to use for prefix lookups during splitting.</param>
    public TrieSplittingPhonemizer(CmuDictionaryPhonemizer dictionary)
    {
        _dictionary = dictionary;
        _trie = BuildTrie(dictionary);
    }

    /// <inheritdoc />
    public bool CanPhonemize(string word)
    {
        var normalized = NormalizeWord(word);
        if (string.IsNullOrEmpty(normalized))
            return false;

        // Handle hyphens - can phonemize if all parts can be phonemized
        if (normalized.Contains('-'))
        {
            var parts = normalized.Split('-', StringSplitOptions.RemoveEmptyEntries);
            return parts.All(CanPhonemize);
        }

        // Can phonemize if we can split it into known words
        var result = FindBestSplit(normalized);
        return result.Success;
    }

    /// <inheritdoc />
    public IReadOnlyList<Phoneme> Phonemize(string word)
    {
        var normalized = NormalizeWord(word);
        if (string.IsNullOrEmpty(normalized))
            return Array.Empty<Phoneme>();

        // Handle hyphens first - split and process each part
        if (normalized.Contains('-'))
        {
            return PhonemizeHyphenated(normalized);
        }

        // Try splitting into known dictionary words
        var result = FindBestSplit(normalized);
        if (result.Success)
        {
            return result.Phonemes;
        }

        return Array.Empty<Phoneme>();
    }

    /// <inheritdoc />
    public IReadOnlyList<IReadOnlyList<Phoneme>> PhonemizeSentence(string text)
    {
        var words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return words.Select(Phonemize).ToList().AsReadOnly();
    }

    private IReadOnlyList<Phoneme> PhonemizeHyphenated(string word)
    {
        var parts = word.Split('-', StringSplitOptions.RemoveEmptyEntries);
        var allPhonemes = new List<Phoneme>();

        foreach (var part in parts)
        {
            var phonemes = Phonemize(part);
            allPhonemes.AddRange(phonemes);
        }

        return allPhonemes.AsReadOnly();
    }

    private SplitResult FindBestSplit(string text)
    {
        if (string.IsNullOrEmpty(text))
            return SplitResult.Empty;

        if (text.Length > MaxWordLength)
            return SplitResult.Failed;

        // Check cache
        if (_cache.TryGetValue(text, out var cached))
            return cached;

        // Try direct lookup (0 splits = best)
        var direct = _dictionary.Phonemize(text);
        if (direct.Count > 0)
        {
            var result = new SplitResult(direct.ToList(), 1);
            _cache[text] = result;
            return result;
        }

        // Try splitting - find all valid prefixes, longest first
        SplitResult? bestResult = null;

        foreach (var prefix in _trie.FindAllPrefixes(text))
        {
            var prefixPhonemes = _dictionary.Phonemize(prefix);
            if (prefixPhonemes.Count == 0)
                continue;

            var remainder = text[prefix.Length..];

            if (string.IsNullOrEmpty(remainder))
            {
                // Prefix is the whole word
                var result = new SplitResult(prefixPhonemes.ToList(), 1);
                if (bestResult == null || result.SegmentCount < bestResult.Value.SegmentCount)
                {
                    bestResult = result;
                }
            }
            else
            {
                // Recursively split remainder
                var remainderResult = FindBestSplit(remainder);
                if (remainderResult.Success)
                {
                    var combined = prefixPhonemes.Concat(remainderResult.Phonemes).ToList();
                    var result = new SplitResult(combined, 1 + remainderResult.SegmentCount);

                    if (bestResult == null || result.SegmentCount < bestResult.Value.SegmentCount)
                    {
                        bestResult = result;
                    }
                }
            }

            // Optimization: if we found a 2-segment split, that's good enough
            // (1-segment would have been found by direct lookup)
            if (bestResult.HasValue && bestResult.Value.SegmentCount <= 2)
                break;
        }

        var finalResult = bestResult ?? SplitResult.Failed;
        _cache[text] = finalResult;
        return finalResult;
    }

    private static WordTrie BuildTrie(CmuDictionaryPhonemizer dictionary)
    {
        var trie = new WordTrie();

        // We need access to the dictionary's words
        // For now, we'll use reflection or add a method to expose them
        // Actually, let's just rebuild from the same resource
        var assembly = typeof(CmuDictionaryPhonemizer).Assembly;
        var resourceName = "GagLib.Resources.cmudict.txt";

        using var stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            return trie;

        using var reader = new StreamReader(stream);
        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";;;"))
                continue;

            var spaceIndex = line.IndexOf(' ');
            if (spaceIndex <= 0)
                continue;

            var word = line[..spaceIndex];

            // Skip alternate pronunciations
            if (word.Contains('('))
                continue;

            trie.Insert(word);
        }

        return trie;
    }

    private static string NormalizeWord(string word)
    {
        return word.Trim().ToUpperInvariant();
    }

    private readonly struct SplitResult
    {
        public static readonly SplitResult Empty = new(Array.Empty<Phoneme>(), 0);
        public static readonly SplitResult Failed = new(Array.Empty<Phoneme>(), int.MaxValue);

        public IReadOnlyList<Phoneme> Phonemes { get; }
        public int SegmentCount { get; }
        public bool Success => SegmentCount > 0 && SegmentCount < int.MaxValue;

        public SplitResult(IReadOnlyList<Phoneme> phonemes, int segmentCount)
        {
            Phonemes = phonemes;
            SegmentCount = segmentCount;
        }
    }
}
