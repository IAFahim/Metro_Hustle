using System;

namespace _src.Scripts.Colliders.Colliders.Data.enums
{
    /// <summary>
    /// Describes the outcomes when the player's main body is determined to be "inside" an obstacle or trigger volume.
    /// This is a [Flags] enum because multiple effects could theoretically occur simultaneously from being inside one volume
    /// (e.g., taking damage AND being slowed AND collecting a hidden item).
    /// </summary>
    [Flags]
    public enum PlayerInsideInteractionFlags
    {
        /// <summary>
        /// No specific interaction while inside this volume. Might be a passthrough decorative element.
        /// </summary>
        None = 0,

        // --- Negative Outcomes ---
        /// <summary>
        /// Player takes direct damage (e.g., running into spikes, electrified field).
        /// </summary>
        TakeDamage = 1 << 0,

        /// <summary>
        /// Player character dies or the run ends (e.g., hitting a solid wall at high speed, fatal trap).
        /// </summary>
        FatalCollision = 1 << 1,

        /// <summary>
        /// Player begins to fall (e.g., entering a trigger volume for a pit). Often leads to FatalCollision.
        /// </summary>
        FallIntoPit = 1 << 2,

        /// <summary>
        /// Player's movement is slowed (e.g., mud, goo, spider web).
        /// </summary>
        ApplySlowDebuff = 1 << 3,

        /// <summary>
        /// Player is stunned or temporarily loses control.
        /// </summary>
        ApplyStunDebuff = 1 << 4,

        /// <summary>
        /// Player is pushed or knocked back.
        /// </summary>
        ApplyPushback = 1 << 5,

        /// <summary>
        /// Player's vision is obscured (e.g., smoke, ink).
        /// </summary>
        ApplyObscuredVision = 1 << 6,

        // --- Positive or Neutral Outcomes ---
        /// <summary>
        /// Player collects a standard item (e.g., coin, point orb).
        /// </summary>
        CollectStandardItem = 1 << 7,

        /// <summary>
        /// Player collects a power-up item.
        /// </summary>
        CollectPowerUp = 1 << 8,

        /// <summary>
        /// Player collects a quest item or special collectible.
        /// </summary>
        CollectQuestItem = 1 << 9,

        /// <summary>
        /// Player receives a speed boost (e.g., running through a boost gate).
        /// </summary>
        ApplySpeedBoost = 1 << 10,

        /// <summary>
        /// Player receives healing or shield.
        /// </summary>
        ApplyHealingOrShield = 1 << 11,

        // --- Trigger-based Outcomes ---
        /// <summary>
        /// Player activates a mechanism or event (e.g., opens a door, triggers a moving platform elsewhere).
        /// </summary>
        ActivateTrigger = 1 << 12,

        /// <summary>
        /// Player enters a new zone or teleports.
        /// </summary>
        ZoneTransitionOrTeleport = 1 << 13,

        /// <summary>
        /// Player enters a safe zone or checkpoint.
        /// </summary>
        EnterSafeZoneOrCheckpoint = 1 << 14,

        /// <summary>
        /// Player enters an area that grants temporary invincibility.
        /// </summary>
        GainTemporaryInvincibility = 1 << 15,

        /// <summary>
        /// Interaction with a specific story element or dialogue trigger.
        /// </summary>
        StoryBeatOrDialogueTrigger = 1 << 16,

        /// <summary>
        /// Reveals a hidden path or area.
        /// </summary>
        RevealHiddenPath = 1 << 17,

        /// <summary>
        /// A purely cosmetic effect is triggered (e.g. passing through harmless particle effects).
        /// </summary>
        CosmeticEffectTrigger = 1 << 18,
    }
}