using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;
using System.Linq;

/// <summary>
/// Class with the source code of the Main.aspx webpage
/// </summary>
public partial class Main : System.Web.UI.Page {

    /*
     *  BELOW ARE METHODS THAT GET CALLED WHEN BUTTONS GET CLICKED
     */

    /// <summary>
    /// Callback method that gets called when ValidatePositionButton is clicked.
    /// Checks if the position that the user entered is valid and stores it in the Session object
    /// </summary>
    /// <param name="sender">object that caused the click event</param>
    /// <param name="eventArgs">arguments for the click event</param>
    protected void ValidatePosition(object sender, EventArgs eventArgs) {
        HideElements();

        string position = PositionInput.Text;

        if (!position.ToLower().Contains("centras") &&
            !position.ToLower().Contains("gynėjas") && !position.ToLower().Contains("puolėjas")) {
            DisplayError("Klaida: Bloga nurodyta pozicija (gali būti centras, puolėjas arba gynėjas)", null, null);
        } else {
            Session["position"] = position;
            InfoText3.CssClass = "InfoLabel";
            InfoText3.Text = "Pozicija sėkmingai įvesta";
            InfoText2.CssClass = "InfoLabel";
            ListLengthInput.CssClass = "InputText";
            MakeListButton.CssClass = "Button";
        }
    }

    /// <summary>
    /// Callback method that gets called when MakeListButton is clicked.
    /// This method reads files from App_Data folder, gets the length of the new list, makes
    /// the new list containing best players, and then displays the initial data and results 
    /// on the page and writes it to a text file
    /// </summary>
    /// <param name="sender">object that caused the click event</param>
    /// <param name="eventArgs">arguments for the click event</param>
    protected void MakePlayerList(object sender, EventArgs eventArgs) {
        HideElements();

        Dictionary<string, List<PlayerStatistics>> stats = new Dictionary<string, List<PlayerStatistics>>();
        List<List<Player>> players = new List<List<Player>>();
        
        if (HandleFiles(stats, players) && GetListLength(players)) {
            if (IsEmpty(stats)) {
                DisplayError("Klaida: Programa nerado nė vieno failo, kuriame būtų rungtynių informacija", null, null);
                return;
            }

            string resultsFile = Server.MapPath("Rezultatai.txt");

            DisplayPlayerTables(players);
            DisplayGameTables(stats);

            WriteTextToFile("Pradiniai duomenys:", false, resultsFile);
            WritePlayerTablesToFile(players, resultsFile);
            WriteGameTablesToFile(stats, resultsFile);

            IEnumerable<PlayerStatistics> bestPlayers = GetBestPlayers(stats, players);

            DisplayResultsTable(bestPlayers);

            WriteTextToFile("Rezultatai:", true, resultsFile);
            WriteResultsToFile(bestPlayers, resultsFile);
        }
    }

    /*
     * BELOW ARE ALL APPLICATION-LOGIC METHODS
     */

    /// <summary>
    /// Writes results (list of best players) to a file (results are appended to the end of the file)
    /// </summary>
    /// <param name="players">IEnumerable<PlayerStatistics> with best players in it</param>
    /// <param name="fileName">file to write to</param>
    protected void WriteResultsToFile(IEnumerable<PlayerStatistics> players, string fileName) {
        using (StreamWriter writer = new StreamWriter(fileName, true)) {
            writer.WriteLine(string.Format("Geriausių \"{0}\" pozicijos žaidėjų sąrašas:", Session["position"] as string));
            writer.WriteLine(string.Format("{0, -20} {1, -20} {2, -20} {3, -20} {4, -20} {5, -20}", 
                "Komanda", "Pavardė", "Vardas", "Žaista minučių", "Pelnyta taškų", "Padaryta klaidų"));

            foreach (PlayerStatistics stats in players) {
                writer.WriteLine(stats.ToString());
            }
        }
    }

