/*
 * Author: Giedrius Kristinaitis
 */ 

using System;

/// <summary>
/// PlayerStatistics class containing player's statistical information: team, name, surname (used to identify
/// the player), minutes played, points scored, fouls made
/// </summary>
public class PlayerStatistics : IComparable<PlayerStatistics>, IEquatable<PlayerStatistics> {

    public string Team { get; set; } // team of the player
    public string Name { get; private set; } // name of the player
    public string Surname { get; private set; } // surname of the player
    public int MinutesPlayed { get; set; } // how many minutes the player spent in game
    public int PointsScored { get; set; } // how many points the player scored
    public int FoulsMade { get; set; } // how many fouls the player made

    /// <summary>
    /// Class constructor with parameters
    /// </summary>
    /// <param name="name">name of the player</param>
    /// <param name="surname">surname of the player</param>
    /// <param name="minutes">how many minutes the player spent in game</param>
    /// <param name="points">how many points the player scored</param>
    /// <param name="fouls">how many fouls the player made</param>
	public PlayerStatistics(string team, string name, string surname, int minutes, int points, int fouls) {
        Team = team;
        Name = name;
        Surname = surname;
        MinutesPlayed = minutes;
        PointsScored = points;
        FoulsMade = fouls;
	}

    /// <summary>
    /// Checks if a PlayerStatistics object comes before, after or at the same position as the other object
    /// when sorting
    /// </summary>
    /// <param name="other">other player's statistics to compare to</param>
    /// <returns>negative number if this player statistics' sorting order if before another object, 
    /// 0 if this player statistics' sorting order is the same as the other player's,
    /// positive number if this player statistics' sorting order is after the other player's</returns>
    public int CompareTo(PlayerStatistics other) {
        if (PointsScored == other.PointsScored) {
            if (MinutesPlayed == other.MinutesPlayed) {
                if (FoulsMade == other.FoulsMade) {
                    return 0;
                } else {
                    return FoulsMade.CompareTo(other.FoulsMade);
                }
            } else {
                return MinutesPlayed.CompareTo(other.MinutesPlayed);
            }
        } else {
            return PointsScored.CompareTo(other.PointsScored) * -1;
        }
    }

    /// <summary>
    /// Checks if a PlayerStatistics object is equal to a given object
    /// </summary>
    /// <param name="obj">object to check for equality</param>
    /// <returns>true if objects are equal, false otherwise</returns>
    public override bool Equals(object obj) {
        if (!ReferenceEquals(obj, null)) {
            PlayerStatistics other = obj as PlayerStatistics;

            if (!ReferenceEquals(other, null)) {
                return Equals(other);
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if a PlayerStatistics object is equal to a given PlayerStatistics object
    /// </summary>
    /// <param name="other">player object to check for equality</param>
    /// <returns>true if objects are equal, false otherwise</returns>
    public bool Equals(PlayerStatistics other) {
        return Team.Equals(other.Team) && Name.Equals(other.Name) && Surname.Equals(other.Surname) &&
            MinutesPlayed == other.MinutesPlayed && PointsScored == other.PointsScored && FoulsMade ==
            other.FoulsMade;
    }

    /// <summary>
    /// Returns the hash code of a PlayerStatistics object
    /// </summary>
    /// <returns>hash code of the a object</returns>
    public override int GetHashCode() {
        return Team.GetHashCode() | Name.GetHashCode() | Surname.GetHashCode()
            | MinutesPlayed.GetHashCode() | PointsScored.GetHashCode() | FoulsMade.GetHashCode();
    }

    /// <summary>
    /// Formats a PlayerStatistics object's fields and turns it into a string.
    /// Used when printing player's statistical information
    /// </summary>
    /// <returns>a formatted string with all of the information stored in the player object</returns>
    public override string ToString() {
        return string.Format("{0, -20} {1, -20} {2, -20} {3, -20} {4, -20} {5, -20}",
            Team, Surname, Name, MinutesPlayed, PointsScored, FoulsMade);
    }
}
