using System;
using System.Collections.Generic;
using System.Linq;
using Exiled.API.Extensions;
using Exiled.API.Features;
using MEC;
using PlayerRoles;
using UnityEngine;
using Utils.NonAllocLINQ;
using Random = UnityEngine.Random;

namespace ScpGiveaway;

public class Giveaway
{
    public Player Starter { get; }
    public RoleTypeId ForRole { get; }
    public HashSet<Player> Participants { get; } = new();
    public DateTime EndTime { get; }
    public int RemainingSeconds => Mathf.RoundToInt((float)(EndTime - DateTime.Now).TotalSeconds);

    public static Giveaway Start(Player starter) => new(starter);

    private Giveaway(Player starter)
    {
        Starter = starter;
        ForRole = starter.Role.Type;
        EndTime = DateTime.Now.AddSeconds(Plugin.Instance.Config.GiveawayTime);
        GiveawayCoroutine().RunCoroutine();
        foreach (var player in Player.List.Where(ply => !ply.IsScp))
        {
            player.Broadcast(8, $"You can join the giveaway for <color=#b8d7a3>{ForRole}</color> by typing <color=#9cdcfe>.JoinGiveaway {ForRole}</color> in your console <color=#569cd6>[~]</color>");
        }
        List.Add(this);
    }

    private IEnumerator<float> GiveawayCoroutine()
    {
        while (true)
        {
            yield return Timing.WaitForSeconds(1f);
            if (DateTime.Now >= EndTime)
            {
                End();
                List.Remove(this);
                yield break;
            }

            if (Starter.Role.Type != ForRole)
            {
                Starter.Broadcast(5, "Your role has changed, the giveaway has been cancelled");
                Participants.ForEach(ply => ply.Broadcast(5, $"Starter's role has changed, the giveaway for <color=#b8d7a3>{ForRole}</color> has been cancelled"));
                List.Remove(this);
                yield break;
            }

            if (RemainingSeconds % 5 == 0)
            {
                var message = $"<color=#dcdcdc>[<color=#569cd6>Giveaway</color>] {RemainingSeconds} seconds left | [<color=#4ec9b0>{Participants.Count}</color>] players joined</color>";
                Starter.SendConsoleMessage(message, "");
                Participants.ForEach(ply => ply.SendConsoleMessage(message, ""));
            }
        }
    }

    //true if player joined, false if player is already in the giveaway
    public bool Join(Player player)
    {
        if (Participants.Add(player))
        {
            string randomColor = Color.HSVToRGB(Random.value, 0.81f, 0.90f).ToHex();
            var message = $"<color=#dcdcdc>[<color=#569cd6>Giveaway</color>] <color={randomColor}>{player.Nickname}</color> has joined</color>";
            Starter.SendConsoleMessage(message, "");
            return true;
        }

        return false;
    }

    public void End()
    {
        if (Starter.IsDead)
        {
            foreach (var participant in Participants)
            {
                participant.Broadcast(5, $"The giveaway was cancelled because the starter ({Starter.Nickname}) died");
            }

            return;
        }

        var winner = Participants.Where(ply => ply is not null).GetRandomValue();
        if (winner is null)
        {
            Starter.Broadcast(5, "No one joined your giveaway");
            return;
        }

        Starter.Broadcast(5, $"{winner.Nickname} has won your giveaway");
        winner.Broadcast(5, $"You have won the giveaway for <color=#b8d7a3>{ForRole}</color>");
        SwapPlayers(Starter, winner);
    }

    private static void SwapPlayers(Player scp, Player human)
    {
        var scpInfo = (scp.Health, RoleType: scp.Role.Type, scp.Position);

        scp.Role.Set(human.Role.Type);
        scp.Position = human.Position;
        scp.Health = human.Health;
        foreach (var item in human.Items)
        {
            item.ChangeItemOwner(human, scp);
        }

        human.Role.Set(scpInfo.RoleType);
        human.Position = scpInfo.Position;
        human.Health = scpInfo.Health;
    }

    public static readonly List<Giveaway> List = new();
}