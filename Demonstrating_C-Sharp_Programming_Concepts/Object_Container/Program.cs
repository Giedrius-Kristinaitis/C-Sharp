/*
 * Author: Giedrius Kristinaitis
 * 
 * All classes should be in seperate files, but for the sake of simplicity
 * they are in one file.
 */

using System;
using System.IO;
using System.Collections.Generic;
using System.Text;

namespace Basketball {

    /// <summary>
    /// entry class containing the Main() method
    /// </summary>
    class Program {

        const string dataFile = "data.csv";
        const string dataTextFile = "dataText.txt";
        const string invitedPlayersFile = "Rinktinė.csv";
        const string clubsFile = "Klubai.csv";

        /// <summary>
        /// entry point of the program
        /// </summary>
        /// <param name="args"></param>
        static void Main(string[] args) {
            Console.OutputEncoding = Encoding.UTF8;

            BasketballTeam team = new BasketballTeam(dataFile);

            PrintOldestPlayers(team);
            PrintShooters(team);

            // save all invited player data to a file
            team.SaveInvitedPlayersData(invitedPlayersFile);

            // save all clubs to a file
            team.SaveClubList(clubsFile);

            // rewrite initial data to a text file
            SaveDataAsTextFile(dataTextFile, team.GetPlayers());
        }

        /// <summary>
        /// prints oldest player data (name, surname, age) to the screen
        /// </summary>
        /// <param name="team">the BasketballTeam to take data from</param>
        static void PrintOldestPlayers(BasketballTeam team) {
            List<BasketballPlayer> oldest = team.GetOldestPlayers();
            Console.WriteLine("Vyriausias(-i) kandidatas(-ai):");

            foreach (BasketballPlayer player in oldest) {
                Console.WriteLine("{0} {1}, amžius: {2}",
                    player.name, player.surname, player.GetAge());
            }
        }

        /// <summary>
        /// prints shooters' data to the screen (name, surname, height)
        /// </summary>
        /// <param name="team">the BasketballTeam to take data from</param>
        static void PrintShooters(BasketballTeam team) {
            List<BasketballPlayer> shooters = team.GetAllPlayersByPosition("puolėjas");
            Console.WriteLine("Puolėjai:");

            foreach (BasketballPlayer player in shooters) {
                Console.WriteLine("{0} {1}, ūgis: {2} cm",
                    player.name, player.surname, player.height);
            }
        }

        /// <summary>
        /// takes initial player data and saves it to a given text file
        /// in a readable format
        /// </summary>
        /// <param name="fileName"></param>
        static void SaveDataAsTextFile(string fileName, List<BasketballPlayer> players) {
            using (StreamWriter writer = new StreamWriter(fileName)) {
                PrintCharacter(writer, '*', 195, true);

                writer.WriteLine("* {0, -25} * {1, -25} * {2, -25} * {3, -15} *" +
                    " {4, -25} * {5, -25} * {6, -15} * {7, -15} *",
                    "Vardas", "Pavardė", "Gimimo data", "Ūgis (cm)", "Pozicija",
                    "Klubas", "Pakviestas", "Kapitonas");

                PrintCharacter(writer, '*', 195, true);

                foreach (BasketballPlayer player in players) {
                    writer.WriteLine("* {0, -25} * {1, -25} * {2, -25} * {3, -15} *" +
                        " {4, -25} * {5, -25} * {6, -15} * {7, -15} *",
                        player.name, player.surname,
                        player.birthDate.ToString("yyyy-MM-dd"),
                        player.height, player.position, player.club, player.invited,
                        player.captain);
                }

                PrintCharacter(writer, '*', 195, true);
            }
        }

        /// <summary>
        /// prints a given character to a StreamWriter how many times wanted
        /// </summary>
        /// <param name="writer">StreamWriter to write to</param>
        /// <param name="character">character to write</param>
        /// <param name="numOfChars">how many characters to write</param>
        /// <param name="newLine">should there be a new line after the characters</param>
        static void PrintCharacter(StreamWriter writer, char character,
                                    int numOfChars, bool newLine) {
            for (int i = 0; i < numOfChars; i++) {
                writer.Write(character);
            }

            if (newLine) {
                writer.WriteLine();
            }
        }
    }

    /// <summary>
    /// basketball team class which contains all player data and performs actions with it
    /// </summary>
    public class BasketballTeam {

        // list of all team players
        private List<BasketballPlayer> players = new List<BasketballPlayer>();

        /// <summary>
        /// empty class constructor
        /// </summary>
        public BasketballTeam() { }

        /// <summary>
        /// class constructor which loads player data
        /// </summary>
        /// <param name="fileName">the file to read the data from</param>
        public BasketballTeam(string fileName) {
            ReadPlayerData(fileName);
        }

        /// <summary>
        /// reads the data of all basketball players from a given file and stores it 
        /// in a list 
        /// </summary>
        /// <param name="fileName">the file to read the data from</param>
        public void ReadPlayerData(string fileName) {
            using (StreamReader reader = new StreamReader(fileName)) {
                string line;

                while ((line = reader.ReadLine()) != null) {
                    // split line data
                    string[] data = line.Split(new char[] { ',', ';' });

                    // create a new player object and add it to the container
                    players.Add(new BasketballPlayer(
                        data[0], data[1],
                        int.Parse(data[2]),
                        int.Parse(data[3]),
                        int.Parse(data[4]),
                        int.Parse(data[5]),
                        data[6], data[7],
                        bool.Parse(data[8]),
                        bool.Parse(data[9])
                    ));
                }
            }
        }

