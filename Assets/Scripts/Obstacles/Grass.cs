using UnityEngine;

/// This Obstacle class just wraps the hierarchy — the actual growth
/// logic lives on GrowableSurface (implements IGrowable).
public class Grass : Obstacle
{
    // No runtime logic needed here — PlantGrowthAbilitySO targets the
    // GrowableSurface child directly, and GrowableSurface activates
    // the bridge geometry itself.
}
