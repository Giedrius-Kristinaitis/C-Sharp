/*
 * Author: Giedrius Kristinaitis
 */

using System;
using System.IO;

/// <summary>
/// Class that manages reading from and writing to files
/// </summary>
public class FileStorage {

    /// <summary>
    /// Reads car data from a given file and stores it in a given list
    /// </summary>
    /// <param name="filePath">file to read from</param>
    /// <param name="list">list to store car data</param>
    public static void ReadCarData(string filePath, CarList list) {
        using (StreamReader reader = new StreamReader(filePath)) {
            string line = null;

            while ((line = reader.ReadLine()) != null) {
                string[] data = line.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                CarData carData = new CarData(
                    data[0], data[1], data[2], int.Parse(data[3]), int.Parse(data[4]));

                list.AddCar(carData);
            }
        }
    }

    /// <summary>
    /// Writes initial car data (before any operations are performed on it) to a file
    /// </summary>
    /// <param name="fileName">file to write to</param>
    /// <param name="list">list containing car data to write</param>
    public static void WriteInitialCarDataToFile(string fileName, CarList list) {
        using (StreamWriter writer = new StreamWriter(fileName)) {
            writer.WriteLine(string.Format("{0, -20} {1, -10} {2, -10} {3, -10} {4, -30}", 
                "Vairuotojas", "Markė", "Numeris", "Metai", "Rida (tūkst. km per metus)"));

            while (list.MoveToNextElement()) {
                writer.WriteLine(list.GetCurrentElement().ToString());
            }
        }
    }

    /// <summary>
    /// Writes program's execution results to a file
    /// </summary>
    /// <param name="fileName">file to write to</param>
    /// <param name="list">list containing sorted car list that are aged between minAge and maxAge</param>
    /// <param name="minAge">minimum age of the car</param>
    /// <param name="maxAge">maximum age of the car</param>
    /// <param name="mostDrivenModel">car model(-s) that are driven the most per year</param>
    public static void WriteResultsToFile(string fileName, CarList list, int minAge, 
        int maxAge, string mostDrivenModel) {

        using (StreamWriter writer = new StreamWriter(fileName)) {
            if (mostDrivenModel == null) {
                writer.WriteLine("Duomenų failas tuščias, todėl rezultatų nėra!");
                return;
            }

            if (list.Count > 0) {
                writer.WriteLine(string.Format("Mašinos, kurių amžius yra tarp {0} ir {1} metų:", minAge, maxAge));
                writer.WriteLine(string.Format("{0, -20} {1, -10} {2, -10} {3, -10} {4, -30}",
                    "Vairuotojas", "Markė", "Numeris", "Metai", "Rida (tūkst. km per metus)"));

                while (list.MoveToNextElement()) {
                    writer.WriteLine(list.GetCurrentElement().ToString());
                }
            } else {
                writer.WriteLine(string.Format("Nėra mašinų, kurių amžius yra tarp {0} ir {1} metų:", minAge, maxAge));
            }
            
            writer.Write("Labiausiai eksploatuojamo(-ų) automobilio(-ių) markė(-ės): " + mostDrivenModel);
        }
    }
}
