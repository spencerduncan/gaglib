using FluentAssertions;
using GagLib.Abstractions;
using GagLib.Transformers;
using Xunit;

namespace GagLib.Tests.Transformers;

public class BallGagTransformerTests
{
    [Fact]
    public void Name_ReturnsBallGag()
    {
        // Arrange
        var transformer = new BallGagTransformer();

        // Act & Assert
        transformer.Name.Should().Be("Ball Gag");
    }

    [Fact]
    public void Transform_WithEmptyPhonemes_ReturnsEmptyString()
    {
        // Arrange
        var transformer = new BallGagTransformer();
        var phonemes = Array.Empty<Phoneme>();

        // Act
        var result = transformer.Transform(phonemes);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public void TransformSentence_WithMultipleWords_PreservesWordBoundaries()
    {
        // Arrange
        var transformer = new BallGagTransformer();
        var sentence = new List<IReadOnlyList<Phoneme>>
        {
            new[] { Phoneme.HH, Phoneme.EH, Phoneme.L, Phoneme.OW },  // hello
            new[] { Phoneme.W, Phoneme.ER, Phoneme.L, Phoneme.D }     // world
        };

        // Act
        var result = transformer.TransformSentence(sentence);

        // Assert
        result.Should().Contain(" ");  // Word boundary preserved
    }

    // TODO: Add tests for specific phoneme transformations
}
