/*
 * Author: Giedrius Kristinaitis
 */

using System;
using System.IO;

/// <summary>
/// Class that manages reading from streams and writing to files
/// </summary>
public class FileStorage {

    /// <summary>
    /// Reads car and driver data from a given stream and stores it in a given list
    /// </summary>
    /// <param name="stream">file to read from</param>
    /// <param name="cars">list to store car data</param>
    /// <param name="drivers">list to store driver data</param>
    public static void ReadData(Stream stream, List<Driver> drivers, List<Car> cars) {
        using (StreamReader reader = new StreamReader(stream, System.Text.Encoding.GetEncoding(1257))) {
            string line = reader.ReadLine();
            string[] data = line.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            int numOfDrivers = int.Parse(data[0]);
            int numOfCars = int.Parse(data[1]);

            ReadDriverData(reader, drivers, numOfDrivers);
            ReadCarData(reader, cars, numOfCars);
        }
    }

    /// <summary>
    /// Reads driver data from a given stream reader
    /// </summary>
    /// <param name="reader">StreamReader to read from</param>
    /// <param name="drivers">list to store drivers in</param>
    /// <param name="numOfDrivers">number of drivers to read</param>
    private static void ReadDriverData(StreamReader reader, List<Driver> drivers, int numOfDrivers) {
        for (int i = 0; i < numOfDrivers; i++) {
            string line = reader.ReadLine();
            string[] data = line.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            drivers.Add(new Driver(data[0], data[1]));
        }
    }

    /// <summary>
    /// Reads car data from a given stream reader
    /// </summary>
    /// <param name="reader">StreamReader to read from</param>
    /// <param name="drivers">list to store cars in</param>
    /// <param name="numOfDrivers">number of cars to read</param>
    private static void ReadCarData(StreamReader reader, List<Car> cars, int numOfCars) {
        for (int i = 0; i < numOfCars; i++) {
            string line = reader.ReadLine();
            string[] data = line.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

            cars.Add(new Car(data[0], data[1], int.Parse(data[2]), int.Parse(data[3])));
        }
    }

    /// <summary>
    /// Writes initial car data (before any operations are performed on it) to a file
    /// </summary>
    /// <param name="fileName">file to write to</param>
    /// <param name="cars">list containing car data to write</param>
    /// <param name="drivers">list containing car data to write</param>
    public static void WriteInitialDataToFile(string fileName, List<Car> cars, List<Driver> drivers) {
        using (StreamWriter writer = new StreamWriter(fileName)) {
            writer.WriteLine(string.Format("{0, -30} {1, -10} {2, -10} {3, -10} {4, -30}", 
                "Vairuotojas(-ai)", "Markė", "Numeris", "Metai", "Rida (tūkst. km per metus)"));
            
            foreach (Car car in cars) {
                writer.WriteLine(string.Format("{0, -30} {1}", car.GetDriverNames(drivers), car.ToString()));
            }
        }
    }

    /// <summary>
    /// Writes program's execution results to a file
    /// </summary>
    /// <param name="fileName">file to write to</param>
    /// <param name="cars">list containing sorted car list that are aged between minAge and maxAge</param>
    /// <param name="cars">list containing all drivers</param>
    /// <param name="minAge">minimum age of the car</param>
    /// <param name="maxAge">maximum age of the car</param>
    /// <param name="mostDrivenModel">car model(-s) that are driven the most per year</param>
    public static void WriteResultsToFile(string fileName, List<Car> cars, List<Driver> drivers, int minAge, 
        int maxAge, string mostDrivenModel) {

        using (StreamWriter writer = new StreamWriter(fileName)) {
            if (mostDrivenModel == null) {
                writer.WriteLine("Duomenų failas tuščias, todėl rezultatų nėra!");
                return;
            }

            if (!cars.IsEmpty()) {
                writer.WriteLine(string.Format("Mašinos, kurių amžius yra tarp {0} ir {1} metų:", minAge, maxAge));
                writer.WriteLine(string.Format("{0, -30} {1, -10} {2, -10} {3, -10} {4, -30}",
                    "Vairuotojas(-ai)", "Markė", "Numeris", "Metai", "Rida (tūkst. km per metus)"));

                foreach (Car car in cars) {
                    writer.WriteLine(string.Format("{0, -30} {1}", car.GetDriverNames(drivers), car.ToString()));
                }
            } else {
                writer.WriteLine(string.Format("Nėra mašinų, kurių amžius yra tarp {0} ir {1} metų:", minAge, maxAge));
            }
            
            writer.Write("Labiausiai eksploatuojamo(-ų) automobilio(-ių) markė(-ės): " + mostDrivenModel);
        }
    }
}
