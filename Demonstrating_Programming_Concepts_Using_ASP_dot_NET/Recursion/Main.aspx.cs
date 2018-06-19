using System;
using System.Web.UI.WebControls;
using System.IO;
using System.Text;

/// <summary>
/// Main class of the webpage
/// </summary>
public partial class Main : System.Web.UI.Page {

    /// <summary>
    /// method that gets called when the webpage loads
    /// </summary>
    /// <param name="sender">object that initiated the page load event</param>
    /// <param name="eventArgs">arguments for the event</param>
    protected void Page_Load(object sender, EventArgs eventArgs) {
        Map cityMap = null;
        int manPositionX = 0;
        int manPositionY = 0;

        ReadData("App_Data/Duom3.txt", out cityMap, out manPositionX, out manPositionY);

        DisplayInitialData(cityMap, manPositionX, manPositionY);
        WriteInitialDataToTextFile("PradDuom.txt", cityMap, manPositionX, manPositionY);
    }

    /// <summary>
    /// writes initial data to a text file
    /// </summary>
    /// <param name="fileName">name of the file to write to</param>
    /// <param name="map">Map object containing map information</param>
    /// <param name="manX">x coordinate of the man's position</param>
    /// <param name="manY">y coordinate of the man's position</param>
    protected void WriteInitialDataToTextFile(string fileName, Map map, int manX, int manY) {
        using (StreamWriter writer = new StreamWriter(Server.MapPath(fileName))) {
            writer.Write("Žemėlapio matricos dydis: ");
            writer.WriteLine(map.MapSize);
            writer.Write("Vyro koordinatės (X; Y) (viršutinis kairysis žemėlapio kampas - (1; 1)): ");
            writer.WriteLine(manX + "; " + manY);
            writer.WriteLine("Žemėlapis:");
            WriteMapToStreamWriter(map.MapData, map.MapSize, writer);
        }
    }

    /// <summary>
    /// writes the results of path finding to a text file
    /// </summary>
    /// <param name="fileName">name of the file to write to</param>
    /// <param name="pathFound">boolean indicating wether a path was found</param>
    /// <param name="map">Map object containing map information</param>
    /// <param name="manX">x coordinate of the man's position</param>
    /// <param name="manY">y coordinate of the man's position</param>
    /// <param name="quartersPassed">number of quarters covered by the path</param>
    protected void WriteResultsToTextFile(string fileName, bool pathFound, char[,] map, 
                                            int manX, int manY, int quartersPassed) {
        using (StreamWriter writer = new StreamWriter(Server.MapPath(fileName))) {
            if (pathFound) {
                writer.WriteLine("Norint pasiekti gėlių parduotuvę reikia įveikti " + quartersPassed + " kvartalus");
                WriteMapToStreamWriter(map, (int) Math.Sqrt(map.Length), writer);
                writer.WriteLine("K - kelias iki parduotuvės");
            } else {
                writer.WriteLine("Nėra tokios gėlių parduotuvės, kuriai pasiekti reikia pereiti ne daugiau kaip 5 kvartalus");
            }
        }
    }

    /// <summary>
    /// writes map information to a given StreamWriter
    /// </summary>
    /// <param name="mapData">2 dimensional char array containing map data</param>
    /// <param name="mapSize">size of the map matrix</param>
    /// <param name="writer">StreamWriter to write to</param>
    protected void WriteMapToStreamWriter(char[,] mapData, int mapSize, StreamWriter writer) {
        for (int y = 0; y < mapSize; y++) {
            for (int x = 0; x < mapSize; x++) {
                writer.Write(mapData[x, y]);
            }

            writer.WriteLine();
        }
    }

    /// <summary>
    /// displays the initial data on the webpage
    /// </summary>
    /// <param name="cityMap">Map object containing map information</param>
    /// <param name="manPositionX">x coordinate of the man's position</param>
    /// <param name="manPositionY">y coordinate of the man's position</param>
    protected void DisplayInitialData(Map cityMap, int manPositionX, int manPositionY) {
        DisplayMap(cityMap, manPositionX, manPositionY, MapDisplayLabel);

        ManCoordinatesLabel.Text = "Vyro koordinatės: " + manPositionX + " nuo kairės į dešinę, " + 
            manPositionY + " nuo viršaus į apačią (Žemėlapyje - X)";
    }

