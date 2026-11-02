using DeadworksManaged.Api;
using DeadworksManaged.Game;

namespace BareKnucleMod;

public static class Globals
{
    public const float MoveSpeed = 450f;
    public const float Acceleration = 4000f;
    public const float TurnRate = 1200f;

    public const float BoundsInset = 16f;

    public const float LookAngle = 70f;

    public const float SpawnLift = 100f;

    public const InputButton MoveMask =
        InputButton.Forward | InputButton.Back | InputButton.MoveLeft | InputButton.MoveRight;

    public static readonly EModifierState[] StatesToRemove =
    {
        EModifierState.HideoutIntro,
    };

    public static readonly EModifierState[] StatesToAdd =
    {
        EModifierState.InfiniteStamina,
        EModifierState.MeleeAttacksOnly,
        EModifierState.HideAmmo,
        EModifierState.HideCrosshair,
    };

    public static readonly EModifierState[] MovementAbilityStates =
    {
        EModifierState.GroundDashing,
        EModifierState.AirDashing,
        EModifierState.Sliding
    };

    public static void Log(this CCitadelPlayerController controller, string message) {
        controller.PrintToConsole($"[Bare Knuckle]: {message}");
    }
}