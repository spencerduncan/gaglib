using FluentAssertions;
using GagLib.Transformers;
using Xunit;

namespace GagLib.Tests.Transformers;

public class FurryGagTransformerTests
{
    // Use seeded random for deterministic tests
    private readonly FurryGagTransformer _transformer = new(new Random(42));

    [Fact]
    public void Name_ReturnsFurryGag()
    {
        _transformer.Name.Should().Be("Furry Gag");
    }

    [Theory]
    [InlineData("you", "chu")]
    [InlineData("You", "Chu")]
    [InlineData("YOU", "CHU")]
    [InlineData("your", "ur")]
    [InlineData("the", "teh")]
    [InlineData("this", "dis")]
    [InlineData("love", "wuv")]
    [InlineData("for", "fur")]
    [InlineData("not", "knot")]
    [InlineData("with", "wif")]
    [InlineData("what", "wat")]
    [InlineData("hi", "hai")]
    [InlineData("bye", "bai")]
    [InlineData("hello", "hewwo")]
    [InlineData("please", "pwease")]
    [InlineData("pretty", "pwetty")]
    [InlineData("little", "wittle")]
    public void TransformWord_WordSubstitutions_ReplacesCorrectly(string input, string expected)
    {
        var result = _transformer.TransformWord(input, null);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("never", "nyevew")]
    [InlineData("no", "nyo")]
    [InlineData("know", "knyow")]
    public void TransformWord_NBeforeVowel_AddsY(string input, string expected)
    {
        var result = _transformer.TransformWord(input, null);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("run", "wun")]
    [InlineData("really", "weawwy")]
    [InlineData("are", "awe")]
    public void TransformWord_RToW_TransformsCorrectly(string input, string expected)
    {
        var result = _transformer.TransformWord(input, null);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("like", "wike")]
    [InlineData("let", "wet")]
    [InlineData("long", "wong")]
    public void TransformWord_LToW_TransformsCorrectly(string input, string expected)
    {
        var result = _transformer.TransformWord(input, null);
        result.Should().Be(expected);
    }

    [Theory]
    [InlineData("that", "fat")]
    [InlineData("there", "fewe")]
    public void TransformWord_ThToF_TransformsCorrectly(string input, string expected)
    {
        var result = _transformer.TransformWord(input, null);
        result.Should().Be(expected);
    }

    [Fact]
    public void TransformWord_PreservesCase()
    {
        _transformer.TransformWord("LOVE", null).Should().Be("WUV");
        _transformer.TransformWord("Love", null).Should().Be("Wuv");
        _transformer.TransformWord("love", null).Should().Be("wuv");
    }

    [Fact]
    public void TransformWord_EmptyString_ReturnsEmpty()
    {
        _transformer.TransformWord("", null).Should().BeEmpty();
    }

    [Fact]
    public void TransformSentenceText_JoinsWithSpaces()
    {
        var words = new List<(string, IReadOnlyList<GagLib.Abstractions.Phoneme>?)>
        {
            ("hello", null),
            ("world", null)
        };

        var result = _transformer.TransformSentenceText(words);

        // Should contain transformed words
        // "hello" → "hewwo" (l before vowel → w)
        // "world" → "wowld" (r→w, but l before d stays l)
        result.Should().Contain("hewwo");
        result.Should().Contain("wowld");
    }

    [Fact]
    public void TransformSentenceText_CanAddSuffixes()
    {
        // Run multiple times to check for suffix additions (uwu, owo, :3, etc.)
        var hasSuffix = false;
        var transformer = new FurryGagTransformer(new Random(12345));
        var suffixes = new[] { "~", "uwu", "owo", ":3", "X3", ">:3", "^w^" };

        for (var i = 0; i < 50; i++)
        {
            var words = new List<(string, IReadOnlyList<GagLib.Abstractions.Phoneme>?)>
            {
                ("I", null),
                ("love", null),
                ("you", null),
                ("so", null),
                ("much", null)
            };

            var result = transformer.TransformSentenceText(words);

            if (suffixes.Any(s => result.Contains(s)))
            {
                hasSuffix = true;
                break;
            }
        }

        hasSuffix.Should().BeTrue("Furry output should sometimes include uwu/owo/:3 suffixes");
    }

    [Fact]
    public void TransformWord_CombinesMultipleTransformations()
    {
        // "really" should have r->w, l->w
        var result = _transformer.TransformWord("really", null);
        result.Should().Be("weawwy");
    }

    [Theory]
    [InlineData("We're", "We'we")]
    [InlineData("don't", "don't")]
    [InlineData("can't", "can't")]
    public void TransformWord_HandlesContractions(string input, string expected)
    {
        // Contractions: N before apostrophe doesn't become NY (only before vowels)
        var result = _transformer.TransformWord(input, null);
        result.Should().Be(expected);
    }
}
