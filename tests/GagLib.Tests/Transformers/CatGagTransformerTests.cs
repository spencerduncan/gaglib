using FluentAssertions;
using GagLib.Abstractions;
using GagLib.Transformers;
using Xunit;

namespace GagLib.Tests.Transformers;

public class CatGagTransformerTests
{
    private readonly CatGagTransformer _transformer = new();

    [Fact]
    public void Name_ReturnsCatGag()
    {
        _transformer.Name.Should().Be("Cat Gag");
    }

    [Fact]
    public void Transform_EmptyList_ReturnsEmptyString()
    {
        _transformer.Transform([]).Should().BeEmpty();
    }

    [Fact]
    public void Transform_ShortVowel_ReturnsMeow()
    {
        // Single short vowel should produce "meow" with emphasis
        var phonemes = new[] { Phoneme.AH };
        var result = _transformer.Transform(phonemes);

        result.Should().Contain("meow");
    }

    [Fact]
    public void Transform_LongVowel_ReturnsMeoww()
    {
        // Long vowel should produce extended "meoww"
        var phonemes = new[] { Phoneme.IY };
        var result = _transformer.Transform(phonemes);

        result.Should().Contain("meoww");
    }

    [Fact]
    public void Transform_MultipleSyllables_ReturnsMultipleMeows()
    {
        // "hello" = HH-AH-L-OW
        var phonemes = new[] { Phoneme.HH, Phoneme.AH, Phoneme.L, Phoneme.OW };
        var result = _transformer.Transform(phonemes);

        // Should have two meows (one for AH, one for OW)
        var meowCount = (result.Length - result.Replace("meow", "").Length) / 4;
        meowCount.Should().Be(2);
    }

    [Fact]
    public void Transform_OnlyConsonants_ReturnsSingleMeow()
    {
        // No vowels should still produce a meow
        var phonemes = new[] { Phoneme.K, Phoneme.T };
        var result = _transformer.Transform(phonemes);

        result.Should().Be("meow");
    }

    [Fact]
    public void Transform_ShortVowel_EndsWithSingleW()
    {
        // Short vowel produces "meow" with single w
        var phonemes = new[] { Phoneme.AH };
        var result = _transformer.Transform(phonemes);

        result.Should().Be("meow");
    }

    [Fact]
    public void Transform_LongVowel_EndsWithDoubleW()
    {
        // Long vowel produces "meoww" with double w
        var phonemes = new[] { Phoneme.IY };
        var result = _transformer.Transform(phonemes);

        result.Should().Be("meoww");
    }

    [Fact]
    public void TransformSentence_JoinsWithSpaces()
    {
        var sentence = new List<IReadOnlyList<Phoneme>>
        {
            new[] { Phoneme.AH },
            new[] { Phoneme.IY }
        };

        var result = _transformer.TransformSentence(sentence);

        result.Should().Contain(" ");
        result.Split(' ').Should().HaveCount(2);
    }
}
