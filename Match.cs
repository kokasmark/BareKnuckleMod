using CameraKit;
using DeadworksManaged.Api;
using DeadworksManaged.Game;

namespace BareKnucleMod;

public sealed class Match
{
    private Arena _arena;
    private readonly PlayerCameras _cameras;
    private readonly Dictionary<int, Fighter> _fighters = new();
    private readonly IHandle _npcSweep;

    public Match(DeadworksManaged.Api.ITimer timer)
    {
        _arena = Arenas.Choose();
        _cameras = new PlayerCameras(timer);
        _npcSweep = timer.Every(64.Ticks(), RemoveNpcs);
    }

    public static void RemoveStaleViews()
    {
        foreach (var controller in Players.GetAll())
            controller.GetHeroPawn()?.RemoveModifier(PlayerCameras.ViewModifier);
    }

    public void Start(string? arena)
    {
        _arena = arena == null
            ? Arenas.Choose()
            : Arenas.All.FirstOrDefault(x => x.Key == arena, Arenas.Choose());

        foreach (var controller in Players.GetAll().ToList())
            StartFor(controller);
    }

    public void Stop(CCitadelPlayerController caller)
    {
        _cameras.Release(caller.Slot);
    }

    public void UnLoad()
    {
        _npcSweep.Cancel();
        _cameras.ReleaseAll();
    }

    public void HideFromOthers(CheckTransmitEvent args) => _cameras.HideFromOthers(args);

    public void AbilityAttempt(AbilityAttemptEvent args)
    {
        if (!_fighters.TryGetValue(args.PlayerSlot, out var fighter))
            return;

        fighter.OnInput(args.HeldButtons, args.ChangedButtons);
        fighter.UpdateModifiers();

        args.BlockedButtons |= Globals.MoveMask;
    }

    private void StartFor(CCitadelPlayerController controller)
    {
        var slot = controller.Slot;

        if (_fighters.ContainsKey(slot))
        {
            controller.Log("The player is already in a match");
            return;
        }

        var pawn = controller.GetHeroPawn();

        if (pawn == null || !pawn.IsAlive)
            return;

        pawn.Teleport(_arena.SpawnPoint, null, System.Numerics.Vector3.Zero);

        var view = _cameras.Take(
            slot,
            pawn,
            _arena.CameraPosition,
            _arena.CameraAngles,
            _arena.CameraSettings.CameraFov,
            onRelease: () =>
            {
                if (_fighters.Remove(slot, out var ended))
                    ended.Restore();
            });

        if (view == null)
        {
            controller.Log("The camera was refused");
            return;
        }

        var fighter = new Fighter
        {
            Controller = controller,
            Pawn = pawn,
            Arena = _arena
        };

        fighter.Setup();

        _fighters[slot] = fighter;
        _cameras.EveryTick(view, () => Tick(slot));

        NetMessages.Send(new CCitadelUserMsg_HudGameAnnouncement
        {
            TitleLocstring = _arena.Name,
            DescriptionLocstring = "Starting match..."
        }, RecipientFilter.Single(slot));

        NetMessages.Send(new CCitadelUserMsg_HudGameAnnouncement
        {
            TitleLocstring = $"{pawn.HeroID.ToDisplayName()} vs ???",
            DescriptionLocstring = $"{controller.PlayerName} vs ???"
        }, RecipientFilter.Single(slot));

        Chat.PrintToChat(slot, "For the best experience run 'citadel_hud_visible false'");
    }

    private void Tick(int slot)
    {
        if (!_fighters.TryGetValue(slot, out var fighter))
            return;

        if (!fighter.IsActive || fighter.ExitRequested)
        {
            _cameras.Release(slot);
            return;
        }

        fighter.Update(CameraMath.TickInterval);
    }

    private static void RemoveNpcs()
    {
        var npcs = Entities.All
            .Where(e => e.IsValid &&
                        (e.DesignerName.Contains("npc_trooper", StringComparison.OrdinalIgnoreCase) ||
                         e.DesignerName.Contains("npc_boss", StringComparison.OrdinalIgnoreCase)))
            .ToArray();

        foreach (var npc in npcs)
        {
            if (npc.IsValid)
                npc.Remove();
        }
    }
}