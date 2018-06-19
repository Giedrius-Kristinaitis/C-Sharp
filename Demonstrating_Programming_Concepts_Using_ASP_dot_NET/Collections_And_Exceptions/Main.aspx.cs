using System;
using System.Collections.Generic;
using System.IO;
using System.Web.UI.WebControls;

/// <summary>
/// Class with the source code of the Main.aspx webpage
/// </summary>
public partial class Main : System.Web.UI.Page {

    /// <summary>
    /// Hides initial data table, results table, error text label,
    /// error cause label and error fix label
    /// </summary>
    protected void HideElements() {
        InitialDataTable.CssClass = "Invisible";
        ResultsTable.CssClass = "Invisible";
        ErrorText.CssClass = "Invisible";
        ErrorCause.CssClass = "Invisible";
        ErrorFix.CssClass = "Invisible";
    }

    /// <summary>
    /// Callback method that gets called when MakeListButton is clicked.
    /// This method reads files from App_Data folder, gets user input, makes
    /// a new list containing best players, and then displays the initial data and results 
    /// on the page and writes it to a text file
    /// </summary>
    /// <param name="sender">object that caused the click event</param>
    /// <param name="eventArgs">arguments for the click event</param>
    protected void MakePlayerList(object sender, EventArgs eventArgs) {
        HideElements();
        List<Player> players = new List<Player>();

        int listLength = 0;
        string position = string.Empty;

        if (GetUserInput(ref listLength, ref position) && HandleFiles(players)) {
            if (players.Count == 0) {
                WritePlayerListToFile(null, "Programa nerado duomenų failų arba jie buvo tušti", Server.MapPath("Rezultatai.txt"), true);
                DisplayError("Klaida: Programa nerado duomenų failų arba jie buvo tušti", null, null);
            } else {
                List<Player> selectedPositionPlayers = players.FindAll(x => x.Position.Equals(position));

                if (listLength > selectedPositionPlayers.Count) {
                    DisplayError("Klaida: Naudingiausių žaidėjų sąrašo ilgis negali būti didesnis už duomenų failuose rastų nurodytos pozicijos žaidėjų skaičių, kuris yra " + selectedPositionPlayers.Count, null, null);
                    return;
                } else if (selectedPositionPlayers.Count == 0) {
                    DisplayError("Nerasta \"" + position + "\" pozicijos žaidėjų", null, null);
                    WritePlayerListToFile(players, "Pradiniai duomenys:", Server.MapPath("Rezultatai.txt"), true);
                    WritePlayerListToFile(null, "Rezultatai:", Server.MapPath("Rezultatai.txt"), false);
                    WritePlayerListToFile(null, "Nerasta \"" + position + "\" pozicijos žaidėjų", Server.MapPath("Rezultatai.txt"), false);
                    return;
                }

                WritePlayerListToFile(players, "Pradiniai duomenys:", Server.MapPath("Rezultatai.txt"), true);
                DisplayPlayerList(players, InitialDataTable);

                List<Player> bestPlayersList = GetBestPlayers(selectedPositionPlayers, listLength);

                if (bestPlayersList.Count > 0) {
                    DisplayPlayerList(bestPlayersList, ResultsTable);
                    WritePlayerListToFile(null, "Rezultatai:", Server.MapPath("Rezultatai.txt"), false);
                    WritePlayerListToFile(bestPlayersList, string.Format("Naudingiausi \"{0}\" pozicijos žaidėjai:", position), Server.MapPath("Rezultatai.txt"), false);
                } else {
                    DisplayError("Nurodytas sąrašo ilgis buvo 0", null, null);
                    WritePlayerListToFile(null, "Rezultatai: Nurodytas sąrašo ilgis buvo 0", Server.MapPath("Rezultatai.txt"), false);
                }
            }
        }
    }

    /// <summary>
    /// Makes a given length list of best players
    /// </summary>
    /// <param name="players">list with all players</param>
    /// <param name="listLength">length of the best player list (cannot be greater than the length
    /// of the list passed as the argument)</param>
    /// <returns>a new list with only the given number of best players</returns>
    protected List<Player> GetBestPlayers(List<Player> players, int listLength) {
        players.Sort();

        Player[] bestPlayers = new Player[listLength];
        players.CopyTo(0, bestPlayers, 0, listLength);

        List<Player> bestPlayersList = new List<Player>(bestPlayers);

        return bestPlayersList;
    }

    /// <summary>
    /// Displays a player list to in a given table (puts player's information into the table)
    /// </summary>
    /// <param name="players">list of the players to display</param>
    /// <param name="table">table to display in</param>
    protected void DisplayPlayerList(List<Player> players, Table table) {
        if (players != null) {
            table.CssClass = "Table";

            foreach (Player player in players) {
                TableRow row = new TableRow();

                row.Cells.Add(CreateTableCell(player.Team));
                row.Cells.Add(CreateTableCell(player.Surname));
                row.Cells.Add(CreateTableCell(player.Name));
                row.Cells.Add(CreateTableCell(player.Position));
                row.Cells.Add(CreateTableCell(player.MinutesPlayed.ToString()));
                row.Cells.Add(CreateTableCell(player.PointsScored.ToString()));
                row.Cells.Add(CreateTableCell(player.FoulsMade.ToString()));
                row.Cells.Add(CreateTableCell(player.DatePlayed));

                table.Rows.Add(row);
            }
        }
    }
    
