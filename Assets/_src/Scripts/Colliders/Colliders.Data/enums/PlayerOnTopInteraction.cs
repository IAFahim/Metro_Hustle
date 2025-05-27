using System;

namespace _src.Scripts.Colliders.Colliders.Data.enums
{
    [Flags]
    public enum PlayerOnTopInteraction
    {
        /// <summary>
        /// No specific interaction from being on top (should not happen if IsOnTopOfBox is true, unless it's a passthrough).
        /// </summary>
        None = 0,

        // --- Standard Surfaces ---
        /// <summary>
        /// A stable, walkable surface. Player can run normally.
        /// Quality of landing (perfect, good, sloppy) can be determined by ActualHeightOnTop.
        /// </summary>
        StablePlatform = 1,

        /// <summary>
        /// Player achieved a "perfect" landing (ActualHeightOnTop is very low), possibly granting a bonus.
        /// </summary>
        StablePlatformPerfectLanding = 2,

        /// <summary>
        /// Player landed but it was "sloppy" or "precarious" (ActualHeightOnTop is high relative to player leg height),
        /// potentially causing a brief stumble animation or increased risk of falling if it's a narrow platform.
        /// </summary>
        StablePlatformSloppyLanding = 3,

        // --- Surfaces with Special Properties ---
        /// <summary>
        /// Surface is slippery (e.g., ice), affecting player control.
        /// </summary>
        SlipperySurface = 4,

        /// <summary>
        /// Surface is bouncy (e.g., trampoline), launching the player upwards.
        /// </summary>
        BouncySurface = 5,

        /// <summary>
        /// Surface crumbles or breaks after a short time or on player's exit.
        /// </summary>
        CrumblingSurface = 6,

        /// <summary>
        /// Surface is sticky, slowing movement or making jumps harder.
        /// </summary>
        StickySurface = 7,

        /// <summary>
        /// Surface acts as a conveyor belt, moving the player in a specific direction.
        /// (Could be further broken down: ConveyorForward, ConveyorBackward, etc.)
        /// </summary>
        ConveyorSurface = 8,

        /// <summary>
        /// Surface causes damage while the player is on it (e.g., spikes, hot surface).
        /// </summary>
        DamagingSurface = 9,

        /// <summary>
        /// Surface provides healing or regeneration while the player is on it.
        /// </summary>
        HealingSurface = 10,

        /// <summary>
        /// Surface provides a speed boost while running on it.
        /// </summary>
        SpeedBoostSurface = 11,

        /// <summary>
        /// Surface slows the player down while running on it (e.g. shallow water, mud).
        /// </summary>
        SlowingSurface = 12,

        // --- Special Platform Types ---
        /// <summary>
        /// A very narrow platform or beam requiring precise balance (may trigger special animations/mechanics).
        /// </summary>
        NarrowLedge = 13,

        /// <summary>
        /// A moving platform (the platform itself translates/rotates).
        /// </summary>
        MovingPlatform = 14,

        /// <summary>
        /// A platform that is a designated safe zone or checkpoint.
        /// </summary>
        SafeZonePlatformOrCheckpoint = 15,

        /// <summary>
        /// A platform that signifies the end of a level or section.
        /// </summary>
        EndZonePlatform = 16,

        /// <summary>
        /// A platform that triggers a specific animation or action (e.g., grind rail start).
        /// </summary>
        ActionTriggerPlatform = 17, // e.g. start grinding, vault
    }
}