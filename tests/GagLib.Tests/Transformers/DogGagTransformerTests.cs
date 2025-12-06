using FluentAssertions;
using GagLib.Abstractions;
using GagLib.Transformers;
using Xunit;

namespace GagLib.Tests.Transformers;

public class DogGagTransformerTests
{
    private readonly DogGagTransformer _transformer = new();

    [Fact]
    public void Name_ReturnsDogGag()
    {
        _transformer.Name.Should().Be("Dog Gag");
    }

    [Fact]
    public void Transform_EmptyList_ReturnsEmptyString()
    {
        _transformer.Transform([]).Should().BeEmpty();
    }

    [Fact]
    public void Transform_VowelStart_PrependsR()
    {
        // "uh" = AH -> ruh
        var phonemes = new[] { Phoneme.AH };
        var result = _transformer.Transform(phonemes);
        result.Should().StartWith("r");
        result.Should().Be("ruh");
    }

    [Fact]
    public void Transform_ConsonantStart_ReplacesWithR()
    {
        // "hello" = HH AH L OW -> r + uh + l + oh = ruhloh
        var phonemes = new[] { Phoneme.HH, Phoneme.AH, Phoneme.L, Phoneme.OW };
        var result = _transformer.Transform(phonemes);
        result.Should().StartWith("r");
        result.Should().NotStartWith("h");
    }

    [Fact]
    public void Transform_UhOh_ReturnsRuhRoh()
    {
        // "uh" = AH, "oh" = OW
        var uh = _transformer.Transform([Phoneme.AH]);
        var oh = _transformer.Transform([Phoneme.OW]);

        uh.Should().Be("ruh");
        oh.Should().Be("roh");
    }

    [Fact]
    public void Transform_ConsonantCluster_ReplacedWithR()
    {
        // "shaggy" = SH AE G IY -> r + a + g + ee
        var phonemes = new[] { Phoneme.SH, Phoneme.AE, Phoneme.G, Phoneme.IY };
        var result = _transformer.Transform(phonemes);
        result.Should().StartWith("r");
        result.Should().Contain("a");
    }

    [Fact]
    public void Transform_OnlyConsonants_ReturnsR()
    {
        var phonemes = new[] { Phoneme.S, Phoneme.T };
        var result = _transformer.Transform(phonemes);
        result.Should().Be("r");
    }

    [Fact]
    public void TransformSentence_MultipleWords_JoinsWithSpaces()
    {
        var sentence = new List<IReadOnlyList<Phoneme>>
        {
            new[] { Phoneme.AH },  // uh -> ruh
            new[] { Phoneme.OW }   // oh -> roh
        };

        var result = _transformer.TransformSentence(sentence);

        result.Should().Be("ruh roh");
    }
}
