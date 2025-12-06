namespace GagLib;

/// <summary>
/// Types of gags available for speech transformation.
/// </summary>
public enum GagType
{
    /// <summary>
    /// Ball gag - blocks mouth, forces nasal sounds.
    /// </summary>
    BallGag,

    /// <summary>
    /// Cow gag - transforms speech into moo sounds.
    /// </summary>
    CowGag,

    /// <summary>
    /// Dog gag - cartoon dog style speech (ruh-roh!).
    /// </summary>
    DogGag,

    /// <summary>
    /// Barking dog gag - transforms speech into barks (ruff, woof, arf, bark, grr).
    /// </summary>
    BarkingDogGag
}
