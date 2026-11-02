// Provided kindly by @jonas12294 on Discord, thank you!

using System.Numerics;
using DeadworksManaged.Api;
using DeadworksManaged.Game;

namespace CameraKit;

public sealed class PlayerCameras
{
    public const string ViewModifier = "modifier_citadel_hideout_intro";
    private const string CameraName = "intro_camera";

    private readonly DeadworksManaged.Api.ITimer _timer;
    private readonly Dictionary<int, CameraView> _views = new();

    public PlayerCameras(DeadworksManaged.Api.ITimer timer) => _timer = timer;

    public CameraView? Take(int slot, CCitadelPlayerPawn pawn, Vector3 origin, Vector3 angles, float fov, Action? onRelease = null)
    {
        Release(slot);

        var view = new CameraView(pawn);
        _views[slot] = view;

        if (Spawn.point_camera(new() { targetname = CameraName, origin = origin, angles = angles, FOV = fov })?.Entity is { } camera)
            view.Camera = camera;

        if (view.Camera == null || pawn.AddModifier(ViewModifier) == null)
        {
            Release(slot);
            return null;
        }

        view.OnRelease = onRelease;
        return view;
    }

    public void EveryTick(CameraView view, Action tick)
    {
        view.Ticker?.Cancel();
        view.Ticker = _timer.Every(1.Ticks(), tick);
    }

    public void Release(int slot)
    {
        if (!_views.Remove(slot, out var view))
            return;

        view.Ticker?.Cancel();

        if (view.Camera is { IsValid: true } camera)
            camera.Remove();

        if (view.Pawn.IsValid)
            view.Pawn.RemoveModifier(ViewModifier);

        view.OnRelease?.Invoke();
    }

    public void ReleaseAll()
    {
        foreach (var slot in _views.Keys.ToList())
            Release(slot);
    }

    public void HideFromOthers(CheckTransmitEvent args)
    {
        foreach (var (slot, view) in _views)
        {
            if (slot != args.PlayerSlot && view.Camera is { IsValid: true } camera)
                args.Hide(camera);
        }
    }
}

public sealed class CameraView
{
    public CCitadelPlayerPawn Pawn { get; }

    internal CBaseEntity? Camera;
    internal IHandle? Ticker;
    internal Action? OnRelease;

    internal CameraView(CCitadelPlayerPawn pawn) => Pawn = pawn;
}

public static class CameraMath
{
    public static float TickInterval => GlobalVars.IntervalPerTick > 0 ? GlobalVars.IntervalPerTick : 1f / 64f;
}