using FluentAssertions;
using GagLib.Abstractions;
using GagLib.Transformers;
using Xunit;

namespace GagLib.Tests.Transformers;

public class BallGagTransformerTests
{
    private readonly BallGagTransformer _transformer = new();

    [Fact]
    public void Name_ReturnsBallGag()
    {
        _transformer.Name.Should().Be("Ball Gag");
    }

    [Fact]
    public void Transform_EmptyList_ReturnsEmptyString()
    {
        _transformer.Transform([]).Should().BeEmpty();
    }

    [Fact]
    public void Transform_Nasals_PassThrough()
    {
        // Nasals come through the nose relatively intact
        var phonemes = new[] { Phoneme.M, Phoneme.N, Phoneme.NG };

        var result = _transformer.Transform(phonemes);

        result.Should().Contain("m");
        result.Should().Contain("n");
        result.Should().Contain("ng");
    }

    [Fact]
    public void Transform_VoicedStops_BecomeNasal()
    {
        // B, D, G can't form properly with mouth blocked
        var phonemes = new[] { Phoneme.B, Phoneme.D, Phoneme.G };

        var result = _transformer.Transform(phonemes);

        result.Should().Contain("m");   // B → mm
        result.Should().Contain("n");   // D → nn
        result.Should().Contain("ngh"); // G → ngh
    }

    [Fact]
    public void Transform_VoicelessStops_BecomeMuffled()
    {
        var phonemes = new[] { Phoneme.P, Phoneme.T, Phoneme.K };

        var result = _transformer.Transform(phonemes);

        result.Should().Contain("mph"); // P
        result.Should().Contain("th");  // T
        result.Should().Contain("kh");  // K
    }

    [Fact]
    public void Transform_Vowels_BecomeMuffled()
    {
        // "hello" phonemes: HH AH L OW
        var phonemes = new[] { Phoneme.HH, Phoneme.AH, Phoneme.L, Phoneme.OW };

        var result = _transformer.Transform(phonemes);

        result.Should().NotBeEmpty();
        result.Should().Contain("uh"); // AH
        result.Should().Contain("oh"); // OW
    }

    [Fact]
    public void Transform_SimplifiesRepeatedCharacters()
    {
        // Multiple M phonemes shouldn't produce "mmmmmm"
        var phonemes = new[] { Phoneme.M, Phoneme.M, Phoneme.M, Phoneme.M, Phoneme.M };

        var result = _transformer.Transform(phonemes);

        // Should cap at 3 repeated chars
        result.Should().Be("mmm");
    }

    [Fact]
    public void Transform_Hello_ProducesMuffledOutput()
    {
        // HELLO: HH AH L OW
        var phonemes = new[] { Phoneme.HH, Phoneme.AH, Phoneme.L, Phoneme.OW };

        var result = _transformer.Transform(phonemes);

        result.Should().NotBeEmpty();
        result.Length.Should().BeGreaterThan(2);
    }

    [Fact]
    public void Transform_World_ProducesMuffledOutput()
    {
        // WORLD: W ER L D
        var phonemes = new[] { Phoneme.W, Phoneme.ER, Phoneme.L, Phoneme.D };

        var result = _transformer.Transform(phonemes);

        result.Should().NotBeEmpty();
        result.Should().Contain("ww"); // W
        result.Should().Contain("rr"); // ER
    }

    [Fact]
    public void TransformSentence_MultipleWords_JoinsWithSpaces()
    {
        var sentence = new List<IReadOnlyList<Phoneme>>
        {
            new[] { Phoneme.HH, Phoneme.AY },           // "hi"
            new[] { Phoneme.DH, Phoneme.EH, Phoneme.R } // "there"
        };

        var result = _transformer.TransformSentence(sentence);

        result.Should().Contain(" ");
        var words = result.Split(' ');
        words.Should().HaveCount(2);
    }

    [Fact]
    public void TransformSentence_EmptyWords_HandledGracefully()
    {
        var sentence = new List<IReadOnlyList<Phoneme>>
        {
            new[] { Phoneme.M },
            Array.Empty<Phoneme>(),
            new[] { Phoneme.N }
        };

        var result = _transformer.TransformSentence(sentence);

        result.Should().NotBeNull();
    }

    [Fact]
    public void Transform_AllPhonemesCovered()
    {
        // Every phoneme should produce some output (no crashes, no gaps)
        foreach (var phoneme in Enum.GetValues<Phoneme>())
        {
            var result = _transformer.Transform([phoneme]);
            result.Should().NotBeEmpty($"Phoneme {phoneme} should produce output");
        }
    }

    [Fact]
    public void Transform_Fricatives_PartiallyEscape()
    {
        // Some air escapes past the gag for fricatives
        var phonemes = new[] { Phoneme.F, Phoneme.S, Phoneme.SH };

        var result = _transformer.Transform(phonemes);

        result.Should().Contain("ff"); // F
        result.Should().Contain("sh"); // SH
    }
}
