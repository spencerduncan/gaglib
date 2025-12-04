#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace GagLib.Abstractions;

/// <summary>
/// ARPAbet phonemes used by the CMU Pronouncing Dictionary.
/// </summary>
public enum Phoneme
{
    // Vowels - Monophthongs
    AA,  // odd, father
    AE,  // at, bat
    AH,  // hut, but
    AO,  // ought, caught
    EH,  // ed, bet
    ER,  // hurt, bird
    IH,  // it, bit
    IY,  // eat, bee
    UH,  // hood, book
    UW,  // two, boot

    // Vowels - Diphthongs
    AW,  // cow, how
    AY,  // hide, my
    EY,  // ate, say
    OW,  // oat, show
    OY,  // toy, boy

    // Consonants - Stops
    B,   // be
    D,   // dee
    G,   // green
    K,   // key
    P,   // pee
    T,   // tea

    // Consonants - Affricates
    CH,  // cheese
    JH,  // jee

    // Consonants - Fricatives
    DH,  // thee
    F,   // fee
    HH,  // he
    S,   // sea
    SH,  // she
    TH,  // thief
    V,   // vee
    Z,   // zee
    ZH,  // seizure

    // Consonants - Nasals
    M,   // me
    N,   // knee
    NG,  // ping

    // Consonants - Liquids
    L,   // lee
    R,   // read

    // Consonants - Semivowels
    W,   // we
    Y,   // yield
}

/// <summary>
/// Extension methods for phoneme classification.
/// </summary>
public static class PhonemeExtensions
{
    private static readonly HashSet<Phoneme> Vowels = new()
    {
        Phoneme.AA, Phoneme.AE, Phoneme.AH, Phoneme.AO, Phoneme.EH,
        Phoneme.ER, Phoneme.IH, Phoneme.IY, Phoneme.UH, Phoneme.UW,
        Phoneme.AW, Phoneme.AY, Phoneme.EY, Phoneme.OW, Phoneme.OY
    };

    private static readonly HashSet<Phoneme> Nasals = new()
    {
        Phoneme.M, Phoneme.N, Phoneme.NG
    };

    private static readonly HashSet<Phoneme> Stops = new()
    {
        Phoneme.B, Phoneme.D, Phoneme.G, Phoneme.K, Phoneme.P, Phoneme.T
    };

    private static readonly HashSet<Phoneme> Fricatives = new()
    {
        Phoneme.DH, Phoneme.F, Phoneme.HH, Phoneme.S, Phoneme.SH,
        Phoneme.TH, Phoneme.V, Phoneme.Z, Phoneme.ZH
    };

    public static bool IsVowel(this Phoneme phoneme) => Vowels.Contains(phoneme);
    public static bool IsNasal(this Phoneme phoneme) => Nasals.Contains(phoneme);
    public static bool IsStop(this Phoneme phoneme) => Stops.Contains(phoneme);
    public static bool IsFricative(this Phoneme phoneme) => Fricatives.Contains(phoneme);
    public static bool IsConsonant(this Phoneme phoneme) => !Vowels.Contains(phoneme);
}
