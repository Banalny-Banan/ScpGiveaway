using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Features;
using MEC;
using Handlers = Exiled.Events.Handlers;
using Player = Exiled.API.Features.Player;

namespace ScpGiveaway;

public class Plugin : Plugin<Config>
{
    public override string Prefix => "ScpGiveaway";
    public override string Name => Prefix;
    public override string Author => "Banalny_Banan";
    public override Version Version { get; } = new Version(1, 0, 0);
    public static Plugin Instance;

    public override void OnEnabled()
    {
        Instance = this;
        Handlers.Server.RoundStarted += OnRoundStarted;
        base.OnEnabled();
    }

    public override void OnDisabled()
    {
        Instance = null;
        Handlers.Server.RoundStarted -= OnRoundStarted;
        base.OnDisabled();
    }

    private IEnumerator<float> OnRoundStarted()
    {
        Giveaway.List.Clear();
        yield return Timing.WaitForSeconds(1f);
        foreach (var player in Player.List.Where(ply => ply.IsScp))
        {
            player.Broadcast(5, "If you dont want to play as SCP, you can start a giveaway by typing <color=#9cdcfe>.Human</color> in you console <color=#569cd6>[~]</color>");
        }
    }
}

