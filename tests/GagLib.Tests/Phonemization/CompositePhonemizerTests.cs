using FluentAssertions;
using GagLib.Phonemization;
using Xunit;

namespace GagLib.Tests.Phonemization;

public class CompositePhonemizerTests
{
    [Fact]
    public void CreateDefault_ReturnsWorkingPhonemizer()
    {
        var composite = CompositePhonemizer.CreateDefault();

        composite.Should().NotBeNull();
        composite.CanPhonemize("hello").Should().BeTrue();
    }

    [Fact]
    public void Phonemize_KnownWord_ReturnsDictionaryResult()
    {
        var composite = CompositePhonemizer.CreateDefault();
        var dictionary = new CmuDictionaryPhonemizer();

        var result = composite.Phonemize("hello");

        result.Should().BeEquivalentTo(dictionary.Phonemize("hello"));
    }

    [Fact]
    public void Phonemize_CompoundWord_SplitsViaTrie()
    {
        var composite = CompositePhonemizer.CreateDefault();
        var dictionary = new CmuDictionaryPhonemizer();

        var result = composite.Phonemize("catdog");

        var expected = dictionary.Phonemize("cat").Count + dictionary.Phonemize("dog").Count;
        result.Should().HaveCount(expected);
    }

    [Fact]
    public void CanPhonemize_UnknownWord_FallsBackToHeuristic()
    {
        var composite = CompositePhonemizer.CreateDefault();

        // HeuristicPhonemizer.CanPhonemize returns true for any non-empty string
        composite.CanPhonemize("xyzzy").Should().BeTrue();
    }
}
