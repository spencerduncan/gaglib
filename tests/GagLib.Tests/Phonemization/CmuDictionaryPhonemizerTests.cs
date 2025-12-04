using FluentAssertions;
using GagLib.Phonemization;
using Xunit;

namespace GagLib.Tests.Phonemization;

public class CmuDictionaryPhonemizerTests
{
    [Fact]
    public void Phonemize_WithEmptyWord_ReturnsEmptyList()
    {
        // Arrange
        var phonemizer = new CmuDictionaryPhonemizer();

        // Act
        var result = phonemizer.Phonemize("");

        // Assert
        result.Should().BeEmpty();
    }

    // TODO: Add tests for dictionary lookup once CMU dict is loaded
}
