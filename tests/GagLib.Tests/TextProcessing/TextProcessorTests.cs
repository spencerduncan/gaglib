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

    [Fact]
    public void Process_FullSentence_EndToEnd()
    {
        var processor = TextProcessor.CreateDefault();

        // Realistic Discord message with words, punctuation, emoji, and mention
        var result = processor.Process("Hello, world! 😀 <@123> How's it going?");

        // Verify structure: words got phonemes, everything else preserved
        var words = result.Where(t => t.Type == TextTokenType.Word).ToList();
        var preserved = result.Where(t => t.Type == TextTokenType.Preserved).ToList();

        // Words should have phonemes
        words.Should().NotBeEmpty();
        words.Should().OnlyContain(w => w.Phonemes != null && w.Phonemes.Count > 0);

        // Check specific words got correct phonemes
        var hello = words.First(w => w.OriginalText.Equals("Hello", StringComparison.OrdinalIgnoreCase));
        hello.Phonemes.Should().Contain(Phoneme.HH);
        hello.Phonemes.Should().Contain(Phoneme.OW);

        var world = words.First(w => w.OriginalText.Equals("world", StringComparison.OrdinalIgnoreCase));
        world.Phonemes.Should().Contain(Phoneme.W);
        world.Phonemes.Should().Contain(Phoneme.ER);

        // Preserved tokens should have no phonemes
        preserved.Should().OnlyContain(p => p.Phonemes == null);

        // Discord mention should be preserved intact
        preserved.Should().Contain(p => p.OriginalText == "<@123>");

        // Emoji should be preserved
        preserved.Should().Contain(p => p.OriginalText.Contains("😀"));

        // Punctuation should be preserved
        preserved.Should().Contain(p => p.OriginalText == ",");
        preserved.Should().Contain(p => p.OriginalText == "!");
        preserved.Should().Contain(p => p.OriginalText == "?");
    }

    [Fact]
    public void Process_CanReconstructOriginalText()
    {
        var processor = TextProcessor.CreateDefault();
        var original = "Hello, world! 😀 <@123>";

        var result = processor.Process(original);

        // Concatenating all original text should give us back the input
        var reconstructed = string.Concat(result.Select(t => t.OriginalText));
        reconstructed.Should().Be(original);
    }

    [Fact]
    public void Process_LetterEmoji_ConvertedAndPhonemized()
    {
        var processor = TextProcessor.CreateDefault();

        // Regional indicators 🇭🇮 should become "HI" and get phonemized
        var result = processor.Process("🇭🇮 there");

        var words = result.Where(t => t.Type == TextTokenType.Word).ToList();

        // Should have "HI" (converted from emoji) and "there"
        words.Should().HaveCount(2);

        var hi = words.First(w => w.OriginalText == "HI");
        hi.Phonemes.Should().NotBeEmpty();
        hi.Phonemes.Should().Contain(Phoneme.HH);
        hi.Phonemes.Should().Contain(Phoneme.AY);

        var there = words.First(w => w.OriginalText.Equals("there", StringComparison.OrdinalIgnoreCase));
        there.Phonemes.Should().Contain(Phoneme.DH);
    }
}