    /// <summary>
    /// Displays results (list of best players) on the webpage
    /// </summary>
    /// <param name="players">IEnumerable<PlayerStatistics> with best players in it</param>
    protected void DisplayResultsTable(IEnumerable<PlayerStatistics> players) {
        Table table = CreateTable(string.Format("Geriausių \"{0}\" pozicijos žaidėjų sąrašas", Session["position"] as string), 
            new string[] {"Komanda", "Pavardė", "Vardas", "Žaista minučių", "Pelnyta taškų", "Padaryta klaidų"});

        foreach (PlayerStatistics stats in players) {
            TableRow row = new TableRow();
            row.Cells.Add(CreateTableCell(stats.Team, 1));
            row.Cells.Add(CreateTableCell(stats.Surname, 1));
            row.Cells.Add(CreateTableCell(stats.Name, 1));
            row.Cells.Add(CreateTableCell(stats.MinutesPlayed.ToString(), 1));
            row.Cells.Add(CreateTableCell(stats.PointsScored.ToString(), 1));
            row.Cells.Add(CreateTableCell(stats.FoulsMade.ToString(), 1));
            table.Rows.Add(row);
        }

        MainForm.Controls.Add(table);
    }

    /// <summary>
    /// Gets user's specified number of best players with user's specified position.
    /// Players are ranked by the number of points they scored, how many time the played and how 
    /// many fouls they made
    /// </summary>
    /// <param name="stats">dictionary with game information</param>
    /// <param name="players">list with all players</param>
    /// <returns>IEnumerable<PlayerStatistics> interface with best players</returns>
    protected IEnumerable<PlayerStatistics> GetBestPlayers(Dictionary<string, List<PlayerStatistics>> stats, 
                                                    List<List<Player>> players) {
        List<PlayerStatistics> bestPlayers = new List<PlayerStatistics>();
        List<Player> correctPositionPlayers = GetCorrectPositionPlayers(players);

        // loop through every list of player statistics found in game files
        foreach (KeyValuePair<string, List<PlayerStatistics>> keyValue in stats) {
            // loop through all players with the position specified by the user 
            // and search for them in the list of player statistics
            foreach (Player player in correctPositionPlayers) {
                IEnumerable<PlayerStatistics> playerStatsEnum = keyValue.Value.Where(x => x.Team.Equals(player.Team) 
                                    && x.Name.Equals(player.Name) && x.Surname.Equals(player.Surname));

                // if a player is found in the list of player statistics, add player's information
                // to the bestPlayers list
                if (playerStatsEnum.Any()) {
                    foreach (PlayerStatistics playerStats in playerStatsEnum) {
                        if (StatisticsExist(bestPlayers, playerStats)) {
                            PlayerStatistics existingStats = bestPlayers.Find(x => x.Team.Equals(playerStats.Team)
                                     && x.Name.Equals(playerStats.Name) && x.Surname.Equals(playerStats.Surname));
                            existingStats.MinutesPlayed += playerStats.MinutesPlayed;
                            existingStats.PointsScored += playerStats.PointsScored;
                            existingStats.FoulsMade += playerStats.FoulsMade;
                        } else {
                            bestPlayers.Add(playerStats);
                        }
                    }
                }
            }
        }

        return bestPlayers.OrderByDescending(x => x.PointsScored)
            .ThenBy(x => x.MinutesPlayed)
            .ThenBy(x => x.FoulsMade)
            .Take((int) Session["listLength"]);
    }

    /// <summary>
    /// Gets all players from list of players who have position specified by the user
    /// </summary>
    /// <param name="players">list of players to look in</param>
    /// <returns>list of all players with user's specified position</returns>
    protected List<Player> GetCorrectPositionPlayers(List<List<Player>> players) {
        List<Player> correctPositionPlayers = new List<Player>();

        foreach (List<Player> value in players) {
            correctPositionPlayers.AddRange(value.Where<Player>(
                x => x.Position.ToLower().Equals(((string) Session["position"]).ToLower())));
        }

        return correctPositionPlayers;
    }

    /// <summary>
    /// Writes given text to a given file
    /// </summary>
    /// <param name="text">text to write to the file</param>
    /// <param name="append">append the text to the end of the file or overwrite it</param>
    /// <param name="fileName">file to write to</param>
    protected void WriteTextToFile(string text, bool append, string fileName) {
        using (StreamWriter writer = new StreamWriter(fileName, append)) {
            writer.WriteLine(text);
        }
    }

    /// <summary>
    /// Writes initial data tables with player information to a given file
    /// </summary>
    /// <param name="players">list containing lists of players</param>
    /// <param name="fileName">file to write to</param>
    protected void WritePlayerTablesToFile(List<List<Player>> players, string fileName) {
        using (StreamWriter writer = new StreamWriter(fileName, true)) {
            foreach (List<Player> playerList in players) {
                writer.WriteLine(string.Format("{0, -20} {1, -20} {2, -20} {3, -10}", "Komanda", "Pavardė", "Vardas", "Poicija"));

                foreach (Player player in playerList) {
                    writer.WriteLine(player.ToString());
                }

                writer.WriteLine();
            }
        }
    }

