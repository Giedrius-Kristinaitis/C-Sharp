/*
 * Author: Giedrius Kristinaitis
 */

/// <summary>
/// Point class to store point coordinates
/// </summary>
public class Point {

    public int X { get; set; } // x coordinate of the point
    public int Y { get; set; } // y coordinate of the point

    /// <summary>
    /// class constructor that sets initial variable values
    /// </summary>
    /// <param name="x">x coordinate of the point</param>
    /// <param name="y">y coordinate of the point</param>
    public Point(int x, int y) {
        X = x;
        Y = y;
    }
}
