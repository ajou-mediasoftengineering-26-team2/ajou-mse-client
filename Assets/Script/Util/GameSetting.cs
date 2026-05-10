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