    /// <summary>
    /// Writes initial data tables with game information to a given file
    /// </summary>
    /// <param name="games">dictionary with game information</param>
    /// <param name="fileName">file to write to</param>
    protected void WriteGameTablesToFile(Dictionary<string, List<PlayerStatistics>> games, string fileName) {
        using (StreamWriter writer = new StreamWriter(fileName, true)) {
            foreach (KeyValuePair<string, List<PlayerStatistics>> value in games) {
                writer.WriteLine(string.Format("", value.Key));
                writer.WriteLine(string.Format("{0, -20} {1, -20} {2, -20} {3, -20} {4, -20} {5, -20}", 
                    "Komanda", "Pavardė", "Vardas", "Žaista minučių", "Surinkta taškų", "Padaryta klaidų"));

                foreach (PlayerStatistics stats in value.Value) {
                    writer.WriteLine(stats.ToString());
                }

                writer.WriteLine();
            }
        }
    }
    
    /// <summary>
    /// Displays initial data tables containing game information on the webpage
    /// </summary>
    /// <param name="stats">dictionary containing information about games</param>
    protected void DisplayGameTables(Dictionary<string, List<PlayerStatistics>> stats) {
        foreach (KeyValuePair<string, List<PlayerStatistics>> keyValue in stats) {
            Table table = CreateTable(string.Format("Pradiniai duomenys: rungtynės {0}", keyValue.Key),
                new string[] { "Komanda", "Pavardė", "Vardas", "Žaista minučių", "Pelnyta taškų", "Padaryta klaidų" });

            foreach (PlayerStatistics playerStats in keyValue.Value) {
                TableRow row = new TableRow();
                row.Cells.Add(CreateTableCell(playerStats.Team, 1));
                row.Cells.Add(CreateTableCell(playerStats.Surname, 1));
                row.Cells.Add(CreateTableCell(playerStats.Name, 1));
                row.Cells.Add(CreateTableCell(playerStats.MinutesPlayed.ToString(), 1));
                row.Cells.Add(CreateTableCell(playerStats.PointsScored.ToString(), 1));
                row.Cells.Add(CreateTableCell(playerStats.FoulsMade.ToString(), 1));
                table.Rows.Add(row);
            }

            MainForm.Controls.Add(table);
        }
    }

    /// <summary>
    /// Displays initial data tables containing player information on the webpage
    /// </summary>
    /// <param name="players">list containing all players</param>
    protected void DisplayPlayerTables(List<List<Player>> players) {
        foreach (List<Player> value in players) {
            Table table = CreateTable("Pradiniai duomenys: žaidėjai",
                new string[] { "Komanda", "Pavardė", "Vardas", "Pozicija" });

            foreach (Player player in value) {
                TableRow row = new TableRow();
                row.Cells.Add(CreateTableCell(player.Team, 1));
                row.Cells.Add(CreateTableCell(player.Surname, 1));
                row.Cells.Add(CreateTableCell(player.Name, 1));
                row.Cells.Add(CreateTableCell(player.Position, 1));
                table.Rows.Add(row);
            }
            
            MainForm.Controls.Add(table);
        }
    }

    /// <summary>
    /// Creates a new Table with header text and 1 row with initial cells
    /// </summary>
    /// <param name="headerText">text to be displayed in the header</param>
    /// <param name="columns">array containing text for each of the cells</param>
    /// <returns>a newly created Table object</returns>
    protected Table CreateTable(string headerText, string[] columns) {
        Table table = new Table();
        TableRow headerRow = new TableRow();
        TableRow columnRow = new TableRow();
        headerRow.Cells.Add(CreateTableCell(headerText, columns.Length));

        foreach (string column in columns) {
            columnRow.Cells.Add(CreateTableCell(column, 1));
        }

        headerRow.CssClass = "Center Dark";
        columnRow.CssClass = "Dark";

        table.Rows.Add(headerRow);
        table.Rows.Add(columnRow);

        table.CssClass = "Table";

        return table;
    }

    /// <summary>
    /// Creates a new table cell with the given text and column span
    /// </summary>
    /// <param name="text">text to be stored in the new table cell</param>
    /// <param name="columnSpan">column span of the cell</param>
    /// <returns>a newly created table cell with the text in it</returns>
    protected TableCell CreateTableCell(string text, int columnSpan) {
        TableCell cell = new TableCell();
        cell.Text = text;

        if (columnSpan > 1) {
            cell.ColumnSpan = columnSpan;
        }

        return cell;
    }

