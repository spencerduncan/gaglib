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
        [GagType.DogGag],
        [GagType.BarkingDogGag],
        [GagType.CatgirlGag],
        [GagType.CatGag],
        [GagType.UwuGag],
        [GagType.FurryGag]
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
    public void Transform_BarkingDogGag_ReturnsDogSounds()
    {
        var result = Gag.Transform(GagType.BarkingDogGag, "hello world");

        // Should contain at least one dog sound
        var hasDogSound = result.Contains("ruff") ||
                          result.Contains("woof") ||
                          result.Contains("arf") ||
                          result.Contains("bark") ||
                          result.Contains("grr");
        hasDogSound.Should().BeTrue();
    }

    [Fact]
    public void Transform_BarkingDogGag_GrowlyWord_AddsGrr()
    {
        // "great" has R and G - should trigger grr
        var result = Gag.Transform(GagType.BarkingDogGag, "great");

        result.Should().Contain("grr");
    }

    [Fact]
    public void Transform_CatgirlGag_TransformsMagic()
    {
        var result = Gag.Transform(GagType.CatgirlGag, "magic");

        // M + A-vowel pattern → nya
        result.Should().Contain("nya");
    }

    [Fact]
    public void Transform_CatgirlGag_TransformsYou()
    {
        var result = Gag.Transform(GagType.CatgirlGag, "you");

        // Y + U-vowel → mew
        result.Should().Contain("mew");
    }

    [Fact]
    public void Transform_CatGag_ReturnsMeowSounds()
    {
        var result = Gag.Transform(GagType.CatGag, "hello");

        result.Should().Contain("meow");
    }

    [Fact]
    public void Transform_UwuGag_TransformsToUwuStyle()
    {
        var result = Gag.Transform(GagType.UwuGag, "hello");

        // Uwu patterns contain w or v as connectors
        (result.Contains("w") || result.Contains("v")).Should().BeTrue();
    }

    [Fact]
    public void Transform_AllGagTypes_ProduceDifferentOutput()
    {
        var input = "hello";

        var ball = Gag.Transform(GagType.BallGag, input);
        var cow = Gag.Transform(GagType.CowGag, input);
        var dog = Gag.Transform(GagType.DogGag, input);
        var barkingDog = Gag.Transform(GagType.BarkingDogGag, input);
        var catgirl = Gag.Transform(GagType.CatgirlGag, input);
        var cat = Gag.Transform(GagType.CatGag, input);
        var uwu = Gag.Transform(GagType.UwuGag, input);
        var furry = Gag.Transform(GagType.FurryGag, input);

        ball.Should().NotBe(cow);
        ball.Should().NotBe(dog);
        ball.Should().NotBe(barkingDog);
        ball.Should().NotBe(catgirl);
        ball.Should().NotBe(cat);
        ball.Should().NotBe(uwu);
        ball.Should().NotBe(furry);
        cow.Should().NotBe(dog);
        cow.Should().NotBe(barkingDog);
        cow.Should().NotBe(catgirl);
        cow.Should().NotBe(cat);
        cow.Should().NotBe(uwu);
        cow.Should().NotBe(furry);
        dog.Should().NotBe(barkingDog);
        dog.Should().NotBe(catgirl);
        dog.Should().NotBe(cat);
        dog.Should().NotBe(uwu);
        dog.Should().NotBe(furry);
        barkingDog.Should().NotBe(catgirl);
        barkingDog.Should().NotBe(cat);
        barkingDog.Should().NotBe(uwu);
        barkingDog.Should().NotBe(furry);
        catgirl.Should().NotBe(cat);
        catgirl.Should().NotBe(uwu);
        catgirl.Should().NotBe(furry);
        cat.Should().NotBe(uwu);
        cat.Should().NotBe(furry);
        uwu.Should().NotBe(furry);
    }

    [Fact]
    public void Transform_FurryGag_TransformsToFurryStyle()
    {
        var result = Gag.Transform(GagType.FurryGag, "hello you");

        // "hello" should become "hewwo", "you" becomes "chu"
        result.Should().Contain("hewwo");
        result.Should().Contain("chu");
    }

    [Fact]
    public void Transform_FurryGag_TransformsRAndL()
    {
        var result = Gag.Transform(GagType.FurryGag, "really love");

        // R and L should become W
        result.Should().Contain("w");
    }
}
