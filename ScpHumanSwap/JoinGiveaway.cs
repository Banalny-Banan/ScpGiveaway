using System;
using System.Linq;
using CommandSystem;
using Exiled.API.Features;
using PlayerRoles;

namespace ScpGiveaway;

[CommandHandler(typeof(ClientCommandHandler))]
public class JoinGiveaway : ICommand, IUsageProvider
{
    public string Command { get; } = "JoinGiveaway";
    public string Description { get; } = "Allows you to join a giveaway for SCP role";
    public string[] Aliases { get; } = { "jg" };
    public string[] Usage { get; } = { "role" };

    public bool Execute(ArraySegment<string> arguments, ICommandSender sender, out string response)
    {
        string ActiveGiveaways()
        {
            return Giveaway.List.Count > 0 ? $"\n<color=#dcdcdc>Active giveaways: {string.Join(", ", Giveaway.List.Select(giveaway => $"[<color=#b8d7a3>{giveaway.ForRole}</color>]"))}</color>"
                : "\n<color=#dcdcdc>There are no active giveaways</color>";
        }

        if (Player.Get(sender) is not Player player)
        {
            response = "<color=#ff3333>You are not a player</color>";
            return false;
        }

        if (player.IsScp)
        {
            response = "<color=#ff3333>You are allready an SCP</color>";
            return false;
        }

        if (Giveaway.List.Count == 0 && !Plugin.Instance.Config.Debug)
        {
            response = "<color=#dcdcdc>There are no active giveaways</color>";
            return false;
        }

        if (arguments.Count != 1)
        {
            response = $"<color=#dcdcdc>Usage: JoinGiveaway <color=#9cdcfe><scp role></color></color>{ActiveGiveaways()}";
            return false;
        }

        //dont accept enum numbers, only names
        
        if (!Enum.TryParse(arguments.At(0), true, out RoleTypeId roleType) || !Enum.IsDefined(typeof(RoleTypeId), roleType))
        {
            response = $"<color=#dcdcdc>Invalid giveaway role \"<color=#ff3333>{arguments.At(0)}</color>\"</color>{ActiveGiveaways()}";
            return false;
        }

        if (Giveaway.List.FirstOrDefault(giv => giv.ForRole == roleType) is not Giveaway giveaway)
        {
            response = $"<color=#dcdcdc>Currently there is no giveaway for <color=#b8d7a3>{roleType}</color></color>{ActiveGiveaways()}";
            return false;
        }

        if (giveaway.Join(player))
        {
            response = $"<color=#dcdcdc>You have successfully joined the giveaway for <color=#b8d7a3>{roleType}</color>";
        }
        else
        {
            response = $"<color=#dcdcdc>You have already joined the giveaway for <color=#b8d7a3>{roleType}</color>";
        }
        
        response += $"There are [<color=#4ec9b0>{giveaway.Participants.Count}</color>] participants including you</color>";
        
        return true;
    }
}