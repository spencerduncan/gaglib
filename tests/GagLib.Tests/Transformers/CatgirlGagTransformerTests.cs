using FluentAssertions;
using GagLib.Abstractions;
using GagLib.Transformers;
using Xunit;

namespace GagLib.Tests.Transformers;

public class CatgirlGagTransformerTests
{
    private readonly CatgirlGagTransformer _transformer = new();

    [Fact]
    public void Name_ReturnsCatgirlGag()
    {
        _transformer.Name.Should().Be("Catgirl Gag");
    }

    [Fact]
    public void Transform_EmptyList_ReturnsEmptyString()
    {
        _transformer.Transform([]).Should().BeEmpty();
    }

    [Fact]
    public void Transform_ShortVowel_ReturnsNyaWithTilde()
    {
        // Short vowels (IH, EH, AE, AH, UH, AA, ER) -> nya
        var phonemes = new[] { Phoneme.AH };
        var result = _transformer.Transform(phonemes);

        result.Should().Be("nya~");
    }

    [Fact]
    public void Transform_LongVowel_ReturnsNyaaWithTilde()
    {
        // Long vowels (IY, UW, OW, EY, AY, AW, OY, AO) -> nyaa
        var phonemes = new[] { Phoneme.OW };
        var result = _transformer.Transform(phonemes);

        result.Should().Be("nyaa~");
    }

    [Fact]
    public void Transform_MultipleSyllables_ConcatenatesNyas()
    {
        // "hello" = HH EH L OW -> two syllables (nya + nyaa)
        var phonemes = new[] { Phoneme.HH, Phoneme.EH, Phoneme.L, Phoneme.OW };
        var result = _transformer.Transform(phonemes);

        result.Should().Be("nyanyaa~");
    }

    [Fact]
    public void Transform_OnlyConsonants_ReturnsNya()
    {
        var phonemes = new[] { Phoneme.S, Phoneme.T };
        var result = _transformer.Transform(phonemes);

        result.Should().Be("nya");
    }

    [Fact]
    public void Transform_AlwaysEndsWithTilde()
    {
        var phonemes = new[] { Phoneme.IY, Phoneme.AH, Phoneme.OW };
        var result = _transformer.Transform(phonemes);

        result.Should().EndWith("~");
    }

    [Fact]
    public void TransformSentence_MultipleWords_JoinsWithSpaces()
    {
        var sentence = new List<IReadOnlyList<Phoneme>>
        {
            new[] { Phoneme.AH },  // short vowel -> nya~
            new[] { Phoneme.OW }   // long vowel -> nyaa~
        };

        var result = _transformer.TransformSentence(sentence);

        result.Should().Be("nya~ nyaa~");
    }
}
