using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

/// <summary>
/// Class of the Main webpage
/// </summary>
public partial class Main : System.Web.UI.Page {

    /// <summary>
    /// Callback method that gets called when the webpage finished loading
    /// Reads initial car data, displays it on the webpage and writes to a file
    /// </summary>
    /// <param name="sender">object sending the callback</param>
    /// <param name="eventArgs">arguments for page load event</param>
    protected void Page_Load(object sender, EventArgs eventArgs) {
        CarList cars = new CarList();
        FileStorage.ReadCarData(Server.MapPath("App_Data/Duom.csv"), cars);
        AddCarsToTable(InitialDataTable, cars);
        FileStorage.WriteInitialCarDataToFile(Server.MapPath("PradDuom.txt"), cars);
    }

    /// <summary>
    /// Called when button with id 'MakeListButton' is clicked
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

        CarList cars = new CarList();
        FileStorage.ReadCarData(Server.MapPath("App_Data/Duom.csv"), cars);
        CarList newList = GetCarsAgedBetween(cars, minAge, maxAge);
        string mostDrivenCarModel = GetMostDrivenCarModel(cars);
        DisplayResults(newList, minAge, maxAge, mostDrivenCarModel);
        FileStorage.WriteResultsToFile(Server.MapPath("Rezultatai.txt"), newList, minAge, maxAge, mostDrivenCarModel);
    }

    /// <summary>
    /// Finds the most driven car model (all models if there are more which are equaly driven)
    /// </summary>
    /// <param name="cars">list containing cars which will be searched</param>
    /// <returns>string containing the most driven model (or models)</returns>
    private string GetMostDrivenCarModel(CarList cars) {
        string model = null;
        int mostKmDriven = 0;

        while (cars.MoveToNextElement()) {
            CarData currentCar = cars.GetCurrentElement();

            if (currentCar.KmPerYear > mostKmDriven) {
                mostKmDriven = currentCar.KmPerYear;
                model = currentCar.Model;
            } else if (currentCar.KmPerYear == mostKmDriven) {
                mostKmDriven = currentCar.KmPerYear;
                model += ", " + currentCar.Model;
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
    private CarList GetCarsAgedBetween(CarList cars, int minAge, int maxAge) {
        CarList newCars = new CarList();

        while (cars.MoveToNextElement()) {
            int carAge = cars.GetCurrentElement().GetAge();

            if (carAge >= minAge && carAge <= maxAge) {
                newCars.AddCar(cars.GetCurrentElement());
            }
        }

        newCars.SortList();
        return newCars;
    }

    /// <summary>
    /// Displays program's execution results on the webpage
    /// </summary>
    /// <param name="cars">new list of cars (sorted, correct age cars)</param>
    /// <param name="minAge">minimum age of the car</param>
    /// <param name="maxAge">maximum age of the car</param>
    /// <param name="mostDrivenModel">most driven model(-s)</param>
    private void DisplayResults(CarList cars, int minAge, int maxAge, string mostDrivenModel) {
        if (mostDrivenModel != null) {
            MostDrivenCar.Text = "Labiausiai eksploatuojamo(-ų) automobilio(-ių) markė(-ės): " + mostDrivenModel;
        }

        if (cars.Count == 0) {
            ResultsLabel.Text = string.Format("Nėra mašinų, kurių amžius yra tarp {0} ir {1} metų", minAge, maxAge);
            ResultsTable.Visible = false;
        } else {
            ResultsLabel.Text = string.Format("Mašinos, kurių amžius yra tarp {0} ir {1} metų:", minAge, maxAge); ;
            ResultsTable.Visible = true;

            AddCarsToTable(ResultsTable, cars);
        }
    }

    /// <summary>
    /// Inserts a new row to a given table which contains car information
    /// </summary>
    /// <param name="table">table to insert into</param>
    /// <param name="cars">list of cars that will be inserted to the table</param>
    private void AddCarsToTable(Table table, CarList cars) {
        while (cars.MoveToNextElement()) {
            CarData data = cars.GetCurrentElement();
            TableRow row = new TableRow();

            row.Cells.Add(CreateTableCell(data.DriverName));
            row.Cells.Add(CreateTableCell(data.Model));
            row.Cells.Add(CreateTableCell(data.Number));
            row.Cells.Add(CreateTableCell(data.ProductionYear.ToString()));
            row.Cells.Add(CreateTableCell(data.KmPerYear.ToString()));

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