using System.Reflection;
using System.Text.RegularExpressions;
using GagLib.Abstractions;

namespace GagLib.Phonemization;

/// <summary>
/// Phonemizer that uses the CMU Pronouncing Dictionary for lookups.
/// </summary>
public partial class CmuDictionaryPhonemizer : IPhonemizer
{
    private readonly Dictionary<string, List<Phoneme>> _dictionary = new(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Initializes a new instance of the <see cref="CmuDictionaryPhonemizer"/> class.
    /// Loads the embedded CMU dictionary.
    /// </summary>
    public CmuDictionaryPhonemizer()
    {
        LoadDictionary();
    }

    /// <summary>
    /// Gets the number of words loaded in the dictionary.
    /// </summary>
    public int WordCount => _dictionary.Count;

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

    private void LoadDictionary()
    {
        var assembly = Assembly.GetExecutingAssembly();
        var resourceName = "GagLib.Resources.cmudict.txt";

        using var stream = assembly.GetManifestResourceStream(resourceName)
            ?? throw new InvalidOperationException($"Could not find embedded resource: {resourceName}");
        using var reader = new StreamReader(stream);

        string? line;
        while ((line = reader.ReadLine()) != null)
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith(";;;"))
                continue;

            ParseLine(line);
        }
    }

    private void ParseLine(string line)
    {
        // Format: "WORD P1 P2 P3" or "WORD(2) P1 P2 P3" for alternates
        var parts = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length < 2)
            return;

        var word = parts[0];

        // Skip alternate pronunciations like "WORD(2)" - keep only primary
        if (AlternatePronunciationRegex().IsMatch(word))
            return;

        var phonemes = new List<Phoneme>();
        for (int i = 1; i < parts.Length; i++)
        {
            if (TryParsePhoneme(parts[i], out var phoneme))
            {
                phonemes.Add(phoneme);
            }
        }

        if (phonemes.Count > 0)
        {
            _dictionary[word] = phonemes;
        }
    }

    private static bool TryParsePhoneme(string phonemeStr, out Phoneme phoneme)
    {
        // Strip stress markers (0, 1, 2) from vowels
        var stripped = StressMarkerRegex().Replace(phonemeStr, "");

        return Enum.TryParse(stripped, ignoreCase: true, out phoneme);
    }

    private static string NormalizeWord(string word)
    {
        // Strip punctuation, normalize case
        var normalized = PunctuationRegex().Replace(word, "");
        return normalized.Trim().ToUpperInvariant();
    }

    [GeneratedRegex(@"\(\d+\)$")]
    private static partial Regex AlternatePronunciationRegex();

    [GeneratedRegex(@"[012]$")]
    private static partial Regex StressMarkerRegex();

    [GeneratedRegex(@"[^\w']")]
    private static partial Regex PunctuationRegex();
}
