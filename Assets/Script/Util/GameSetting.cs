using System;
using System.Collections.Generic;
//202322158 이준상


/// <summary>
/// A static class that manages constants and configuration values used throughout the game.
/// </summary>
public class GameSetting
{
    // Points required to win a round
    public static readonly int ROUNDWINING = 3;
    // Base attack case value
    public static readonly int ATTACK = 5;
    // Base defense case value
    public static readonly int DEFANSE = 5;
    
    // System notification messages for login
    public static string LOGINSUCCESS = "Login successful.";
    public static string LOGINERROR = "Login failed.";
    
    // Maximum health points for the character
    public static readonly float maxHP = 10;
    
    // Standardized date-time format for database and logging
    public static string format = "yyyy-MM-dd'T'HH:mm:ss.fff"; // Use fff instead of SSS


    public static readonly Dictionary<CameraType, int> DELAY_MAP = new Dictionary<CameraType, int>()
    {
        { CameraType.Camera1, 0 },
        { CameraType.Camera2, 1500 }
    };
    
    public static bool TryParseHandAction(string handChoice, out HandActionType action)
    {
        if (string.IsNullOrWhiteSpace(handChoice))
        {
            action = HandActionType.SHAKE_OVER_HANDS;
            return false;
        }

        if (Enum.TryParse(handChoice, true, out action))
        {
            return true;
        }

        switch (handChoice.Trim().ToLowerInvariant())
        {
            case "left":
                action = HandActionType.SINGLE_HAND_FLIP_LEFT;
                return true;
            case "right":
                action = HandActionType.SINGLE_HAND_FLIP_RIGHT;
                return true;
            case "both":
                action = HandActionType.BOTH_HANDS_FLIP;
                return true;
            case "stab":
            case "slice":
            case "defense":
                action = HandActionType.INSERT_BETWEEN_HANDS;
                return true;
            case "wave":
            case "pause":
            case "ok":
            case "none":
                action = HandActionType.SHAKE_OVER_HANDS;
                return true;
            default:
                action = HandActionType.SHAKE_OVER_HANDS;
                return false;
        }
    }
}

/// <summary>
/// Enum defining the types of hand gestures/actions recognized in the game.
/// </summary>
public enum HandActionType
{
    /** Flipping the left hand (1) */
    SINGLE_HAND_FLIP_LEFT = 1,
    /** Flipping the right hand (2) */
    SINGLE_HAND_FLIP_RIGHT = 2,
    /** Flipping both hands simultaneously (3) */
    BOTH_HANDS_FLIP = 3,
    /** Inserting motion (4) */
    INSERT_BETWEEN_HANDS = 4,
    /** Shaking hands / Default idle state (5) */
    SHAKE_OVER_HANDS = 5,
}
