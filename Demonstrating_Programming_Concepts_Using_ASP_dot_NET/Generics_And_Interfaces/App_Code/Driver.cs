/*
 * Author: Giedrius Kristinaitis
 */

using System;

/// <summary>
/// Class storing driver's information: name and the number of the car
/// </summary>
public class Driver : IComparable, IEquatable<Driver> {

    public string Name { get; set; } // last name of the driver
    public string CarNumber { get; set; } // number plate of the car

    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="name">last name of the driver</param>
    /// <param name="carNumber">number plate of the car</param>
	public Driver(string name, string carNumber) {
        Name = name;
        CarNumber = carNumber;
	}

    /// <summary>
    /// Compares this instance of the object to a given object
    /// </summary>
    /// <param name="obj">object to compare to</param>
    /// <returns>
    ///     -1 if this object's sorting order is before the given object
    ///     0 if both objects are equal when sorting
    ///     1 if this object's sorting order is after the given object
    /// </returns>
    public int CompareTo(object obj) {
        Driver other = obj as Driver;

        if (ReferenceEquals(other, null)) {
            throw new ArgumentException("Argument must be of type Driver");
        } else {
            return string.Compare(Name, other.Name, StringComparison.CurrentCulture);
        }
    }

    /// <summary>
    /// Checks if this instance of the object is equal to a given Driver object
    /// </summary>
    /// <param name="other">object to compare to</param>
    /// <returns>true if both objects are equal (driver name matches)</returns>
    public bool Equals(Driver other) {
        if (string.Compare(Name, other.Name, StringComparison.CurrentCulture) == 0) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if this instance of the object is equal to a given object
    /// </summary>
    /// <param name="obj">object to compare to</param>
    /// <returns>true if both objects are equal (both are of 
    /// type Driver and driver name matches)</returns>
    public override bool Equals(object obj) {
        if (ReferenceEquals(obj, null)) {
            return false;
        }

        Driver other = obj as Driver;

        if (ReferenceEquals(other, null)) {
            return false;
        } else {
            return Equals(other);
        }
    }

    /// <summary>
    /// Gets the hash code of the current instance of the object
    /// </summary>
    /// <returns>hash code as integer</returns>
    public override int GetHashCode() {
        return Name.GetHashCode();
    }

    /// <summary>
    /// Formats the current instance of the object as a string to be displayed later
    /// </summary>
    /// <returns>formatted string with this object's field values</returns>
    public override string ToString() {
        return string.Format("{0, -20} {1, -20}", Name, CarNumber);
    }
}
