using System.Numerics;
using DeadworksManaged.Api;

namespace BareKnucleMod;

public class Fighter
{
    public required CCitadelPlayerController Controller;
    public required CCitadelPlayerPawn Pawn;
    public required Arena Arena;

    public bool ExitRequested;

    private InputButton _held;
    private bool _facingRight = true;
    private float _yaw;
    private float _pitch;
    private float _targetYaw;

    public bool IsActive => Pawn.IsValid && Pawn.IsAlive && Controller.IsValid;

    public void Setup()
    {
        _yaw = _targetYaw = FacingYaw(true);
        _pitch = 0f;

        foreach (var ability in Pawn.AbilityComponent.Abilities)
        {
            if (ability.IsSignature)
                ability.UpgradeBits = 0b11111;
        }

        // Pawn.Health = 500;
        UpdateModifiers();
    }

    public void Restore()
    {
        if (!Pawn.IsValid || Pawn.ModifierProp is not { } mods)
            return;

        foreach (var state in Globals.StatesToAdd)
            mods.SetModifierState(state, false);
    }

    public void OnInput(InputButton held, InputButton changed)
    {
        _held = held;

        if ((changed & held & InputButton.Use) != 0)
            ExitRequested = true;
    }

    public void UpdateModifiers()
    {
        if (Pawn.ModifierProp is not { } mods)
            return;

        foreach (var state in Globals.StatesToRemove)
            mods.SetModifierState(state, false);

        foreach (var state in Globals.StatesToAdd)
            mods.SetModifierState(state, true);
    }

    public void Update(float dt)
    {
        UpdateModifiers();

        var direction = Axis(InputButton.MoveRight, InputButton.MoveLeft);

        if (direction != 0f)
            Face(direction > 0f);

        var step = Globals.TurnRate * dt;
        _yaw = Approach(_yaw, _targetYaw, step);
        _pitch = Approach(_pitch, Axis(InputButton.Back, InputButton.Forward) * Globals.LookAngle, step);

        Move(direction, dt);

        Controller.SetCameraAngles(_pitch, _yaw, 0f);
        Pawn.SetRotation(new Vector3(_pitch, _yaw, 0f));
    }

    private float Axis(InputButton positive, InputButton negative)
    {
        return ((_held & positive) != 0 ? 1f : 0f) - ((_held & negative) != 0 ? 1f : 0f);
    }

    private static float WrapAngle(float angle)
    {
        return ((angle + 180f) % 360f + 360f) % 360f - 180f;
    }

    private static float Approach(float current, float target, float step)
    {
        return WrapAngle(current + Math.Clamp(WrapAngle(target - current), -step, step));
    }

    private float FacingYaw(bool right)
    {
        var direction = right ? Arena.Right : -Arena.Right;
        return MathF.Atan2(direction.Y, direction.X) * (180f / MathF.PI);
    }

    private void Face(bool right)
    {
        if (right == _facingRight)
            return;

        _facingRight = right;
        _targetYaw = FacingYaw(right);
    }

    private bool IsMovementAbilityActive()
    {
        if (Pawn.ModifierProp is not { } mods)
            return false;

        foreach (var state in Globals.MovementAbilityStates)
            if (mods.HasModifierState(state))
                return true;

        return false;
    }

    private void Move(float direction, float dt)
    {
        if (dt <= 0f || !Pawn.IsValid)
            return;

        var position = Pawn.Position;
        var velocity = Pawn.AbsVelocity;

        if (Arena.TryReturn(position, out var corrected))
        {
            Pawn.Teleport(corrected, null, Vector3.Zero);
            Pawn.AbsVelocity = Vector3.Zero;
            return;
        }

        if (IsMovementAbilityActive())
            return;

        var velocityAlong = Vector3.Dot(velocity, Arena.Right);
        var step = Globals.Acceleration * dt;
        velocityAlong += Math.Clamp(direction * Globals.MoveSpeed - velocityAlong, -step, step);

        var target = Arena.Right * velocityAlong;

        if (MathF.Abs(target.X - velocity.X) < 0.01f && MathF.Abs(target.Y - velocity.Y) < 0.01f)
            return;

        Pawn.AbsVelocity = new Vector3(target.X, target.Y, velocity.Z);
    }
}