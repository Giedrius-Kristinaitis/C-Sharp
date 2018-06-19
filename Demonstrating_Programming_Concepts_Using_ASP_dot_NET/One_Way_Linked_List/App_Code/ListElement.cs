/*
 * Author: Giedrius Kristinaitis
 */

/// <summary>
/// Class used by CarList class (which is a linked list)
/// Contains information about data stored in this element and a reference to the next element
/// </summary>
public class ListElement {

    public CarData Data { get; set; } // data stored in this element
    public ListElement NextElement { get; set; } // reference to the next list element

    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="data">data about a car which will be stored in this element</param>
    /// <param name="nextElement">reference to the next list element</param>
	public ListElement(CarData data, ListElement nextElement) {
        Data = data;
        NextElement = nextElement;
	}

    /// <summary>
    /// Checks if one ListElement is less than another
    /// Used when sorting a list
    /// </summary>
    /// <param name="lhs">left-hand-side ListElement object of the comparison</param>
    /// <param name="rhs">right-hand-side ListElement object of the comparison</param>
    /// <returns>true if lhs is less than rhs</returns>
    public static bool operator < (ListElement lhs, ListElement rhs) {
        return lhs.Data < rhs.Data;
    }

    /// <summary>
    /// Checks if one ListElement is greater than another
    /// Used when sorting a list
    /// </summary>
    /// <param name="lhs">left-hand-side ListElement object of the comparison</param>
    /// <param name="rhs">right-hand-side ListElement object of the comparison</param>
    /// <returns>true if lhs is greater than rhs</returns>
    public static bool operator > (ListElement lhs, ListElement rhs) {
        return lhs.Data > rhs.Data;
    }
}
