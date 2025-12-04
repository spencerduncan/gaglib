using FluentAssertions;
using GagLib.Phonemization;
using Xunit;

namespace GagLib.Tests.Phonemization;

public class TrieSplittingPhonemizerTests
{
    private readonly CmuDictionaryPhonemizer _dictionary;
    private readonly TrieSplittingPhonemizer _splitter;

    public TrieSplittingPhonemizerTests()
    {
        _dictionary = new CmuDictionaryPhonemizer();
        _splitter = new TrieSplittingPhonemizer(_dictionary);
    }

    [Fact]
    public void Phonemize_KnownWord_ReturnsDictionaryResult()
    {
        var result = _splitter.Phonemize("hello");

        result.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(_dictionary.Phonemize("hello"));
    }

    [Fact]
    public void Phonemize_CompoundWord_SplitsCorrectly()
    {
        // "catdog" = "cat" + "dog"
        var catPhonemes = _dictionary.Phonemize("cat");
        var dogPhonemes = _dictionary.Phonemize("dog");

        var result = _splitter.Phonemize("catdog");

        result.Should().HaveCount(catPhonemes.Count + dogPhonemes.Count);
    }

    [Fact]
    public void Phonemize_PrefersDictionaryOverSplit()
    {
        // "together" is in the dictionary - should NOT split into "to" + "get" + "her"
        var directLookup = _dictionary.Phonemize("together");

        var result = _splitter.Phonemize("together");

        result.Should().BeEquivalentTo(directLookup);
    }

    [Fact]
    public void Phonemize_HyphenatedWord_SplitsOnHyphen()
    {
        var wellPhonemes = _dictionary.Phonemize("well");
        var knownPhonemes = _dictionary.Phonemize("known");

        var result = _splitter.Phonemize("well-known");

        result.Should().HaveCount(wellPhonemes.Count + knownPhonemes.Count);
    }

    [Fact]
    public void CanPhonemize_KnownWord_ReturnsTrue()
    {
        _splitter.CanPhonemize("hello").Should().BeTrue();
    }

    [Fact]
    public void CanPhonemize_SplittableWord_ReturnsTrue()
    {
        _splitter.CanPhonemize("catdog").Should().BeTrue();
    }

    [Fact]
    public void CanPhonemize_Punctuation_ReturnsFalse()
    {
        _splitter.CanPhonemize("'").Should().BeFalse();
    }

    [Fact]
    public void Phonemize_Gibberish_SplitsIntoLetters()
    {
        // Single letters are in the dictionary, so gibberish gets split letter by letter
        var result = _splitter.Phonemize("xyz");

        result.Should().NotBeEmpty();
    }

    [Fact]
    public void Phonemize_Punctuation_ReturnsEmpty()
    {
        var result = _splitter.Phonemize("'");

        result.Should().BeEmpty();
    }

    [Fact]
    public void Phonemize_CachesResults()
    {
        var result1 = _splitter.Phonemize("catdog");
        var result2 = _splitter.Phonemize("catdog");

        result1.Should().BeEquivalentTo(result2);
    }
}
