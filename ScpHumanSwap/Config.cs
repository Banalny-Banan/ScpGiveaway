using System.ComponentModel;
using Exiled.API.Interfaces;

namespace ScpGiveaway;

public class Config : IConfig
{
    public bool IsEnabled { get; set; } = true;
    public bool Debug { get; set; } = false;
    [Description("The time in seconds that SCPs get to start the giveaway from the beginning of the round")]
    public float TimeToStart { get; set; } = 45f;
    [Description("The time in seconds that players will have to join the giveaway")]
    public float GiveawayTime { get; set; } = 25f;
}