using FluentAssertions;
using GagLib.Abstractions;
using GagLib.Transformers;
using Xunit;

namespace GagLib.Tests.Transformers;

public class BarkingDogGagTransformerTests
{
    private readonly BarkingDogGagTransformer _transformer = new();

    [Fact]
    public void Name_ReturnsBarkingDogGag()
    {
        _transformer.Name.Should().Be("Barking Dog Gag");
    }

    [Fact]
    public void Transform_EmptyList_ReturnsEmptyString()
    {
        _transformer.Transform([]).Should().BeEmpty();
    }

    [Fact]
    public void Transform_ShortVowel_ReturnsShortSound()
    {
        // Short vowels (IH, EH, AE, AH, UH, AA, ER) -> ruff/arf
        var phonemes = new[] { Phoneme.AH };
        var result = _transformer.Transform(phonemes);

        (result == "ruff" || result == "arf").Should().BeTrue();
    }

    [Fact]
    public void Transform_LongVowel_ReturnsLongSound()
    {
        // Long vowels (IY, UW, OW, EY, AY, AW, OY, AO) -> woof/bark
        var phonemes = new[] { Phoneme.OW };
        var result = _transformer.Transform(phonemes);

        (result == "woof" || result == "bark").Should().BeTrue();
    }

    [Fact]
    public void Transform_MultipleSyllables_AlternatesSounds()
    {
        // "hello" = HH EH L OW -> two syllables
        var phonemes = new[] { Phoneme.HH, Phoneme.EH, Phoneme.L, Phoneme.OW };
        var result = _transformer.Transform(phonemes);

        // Should have two sounds (short + long)
        result.Should().NotBeEmpty();
        result.Length.Should().BeGreaterThan(4); // at least two sounds
    }

    [Fact]
    public void Transform_OnlyConsonants_ReturnsRuff()
    {
        var phonemes = new[] { Phoneme.S, Phoneme.T };
        var result = _transformer.Transform(phonemes);

        result.Should().Be("ruff");
    }

    [Fact]
    public void Transform_GrowlyConsonant_AddsGrr()
    {
        // Word with R consonant should add grr
        var phonemes = new[] { Phoneme.G, Phoneme.R, Phoneme.EY, Phoneme.T };
        var result = _transformer.Transform(phonemes);

        result.Should().Contain("grr");
    }

    [Fact]
    public void Transform_NoGrowlyConsonant_NoGrr()
    {
        // Word without R, G, K should not have grr
        var phonemes = new[] { Phoneme.HH, Phoneme.EH, Phoneme.L, Phoneme.OW };
        var result = _transformer.Transform(phonemes);

        result.Should().NotContain("grr");
    }

    [Fact]
    public void TransformSentence_MultipleWords_JoinsWithSpaces()
    {
        var sentence = new List<IReadOnlyList<Phoneme>>
        {
            new[] { Phoneme.AH },  // short vowel
            new[] { Phoneme.OW }   // long vowel
        };

        var result = _transformer.TransformSentence(sentence);

        result.Should().Contain(" ");
        var parts = result.Split(' ');
        parts.Length.Should().Be(2);
    }
}
