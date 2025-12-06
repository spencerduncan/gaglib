using FluentAssertions;
using GagLib.Abstractions;
using GagLib.Transformers;
using Xunit;

namespace GagLib.Tests.Transformers;

public class CowGagTransformerTests
{
    private readonly CowGagTransformer _transformer = new();

    [Fact]
    public void Name_ReturnsCowGag()
    {
        _transformer.Name.Should().Be("Cow Gag");
    }

    [Fact]
    public void Transform_EmptyList_ReturnsEmptyString()
    {
        _transformer.Transform([]).Should().BeEmpty();
    }

    [Fact]
    public void Transform_SingleShortVowel_ReturnsMoo()
    {
        // Short vowel + final extension
        var phonemes = new[] { Phoneme.AH };
        var result = _transformer.Transform(phonemes);
        result.Should().Be("mooo"); // moo + o
    }

    [Fact]
    public void Transform_SingleLongVowel_ReturnsMooo()
    {
        // Long vowel + final extension
        var phonemes = new[] { Phoneme.OW };
        var result = _transformer.Transform(phonemes);
        result.Should().Be("moooo"); // mooo + o
    }

    [Fact]
    public void Transform_TwoVowels_TwoSyllables()
    {
        // "hello" = HH AH L OW (short + long)
        var phonemes = new[] { Phoneme.HH, Phoneme.AH, Phoneme.L, Phoneme.OW };
        var result = _transformer.Transform(phonemes);
        result.Should().Be("moomoooo"); // moo + mooo + o
    }

    [Fact]
    public void Transform_ThreeVowels_ThreeSyllables()
    {
        // "beautiful" = B Y UW T IH F AH L (long + short + short)
        var phonemes = new[] { Phoneme.B, Phoneme.Y, Phoneme.UW, Phoneme.T, Phoneme.IH, Phoneme.F, Phoneme.AH, Phoneme.L };
        var result = _transformer.Transform(phonemes);
        result.Should().Be("mooomoomooo"); // mooo + moo + moo + o
    }

    [Fact]
    public void Transform_OnlyConsonants_ReturnsMoo()
    {
        // No vowels - fallback to single moo
        var phonemes = new[] { Phoneme.B, Phoneme.L, Phoneme.T };
        var result = _transformer.Transform(phonemes);
        result.Should().Be("moo");
    }

    [Fact]
    public void Transform_Diphthongs_AreLong()
    {
        // AY, AW, OY should all be long
        _transformer.Transform([Phoneme.AY]).Should().Be("moooo");
        _transformer.Transform([Phoneme.AW]).Should().Be("moooo");
        _transformer.Transform([Phoneme.OY]).Should().Be("moooo");
    }

    [Fact]
    public void TransformSentence_MultipleWords_JoinsWithSpaces()
    {
        var sentence = new List<IReadOnlyList<Phoneme>>
        {
            new[] { Phoneme.AH },  // short
            new[] { Phoneme.OW }   // long
        };

        var result = _transformer.TransformSentence(sentence);

        result.Should().Contain(" ");
        var words = result.Split(' ');
        words.Should().HaveCount(2);
    }
}
