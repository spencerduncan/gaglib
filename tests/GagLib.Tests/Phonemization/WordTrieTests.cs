using FluentAssertions;
using GagLib.Phonemization;
using Xunit;

namespace GagLib.Tests.Phonemization;

public class WordTrieTests
{
    [Fact]
    public void Insert_And_Contains_Work()
    {
        var trie = new WordTrie();

        trie.Insert("hello");
        trie.Insert("world");

        trie.Contains("hello").Should().BeTrue();
        trie.Contains("world").Should().BeTrue();
        trie.Contains("foo").Should().BeFalse();
    }

    [Fact]
    public void Contains_IsCaseInsensitive()
    {
        var trie = new WordTrie();
        trie.Insert("Hello");

        trie.Contains("hello").Should().BeTrue();
        trie.Contains("HELLO").Should().BeTrue();
        trie.Contains("HeLLo").Should().BeTrue();
    }

    [Fact]
    public void Count_TracksInsertedWords()
    {
        var trie = new WordTrie();

        trie.Count.Should().Be(0);

        trie.Insert("one");
        trie.Count.Should().Be(1);

        trie.Insert("two");
        trie.Count.Should().Be(2);

        // Duplicate insert should not increase count
        trie.Insert("one");
        trie.Count.Should().Be(2);
    }

    [Fact]
    public void HasPrefix_ReturnsTrueForValidPrefixes()
    {
        var trie = new WordTrie();
        trie.Insert("hello");
        trie.Insert("help");
        trie.Insert("helicopter");

        trie.HasPrefix("hel").Should().BeTrue();
        trie.HasPrefix("help").Should().BeTrue();
        trie.HasPrefix("hello").Should().BeTrue();
        trie.HasPrefix("h").Should().BeTrue();
        trie.HasPrefix("").Should().BeTrue();  // Empty prefix matches anything
    }

    [Fact]
    public void HasPrefix_ReturnsFalseForInvalidPrefixes()
    {
        var trie = new WordTrie();
        trie.Insert("hello");

        trie.HasPrefix("world").Should().BeFalse();
        trie.HasPrefix("hx").Should().BeFalse();
    }

    [Fact]
    public void FindLongestPrefix_ReturnsLongestWordThatIsPrefixOfText()
    {
        var trie = new WordTrie();
        trie.Insert("sun");
        trie.Insert("sunflower");
        trie.Insert("sunny");

        // "sunflower" is in the trie and matches exactly
        trie.FindLongestPrefix("sunflower").Should().Be("SUNFLOWER");

        // "sunflowers" - longest prefix word is "sunflower"
        trie.FindLongestPrefix("sunflowers").Should().Be("SUNFLOWER");

        // "sunshine" - longest prefix word is "sun" (sunny doesn't match)
        trie.FindLongestPrefix("sunshine").Should().Be("SUN");
    }

    [Fact]
    public void FindLongestPrefix_ReturnsNullWhenNoMatch()
    {
        var trie = new WordTrie();
        trie.Insert("hello");

        trie.FindLongestPrefix("world").Should().BeNull();
        trie.FindLongestPrefix("").Should().BeNull();
    }

    [Fact]
    public void FindAllPrefixes_ReturnsAllMatchingWordsLongestFirst()
    {
        var trie = new WordTrie();
        trie.Insert("a");
        trie.Insert("an");
        trie.Insert("ant");
        trie.Insert("ante");

        var prefixes = trie.FindAllPrefixes("anteater").ToList();

        prefixes.Should().HaveCount(4);
        prefixes[0].Should().Be("ANTE");  // Longest first
        prefixes[1].Should().Be("ANT");
        prefixes[2].Should().Be("AN");
        prefixes[3].Should().Be("A");
    }

    [Fact]
    public void Insert_EmptyString_IsIgnored()
    {
        var trie = new WordTrie();

        trie.Insert("");
        trie.Insert(null!);

        trie.Count.Should().Be(0);
    }
}