    /// <summary>
    /// Hides elements that are not required to be displayed when the page first loads
    /// </summary>
    protected void HideElements() {
        ErrorText.CssClass = "Invisible";
        ErrorCause.CssClass = "Invisible";
        ErrorFix.CssClass = "Invisible";
        InfoText3.CssClass = "Invisible";
    }
    
    /// <summary>
    /// Gets user input from the list length text box and stores it in the Session object
    /// Handles thrown exceptions if the data entered is invalid
    /// </summary>
    /// <param name="players">list containing all players</param>
    /// <returns>true if user entered information was retrieved successfully, false if there
    /// was a problem (the data was invalid)</returns>
    protected bool GetListLength(List<List<Player>> players) {
        try {
            Session["listLength"] = int.Parse(ListLengthInput.Text);

            if (CountPlayers(players, Session["position"] as string) == 0) {
                DisplayError("Nurodytos pozicijos žaidėjų duomenų failuose nerasta", null, null);
                return false;
            }

            if (((int) Session["listLength"]) < 1 || ((int) Session["listLength"]) > CountPlayers(players, Session["position"] as string)) {
                DisplayError("Klaida: Blogas sąrašo ilgio skaičius", null,
                    string.Format("Sprendimas: Į sąrašo ilgio laukelį įveskite naturalųjį skaičių, kuris neviršytų nurodytos pozicijos žaidėjų skaičiaus, kuris yra {0}",
                    CountPlayers(players, Session["position"] as string)));
                return false;
            }
        } catch (FormatException) {
            DisplayError("Klaida: Sąrašo ilgio skaičius įvestas blogu formatu", null,
                string.Format("Sprendimas: Į sąrašo ilgio laukelį įveskite naturalųjį skaičių, kuris neviršytų nurodytos pozicijos žaidėjų skaičiaus, kuris yra {0}",
                CountPlayers(players, Session["position"] as string)));
            return false;
        } catch (OverflowException) {
            DisplayError("Klaida: Sąrašo ilgio skaičius per didelis", null,
                string.Format("Sprendimas: Į sąrašo ilgio laukelį įveskite naturalųjį skaičių, kuris neviršytų nurodytos pozicijos žaidėjų skaičiaus, kuris yra {0}",
                CountPlayers(players, Session["position"] as string)));
            return false;
        }

        return true;
    }

    /// <summary>
    /// Counts how many players in a given list have specified position
    /// </summary>
    /// <param name="players">player list to look in</param>
    /// <param name="position">what position players should be counted</param>
    /// <returns>number of specified position players in the list</returns>
    protected int CountPlayers(List<List<Player>> players, string position) {
        int count = 0;

        foreach (List<Player> value in players) {
            count += value.Count(player => player.Position.ToLower().Equals(position.ToLower()));
        }

        return count;
    }

