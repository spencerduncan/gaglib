using FluentAssertions;
using GagLib.Abstractions;
using GagLib.Transformers;
using Xunit;

namespace GagLib.Tests.Transformers;

public class CatgirlGagTransformerTests
{
    // Use seeded random for deterministic tests
    private readonly CatgirlGagTransformer _transformer = new(new Random(42));

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
    public void Transform_MagicPattern_BecomesNya()
    {
        // M + A-type vowel at start → "nya" (magic → nyagic)
        var phonemes = new[] { Phoneme.M, Phoneme.AE, Phoneme.JH, Phoneme.IH, Phoneme.K };
        var result = _transformer.Transform(phonemes);

        result.Should().StartWith("nya");
    }

    [Fact]
    public void Transform_YouPattern_BecomesMew()
    {
        // Y + U-type vowel → "mew" (you → mew)
        var phonemes = new[] { Phoneme.Y, Phoneme.UW };
        var result = _transformer.Transform(phonemes);

        result.Should().Contain("mew");
    }

    [Fact]
    public void Transform_ApplePattern_NyaPrefix()
    {
        // Word starting with A-type vowel → "nya" prefix
        var phonemes = new[] { Phoneme.AE, Phoneme.P, Phoneme.AH, Phoneme.L };
        var result = _transformer.Transform(phonemes);

        result.Should().StartWith("nya");
    }

    [Fact]
    public void Transform_NothingPattern_NyVowel()
    {
        // N + vowel → "ny" + vowel sound
        var phonemes = new[] { Phoneme.N, Phoneme.AH, Phoneme.TH, Phoneme.IH, Phoneme.NG };
        var result = _transformer.Transform(phonemes);

        result.Should().StartWith("ny");
    }

    [Fact]
    public void Transform_NoTriggers_KeepsStructure()
    {
        // Words without triggers keep their structure
        var phonemes = new[] { Phoneme.W, Phoneme.ER, Phoneme.L, Phoneme.D };
        var result = _transformer.Transform(phonemes);

        // Should be phonetic approximation, not heavily transformed
        result.Should().Contain("er");
        result.Should().Contain("l");
    }

    [Fact]
    public void Transform_PreservesConsonants()
    {
        // Non-triggering consonants are preserved
        var phonemes = new[] { Phoneme.K, Phoneme.AE, Phoneme.T };
        var result = _transformer.Transform(phonemes);

        result.Should().Contain("k");
        result.Should().Contain("t");
    }

    [Fact]
    public void TransformSentence_JoinsWithSpaces()
    {
        var sentence = new List<IReadOnlyList<Phoneme>>
        {
            new[] { Phoneme.AE },  // A-type vowel
            new[] { Phoneme.Y, Phoneme.UW }  // you → mew
        };

        var result = _transformer.TransformSentence(sentence);

        result.Should().Contain(" ");
        result.Should().Contain("nya");
        result.Should().Contain("mew");
    }
}