    /// <summary>
    /// Creates a new table cell with the given text
    /// </summary>
    /// <param name="text">text to be stored in the new table cell</param>
    /// <returns>a newly created table cell with the text in it</returns>
    protected TableCell CreateTableCell(string text) {
        TableCell cell = new TableCell();
        cell.Text = text;
        return cell;
    }

    /// <summary>
    /// Writes a player list to a given file (writes information about all players in the list)
    /// </summary>
    /// <param name="players">list with the players to print to the file</param>
    /// <param name="message">optional text to be written before players' information,
    /// if it is null, then nothing is written before the players' information</param>
    /// <param name="fileName">name of the file to write to</param>
    /// <param name="overwrite">if this is true, then the content of the given file is 
    /// overwritten by the new content, if false, the new content is appended to the end of
    /// the file</param>
    protected void WritePlayerListToFile(List<Player> players, string message, string fileName, bool overwrite) {
        using (StreamWriter writer = overwrite ? File.CreateText(fileName) : File.AppendText(fileName)) {
            if (message != null) {
                writer.WriteLine(message);
            }

            if (players != null && players.Count > 0) {
                writer.WriteLine(string.Format("{0, -20} {1, -20} {2, -20} {3, -10} {4, -10} {5, -10} {6, -10} {7, -12}",
                    "Komanda", "Pavardė", "Vardas", "Pozicija", "Minutės", "Taškai", "Klaidos", "Data"));
            } else {
                return;
            }

            foreach (Player player in players) {
                writer.WriteLine(player.ToString());
            }
        }
    }

    /// <summary>
    /// Gets user input from the position and list length text boxes.
    /// Handles thrown exceptions if the data entered is invalid
    /// </summary>
    /// <param name="listLength">reference to the integer the entered list length will be stored in</param>
    /// <param name="position">reference to the string object the entered position will be stored in</param>
    /// <returns>true if user entered information was retrieved successfully, false if there
    /// was a problem (the data was invalid)</returns>
    protected bool GetUserInput(ref int listLength, ref string position) {
        try {
            listLength = int.Parse(ListLengthInput.Text);
            position = PositionInput.Text;

            if (!position.ToLower().Contains("centras") &&
                !position.ToLower().Contains("gynėjas") && !position.ToLower().Contains("puolėjas")) {
                DisplayError("Klaida: Bloga nurodyta pozicija (gali būti centras, puolėjas arba gynėjas)", null, null);
                return false;
            }

            if (listLength < 0) {
                DisplayError("Klaida: Sąrašo ilgis turi būti teigiamas skaičius", null, null);
                return false;
            }
        } catch (FormatException ex) {
            DisplayError("Klaida: Sąrašo ilgio skaičius įvestas blogu formatu", null,
                "Sprendimas: Į sąrašo ilgio laukelį įveskite naturalųjį skaičių");
            return false;
        } catch (OverflowException ex) {
            DisplayError("Klaida: Sąrašo ilgio skaičius per didelis", null,
                "Sprendimas: Į sąrašo ilgio laukelį įveskite naturalųjį skaičių, kuris neviršytų " + int.MaxValue);
            return false;
        }

        return true;
    }

