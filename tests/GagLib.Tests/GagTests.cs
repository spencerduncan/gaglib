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

    [Fact]
    public void Transform_CowGag_ReturnsMooSounds()
    {
        var result = Gag.Transform(GagType.CowGag, "hello");

        result.Should().Contain("moo");
        result.Should().NotBe("hello");
    }

    [Fact]
    public void Transform_CowGag_PreservesPunctuation()
    {
        var result = Gag.Transform(GagType.CowGag, "hello, world!");

        result.Should().Contain(",");
        result.Should().Contain("!");
        result.Should().Contain("moo");
    }

    [Fact]
    public void Transform_CowGag_PreservesDiscordEmbed()
    {
        var result = Gag.Transform(GagType.CowGag, "hey <@123>");

        result.Should().Contain("<@123>");
        result.Should().Contain("moo");
    }

    [Fact]
    public void Transform_DifferentGagTypes_ProduceDifferentOutput()
    {
        var input = "hello world";

        var ballGag = Gag.Transform(GagType.BallGag, input);
        var cowGag = Gag.Transform(GagType.CowGag, input);

        ballGag.Should().NotBe(cowGag);
        ballGag.Should().NotContain("moo");
        cowGag.Should().Contain("moo");
    }

    [Fact]
    public void Transform_DogGag_AddsRSounds()
    {
        var result = Gag.Transform(GagType.DogGag, "hello");

        result.Should().StartWith("r");
        result.Should().NotBe("hello");
    }

    [Fact]
    public void Transform_DogGag_PreservesPunctuation()
    {
        var result = Gag.Transform(GagType.DogGag, "uh oh!");

        result.Should().Contain("!");
        result.Should().Contain("ruh");
        result.Should().Contain("roh");
    }

    [Fact]
    public void Transform_DogGag_PreservesDiscordEmbed()
    {
        var result = Gag.Transform(GagType.DogGag, "hey <@123>");

        result.Should().Contain("<@123>");
        result.Should().StartWith("r");
    }

    [Fact]
    public void Transform_AllGagTypes_ProduceDifferentOutput()
    {
        var input = "hello";

        var ball = Gag.Transform(GagType.BallGag, input);
        var cow = Gag.Transform(GagType.CowGag, input);
        var dog = Gag.Transform(GagType.DogGag, input);

        ball.Should().NotBe(cow);
        ball.Should().NotBe(dog);
        cow.Should().NotBe(dog);
    }
}
