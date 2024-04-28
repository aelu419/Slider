
using System.Collections.Generic;

public static class Areas
{
    // This looks a bit horrendous, I know, but StackExchange offers this input:
    // https://stackoverflow.com/questions/268084/creating-a-constant-dictionary-in-c-sharp
    // It's mostly autogenerated anyway and I find it quite readable, so I think it's fine!
    public static string GetDiscordName(this Area area) => area switch
    {
        Area.None => "None",
        Area.Village => "Exploring the Village",
        Area.Caves => "Speluking in the Caves",
        Area.Ocean => "Sailing the Ocean",
        Area.Jungle => "Traversing the Jungle",
        Area.Desert => "Trekking Through the Desert",
        Area.Factory => "Powering up the Factory",
        Area.Mountain => "Riding Through the Mountain",
        Area.Military => "Commiting Warcrimes",
        Area.MagiTech => "Playing With Portals",
        // Area.Space => "Exploring the Galaxy",
        _ => "None"
    };

    public static string GetDisplayName(this Area area) => area switch
    {
        Area.None => "None",
        Area.Village => "Stonybrook Village",
        Area.Caves => "Chloro Caves",
        Area.Ocean => "Shifting Seas",
        Area.Jungle => "Canopy Town",
        Area.Desert => "The Impact Zone",
        Area.Factory => "Rocket Factory Center",
        Area.Mountain => "Crystal Mines",
        Area.Military => "The Flats",
        Area.MagiTech => "MRI",
        // Area.Space => "Space",
        _ => "None"
    };
}

public enum Area {
    None,
    Village,
    Caves,
    Ocean,
    Jungle,
    Desert,
    Factory,
    Mountain,
    Military,
    MagiTech
    // Space
}

public enum bottleState { 
    empty, 
    cactus, 
    dirty, 
    clean 
}

public enum MinecartState {
    Empty,
    Crystal,
    Lava,
}