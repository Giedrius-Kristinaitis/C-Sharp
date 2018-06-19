/*
 * Author: Giedrius Kristinaitis
 */

using System;
using System.Collections.Generic;

/// <summary>
/// Map class that finds one of possible paths from the man to a flower store
/// </summary>
public class Map {

    // char array containing data of all points on the map
    public char[,] MapData { get; private set; }

    // size of the map matrix (array)
    public int MapSize { get; private set; }

    /// <summary>
    /// class constructor that sets initial variable values
    /// </summary>
    /// <param name="mapData">2 dimensional char array containing data of all points on the map</param>
    /// <param name="mapSize">size of the map matrix</param>
    public Map(char[,] mapData, int mapSize) {
        MapData = mapData;
        MapSize = mapSize;
    }

    /// <summary>
    /// finds one of all possible paths from the man to a flower store and checks if the path
    /// is valid (no more than 5 quarters are passed on the way to the store)
    /// </summary>
    /// <param name="manPositionX">x coordinate of the current man's position</param>
    /// <param name="manPositionY">y coordinate of the current man's position</param>
    /// <param name="pathFound">boolean indicating wether a path was found</param>
    /// <param name="quartersPassed">number of quarters passed while going to the flower store</param>
    /// <returns>2 dimensional char array with map points in it.
    /// path to the store is marked with character 'K' in the array
    /// returns null if a path is not found</returns>
    public char[,] FindPathToFlowerStore(int manPositionX, int manPositionY, out bool pathFound,
                                            out int quartersPassed) {
        pathFound = false;
        quartersPassed = 0;

        for (int x = 0; x < MapSize; x++) {
            for (int y = 0; y < MapSize; y++) {
                char mapPoint = MapData[x, y];

                if (Char.IsLetter(mapPoint) && mapPoint == 'G') {
                    char[,] mapWithPath = FindPathToPoint(x, y, manPositionX, manPositionY,
                                            out pathFound, out quartersPassed);

                    if (pathFound) {
                        return mapWithPath;
                    }
                }
            }
        }

        return null;
    }

    /// <summary>
    /// finds a path from one point on the map to another and marks it in the map
    /// </summary>
    /// <param name="fromX">x coordinate of the starting point</param>
    /// <param name="fromY">y coordinate of the starting point</param>
    /// <param name="toX">x coordinate of the destination point</param>
    /// <param name="toY">y coordinate of the destination point</param>
    /// <param name="pathFound">boolean indicating wether a path was found</param>
    /// <param name="quartersPassed">number of quarters passed while going from one point to another</param>
    /// <returns>2 dimensional char array with map points in it.
    /// path is marked with character 'K' in the array
    /// returns a copy of initial map if a path is not found</returns>
    private char[,] FindPathToPoint(int fromX, int fromY, int toX, int toY, out bool pathFound, 
                                        out int quartersPassed) {
        int[,] mapDistances = new int[MapSize, MapSize];
        FillDistancesArray(ref mapDistances, -1);

        List<Point> points = new List<Point>();
        points.Add(new Point(fromX, fromY));

        ProcessPoints(ref mapDistances, points, 0);

        return AddPathToMap(mapDistances, toX, toY, fromX, fromY, out pathFound, out quartersPassed);
    }

    /// <summary>
    /// marks a path on the map with character 'K'
    /// </summary>
    /// <param name="mapDistances">2 dimensional integer array containing each point's
    /// distance from the destination point</param>
    /// <param name="fromX">x coordinate of the starting point</param>
    /// <param name="fromY">y coordinate of the starting point</param>
    /// <param name="toX">x coordinate of the destination point</param>
    /// <param name="toY">y coordinate of the destination point</param>
    /// <param name="pathValid">boolean indicating wether a path is found and is also valid</param>
    /// <param name="quartersPassed">number of quarters passed while travelling from
    /// one point to another</param>
    /// <returns>2 dimensional char array with the path marked on the map
    /// returns a copy of initial map array if a path is not found</returns>
    private char[,] AddPathToMap(int[,] mapDistances, int fromX, int fromY,
                                int toX, int toY, out bool pathValid, out int quartersPassed) {
        pathValid = false;
        quartersPassed = 1;
        char[,] mapData = (char[,]) MapData.Clone();

        if (mapDistances[fromX, fromY] == -1) {
            return null;
        }
        
        MoveToPoint(ref mapData, mapDistances, new Point(fromX, fromY), ref quartersPassed);

        if (quartersPassed <= 5) {
            pathValid = true;
        }

        return mapData;
    }

