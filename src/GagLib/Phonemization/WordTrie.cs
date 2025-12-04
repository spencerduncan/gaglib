namespace GagLib.Phonemization;

/// <summary>
/// A trie data structure for efficient word prefix lookups.
/// </summary>
public class WordTrie
{
    private readonly TrieNode _root = new();

    /// <summary>
    /// Gets the number of words in the trie.
    /// </summary>
    public int Count { get; private set; }

    /// <summary>
    /// Inserts a word into the trie.
    /// </summary>
    /// <param name="word">The word to insert.</param>
    public void Insert(string word)
    {
        if (string.IsNullOrEmpty(word))
            return;

        var node = _root;
        foreach (var c in word.ToUpperInvariant())
        {
            node = node.GetOrAddChild(c);
        }

        if (!node.IsEndOfWord)
        {
            node.IsEndOfWord = true;
            Count++;
        }
    }

    /// <summary>
    /// Checks if the trie contains the exact word.
    /// </summary>
    /// <param name="word">The word to check.</param>
    /// <returns>True if the word exists in the trie.</returns>
    public bool Contains(string word)
    {
        if (string.IsNullOrEmpty(word))
            return false;

        var node = FindNode(word);
        return node?.IsEndOfWord ?? false;
    }

    /// <summary>
    /// Checks if any word in the trie starts with the given prefix.
    /// </summary>
    /// <param name="prefix">The prefix to check.</param>
    /// <returns>True if any word starts with the prefix.</returns>
    public bool HasPrefix(string prefix)
    {
        if (string.IsNullOrEmpty(prefix))
            return true;

        return FindNode(prefix) != null;
    }

    /// <summary>
    /// Finds the longest word in the trie that is a prefix of the given text.
    /// </summary>
    /// <param name="text">The text to find a prefix word for.</param>
    /// <returns>The longest prefix word, or null if none found.</returns>
    public string? FindLongestPrefix(string text)
    {
        if (string.IsNullOrEmpty(text))
            return null;

        var normalized = text.ToUpperInvariant();
        var node = _root;
        string? longestWord = null;
        var currentPrefix = new System.Text.StringBuilder();

        foreach (var c in normalized)
        {
            var child = node.GetChild(c);
            if (child == null)
                break;

            currentPrefix.Append(c);
            node = child;

            if (node.IsEndOfWord)
            {
                longestWord = currentPrefix.ToString();
            }
        }

        return longestWord;
    }

    /// <summary>
    /// Finds all words in the trie that are prefixes of the given text, ordered longest first.
    /// </summary>
    /// <param name="text">The text to find prefix words for.</param>
    /// <returns>All prefix words, longest first.</returns>
    public IEnumerable<string> FindAllPrefixes(string text)
    {
        if (string.IsNullOrEmpty(text))
            yield break;

        var normalized = text.ToUpperInvariant();
        var node = _root;
        List<string> prefixes = [];
        var currentPrefix = new System.Text.StringBuilder();

        foreach (var c in normalized)
        {
            var child = node.GetChild(c);
            if (child == null)
                break;

            currentPrefix.Append(c);
            node = child;

            if (node.IsEndOfWord)
            {
                prefixes.Add(currentPrefix.ToString());
            }
        }

        // Return longest first
        for (int i = prefixes.Count - 1; i >= 0; i--)
        {
            yield return prefixes[i];
        }
    }

    private TrieNode? FindNode(string text)
    {
        var normalized = text.ToUpperInvariant();
        var node = _root;

        foreach (var c in normalized)
        {
            var child = node.GetChild(c);
            if (child == null)
                return null;
            node = child;
        }

        return node;
    }

    private class TrieNode
    {
        private readonly Dictionary<char, TrieNode> _children = new();

        public bool IsEndOfWord { get; set; }

        public TrieNode GetOrAddChild(char c)
        {
            if (!_children.TryGetValue(c, out var child))
            {
                child = new TrieNode();
                _children[c] = child;
            }
            return child;
        }

        public TrieNode? GetChild(char c)
        {
            return _children.GetValueOrDefault(c);
        }
    }
}
