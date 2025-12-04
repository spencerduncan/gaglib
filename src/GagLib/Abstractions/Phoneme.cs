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