    /// <summary>
    /// moves to a point on the map and marks it with character 'K'
    /// this method is used when marking a path on the map
    /// this method uses recursion to move to other points on the map
    /// </summary>
    /// <param name="mapData">2 dimensional char array with point data in it</param>
    /// <param name="mapDistances">2 dimensional integer array containing distance of each point
    /// from the destination point</param>
    /// <param name="point">point to move to and mark</param>
    /// <param name="quartersPassed">number of quarters passed while moving point by point</param>
    private void MoveToPoint(ref char[,] mapData, int[,] mapDistances, Point point,
                                ref int quartersPassed) {
        if (point == null) {
            return;
        }

        if (mapData[point.X, point.Y] == 'G') {
            mapData[point.X, point.Y] = 'K';
            return;
        }

        mapData[point.X, point.Y] = 'K';

        MoveToPoint(ref mapData, mapDistances, GetNextPoint(point, mapDistances), ref quartersPassed);

        if (MapData[point.X, point.Y] == '.') {
            quartersPassed++;
        }
    }

    /// <summary>
    /// gets next point to move to based on it's distance from the destination point
    /// </summary>
    /// <param name="currentPoint">point from which to move to the next one</param>
    /// <param name="mapDistances">2 dimensional integer array containing distance
    /// of each point from the destination point</param>
    /// <returns>next point to move to</returns>
    private Point GetNextPoint(Point currentPoint, int[,] mapDistances) {
        Point leftPoint = new Point(currentPoint.X - 1, currentPoint.Y);
        Point rightPoint = new Point(currentPoint.X + 1, currentPoint.Y);
        Point topPoint = new Point(currentPoint.X, currentPoint.Y - 1);
        Point bottomPoint = new Point(currentPoint.X, currentPoint.Y + 1);

        if (mapDistances[currentPoint.X, currentPoint.Y] == 0) {
            return null;
        }
        if (NeighbourPointIsValid(currentPoint, leftPoint, mapDistances, true) && 
            mapDistances[leftPoint.X, leftPoint.Y] < mapDistances[currentPoint.X, currentPoint.Y]) {
            return leftPoint;
        }
        if (NeighbourPointIsValid(currentPoint, rightPoint, mapDistances, true) &&
            mapDistances[rightPoint.X, rightPoint.Y] < mapDistances[currentPoint.X, currentPoint.Y]) {
            return rightPoint;
        }
        if (NeighbourPointIsValid(currentPoint, topPoint, mapDistances, true) &&
            mapDistances[topPoint.X, topPoint.Y] < mapDistances[currentPoint.X, currentPoint.Y]) {
            return topPoint;
        }
        if (NeighbourPointIsValid(currentPoint, bottomPoint, mapDistances, true) &&
            mapDistances[bottomPoint.X, bottomPoint.Y] < mapDistances[currentPoint.X, currentPoint.Y]) {
            return bottomPoint;
        }

        return null;
    }

    /// <summary>
    /// loops through a list of points and processes each point
    /// processing of a point is done by setting it's distance from the first processed point
    /// (which has a distance of 0) and getting neighbour points which will be processed later
    /// when the method calls itself with new list of points to process
    /// </summary>
    /// <param name="mapDistances">2 dimensional integer array containing distance
    /// of each point from the first point</param>
    /// <param name="points">list of points to process</param>
    /// <param name="distanceFromSource">distance from the first processed point (increased 
    /// each time this method calls itself with new points to process)</param>
    private void ProcessPoints(ref int[,] mapDistances, List<Point> points, int distanceFromSource) {
        for (int i = 0; i < points.Count; i++) {
            mapDistances[points[i].X, points[i].Y] = distanceFromSource;
        }

        List<Point> neighbourPoints = GetNeighbourPoints(points, mapDistances);

        if (neighbourPoints.Count > 0) {
            ProcessPoints(ref mapDistances, neighbourPoints, distanceFromSource + 1);
        }
    }

    /// <summary>
    /// loops through a list of points and for each point gets all the points that are next
    /// to it (neighbour points) and stores them in another list
    /// a point is added to the new list if it doesn't already exist in it
    /// </summary>
    /// <param name="points">list of points to loop through</param>
    /// <param name="mapDistances">2 dimensional integer array containing distance
    /// of each point from the first point</param>
    /// <returns>list of all points that are next to the points that are being looped through</returns>
    private List<Point> GetNeighbourPoints(List<Point> points, int[,] mapDistances) {
        List<Point> neighbourPoints = new List<Point>();

        for (int i = 0; i < points.Count; i++) {
            Point point = points[i];

            AddNeighbourPointsToList(neighbourPoints, GetNeighbourPoints(point, mapDistances));
        }

        return neighbourPoints;
    }