    /// <summary>
    /// Tries to read all user created files found in the App_Data folder and handles exceptions 
    /// </summary>
    /// <param name="players">list to store players found in files</param>
    /// <returns>true if all the files were read successfully, false if an exception occured
    /// and files could not be read properly</returns>
    protected bool HandleFiles(List<Player> players) {
        try {
            FindAndReadFiles(players, Server.MapPath("App_Data"));
        } catch (FileException ex) {
            DisplayError("Klaida: " + ex.Message, "Priežastis: " + ex.Cause, "Sprendimas: " + ex.Fix);
            return false;
        } catch (FormatException ex) {
            DisplayError("Klaida: " + ex.Message, "Priežastis: Blogas duomenų failo skaičiaus formatas",
                "Sprendimas: Įsitikinti, kad duomenų failuose visi skaičiai yra natūralieji");
            return false;
        } catch (OverflowException ex) {
            DisplayError("Klaida: " + ex.Message, "Priežastis: Per didelis skaičius duomenų faile",
                "Sprendimas: Įsitikinti, kad skaičiai duomenų failuose neviršija " + int.MaxValue);
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
    /// Calls method to find all files that the program supports in the given directory and 
    /// it's sub-directories, puts them into correct list to be read and then calls a suitable 
    /// method that reads the files (this is done because files with player statistics
    /// need to be read before files with player positions)
    /// </summary>
    /// <param name="players">list to store players found in files</param>
    /// <param name="directory">directory to look for files in</param>
    protected void FindAndReadFiles(List<Player> players, string directory) {
        List<string> gameFiles = new List<string>();
        List<string> playerFiles = new List<string>();

        FindAllFiles(gameFiles, playerFiles, directory);

        ReadGameFiles(players, gameFiles.ToArray());
        ReadPlayerFiles(players, playerFiles.ToArray());
    }

    /// <summary>
    /// Finds all files in the given directory and it's sub-directories and puts the file 
    /// names into correct lists to be read later
    /// </summary>
    /// <param name="gameFiles">list to put game file names into</param>
    /// <param name="playerFiles">list to put player file names into</param>
    /// <param name="directory">directory to for files look in</param>
    protected void FindAllFiles(List<string> gameFiles, List<string> playerFiles, string directory) {
        string[] files = Directory.GetFileSystemEntries(directory);
        
        foreach (string file in files) {
            if (Directory.Exists(file)) { // the file variable points to a directory, call FindAllFiles again
                FindAllFiles(gameFiles, playerFiles, file);
            } else if(File.Exists(file)) { // the file variable points to a file
                if (file.ToLower().Contains("rungtynes")) {
                    gameFiles.Add(file);
                } else if (file.ToLower().Contains("zaidejai")) {
                    playerFiles.Add(file);
                }
            }
        }
    }

    /// <summary>
    /// Reads all given files with game information (date, player statistics) and stores everything
    /// in a given player list
    /// </summary>
    /// <param name="players">list to store player information in</param>
    /// <param name="files">files to read</param>
    protected void ReadGameFiles(List<Player> players, string[] files) {
        foreach (string file in files) {
            ReadGameFile(players, file);
        }
    }

    /// <summary>
    /// Reads all given files with player information (team, name, surname, position) and stores everything
    /// in a given player list
    /// </summary>
    /// <param name="players">list to store player information in</param>
    /// <param name="files">files to read</param>
    protected void ReadPlayerFiles(List<Player> players, string[] files) {
        foreach (string file in files) {
            ReadPlayerFile(players, file);
        }
    }

    /// <summary>
    /// Reads a given file containing information about a game
    /// (date of the game, player statistics).
    /// Throws an exception if the given file cannot be read
    /// </summary>
    /// <param name="players">list to store players found in the file</param>
    /// <param name="fileName">name of the file to read</param>
    protected void ReadGameFile(List<Player> players, string fileName) {
        using (StreamReader reader = new StreamReader(fileName)) {
            string line = reader.ReadLine();
            string date = line;

            while ((line = reader.ReadLine()) != null) {
                string[] data = line.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                if (data.Length != 6) {
                    throw new FileException(string.Format("Blogas duomenų failo \"{0}\" eilutės formatas", fileName),
                        "Failo eilutėje yra per mažai arba per daug duomenų laukų",
                        "Pertvarkyti duomenų failą taip, kad jo eilutės formatas būtų toks (skliausteliuose nurodytas duomenų tipas, to faile rašyti nereikia): komandos pavadinimas (string);žaidėjo pavardė (string);žaidėjo vardas (string);žaistų minučių skaičius (int);pelnytų taškų skaičius (int);padarytų klaidų skaičius (int)");
                } else {
                    players.Add(new Player(data[0], data[2], data[1], "", int.Parse(data[3]),
                        int.Parse(data[4]), int.Parse(data[5]), date));
                }
            }
        }
    }

    /// <summary>
    /// Reads a given file containing information about players (team, name, surname and position).
    /// Throws an exception if the given file cannot be read
    /// </summary>
    /// <param name="players">list already initialized players whose position field will
    /// be initialized</param>
    /// <param name="fileName">name of the file to read</param>
    protected void ReadPlayerFile(List<Player> players, string fileName) {
        using (StreamReader reader = new StreamReader(fileName)) {
            string line = null;

            while ((line = reader.ReadLine()) != null) {
                string[] data = line.Split(new char[] { ',', ';' }, StringSplitOptions.RemoveEmptyEntries);

                if (data.Length != 4) {
                    throw new FileException(string.Format("Blogas duomenų failo \"{0}\" eilutės formatas", fileName),
                        "Failo eilutėje yra per mažai arba per daug duomenų laukų",
                        "Pertvarkyti duomenų failą taip, kad jo eilutės formatas būtų toks (skliausteliuose nurodytas duomenų tipas, to faile rašyti nereikia): komandos pavadinimas (string);žaidėjo pavardė (string);žaidėjo vardas (string);žaidėjo pozicija (string)");
                } else {
                    List<Player> foundPlayers = players.FindAll(x => x.Name.Equals(data[2]) 
                                                        && x.Surname.Equals(data[1])
                                                        && x.Team.Equals(data[0]));

                    foreach (Player player in foundPlayers) {
                        player.Position = data[3];
                    }
                }
            }
        }
    }
}