using System;

namespace _src.Scripts.Colliders.Colliders.Data
{
    /// <summary>
    /// Describes the immediate consequence or state related to the player's "tip" (forwardmost point)
    /// interacting with an obstacle's volume. This often serves as a precursor or warning.
    /// </summary>
    [Flags]
    public enum PlayerTipInteraction
    {
        /// <summary>
        /// No specific interaction involving the tip is currently active.
        /// </summary>
        None = 0,

        /// <summary>
        /// The player's tip has entered a detection zone, indicating an obstacle or interactable is directly ahead.
        /// (e.g., visual/audio warning, object highlights). This is the generic "something is coming" state.
        /// </summary>
        ApproachingHazard = 1,

        /// <summary>
        /// The player's tip has entered a zone indicating an upcoming beneficial interactable (e.g. powerup, ramp).
        /// </summary>
        ApproachingBenefit = 2,

        /// <summary>
        /// The tip entering this specific zone triggers a contextual hint or tutorial message about the upcoming obstacle/mechanic.
        /// This can be used for the "hints for tip inside" if the hint appears *as* the tip enters, preparing for the "inside" part.
        /// </summary>
        HintTriggerActivated = 3,

        /// <summary>
        /// The player's tip makes a glancing contact, resulting in a minor, non-critical effect.
        /// (e.g., sparks, a brief slowdown, a specific sound, minor score penalty/bonus for "grazing").
        /// </summary>
        GrazeEffect = 4,

        /// <summary>
        /// The tip interaction causes a slight deflection or pushback, altering the player's trajectory minimally.
        /// </summary>
        MinorDeflection = 5,

        /// <summary>
        /// Tip enters a "soft" barrier that slows the player down slightly before a potential full collision.
        /// </summary>
        SoftBarrierContact = 6,
    }
}