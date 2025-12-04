using FluentAssertions;
using GagLib.Abstractions;
using GagLib.TextProcessing;
using Xunit;

namespace GagLib.Tests.TextProcessing;

public class TextTokenizerTests
{
    private readonly TextTokenizer _tokenizer = new();

    [Fact]
    public void Tokenize_SimpleWord_ReturnsWordToken()
    {
        var result = _tokenizer.Tokenize("hello");

        result.Should().HaveCount(1);
        result[0].Type.Should().Be(TextTokenType.Word);
        result[0].OriginalText.Should().Be("hello");
    }

    [Fact]
    public void Tokenize_MultipleWords_ReturnsWordTokensWithSpaces()
    {
        var result = _tokenizer.Tokenize("hello world");

        result.Should().HaveCount(3);
        result[0].Type.Should().Be(TextTokenType.Word);
        result[0].OriginalText.Should().Be("hello");
        result[1].Type.Should().Be(TextTokenType.Preserved);
        result[1].OriginalText.Should().Be(" ");
        result[2].Type.Should().Be(TextTokenType.Word);
        result[2].OriginalText.Should().Be("world");
    }

    [Fact]
    public void Tokenize_Punctuation_PreservesPunctuation()
    {
        var result = _tokenizer.Tokenize("hello!");

        result.Should().HaveCount(2);
        result[0].Type.Should().Be(TextTokenType.Word);
        result[0].OriginalText.Should().Be("hello");
        result[1].Type.Should().Be(TextTokenType.Preserved);
        result[1].OriginalText.Should().Be("!");
    }

    [Fact]
    public void Tokenize_DiscordUserMention_PreservesMention()
    {
        var result = _tokenizer.Tokenize("hey <@123456789>");

        result.Should().HaveCount(3);
        result[2].Type.Should().Be(TextTokenType.Preserved);
        result[2].OriginalText.Should().Be("<@123456789>");
    }

    [Fact]
    public void Tokenize_DiscordChannelMention_PreservesMention()
    {
        var result = _tokenizer.Tokenize("check <#987654321>");

        result.Should().HaveCount(3);
        result[2].Type.Should().Be(TextTokenType.Preserved);
        result[2].OriginalText.Should().Be("<#987654321>");
    }

    [Fact]
    public void Tokenize_DiscordRoleMention_PreservesMention()
    {
        var result = _tokenizer.Tokenize("ping <@&111222333>");

        result.Should().HaveCount(3);
        result[2].Type.Should().Be(TextTokenType.Preserved);
        result[2].OriginalText.Should().Be("<@&111222333>");
    }

    [Fact]
    public void Tokenize_DiscordCustomEmoji_PreservesEmoji()
    {
        var result = _tokenizer.Tokenize("nice <:smile:123456>");

        result.Should().HaveCount(3);
        result[2].Type.Should().Be(TextTokenType.Preserved);
        result[2].OriginalText.Should().Be("<:smile:123456>");
    }

    [Fact]
    public void Tokenize_DiscordAnimatedEmoji_PreservesEmoji()
    {
        var result = _tokenizer.Tokenize("wow <a:dance:789012>");

        result.Should().HaveCount(3);
        result[2].Type.Should().Be(TextTokenType.Preserved);
        result[2].OriginalText.Should().Be("<a:dance:789012>");
    }

    [Fact]
    public void Tokenize_UnicodeEmoji_PreservesEmoji()
    {
        var result = _tokenizer.Tokenize("great job \U0001F600");

        // "great" " " "job" " " "😀"
        result.Should().HaveCount(5);
        result[4].Type.Should().Be(TextTokenType.Preserved);
        result[4].OriginalText.Should().Be("\U0001F600");
    }

    [Fact]
    public void Tokenize_RegionalIndicators_ConvertsToLetters()
    {
        // 🇭🇮 = HI
        var result = _tokenizer.Tokenize("\U0001F1ED\U0001F1EE");

        result.Should().HaveCount(1);
        result[0].Type.Should().Be(TextTokenType.Word);
        result[0].OriginalText.Should().Be("HI");
    }

    [Fact]
    public void Tokenize_EnclosedAlphanumerics_ConvertsToLetters()
    {
        // Ⓐ Ⓑ = AB
        var result = _tokenizer.Tokenize("\u24B6\u24B7");

        result.Should().HaveCount(1);
        result[0].Type.Should().Be(TextTokenType.Word);
        result[0].OriginalText.Should().Be("AB");
    }

    [Fact]
    public void Tokenize_NegativeSquaredLetters_ConvertsToLetters()
    {
        // 🅰🅱 = AB
        var result = _tokenizer.Tokenize("\U0001F170\U0001F171");

        result.Should().HaveCount(1);
        result[0].Type.Should().Be(TextTokenType.Word);
        result[0].OriginalText.Should().Be("AB");
    }

    [Fact]
    public void Tokenize_MixedContent_HandlesAllTypes()
    {
        var result = _tokenizer.Tokenize("hey <@123> check this \U0001F600!");

        // "hey" " " "<@123>" " " "check" " " "this" " " "😀" "!"
        result.Should().HaveCount(10);

        result[0].Type.Should().Be(TextTokenType.Word);
        result[0].OriginalText.Should().Be("hey");

        result[2].Type.Should().Be(TextTokenType.Preserved);
        result[2].OriginalText.Should().Be("<@123>");

        result[8].Type.Should().Be(TextTokenType.Preserved);
        result[8].OriginalText.Should().Be("\U0001F600");

        result[9].Type.Should().Be(TextTokenType.Preserved);
        result[9].OriginalText.Should().Be("!");
    }

    [Fact]
    public void Tokenize_EmptyString_ReturnsEmptyList()
    {
        var result = _tokenizer.Tokenize("");

        result.Should().BeEmpty();
    }

    [Fact]
    public void Tokenize_NullString_ReturnsEmptyList()
    {
        var result = _tokenizer.Tokenize(null!);

        result.Should().BeEmpty();
    }
}
