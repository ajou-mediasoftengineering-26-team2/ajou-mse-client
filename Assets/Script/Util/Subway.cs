using System.Collections.Generic;
//202322158 이준상
public enum StationType
{
    Unknown,
    CityHall,     // CITY_HALL
    Seongsu,      // SEONGSU
    Gangnam,      // GANGNAM
    Sillim,       // SILLIM
    HongikUniv    // HONGIK_UNIV
}

public static class StationConverter
{
    // 서버 문자열 -> Enum 매핑
    private static readonly Dictionary<string, StationType> ServerToEnum = new Dictionary<string, StationType>
    {
        { "CITY_HALL", StationType.CityHall },
        { "SEONGSU", StationType.Seongsu },
        { "GANGNAM", StationType.Gangnam },
        { "SILLIM", StationType.Sillim },
        { "HONGIK_UNIV", StationType.HongikUniv }
    };

    // Enum -> 표시용 텍스트 매핑
    private static readonly Dictionary<StationType, string> EnumToDisplayName = new Dictionary<StationType, string>
    {
        { StationType.CityHall, "City Hall" },
        { StationType.Seongsu, "Seongsu" },
        { StationType.Gangnam, "Gangnam" },
        { StationType.Sillim, "Sillim" },
        { StationType.HongikUniv, "Hongik Univ." }
    };

    // 서버 값으로부터 Enum 가져오기
    public static StationType GetType(string serverValue)
    {
        if (string.IsNullOrEmpty(serverValue)) return StationType.Unknown;
        return ServerToEnum.TryGetValue(serverValue.ToUpper(), out var type) ? type : StationType.Unknown;
    }

    // Enum으로부터 표시용 이름 가져오기 (Sillim, Hongik Univ 등)
    public static string GetDisplayName(StationType type)
    {
        return EnumToDisplayName.TryGetValue(type, out var name) ? name : "Unknown";
    }
}