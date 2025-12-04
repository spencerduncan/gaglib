using FluentAssertions;
using GagLib.Abstractions;
using GagLib.TextProcessing;
using Xunit;

namespace GagLib.Tests.TextProcessing;

public class TextProcessorTests
{
    [Fact]
    public void Process_WordTokens_GetPhonemes()
    {
        var processor = TextProcessor.CreateDefault();

        var result = processor.Process("hello");

        result.Should().HaveCount(1);
        result[0].Type.Should().Be(TextTokenType.Word);
        result[0].Phonemes.Should().NotBeNull();
        result[0].Phonemes.Should().NotBeEmpty();
    }

    [Fact]
    public void Process_PreservedTokens_NoPhonemes()
    {
        var processor = TextProcessor.CreateDefault();

        var result = processor.Process("<@123>");

        result.Should().HaveCount(1);
        result[0].Type.Should().Be(TextTokenType.Preserved);
        result[0].Phonemes.Should().BeNull();
    }

    [Fact]
    public void Process_MixedContent_OnlyWordsGetPhonemes()
    {
        var processor = TextProcessor.CreateDefault();

        var result = processor.Process("hello <@123>");

        var wordToken = result.First(t => t.Type == TextTokenType.Word);
        var preservedToken = result.First(t => t.OriginalText == "<@123>");

        wordToken.Phonemes.Should().NotBeNull();
        wordToken.Phonemes.Should().NotBeEmpty();
        preservedToken.Phonemes.Should().BeNull();
    }

    [Fact]
    public void Process_EmptyString_ReturnsEmptyList()
    {
        var processor = TextProcessor.CreateDefault();

        var result = processor.Process("");

        result.Should().BeEmpty();
    }

    [Fact]
    public void Process_PreservesTokenOrder()
    {
        var processor = TextProcessor.CreateDefault();

        var result = processor.Process("a b c");

        result.Should().HaveCount(5); // a, space, b, space, c
        result[0].OriginalText.Should().Be("a");
        result[1].OriginalText.Should().Be(" ");
        result[2].OriginalText.Should().Be("b");
        result[3].OriginalText.Should().Be(" ");
        result[4].OriginalText.Should().Be("c");
    }
}
