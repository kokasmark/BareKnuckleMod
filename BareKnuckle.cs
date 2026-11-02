using CameraKit;
using DeadworksManaged.Api;

namespace BareKnucleMod;

public class BareKnuckle : DeadworksPluginBase
{
    public override string Name => "Bare Knuckle";

    private Match? _match;
    private Match Current => _match ??= new Match(Timer);

    public override void OnLoad(bool isReload)
    {
        if (isReload)
            Match.RemoveStaleViews();
    }

    public override void OnUnload() => Current.UnLoad();

    public override void OnCheckTransmit(CheckTransmitEvent args) => Current.HideFromOthers(args);

    public override void OnAbilityAttempt(AbilityAttemptEvent args) => Current.AbilityAttempt(args);

    [Command("start", Description = "Bare Knuckle: Starting the match.")]
    public void CmdStart(CCitadelPlayerController caller, string? arena = null) => Current.Start(arena);

    [Command("stop", Description = "Bare Knuckle: Stopping the match.")]
    public void CmdStop(CCitadelPlayerController caller) => Current.Stop(caller);

    [Command("map_list", Description = "Bare Knuckle: Lists the name and keys of the Arenas.")]
    public void CmdMapList(CCitadelPlayerController caller)
    {
        foreach(var arena in Arenas.All)
        {
            Chat.PrintToChat(caller.Slot, $"{arena.Name} - {arena.Key}");
        }   
    }
}