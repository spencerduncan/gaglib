using FluentAssertions;
using GagLib.Abstractions;
using GagLib.Transformers;
using Xunit;

namespace GagLib.Tests.Transformers;

public class UwuGagTransformerTests
{
    // Use seeded random for deterministic tests
    private readonly UwuGagTransformer _transformer = new(new Random(42));

    [Fact]
    public void Name_ReturnsUwuGag()
    {
        _transformer.Name.Should().Be("Uwu Gag");
    }

    [Fact]
    public void Transform_EmptyList_ReturnsEmptyString()
    {
        _transformer.Transform([]).Should().BeEmpty();
    }

    [Fact]
    public void Transform_SingleUVowel_ProducesUwuPattern()
    {
        // U-like vowel should produce uwu-style pattern
        var phonemes = new[] { Phoneme.UW };
        var result = _transformer.Transform(phonemes);

        // Should contain u, w, and be uwu-ish
        result.Should().Contain("w");
        result.Should().MatchRegex("[uüùúûŭ]");
    }

    [Fact]
    public void Transform_SingleOVowel_ProducesOwoPattern()
    {
        // O-like vowel should produce owo-style pattern
        var phonemes = new[] { Phoneme.OW };
        var result = _transformer.Transform(phonemes);

        // Should contain o and w
        result.Should().MatchRegex("[oöòóôŏ]");
    }

    [Fact]
    public void Transform_SingleAVowel_ProducesAwaOrNyaPattern()
    {
        // A-like vowel should produce awa/nya-style pattern
        var phonemes = new[] { Phoneme.AE };
        var result = _transformer.Transform(phonemes);

        // Should be awa, awo, awu, nya, nyáa, or nyanya style
        (result.Contains("w") || result.Contains("ny")).Should().BeTrue();
    }

    [Fact]
    public void Transform_MultipleVowels_ProducesMultipleSyllables()
    {
        // "hello" has 2 vowels (EH, OW)
        var phonemes = new[] { Phoneme.HH, Phoneme.EH, Phoneme.L, Phoneme.OW };
        var result = _transformer.Transform(phonemes);

        // Should be longer than a single syllable pattern
        result.Length.Should().BeGreaterThan(3);
    }

    [Fact]
    public void Transform_NoVowels_ReturnsSingleUwu()
    {
        // No vowels should return "uwu"
        var phonemes = new[] { Phoneme.K, Phoneme.T };
        var result = _transformer.Transform(phonemes);

        result.Should().Be("uwu");
    }

    [Fact]
    public void Transform_OutputContainsUwuStylePatterns()
    {
        // Various inputs should produce uwu-style output
        var phonemes = new[] { Phoneme.HH, Phoneme.AH, Phoneme.L, Phoneme.OW };
        var result = _transformer.Transform(phonemes);

        // Output should look uwu-ish (contain w/v and vowels)
        result.Should().MatchRegex("[wv]");
    }

    [Fact]
    public void TransformSentence_JoinsWithSpaces()
    {
        var sentence = new List<IReadOnlyList<Phoneme>>
        {
            new[] { Phoneme.HH, Phoneme.AH },
            new[] { Phoneme.L, Phoneme.OW }
        };

        var result = _transformer.TransformSentence(sentence);

        // Should have space between words
        result.Should().Contain(" ");
    }

    [Fact]
    public void TransformSentence_ShortSentence_AddsUwuOrOwoSuffix()
    {
        // Short sentence should get suffix
        var sentence = new List<IReadOnlyList<Phoneme>>
        {
            new[] { Phoneme.HH, Phoneme.AY }
        };

        var result = _transformer.TransformSentence(sentence);

        // Should end with uwu or owo
        (result.EndsWith("uwu") || result.EndsWith("owo")).Should().BeTrue();
    }

    [Fact]
    public void Transform_CanProduceDiacritics()
    {
        // Run multiple times to check for diacritics
        var hasDiacritic = false;
        var transformer = new UwuGagTransformer(new Random(12345));

        for (var i = 0; i < 50; i++)
        {
            var phonemes = new[] { Phoneme.UW, Phoneme.OW, Phoneme.AE };
            var result = transformer.Transform(phonemes);

            if (result.Any(c => "üùúûŭöòóôŏáā".Contains(c)))
            {
                hasDiacritic = true;
                break;
            }
        }

        hasDiacritic.Should().BeTrue("Uwu output should sometimes include tonal diacritics");
    }
}
