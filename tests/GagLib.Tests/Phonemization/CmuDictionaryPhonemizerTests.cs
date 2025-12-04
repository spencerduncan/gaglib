using FluentAssertions;
using GagLib.Abstractions;
using GagLib.Phonemization;
using Xunit;

namespace GagLib.Tests.Phonemization;

public class CmuDictionaryPhonemizerTests
{
    [Fact]
    public void Constructor_LoadsDictionary_WithManyWords()
    {
        var phonemizer = new CmuDictionaryPhonemizer();

        phonemizer.WordCount.Should().BeGreaterThan(100000);
    }

    [Fact]
    public void Phonemize_WithKnownWord_ReturnsPhonemes()
    {
        var phonemizer = new CmuDictionaryPhonemizer();

        var result = phonemizer.Phonemize("hello");

        result.Should().NotBeEmpty();
        result.Should().BeEquivalentTo(new[]
        {
            Phoneme.HH, Phoneme.AH, Phoneme.L, Phoneme.OW
        });
    }

    [Fact]
    public void Phonemize_WithUnknownWord_ReturnsEmpty()
    {
        var phonemizer = new CmuDictionaryPhonemizer();

        var result = phonemizer.Phonemize("xyzzyplugh");

        result.Should().BeEmpty();
    }

    [Fact]
    public void Phonemize_IsCaseInsensitive()
    {
        var phonemizer = new CmuDictionaryPhonemizer();

        var lower = phonemizer.Phonemize("hello");
        var upper = phonemizer.Phonemize("HELLO");
        var mixed = phonemizer.Phonemize("HeLLo");

        lower.Should().BeEquivalentTo(upper);
        lower.Should().BeEquivalentTo(mixed);
    }

    [Fact]
    public void CanPhonemize_WithKnownWord_ReturnsTrue()
    {
        var phonemizer = new CmuDictionaryPhonemizer();

        phonemizer.CanPhonemize("hello").Should().BeTrue();
    }

    [Fact]
    public void CanPhonemize_WithUnknownWord_ReturnsFalse()
    {
        var phonemizer = new CmuDictionaryPhonemizer();

        phonemizer.CanPhonemize("xyzzyplugh").Should().BeFalse();
    }

    [Fact]
    public void Phonemize_StripsPunctuation()
    {
        var phonemizer = new CmuDictionaryPhonemizer();

        var withPunctuation = phonemizer.Phonemize("hello!");
        var without = phonemizer.Phonemize("hello");

        withPunctuation.Should().BeEquivalentTo(without);
    }
}
