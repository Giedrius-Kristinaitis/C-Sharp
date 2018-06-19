using System;

/// <summary>
/// Player class containing player's information: team, name, surname, position
/// </summary>
public class Player : IComparable<Player>, IEquatable<Player> {

    public string Team { get; set; } // name of the player's team
    public string Name { get; private set; } // name of the player
    public string Surname { get; private set; } // surname of the player
    public string Position { get; set; } // position the player is playing in

    /// <summary>
    /// Class constructor with parameters
    /// </summary>
    /// <param name="team">name of the player's team</param>
    /// <param name="name">name of the player</param>
    /// <param name="surname">surname of the player</param>
    /// <param name="position">position the player is playing in</param>
    public Player(string team, string name, string surname, string position) {
        Team = team;
        Name = name;
        Surname = surname;
        Position = position;
    }

    /// <summary>
    /// Checks if a player object comes before, after or at the same position as the other object
    /// when sorting
    /// </summary>
    /// <param name="other">other player to compare to</param>
    /// <returns>negative number if this player's sorting order if before another object, 
    /// 0 if this player's sorting order is the same as the other player's,
    /// positive number if this player's sorting order is after the other player's</returns>
    public int CompareTo(Player other) {
        return Surname.CompareTo(other.Surname) == 0 ? Name.CompareTo(other.Name)
            : Surname.CompareTo(other.Surname);
    }

    /// <summary>
    /// Checks if a player object is equal to a given object
    /// </summary>
    /// <param name="obj">object to check for equality</param>
    /// <returns>true if objects are equal, false otherwise</returns>
    public override bool Equals(object obj) {
        if (!ReferenceEquals(obj, null)) {
            Player other = obj as Player;

            if (!ReferenceEquals(other, null)) {
                return Equals(other);
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if a player object is equal to a given player object
    /// </summary>
    /// <param name="other">player object to check for equality</param>
    /// <returns>true if objects are equal, false otherwise</returns>
    public bool Equals(Player other) {
        return Team.Equals(other.Team) && Name.Equals(other.Name) && Surname.Equals(other.Surname);
    }

    /// <summary>
    /// Returns the hash code of a player object
    /// </summary>
    /// <returns>hash code of the a object</returns>
    public override int GetHashCode() {
        return Team.GetHashCode() | Name.GetHashCode() | Surname.GetHashCode()
                | Position.GetHashCode();
    }

    /// <summary>
    /// Formats a player object's fields and turns it into a string.
    /// Used when printing player's information
    /// </summary>
    /// <returns>a formatted string with all of the information stored in the player object</returns>
    public override string ToString() {
        return string.Format("{0, -20} {1, -20} {2, -20} {3, -10}", 
            Team, Surname, Name, Position);
    }
}