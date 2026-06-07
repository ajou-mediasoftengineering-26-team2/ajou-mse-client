using System.Collections.Generic;
//202322158 이준상
/// <summary>
/// Enum defining the various subway station types.
/// </summary>
public enum StationType
{
    Unknown,
    CityHall,     // CITY_HALL
    Seongsu,      // SEONGSU
    Gangnam,      // GANGNAM
    Sillim,       // SILLIM
    HongikUniv    // HONGIK_UNIV
}

/// <summary>
/// Utility class responsible for converting between server strings, internal Enums, and UI display names.
/// </summary>
public static class StationConverter
{
    // Mapping from raw server strings to their corresponding StationType Enums.
    private static readonly Dictionary<string, StationType> ServerToEnum = new Dictionary<string, StationType>
    {
        { "CITY_HALL", StationType.CityHall },
        { "SEONGSU", StationType.Seongsu },
        { "GANGNAM", StationType.Gangnam },
        { "SILLIM", StationType.Sillim },
        { "HONGIK_UNIV", StationType.HongikUniv }
    };
    
    
    public static readonly Dictionary<string, (string Title, string Description)> StationToDescription = new Dictionary<string, (string Title, string Description)>
    {
        { 
            "HONGIK_UNIV", 
            ("This station is chaotic.", "Stat levels are maximized from the start.") 
        },
        { 
            "CITY_HALL", 
            ("Law Abiding Citizen / Certain actions are prohibited at this station (nothing happens if you perform them).", "Prohibited actions are randomized.") 
        },
        { 
            "SEONGSU", 
            ("This station is a landmark party. Welcome to the wonderland!", "Increased number of items.") 
        },
        { 
            "GANGNAM", 
            ("This station is exhausting.", "Max HP is lowered.") 
        },
        { 
            "SILLIM", 
            ("This station is a residential area. Slow.", "No changes.") 
        }
    };

    // Mapping from StationType Enums to user-friendly text for the UI.
    private static readonly Dictionary<StationType, string> EnumToDisplayName = new Dictionary<StationType, string>
    {
        { StationType.CityHall, "City Hall" },
        { StationType.Seongsu, "Seongsu" },
        { StationType.Gangnam, "Gangnam" },
        { StationType.Sillim, "Sillim" },
        { StationType.HongikUniv, "Hongik Univ." }
    };

    /// <summary>
    /// Converts a server-provided string value into the matching StationType Enum.
    /// </summary>
    /// <param name="serverValue">The raw string from the database/server (e.g., "GANGNAM")</param>
    public static StationType GetType(string serverValue)
    {
        if (string.IsNullOrEmpty(serverValue)) return StationType.Unknown;
        
        // Use ToUpper() for case-insensitive matching and return Unknown if not found.
        return ServerToEnum.TryGetValue(serverValue.ToUpper(), out var type) ? type : StationType.Unknown;
    }

    /// <summary>
    /// Retrieves the clean display name for a given StationType.
    /// </summary>
    /// <param name="type">The Enum value used in code</param>
    public static string GetDisplayName(StationType type)
    {
        // Returns "Unknown" if the Enum is not registered in the dictionary.
        return EnumToDisplayName.TryGetValue(type, out var name) ? name : "Unknown";
    }
}