/*
 * Author: Giedrius Kristinaitis
 */

using System;

/// <summary>
/// Class containing information about a car: driver's name, model, number, production year, km driven
/// </summary>
public class CarData {

    public string DriverName { get; set; } // last name of the car's driver
    public string Model { get; set; } // model of the car
    public string Number { get; set; } // number plate of the car
    public int ProductionYear { get; set; } // year the car was made
    public int KmPerYear { get; set; } // km driven per year (in thousands)

    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="driverName">last name of the driver</param>
    /// <param name="model">model of the car</param>
    /// <param name="number">number plate of the car</param>
    /// <param name="productionYear">year the car was made</param>
    /// <param name="kmPerYear">km driven per year (in thousands)</param>
	public CarData(string driverName, string model, string number, int productionYear, int kmPerYear) {
        DriverName = driverName;
        Model = model;
        Number = number;
        ProductionYear = productionYear;
        KmPerYear = kmPerYear;
	}

    /// <summary>
    /// Calculates age of the car
    /// </summary>
    /// <returns>age of the car</returns>
    public int GetAge() {
        return DateTime.Now.Year - ProductionYear;
    }

    /// <summary>
    /// Checks if a car is less than another car, used when sorting cars
    /// comparison keys: model and age
    /// </summary>
    /// <param name="lhs">left-hand-side car object of the comparison</param>
    /// <param name="rhs">right-hand-side car object of the comparison</param>
    /// <returns>true if lhs is less than rhs</returns>
    public static bool operator < (CarData lhs, CarData rhs) {
        int modelComparisonResult = String.Compare(lhs.Model, rhs.Model, StringComparison.CurrentCulture);

        if (modelComparisonResult == 0) {
            return lhs.GetAge() < rhs.GetAge();
        } else {
            return modelComparisonResult < 0;
        }
    }

    /// <summary>
    /// Checks if a car is greater than another car, used when sorting cars
    /// comparison keys: model and age
    /// </summary>
    /// <param name="lhs">left-hand-side car object of the comparison</param>
    /// <param name="rhs">right-hand-side car object of the comparison</param>
    /// <returns>true if lhs is greater than rhs</returns>
    public static bool operator > (CarData lhs, CarData rhs) {
        return rhs < lhs;
    }

    /// <summary>
    /// Turns car information into a formatted string
    /// </summary>
    /// <returns>formatted string containg information from all Car class fields</returns>
    public override string ToString() {
        return string.Format("{0, -20} {1, -10} {2, -10} {3, -10} {4, -6}",
            DriverName, Model, Number, ProductionYear, KmPerYear);
    }
}
