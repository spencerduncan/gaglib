using FluentAssertions;
using Xunit;

namespace GagLib.Tests;

public class GagTests
{
    [Fact]
    public void Transform_SimpleWord_ReturnsMuffledOutput()
    {
        var result = Gag.Transform(GagType.BallGag, "hello");

        result.Should().NotBeEmpty();
        result.Should().NotBe("hello");
    }

    [Fact]
    public void Transform_EmptyString_ReturnsEmpty()
    {
        Gag.Transform(GagType.BallGag, "").Should().BeEmpty();
    }

    [Fact]
    public void Transform_Null_ReturnsNull()
    {
        Gag.Transform(GagType.BallGag, null!).Should().BeNull();
    }

    [Fact]
    public void Transform_PreservesPunctuation()
    {
        var result = Gag.Transform(GagType.BallGag, "hello, world!");

        result.Should().Contain(",");
        result.Should().Contain("!");
    }

    [Fact]
    public void Transform_PreservesDiscordMention()
    {
        var result = Gag.Transform(GagType.BallGag, "hey <@123456789>");

        result.Should().Contain("<@123456789>");
    }

    [Fact]
    public void Transform_PreservesDiscordEmoji()
    {
        var result = Gag.Transform(GagType.BallGag, "lol <:smile:123>");

        result.Should().Contain("<:smile:123>");
    }

    [Fact]
    public void Transform_PreservesAnimatedEmoji()
    {
        var result = Gag.Transform(GagType.BallGag, "haha <a:laugh:456>");

        result.Should().Contain("<a:laugh:456>");
    }

    [Fact]
    public void Transform_PreservesUnicodeEmoji()
    {
        var result = Gag.Transform(GagType.BallGag, "hi 😀");

        result.Should().Contain("😀");
    }

    [Fact]
    public void Transform_ConvertsLetterEmoji()
    {
        // Regional indicators should become letters and get gagged
        var result = Gag.Transform(GagType.BallGag, "🇭🇮");

        // Should NOT contain the original emoji (it was converted)
        result.Should().NotContain("🇭");
        result.Should().NotContain("🇮");
        // Should be gagged output
        result.Should().NotBeEmpty();
    }

    [Fact]
    public void Transform_PreservesSpacing()
    {
        var result = Gag.Transform(GagType.BallGag, "one two three");

        // Should have spaces between words
        var parts = result.Split(' ');
        parts.Length.Should().BeGreaterThanOrEqualTo(3);
    }

    [Fact]
    public void Transform_FullSentence_EndToEnd()
    {
        var input = "Hello, world! 😀 <@123> How are you?";
        var result = Gag.Transform(GagType.BallGag, input);

        // Words should be transformed
        result.Should().NotContain("Hello");
        result.Should().NotContain("world");
        result.Should().NotContain("How");
        result.Should().NotContain("are");
        result.Should().NotContain("you");

        // Preserved content should remain
        result.Should().Contain(",");
        result.Should().Contain("!");
        result.Should().Contain("?");
        result.Should().Contain("😀");
        result.Should().Contain("<@123>");
    }

    [Fact]
    public void Transform_MadeUpWord_StillWorks()
    {
        // Heuristic fallback should handle unknown words
        var result = Gag.Transform(GagType.BallGag, "blorphington");

        result.Should().NotBeEmpty();
        result.Should().NotBe("blorphington");
    }
}