    /// <summary>
    /// adds all neighbour points from one list to another list of neighbour points
    /// </summary>
    /// <param name="neighbourPoints">list of points to add to</param>
    /// <param name="neighbourPointsToAdd">list of points that will be added to list 'neighbourPoints'</param>
    private void AddNeighbourPointsToList(List<Point> neighbourPoints, List<Point> neighbourPointsToAdd) {
        for (int i = 0; i < neighbourPointsToAdd.Count; i++) {
            if (!PointExists(neighbourPoints, neighbourPointsToAdd[i])) {
                neighbourPoints.Add(neighbourPointsToAdd[i]);
            }
        }
    }

    /// <summary>
    /// checks if a point exists in a list
    /// </summary>
    /// <param name="points">list of points to check</param>
    /// <param name="point">point which existence will be checked</param>
    /// <returns>true if a point exists in the list, false otherwise</returns>
    private bool PointExists(List<Point> points, Point point) {
        for (int i = 0; i < points.Count; i++) {
            if (points[i].X == point.X && points[i].Y == point.Y) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// gets all valid points that are next to the given point
    /// </summary>
    /// <param name="point">point which neighbours to get</param>
    /// <param name="mapDistances">2 dimensional integer array containing distance
    /// of each point from the first point</param>
    /// <returns>list of neighbour points</returns>
    private List<Point> GetNeighbourPoints(Point point, int[,] mapDistances) {
        List<Point> neighbourPoints = new List<Point>();

        Point leftPoint = new Point(point.X - 1, point.Y);
        Point rightPoint = new Point(point.X + 1, point.Y);
        Point topPoint = new Point(point.X, point.Y - 1);
        Point bottomPoint = new Point(point.X, point.Y + 1);

        if (NeighbourPointIsValid(point, leftPoint, mapDistances, false)) {
            neighbourPoints.Add(leftPoint);
        }
        if (NeighbourPointIsValid(point, rightPoint, mapDistances, false)) {
            neighbourPoints.Add(rightPoint);
        }
        if (NeighbourPointIsValid(point, topPoint, mapDistances, false)) {
            neighbourPoints.Add(topPoint);
        }
        if (NeighbourPointIsValid(point, bottomPoint, mapDistances, false)) {
            neighbourPoints.Add(bottomPoint);
        }

        return neighbourPoints;
    }

    /// <summary>
    /// checks if a neighbour point is valid (it is not out of the map bounds and can be passed)
    /// </summary>
    /// <param name="currentPoint">any valid point on the map</param>
    /// <param name="neighbourPoint">neighbour point of the current point</param>
    /// <param name="mapDistances">2 dimensional integer array containing distance
    /// of each point from the first point</param>
    /// <param name="ignoreDistance">boolean indicating wether point's distance from the starting 
    /// pisition should be ignored when validating it</param>
    /// <returns>true if the neighbour point is valid, false otherwise</returns>
    private bool NeighbourPointIsValid(Point currentPoint, Point neighbourPoint, int[,] mapDistances,
                                        bool ignoreDistance) {
        if (neighbourPoint.X < 0 || neighbourPoint.X >= MapSize) {
            return false;
        }
        if (neighbourPoint.Y < 0 || neighbourPoint.Y >= MapSize) {
            return false;
        }
        if (!ignoreDistance && mapDistances[neighbourPoint.X, neighbourPoint.Y] != -1) {
            return false;
        }
        if (MapData[neighbourPoint.X, neighbourPoint.Y] == '1') {
            return false;
        }
        if (MapData[currentPoint.X, currentPoint.Y] == '.' && MapData[neighbourPoint.X, neighbourPoint.Y] == '.') {
            return false;
        }

        return true;
    }

    /// <summary>
    /// fills an array containing distances of each point with a given value
    /// </summary>
    /// <param name="mapDistances">array to fill</param>
    /// <param name="value">value to fill with</param>
    private void FillDistancesArray(ref int[,] mapDistances, int value) {
        for (int i = 0; i < MapSize; i++) {
            for (int j = 0; j < MapSize; j++) {
                mapDistances[i, j] = value;
            }
        }
    }
}