        /// <summary>
        /// saves all the data of invited players to a given file
        /// </summary>
        /// <param name="fileName">the file to save the data to</param>
        public void SaveInvitedPlayersData(string fileName) {
            List<BasketballPlayer> invitedPlayers = GetAllInvitedPlayers();

            using (StreamWriter writer = new StreamWriter(fileName)) {
                foreach (BasketballPlayer player in invitedPlayers) {
                    writer.WriteLine("{0};{1};{2};{3};{4};{5};{6};{7};{8};{9}",
                        player.name,
                        player.surname,
                        player.birthDate.Year,
                        player.birthDate.Month,
                        player.birthDate.Day,
                        player.height,
                        player.position,
                        player.club,
                        player.invited,
                        player.captain);
                }
            }
        }

        /// <summary>
        /// save the list of player clubs to a file
        /// </summary>
        /// <param name="fileName">the file to write the data to</param>
        public void SaveClubList(string fileName) {
            List<string> clubs = GetAllPlayerClubs();

            using (StreamWriter writer = new StreamWriter(fileName)) {
                for (int i = 0; i < clubs.Count; i++) {
                    writer.Write("{0}", clubs[i]);

                    if (i < clubs.Count - 1) {
                        writer.WriteLine(";");
                    }
                }
            }
        }

        /// <summary>
        /// finds the oldest player of the team
        /// </summary>
        /// <returns>a reference to the oldest player object</returns>
        public List<BasketballPlayer> GetOldestPlayers() {
            List<BasketballPlayer> oldestPlayersList = new List<BasketballPlayer>();
            BasketballPlayer oldestPlayer = FindOldestPlayer();

            // lists all oldest players if there are more than 1
            foreach (BasketballPlayer player in players) {
                if (player.birthDate == oldestPlayer.birthDate) {
                    oldestPlayersList.Add(player);
                }
            }

            return oldestPlayersList;
        }

        /// <summary>
        /// finds the oldest player in the team
        /// </summary>
        /// <returns>oldest player object</returns>
        public BasketballPlayer FindOldestPlayer() {
            BasketballPlayer oldestPlayer = this.players[0];

            // finds the oldest player
            for (int i = 1; i < players.Count; i++) {
                if (players[i].birthDate < oldestPlayer.birthDate) {
                    oldestPlayer = players[i];
                }
            }

            return oldestPlayer;
        }

        /// <summary>
        /// finds all players of given position and returns a list containing them
        /// </summary>
        /// <param name="position">the position to search for</param>
        /// <returns>list containing the players</returns>
        public List<BasketballPlayer> GetAllPlayersByPosition(string position) {
            List<BasketballPlayer> players = new List<BasketballPlayer>();

            foreach (BasketballPlayer player in this.players) {
                if (player.position.Contains(position)) {
                    players.Add(player);
                }
            }

            return players;
        }

        /// <summary>
        /// finds all the invited players
        /// </summary>
        /// <returns>a list containing players</returns>
        public List<BasketballPlayer> GetAllInvitedPlayers() {
            List<BasketballPlayer> players = new List<BasketballPlayer>();

            foreach (BasketballPlayer player in this.players) {
                if (player.invited) {
                    players.Add(player);
                }
            }

            return players;
        }

        /// <summary>
        /// finds all clubs in which the players play
        /// </summary>
        /// <returns>a list containing club names</returns>
        public List<string> GetAllPlayerClubs() {
            List<string> clubs = new List<string>();

            foreach (BasketballPlayer player in players) {
                if (!clubs.Contains(player.club)) {
                    clubs.Add(player.club);
                }
            }

            return clubs;
        }

        /// <summary>
        /// get all players
        /// </summary>
        /// <returns>List of players</returns>
        public List<BasketballPlayer> GetPlayers() {
            return players;
        }
    }

    /// <summary>
    /// the class containing all information about a basketball player
    /// </summary>
    public class BasketballPlayer {

        public string name { get; set; }
        public string surname { get; set; }
        public DateTime birthDate { get; set; } // date format: yyyy-mm-dd
        public int height { get; set; }
        public string position { get; set; }
        public string club { get; set; }
        public bool invited { get; set; }
        public bool captain { get; set; }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="name">name of the player</param>
        /// <param name="surname">surname of the player</param>
        /// <param name="year">year of birth</param>
        /// <param name="month">month of birth</param>
        /// <param name="day">day of birth</param>
        /// <param name="height">height of the player</param>
        /// <param name="position">position of the player</param>
        /// <param name="club">club of the player</param>
        /// <param name="invited">is the player invited to the national team</param>
        /// <param name="captain">is the player the captain of the team</param>
        public BasketballPlayer(string name, string surname, int year,
                                int month, int day, int height, string position,
                                string club, bool invited, bool captain) {
            this.name = name;
            this.surname = surname;
            this.height = height;
            this.position = position;
            this.club = club;
            this.invited = invited;
            this.captain = captain;

            birthDate = new DateTime(year, month, day);
        }

        /// <summary>
        /// calculates the age of the player
        /// </summary>
        /// <returns>age as integer</returns>
        public int GetAge() {
            DateTime now = DateTime.Now;
            return (int)(now - birthDate).TotalDays / 365;
        }
    }
}
