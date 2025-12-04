using FluentAssertions;
using GagLib.Abstractions;
using GagLib.Phonemization;
using Xunit;

namespace GagLib.Tests.Phonemization;

public class HeuristicPhonemizerTests
{
    private readonly HeuristicPhonemizer _phonemizer = new();

    [Theory]
    [InlineData("cat")]
    [InlineData("xyz")]
    [InlineData("blorp")]
    public void CanPhonemize_AnyWord_ReturnsTrue(string word)
    {
        _phonemizer.CanPhonemize(word).Should().BeTrue();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    public void CanPhonemize_EmptyOrWhitespace_ReturnsFalse(string? word)
    {
        _phonemizer.CanPhonemize(word!).Should().BeFalse();
    }

    [Fact]
    public void Phonemize_SimpleWord_ReturnsPhonemes()
    {
        var result = _phonemizer.Phonemize("cat");

        result.Should().NotBeEmpty();
        result.Should().Contain(Phoneme.K);  // c
        result.Should().Contain(Phoneme.AE); // a
        result.Should().Contain(Phoneme.T);  // t
    }

    [Fact]
    public void Phonemize_Digraph_TH()
    {
        var result = _phonemizer.Phonemize("the");

        result.Should().Contain(Phoneme.TH);
        result.Should().HaveCount(2); // TH + e
    }

    [Fact]
    public void Phonemize_Digraph_SH()
    {
        var result = _phonemizer.Phonemize("ship");

        result.Should().Contain(Phoneme.SH);
        result.Should().Contain(Phoneme.P);
    }

    [Fact]
    public void Phonemize_Digraph_CH()
    {
        var result = _phonemizer.Phonemize("chip");

        result.Should().Contain(Phoneme.CH);
        result.Should().Contain(Phoneme.P);
    }

    [Fact]
    public void Phonemize_VowelDigraph_EE()
    {
        var result = _phonemizer.Phonemize("see");

        result.Should().Contain(Phoneme.S);
        result.Should().Contain(Phoneme.IY);
    }

    [Fact]
    public void Phonemize_VowelDigraph_OO()
    {
        var result = _phonemizer.Phonemize("food");

        result.Should().Contain(Phoneme.F);
        result.Should().Contain(Phoneme.UW);
        result.Should().Contain(Phoneme.D);
    }

    [Fact]
    public void Phonemize_Pattern_TION()
    {
        var result = _phonemizer.Phonemize("nation");

        result.Should().Contain(Phoneme.N);
        result.Should().Contain(Phoneme.SH);  // tion → SH
    }

    [Fact]
    public void Phonemize_Pattern_ING()
    {
        var result = _phonemizer.Phonemize("sing");

        result.Should().Contain(Phoneme.S);
        result.Should().Contain(Phoneme.IH);
        result.Should().Contain(Phoneme.NG);
    }

    [Fact]
    public void Phonemize_SilentLetters_KN()
    {
        var result = _phonemizer.Phonemize("know");

        // kn → N (k silent)
        result.Should().Contain(Phoneme.N);
        result.Should().NotContain(Phoneme.K);
    }

    [Fact]
    public void Phonemize_CaseInsensitive()
    {
        var lower = _phonemizer.Phonemize("cat");
        var upper = _phonemizer.Phonemize("CAT");
        var mixed = _phonemizer.Phonemize("CaT");

        lower.Should().BeEquivalentTo(upper);
        lower.Should().BeEquivalentTo(mixed);
    }

    [Fact]
    public void Phonemize_MadeUpWord_StillProducesPhonemes()
    {
        // Heuristic should handle any letter combination
        var result = _phonemizer.Phonemize("blorphington");

        result.Should().NotBeEmpty();
        result.Should().Contain(Phoneme.B);
        result.Should().Contain(Phoneme.L);
    }

    [Fact]
    public void Phonemize_EmptyString_ReturnsEmpty()
    {
        _phonemizer.Phonemize("").Should().BeEmpty();
    }
}
