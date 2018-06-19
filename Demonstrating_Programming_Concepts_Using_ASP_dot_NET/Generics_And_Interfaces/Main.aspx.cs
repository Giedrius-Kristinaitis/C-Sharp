/*
 * Author: Giedrius Kristinaitis
 */

using System;
using System.Web.UI.WebControls;

/// <summary>
/// Class of the Main webpage
/// </summary>
public partial class Main : System.Web.UI.Page {

    /// <summary>
    /// Called when button with id 'MakeListButton' is clicked
    /// Reads initial car data, displays it on the webpage and writes to a file
    /// Makes a new list with cars than are aged between user given values, sorts it,
    /// finds the most driven car model(-s), displays results on the webpage and writes it to a file
    /// </summary>
    /// <param name="sender">object sending the callback</param>
    /// <param name="eventArgs">arguments for click event</param>
    protected void MakeList(object sender, EventArgs eventArgs) {
        if (M1.Text.Equals("") || M2.Text.Equals("")) {
            return;
        }

        int minAge = int.Parse(M1.Text);
        int maxAge = int.Parse(M2.Text);

        // switch minAge and maxAge values if needed
        if (minAge > maxAge) {
            int temp = minAge;
            minAge = maxAge;
            maxAge = temp;
        }

        List<Car> cars = new List<Car>();
        List<Driver> drivers = new List<Driver>();

        if (!(FileUpload1.HasFile && FileUpload1.FileName.EndsWith(".csv"))) {
            return;
        }

        // read and display initial data
        FileStorage.ReadData(FileUpload1.FileContent, drivers, cars);
        FileStorage.WriteInitialDataToFile(Server.MapPath("PradDuom.txt"), cars, drivers);
        InitialDataTable.Visible = true;
        AddCarsToTable(InitialDataTable, cars, drivers);

        // make a new car list and display results
        List<Car> newCarList = GetCarsAgedBetween(cars, minAge, maxAge);
        string mostDrivenCarModel = GetMostDrivenCarModel(cars);
        DisplayResults(newCarList, drivers, minAge, maxAge, mostDrivenCarModel);
        FileStorage.WriteResultsToFile(Server.MapPath("Rezultatai.txt"), newCarList, drivers, 
            minAge, maxAge, mostDrivenCarModel);
    }

    /// <summary>
    /// Finds the most driven car model (all models if there are more which are equaly driven)
    /// </summary>
    /// <param name="cars">list containing cars which will be searched</param>
    /// <returns>string containing the most driven model (or models)</returns>
    private string GetMostDrivenCarModel(List<Car> cars) {
        string model = null;
        int mostKmDriven = 0;

        foreach(Car car in cars) {
            if (car.KmPerYear > mostKmDriven) {
                mostKmDriven = car.KmPerYear;
                model = car.Model;
            } else if (car.KmPerYear == mostKmDriven) {
                if (!model.Contains(car.Model)) {
                    model += ", " + car.Model;
                }
            }
        }

        return model;
    }

    /// <summary>
    /// Loops through a list and stores all cars that are aged between 
    /// minAge and maxAge in a new list 
    /// </summary>
    /// <param name="cars">list of cars to pick from</param>
    /// <param name="minAge">minimum age of the car</param>
    /// <param name="maxAge">maximum age of the car</param>
    /// <returns>new list of cars aged between minAge and maxAge</returns>
    private List<Car> GetCarsAgedBetween(List<Car> cars, int minAge, int maxAge) {
        List<Car> newCars = new List<Car>();

        foreach (Car car in cars) {
            int carAge = car.GetAge();

            if (carAge >= minAge && carAge <= maxAge) {
                newCars.Add(car);
            }
        }

        newCars.Sort();
        return newCars;
    }

    /// <summary>
    /// Displays program's execution results on the webpage
    /// </summary>
    /// <param name="cars">new list of cars (sorted, correct age cars)</param>
    /// <param name="minAge">minimum age of the car</param>
    /// <param name="maxAge">maximum age of the car</param>
    /// <param name="mostDrivenModel">most driven model(-s)</param>
    private void DisplayResults(List<Car> cars, List<Driver> drivers, int minAge, int maxAge, string mostDrivenModel) {
        if (mostDrivenModel != null) {
            MostDrivenCar.Text = "Labiausiai eksploatuojamo(-ų) automobilio(-ių) markė(-ės): " + mostDrivenModel;
        }

        if (cars.IsEmpty()) {
            ResultsLabel.Text = string.Format("Nėra mašinų, kurių amžius yra tarp {0} ir {1} metų", minAge, maxAge);
            ResultsTable.Visible = false;
        } else {
            ResultsLabel.Text = string.Format("Mašinos, kurių amžius yra tarp {0} ir {1} metų:", minAge, maxAge); ;
            ResultsTable.Visible = true;

            AddCarsToTable(ResultsTable, cars, drivers);
        }
    }

    /// <summary>
    /// Inserts a new row to a given table which contains car information
    /// </summary>
    /// <param name="table">table to insert into</param>
    /// <param name="cars">list of cars that will be inserted to the table</param>
    private void AddCarsToTable(Table table, List<Car> cars, List<Driver> drivers) {
        foreach (Car car in cars) {
            TableRow row = new TableRow();

            row.Cells.Add(CreateTableCell(car.GetDriverNames(drivers)));
            row.Cells.Add(CreateTableCell(car.Model));
            row.Cells.Add(CreateTableCell(car.Number));
            row.Cells.Add(CreateTableCell(car.ProductionYear.ToString()));
            row.Cells.Add(CreateTableCell(car.KmPerYear.ToString()));

            table.Rows.Add(row);
        }
    }

    /// <summary>
    /// Creates a new TableCell object and sets it's text to a given value
    /// </summary>
    /// <param name="text">text to put in the table cell</param>
    /// <returns>newly created table cell</returns>
    private TableCell CreateTableCell(string text) {
        TableCell cell = new TableCell();
        cell.Text = text;
        return cell;
    }
}
