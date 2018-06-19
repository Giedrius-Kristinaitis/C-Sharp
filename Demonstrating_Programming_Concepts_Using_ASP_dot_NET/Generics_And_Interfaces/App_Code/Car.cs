using System;

/// <summary>
/// Class storing car's information: model, number, year produced, km driven per year (in thousands)
/// </summary>
public class Car : IComparable, IEquatable<Car> {

    public string Model { get; set; } // model of the car
    public string Number { get; set; } // number plate of the car
    public int ProductionYear { get; set; } // year the car was produced
    public int KmPerYear { get; set; } // km driven per year in thousands

    /// <summary>
    /// Class constructor
    /// </summary>
    /// <param name="model">model of the car</param>
    /// <param name="number">number plate of the car</param>
    /// <param name="productionYear">year the car was produced</param>
    /// <param name="kmPerYear">km driven per year in thousands</param>
	public Car(string model, string number, int productionYear, int kmPerYear) {
        Model = model;
        Number = number;
        ProductionYear = productionYear;
        KmPerYear = kmPerYear;
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
        Car other = obj as Car;

        if (ReferenceEquals(other, null)) {
            throw new ArgumentException("Argument must be of type Car");
        } else {
            int modelComparisonResult = string.Compare(Model, other.Model, StringComparison.CurrentCulture);

            if (modelComparisonResult == 0) {
                return GetAge().CompareTo(other.GetAge());
            }

            return modelComparisonResult;
        }
    }

    /// <summary>
    /// Checks if this instance of the object is equal to a given Car object
    /// </summary>
    /// <param name="other">object to compare to</param>
    /// <returns>true if both objects are equal (model and age matches)</returns>
    public bool Equals(Car other) {
        if (Model.CompareTo(other.Model) == 0 && GetAge() == other.GetAge()) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if this instance of the object is equal to a given object
    /// </summary>
    /// <param name="other">object to compare to</param>
    /// <returns>true if both objects are equal (both are of type Car and model and age matches)</returns>
    public override bool Equals(object obj) {
        if (ReferenceEquals(obj, null)) {
            return false;
        }

        Car other = obj as Car;

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
        return Number.GetHashCode();
    }

    /// <summary>
    /// Calculates the age of the car
    /// </summary>
    /// <returns>age of the car in years</returns>
    public int GetAge() {
        return DateTime.Now.Year - ProductionYear;
    }

    /// <summary>
    /// Formats the current instance of the object as a string to be displayed later
    /// </summary>
    /// <returns>formatted string with this object's field values</returns>
    public override string ToString() {
        return string.Format("{0, -10} {1, -10} {2, -10} {3, -30}", Model, Number, ProductionYear, KmPerYear);
    }

    /// <summary>
    /// Finds all drivers that are driving this car
    /// </summary>
    /// <param name="drivers">list of drivers to search in</param>
    /// <returns>names of all drivers of this car as a string</returns>
    public string GetDriverNames(List<Driver> drivers) {
        string driverNames = "";

        foreach (Driver driver in drivers) {
            if (driver.CarNumber.Equals(Number)) {
                if (!driverNames.Contains(driver.Name)) {
                    driverNames += driver.Name + " ";
                }
            }
        }

        return driverNames;
    }
}