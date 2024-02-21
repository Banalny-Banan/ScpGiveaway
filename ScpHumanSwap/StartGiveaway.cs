using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;

namespace ScpGiveaway;

[CommandHandler(typeof(ClientCommandHandler))]
public class StartGiveaway : ICommand, IUsageProvider
{
    public string Command { get; } = "StartGiveaway";

    public string Description { get; } = "Starts a giveaway for your scp role";

    public string[] Aliases { get; } = { "human" };

    public string[] Usage { get; } = { };

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        if (Round.ElapsedTime.TotalSeconds > Plugin.Instance.Config.TimeToStart && !Plugin.Instance.Config.Debug)
        {
            response = $"{Round.ElapsedTime.TotalSeconds:0}/{Plugin.Instance.Config.TimeToStart:0} seconds have passed since the round started. Its too late to swap";
            return false;
        }

        if (Player.Get(sender) is not Player player)
        {
            response = "<color=#ff3333>You are not a player</color>";
            return false;
        }

        if (!player.IsScp)
        {
            response = "<color=#ff3333>You are not an SCP</color>";
            return false;
        }

        if (Giveaway.List.Any(giveaway => giveaway.Starter == player))
        {
            response = "<color=#ff3333>You have already started a giveaway</color>";
            return false;
        }

        Giveaway.Start(player);

        response = $"<color=#dcdcdc>You have started a giveaway for <color=#b8d7a3>{player.Role.Type}</color>. It will end in {Plugin.Instance.Config.GiveawayTime} seconds</color>";
        return true;
    }
}