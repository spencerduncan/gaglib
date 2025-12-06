using FluentAssertions;
using Xunit;

namespace GagLib.Tests;

public class GagTests
{
    // All gag types for parameterized tests
    public static IEnumerable<object[]> AllGagTypes =>
    [
        [GagType.BallGag],
        [GagType.CowGag],
        [GagType.DogGag]
    ];

    [Theory]
    [MemberData(nameof(AllGagTypes))]
    public void Transform_TransformsWords(GagType gagType)
    {
        var result = Gag.Transform(gagType, "hello");

        result.Should().NotBeEmpty();
        result.Should().NotBe("hello");
    }

    [Theory]
    [MemberData(nameof(AllGagTypes))]
    public void Transform_EmptyString_ReturnsEmpty(GagType gagType)
    {
        Gag.Transform(gagType, "").Should().BeEmpty();
    }

    [Theory]
    [MemberData(nameof(AllGagTypes))]
    public void Transform_Null_ReturnsNull(GagType gagType)
    {
        Gag.Transform(gagType, null!).Should().BeNull();
    }

    [Theory]
    [MemberData(nameof(AllGagTypes))]
    public void Transform_PreservesPunctuation(GagType gagType)
    {
        var result = Gag.Transform(gagType, "hello, world!");

        result.Should().Contain(",");
        result.Should().Contain("!");
    }

    [Theory]
    [MemberData(nameof(AllGagTypes))]
    public void Transform_PreservesDiscordMention(GagType gagType)
    {
        var result = Gag.Transform(gagType, "hey <@123456789>");

        result.Should().Contain("<@123456789>");
    }

    [Theory]
    [MemberData(nameof(AllGagTypes))]
    public void Transform_PreservesDiscordEmoji(GagType gagType)
    {
        var result = Gag.Transform(gagType, "lol <:smile:123>");

        result.Should().Contain("<:smile:123>");
    }

    [Theory]
    [MemberData(nameof(AllGagTypes))]
    public void Transform_PreservesAnimatedEmoji(GagType gagType)
    {
        var result = Gag.Transform(gagType, "haha <a:laugh:456>");

        result.Should().Contain("<a:laugh:456>");
    }

    [Theory]
    [MemberData(nameof(AllGagTypes))]
    public void Transform_PreservesUnicodeEmoji(GagType gagType)
    {
        var result = Gag.Transform(gagType, "hi 😀");

        result.Should().Contain("😀");
    }

    [Theory]
    [MemberData(nameof(AllGagTypes))]
    public void Transform_PreservesSpacing(GagType gagType)
    {
        var result = Gag.Transform(gagType, "one two three");

        var parts = result.Split(' ');
        parts.Length.Should().BeGreaterThanOrEqualTo(3);
    }

    [Theory]
    [MemberData(nameof(AllGagTypes))]
    public void Transform_ConvertsLetterEmoji(GagType gagType)
    {
        var result = Gag.Transform(gagType, "🇭🇮");

        result.Should().NotContain("🇭");
        result.Should().NotContain("🇮");
        result.Should().NotBeEmpty();
    }

    [Theory]
    [MemberData(nameof(AllGagTypes))]
    public void Transform_MadeUpWord_StillWorks(GagType gagType)
    {
        var result = Gag.Transform(gagType, "blorphington");

        result.Should().NotBeEmpty();
        result.Should().NotBe("blorphington");
    }

    [Theory]
    [MemberData(nameof(AllGagTypes))]
    public void Transform_FullSentence_PreservesStructure(GagType gagType)
    {
        var input = "Hello, world! 😀 <@123> How are you?";
        var result = Gag.Transform(gagType, input);

        // Words should be transformed
        result.Should().NotContain("Hello");
        result.Should().NotContain("world");

        // Preserved content should remain
        result.Should().Contain(",");
        result.Should().Contain("!");
        result.Should().Contain("?");
        result.Should().Contain("😀");
        result.Should().Contain("<@123>");
    }

    // Gag-specific behavior tests
    [Fact]
    public void Transform_BallGag_ProducesMuffledSounds()
    {
        var result = Gag.Transform(GagType.BallGag, "hello");

        // Ball gag produces phonetic muffled output
        result.Should().NotContain("moo");
        result.Should().NotStartWith("r");
    }

    [Fact]
    public void Transform_CowGag_ReturnsMooSounds()
    {
        var result = Gag.Transform(GagType.CowGag, "hello");

        result.Should().Contain("moo");
    }

    [Fact]
    public void Transform_DogGag_AddsRSounds()
    {
        var result = Gag.Transform(GagType.DogGag, "hello");

        result.Should().StartWith("r");
    }

    [Fact]
    public void Transform_DogGag_UhOh_ReturnsRuhRoh()
    {
        var result = Gag.Transform(GagType.DogGag, "uh oh");

        result.Should().Contain("ruh");
        result.Should().Contain("roh");
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