    /// <summary>
    /// Tries to read all user created files found in the App_Data folder and handles exceptions 
    /// </summary>
    /// <param name="stats">dictionary to store player statistics in</param>
    /// <param name="players">list to store players in</param>
    /// <returns>true if all the files were read successfully, false if an exception occured
    /// and files could not be read properly</returns>
    protected bool HandleFiles(Dictionary<string, List<PlayerStatistics>> stats, List<List<Player>> players) {
        try {
            ReadAllFiles(stats, players, Server.MapPath("App_Data"));
        } catch (FileException ex) {
            string cause = ex.Cause == null ? null : "Priežastis: " + ex.Cause;
            DisplayError("Klaida: " + ex.Message, cause, "Sprendimas: " + ex.Fix);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Displays an error message on the webpage. If passed arguments are null, then they are not 
    /// displayed
    /// </summary>
    /// <param name="message">error message</param>
    /// <param name="cause">what caused the error</param>
    /// <param name="fix">a possible fix for the problem</param>
    protected void DisplayError(string message, string cause, string fix) {
        if (message != null) {
            ErrorText.CssClass = "InfoLabel Visible Red";
            ErrorText.Text = message;
        }

        if (cause != null) {
            ErrorCause.CssClass = "InfoLabel Visible Red";
            ErrorCause.Text = cause;
        }

        if (fix != null) {
            ErrorFix.CssClass = "InfoLabel Visible Red";
            ErrorFix.Text = fix;
        }
    }

    /// <summary>
    /// Finds and reads all files in the given directory and it's sub-directories
    /// </summary>
    /// <param name="stats">dictionary to store player statistics in</param>
    /// <param name="players">list to store players in</param>
    /// <param name="directory">directory to look for files in</param>
    protected void ReadAllFiles(Dictionary<string, List<PlayerStatistics>> stats, List<List<Player>> players,
                                            string directory) {
        string[] files = Directory.GetFileSystemEntries(directory);
        
        foreach (string file in files) {
            if (Directory.Exists(file)) { // the file variable points to a directory, call ReadAllFiles again
                ReadAllFiles(stats, players, file);
            } else if(File.Exists(file)) { // the file variable points to a file, read it
                if (file.ToLower().Contains("rungtynes")) {
                    ReadGameFile(stats, file);
                } else if (file.ToLower().Contains("zaidejai")) {
                    ReadPlayerFile(players, file);
                }
            }
        }
    }

    /// <summary>
    /// Reads a given file containing information about a game
    /// (date of the game, player statistics).
    /// Throws an exception if the given file cannot be read
    /// </summary>
    /// <param name="stats">dictionary to store player statistics in</param>
    /// <param name="fileName">name of the file to read</param>
    protected void ReadGameFile(Dictionary<string, List<PlayerStatistics>> stats, string fileName) {
        using (StreamReader reader = new StreamReader(fileName)) {
            string line = reader.ReadLine();
            string dateString = line;
            int lineNumber = 1;

            if (!IsDateValid(dateString)) {
                throw new FileException(string.Format("Faile \"{0}\" blogai įvesta data", fileName),
                    null, "Įvesti datą formatu metai-mėnuo-diena");
            }

            stats.Add(dateString, new List<PlayerStatistics>());

            while ((line = reader.ReadLine()) != null) {
                string[] data = line.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                lineNumber++;

                if (data.Length != 6) {
                    throw new FileException(string.Format("Blogas duomenų failo \"{0}\" {1} eilutės formatas", fileName, lineNumber),
                        "Failo eilutėje yra per mažai arba per daug duomenų laukų",
                        "Pertvarkyti duomenų failą taip, kad jo eilutės formatas būtų toks (skliausteliuose nurodytas duomenų tipas, to faile rašyti nereikia): komandos pavadinimas (string);žaidėjo pavardė (string);žaidėjo vardas (string);žaistų minučių skaičius (int);pelnytų taškų skaičius (int);padarytų klaidų skaičius (int)");
                } else {
                    try {
                        PlayerStatistics playerStats = new PlayerStatistics(data[0], data[2], data[1],
                            int.Parse(data[3]), int.Parse(data[4]), int.Parse(data[5]));

                        if (StatisticsExist(stats[dateString], playerStats)) {
                            throw new FileException(string.Format("Faile \"{0}\" {1} eilutėje rastas pasikartojantis žaidėjas", fileName, lineNumber),
                                "Žaidėjo komanda, pavardė ir vardas sutampa su kitur tame pačiame faile aprašyto žaidėjo komanda, pavarde ir vardu",
                                "Įvesti unikalią žaidėjo informaciją, kad komandos pavadinimas, pavardė ir vardas nesikartotų");
                        }

                        stats[dateString].Add(playerStats);
                    } catch (Exception ex) {
                        if (ex as FileException != null) {
                            throw;
                        }

                        throw new FileException(string.Format("Faile \"{0}\" blogai įvesta {1} eilutė", fileName, lineNumber),
                            string.Format("Blogas skaičiaus formatas arba skaičius viršijo leistiną ribą (visi skaičiai turi būti natūralieji ir neviršyti {0})", int.MaxValue),
                            "Pertvarkyti duomenų failą taip, kad jo eilutės formatas būtų toks (skliausteliuose nurodytas duomenų tipas, to faile rašyti nereikia): komandos pavadinimas (string);žaidėjo pavardė (string);žaidėjo vardas (string);žaistų minučių skaičius (int);pelnytų taškų skaičius (int);padarytų klaidų skaičius (int)");
                    }
                }
            }
        }
    }

    /// <summary>
    /// Reads a given file containing information about players (team, name, surname and position).
    /// Throws an exception if the given file cannot be read
    /// </summary>
    /// <param name="players">list of lists of players to store players in</param>
    /// <param name="fileName">name of the file to read</param>
    protected void ReadPlayerFile(List<List<Player>> players, string fileName) {
        using (StreamReader reader = new StreamReader(fileName)) {
            string line = null;
            int lineNumber = 0;
            int index = players.Count;
            players.Add(new List<Player>());

            while ((line = reader.ReadLine()) != null) {
                string[] data = line.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);
                lineNumber++;

                if (data.Length != 4) {
                    throw new FileException(string.Format("Blogas duomenų failo \"{0}\" {1} eilutės formatas", fileName, lineNumber),
                        "Failo eilutėje yra per mažai arba per daug duomenų laukų",
                        "Pertvarkyti duomenų failą taip, kad jo eilutės formatas būtų toks (skliausteliuose nurodytas duomenų tipas, to faile rašyti nereikia): komandos pavadinimas (string);žaidėjo pavardė (string);žaidėjo vardas (string);žaidėjo pozicija (string)");
                } else {
                    if (data[3].ToLower() != "centras" && data[3].ToLower() != "puolėjas" && data[3].ToLower() != "gynėjas") {
                        throw new FileException(string.Format("Faile \"{0}\" blogai įvesta {1} eilutė", fileName, lineNumber),
                            "Failo eilutėje blogai įvesta žaidėjo pozicija",
                            "Įvesti tinkamą poziciją: gali būti centras, puolėjas arba gynėjas");
                    } else {
                        Player player = new Player(data[0], data[2], data[1], data[3]);

                        if (PlayerExists(players, player)) {
                            throw new FileException(string.Format("Faile \"{0}\" {1} eilutėje rastas pasikartojantis žaidėjas", fileName, lineNumber),
                                "Žaidėjo komanda, pavardė ir vardas sutampa su kitur aprašyto žaidėjo komanda, pavarde ir vardu",
                                "Įvesti unikalią žaidėjo informaciją, kad komandos pavadinimas, pavardė ir vardas nesikartotų");
                        }

                        players[index].Add(player);
                    }
                }
            }
        }
    }

    /// <summary>
    /// Checks if a date string is valid (is in a correct format)
    /// </summary>
    /// <param name="date">date string to check</param>
    /// <returns>true if the date is valid, false otherwise</returns>
    protected bool IsDateValid(string date) {
        try {
            DateTime.Parse(date);
            return true;
        } catch {
            return false;
        }
    }
    
    /// <summary>
    /// Checks if player already exist in a list of players
    /// </summary>
    /// <param name="players">list of lists with player</param>
    /// <param name="player">player to look for</param>
    /// <returns>true if player exists, false otherwise</returns>
    protected bool PlayerExists(List<List<Player>> players, Player player) {
        foreach (List<Player> playerList in players) {
            if (playerList.Count(x => x.Team.Equals(player.Team) && x.Surname.Equals(player.Surname)
                                    && x.Name.Equals(player.Name)) > 0) {
                return true;
            }
        }

        return false;
    }

    /// <summary>
    /// Checks if player statistics already exist in a list of player statistics
    /// </summary>
    /// <param name="allStats">list with player statistics</param>
    /// <param name="stats">player statistics to look for</param>
    /// <returns>true if player statistics exists, false otherwise</returns>
    protected bool StatisticsExist(List<PlayerStatistics> allStats, PlayerStatistics stats) {
        if (allStats.Count(x => x.Team.Equals(stats.Team) && x.Surname.Equals(stats.Surname)
                                    && x.Name.Equals(stats.Name)) > 0) {
            return true;
        }

        return false;
    }

    /// <summary>
    /// Checks if all lists in a list of lists of players are empty
    /// </summary>
    /// <param name="list">list of lists to check</param>
    /// <returns>true if all lists are empty, false otherwise</returns>
    protected bool IsEmpty(List<List<Player>> list) {
        foreach (List<Player> playerList in list) {
            if (playerList.Any()) {
                return false;
            }
        }

        return true;
    }

    /// <summary>
    /// Checks if all lists in a dictionary where the value is a list of player statistics are empty
    /// </summary>
    /// <param name="dictionary">dictionary to check</param>
    /// <returns>true if all lists are empty, false otherwise</returns>
    protected bool IsEmpty(Dictionary<string, List<PlayerStatistics>> dictionary) {
        foreach (KeyValuePair<string, List<PlayerStatistics>> pair in dictionary) {
            if (pair.Value.Any()) {
                return false;
            }
        }

        return true;
    }
}