    /// <summary>
    /// displays map information on the webpage
    /// </summary>
    /// <param name="map">Map object containing map information</param>
    /// <param name="manPositionX">x coordinate of the man's position</param>
    /// <param name="manPositionY">y coordinate of the man's position</param>
    /// <param name="label">Label to which map information will be written</param>
    protected void DisplayMap(Map map, int manPositionX, int manPositionY, Label label) {
        label.Font.Name = "Courier New";

        StringBuilder labelText = new StringBuilder();

        for (int y = 0; y < map.MapSize; y++) {
            for (int x = 0; x < map.MapSize; x++) {
                if (x == manPositionX - 1 && y == manPositionY - 1) {
                    labelText.Append('X');
                } else {
                    labelText.Append(map.MapData[x, y]);
                }
            }

            labelText.Append("<br/>");
        }

        label.Text = labelText.ToString();
    }

    /// <summary>
    /// displays the results of path finding on the webpage
    /// </summary>
    /// <param name="pathFound">boolean indicating wether a path was found</param>
    /// <param name="map">Map object containing map information with the path marked in it</param>
    protected void DisplayResults(bool pathFound, Map map) {
        InfoLabel3.Visible = true;
        ResultsDisplayLabel.Visible = true;
        FindPathButton.Enabled = false;
        FindPathButton.Visible = false;

        if (pathFound) {
            MapExplanationLabel2.Visible = true;
            DisplayMap(map, -10, -10, ResultsDisplayLabel);
        } else {
            ResultsDisplayLabel.Text = "Nėra tokios gėlių parduotuvės, kuriai pasiekti reiktų pereiti ne daugiau nei 5 kvartalus";
        }
    }

    /// <summary>
    /// OnClick() method of the button FindPathButton
    /// executes path finding algorithm of the Map class
    /// </summary>
    /// <param name="sender">object which caused the click event</param>
    /// <param name="eventArgs">arguments for the click event</param>
    protected void FindPathToFlowerStore(object sender, EventArgs eventArgs) {
        Map cityMap = null;
        int manPositionX = 0;
        int manPositionY = 0;

        ReadData("App_Data/Duom3.txt", out cityMap, out manPositionX, out manPositionY);

        bool pathFound = false;
        int quartersPassed = 0;
        char[,] mapWithPath = cityMap.FindPathToFlowerStore(manPositionX - 1,
                                            manPositionY - 1, out pathFound, out quartersPassed);

        DisplayResults(pathFound, new Map(mapWithPath, cityMap.MapSize));
        WriteResultsToTextFile("Rezultatai.txt", pathFound, mapWithPath, manPositionX, 
            manPositionY, quartersPassed);
    }

    /// <summary>
    /// reads initial data from a text file
    /// </summary>
    /// <param name="fileName">name of the file to read from</param>
    /// <param name="cityMap">Map object in which map information will be stored</param>
    /// <param name="manPositionX">integer in which the x coordinate of man's position will 
    /// be stored</param>
    /// <param name="manPositionY">integer in which the y coordinate of man's position will 
    /// be stored</param>
    protected void ReadData(string fileName, out Map cityMap, 
                            out int manPositionX, out int manPositionY) {
        using (StreamReader reader = new StreamReader(Server.MapPath(fileName))) {
            string[] firstLineData = reader.ReadLine().Split(' ');

            int mapSize = int.Parse(firstLineData[0]);
            manPositionY = int.Parse(firstLineData[1]);
            manPositionX = int.Parse(firstLineData[2]);

            cityMap = FillMapMatrix(reader, mapSize);
        }
    }

    /// <summary>
    /// fills the Map object with map information which is read from a stream writer linked 
    /// to the data file
    /// </summary>
    /// <param name="reader">StreamReader to read from</param>
    /// <param name="mapSize">size of the map matix (array)</param>
    /// <returns>new Map object with map information in it</returns>
    protected Map FillMapMatrix(StreamReader reader, int mapSize) {
        char[,] mapData = new char[mapSize, mapSize];
        string line = null;
        int lineNumber = 0;

        while ((line = reader.ReadLine()) != null) {
            for (int i = 0; i < line.Length; i++) {
                mapData[i, lineNumber] = line[i];
            }

            lineNumber++;
        }

        return new Map(mapData, mapSize);
    }
}