using System.Text;
using System.Text.RegularExpressions;
using GagLib.Abstractions;

namespace GagLib.TextProcessing;

/// <summary>
/// Tokenizes text into words and preserved content using regex patterns.
/// </summary>
public partial class TextTokenizer : ITextTokenizer
{
    // Discord patterns: mentions and custom emoji
    [GeneratedRegex(@"<[@#][&]?\d+>|<a?:\w+:\d+>")]
    private static partial Regex DiscordPattern();

    // Word characters
    [GeneratedRegex(@"\w+")]
    private static partial Regex WordPattern();

    /// <inheritdoc />
    public IReadOnlyList<TextToken> Tokenize(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Array.Empty<TextToken>();

        var tokens = new List<TextToken>();
        var position = 0;

        while (position < text.Length)
        {
            var (token, length) = TryMatchPattern(text, position);

            if (token != null)
            {
                tokens.Add(token);
                position += length;
            }
            else
            {
                // No pattern matched - take single character as preserved
                // But handle surrogate pairs correctly
                var charLength = char.IsHighSurrogate(text[position]) && position + 1 < text.Length
                    ? 2
                    : 1;
                tokens.Add(TextToken.Preserved(text.Substring(position, charLength)));
                position += charLength;
            }
        }

        return tokens.AsReadOnly();
    }

    private static (TextToken? token, int length) TryMatchPattern(string text, int position)
    {
        var remaining = text[position..];

        // 1. Discord patterns (highest priority - preserve)
        var discordMatch = DiscordPattern().Match(remaining);
        if (discordMatch.Success && discordMatch.Index == 0)
        {
            return (TextToken.Preserved(discordMatch.Value), discordMatch.Length);
        }

        // 2. Check for letter-like emoji at current position
        var (letterResult, letterLength) = TryMatchLetterEmoji(text, position);
        if (letterResult != null)
        {
            return (letterResult, letterLength);
        }

        // 3. Check for regular emoji (preserve)
        var (emojiResult, emojiLength) = TryMatchEmoji(text, position);
        if (emojiResult != null)
        {
            return (emojiResult, emojiLength);
        }

        // 4. Words
        var wordMatch = WordPattern().Match(remaining);
        if (wordMatch.Success && wordMatch.Index == 0)
        {
            return (TextToken.Word(wordMatch.Value), wordMatch.Length);
        }

        return (null, 0);
    }

    private static (TextToken? token, int length) TryMatchLetterEmoji(string text, int position)
    {
        var sb = new StringBuilder();
        var consumed = 0;
        var current = position;

        while (current < text.Length)
        {
            if (!char.IsHighSurrogate(text[current]))
            {
                // Check BMP letter-like characters
                var c = text[current];
                var letter = ConvertBmpLetterEmoji(c);
                if (letter.HasValue)
                {
                    sb.Append(letter.Value);
                    consumed++;
                    current++;
                    continue;
                }
                break;
            }

            // Handle surrogate pairs
            if (current + 1 >= text.Length)
                break;

            var high = text[current];
            var low = text[current + 1];
            var codePoint = char.ConvertToUtf32(high, low);

            var convertedLetter = ConvertSurrogateLetterEmoji(codePoint);
            if (convertedLetter.HasValue)
            {
                sb.Append(convertedLetter.Value);
                consumed += 2;
                current += 2;
            }
            else
            {
                break;
            }
        }

        if (sb.Length > 0)
        {
            return (TextToken.Word(sb.ToString()), consumed);
        }

        return (null, 0);
    }

    private static char? ConvertBmpLetterEmoji(char c)
    {
        // Circled Latin Capital Letters Ⓐ-Ⓩ (U+24B6-U+24CF)
        if (c >= '\u24B6' && c <= '\u24CF')
            return (char)('A' + (c - '\u24B6'));

        // Circled Latin Small Letters ⓐ-ⓩ (U+24D0-U+24E9)
        if (c >= '\u24D0' && c <= '\u24E9')
            return (char)('A' + (c - '\u24D0'));

        return null;
    }

    private static char? ConvertSurrogateLetterEmoji(int codePoint)
    {
        // Regional indicator symbols 🇦-🇿 (U+1F1E6-U+1F1FF)
        if (codePoint >= 0x1F1E6 && codePoint <= 0x1F1FF)
            return (char)('A' + (codePoint - 0x1F1E6));

        // Negative Squared Latin Capital Letters 🅰-🅿 (U+1F170-U+1F17F)
        if (codePoint >= 0x1F170 && codePoint <= 0x1F189)
            return (char)('A' + (codePoint - 0x1F170));

        // Special cases
        if (codePoint == 0x1F17E) return 'O';  // 🅾
        if (codePoint == 0x1F17F) return 'P';  // 🅿

        return null;
    }

    private static (TextToken? token, int length) TryMatchEmoji(string text, int position)
    {
        if (!char.IsHighSurrogate(text[position]))
        {
            // Check BMP emoji ranges
            var c = text[position];
            if (IsBmpEmoji(c))
            {
                // Check for variation selector
                var length = 1;
                if (position + 1 < text.Length && text[position + 1] == '\uFE0F')
                    length = 2;
                return (TextToken.Preserved(text.Substring(position, length)), length);
            }
            return (null, 0);
        }

        // Handle surrogate pairs
        if (position + 1 >= text.Length)
            return (null, 0);

        var high = text[position];
        var low = text[position + 1];
        var codePoint = char.ConvertToUtf32(high, low);

        if (IsSurrogateEmoji(codePoint))
        {
            var length = 2;
            // Check for variation selector
            if (position + 2 < text.Length && text[position + 2] == '\uFE0F')
                length = 3;
            return (TextToken.Preserved(text.Substring(position, length)), length);
        }

        return (null, 0);
    }

    private static bool IsBmpEmoji(char c)
    {
        // Miscellaneous Symbols (U+2600-U+26FF)
        if (c >= '\u2600' && c <= '\u26FF') return true;
        // Dingbats (U+2700-U+27BF)
        if (c >= '\u2700' && c <= '\u27BF') return true;
        return false;
    }

    private static bool IsSurrogateEmoji(int codePoint)
    {
        // Skip letter-like emoji (handled separately)
        if (codePoint >= 0x1F1E6 && codePoint <= 0x1F1FF) return false;  // Regional indicators
        if (codePoint >= 0x1F170 && codePoint <= 0x1F19A) return false;  // Enclosed alphanumerics

        // Miscellaneous Symbols and Pictographs (U+1F300-U+1F5FF)
        if (codePoint >= 0x1F300 && codePoint <= 0x1F5FF) return true;
        // Emoticons (U+1F600-U+1F64F)
        if (codePoint >= 0x1F600 && codePoint <= 0x1F64F) return true;
        // Transport and Map Symbols (U+1F680-U+1F6FF)
        if (codePoint >= 0x1F680 && codePoint <= 0x1F6FF) return true;
        // Supplemental Symbols and Pictographs (U+1F900-U+1F9FF)
        if (codePoint >= 0x1F900 && codePoint <= 0x1F9FF) return true;
        // Chess Symbols, Extended-A (U+1FA00-U+1FA6F)
        if (codePoint >= 0x1FA00 && codePoint <= 0x1FA6F) return true;
        // Symbols and Pictographs Extended-A (U+1FA70-U+1FAFF)
        if (codePoint >= 0x1FA70 && codePoint <= 0x1FAFF) return true;

        return false;
    }
}
