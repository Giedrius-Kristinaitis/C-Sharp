/*
 * Author: Giedrius Kristinaitis
 * 
 * For the sake of simplicity all classes are kept in the same source file 
 */

using System;
using System.IO;
using System.Text;

namespace Money {

    /// <summary>
    /// main program class containing the Main() method
    /// </summary>
    class Program {

        public static int NumberOfBanknoteTypes = 6; // number of different banknotes in the data file

        /// <summary>
        /// entry point of the program
        /// </summary>
        /// <param name="args">arguments for the program</param>
        static void Main(string[] args) {
            Console.OutputEncoding = Encoding.UTF8;

            BanknoteContainer banknotes = new BanknoteContainer(NumberOfBanknoteTypes);
            ClientContainer clients = new ClientContainer(100);

            ReadData("U3.txt", banknotes, clients);
            PrintInitialDataToTextFile("pradDuom.txt", banknotes, clients);

            ATM atm = new ATM(banknotes);
            PerformWithdrawOperations(atm, clients);

            PrintResults("rezult.txt", atm, clients);
        }

        /// <summary>
        /// prints execution results to a given file
        /// </summary>
        /// <param name="fileName">file name to write data</param>
        /// <param name="atm">ATM object on which money withdrawing was performed</param>
        /// <param name="clients">ClientContainer object containing client data</param>
        static void PrintResults(string fileName, ATM atm, ClientContainer clients) {
            using (StreamWriter writer = new StreamWriter(fileName)) {
                for (int i = 0; i < clients.Count; i++) {
                    writer.WriteLine("{0}", clients.GetClient(i).WithdrawOperationString);
                }

                writer.WriteLine();
                writer.WriteLine("{0}", atm.ToString());
            }
        }

        /// <summary>
        /// loops through all clients and tries to withdraw money from the ATM
        /// </summary>
        /// <param name="atm">ATM to withdraw money from</param>
        /// <param name="clients">ClientContainer from which client data will be taken</param>
        static void PerformWithdrawOperations(ATM atm, ClientContainer clients) {
            for (int i = 0; i < clients.Count; i++) {
                atm.WithdrawMoney(clients.GetClient(i));
            }
        }

        /// <summary>
        /// writes initial data to a seperate text file formatted as two tables:
        /// one for banknote data and another one for client data
        /// </summary>
        /// <param name="fileName">file name to write data to</param>
        /// <param name="banknotes">BanknoteContainer containing initial banknote data</param>
        /// <param name="clients">ClientContainer containing initial client data</param>
        static void PrintInitialDataToTextFile(string fileName, BanknoteContainer banknotes, ClientContainer clients) {
            using (StreamWriter writer = new StreamWriter(fileName)) {
                writer.WriteLine("Banknotes:");
                writer.WriteLine("{0, -20} {1, -20}", "Banknote wealth", "Banknote count");
                PrintBanknotes(writer, banknotes);

                writer.WriteLine();
                writer.WriteLine("Clients:");
                writer.WriteLine("{0, -20} {1, -20} {2, -20}", 
                    "Bank account number", "Current ammount", "Ammount to withdraw");
                PrintClients(writer, clients);
            }
        }
        
        /// <summary>
        /// loops through all Banknote objects in a BanknoteContainer and writes formatted banknote strings to
        /// a StreamWriter
        /// </summary>
        /// <param name="writer">StreamWriter to write to</param>
        /// <param name="banknotes">BanknoteContainer containing banknotes to print</param>
        static void PrintBanknotes(StreamWriter writer, BanknoteContainer banknotes) {
            for (int i = 0; i < banknotes.Count; i++) {
                writer.WriteLine("{0}", banknotes.GetBanknote(i).ToString());
            }
        }

        /// <summary>
        /// loops through all Client objects in a ClientContainer and writes formatted client strings to
        /// a StreamWriter
        /// </summary>
        /// <param name="writer">StreamWriter to write to</param>
        /// <param name="clients">ClientContainer containing cliets to print</param>
        static void PrintClients(StreamWriter writer, ClientContainer clients) {
            for (int i = 0; i < clients.Count; i++) {
                writer.WriteLine("{0}", clients.GetClient(i).ToString());
            }
        }

        /// <summary>
        /// reads banknote and client data from a given file and stores it in BanknoteContainer and ClientContainer
        /// </summary>
        /// <param name="fileName">file name to read from</param>
        /// <param name="banknotes">BanknoteContainer to store Banknote objects in</param>
        /// <param name="clients">ClientContainer to store Client objects in</param>
        static void ReadData(string fileName, BanknoteContainer banknotes, ClientContainer clients) {
            using (StreamReader reader = new StreamReader(fileName)) {
                CreateBankotes(reader.ReadLine().Split(' '), reader.ReadLine().Split(' '), banknotes);

                int clientCount = int.Parse(reader.ReadLine());

                for (int i = 0; i < clientCount; i++) {
                    string line = reader.ReadLine();
                    string bankAccountNumber = line.Substring(0, 4);
                    string[] data = line.Substring(4).Split(new char[] { ' ' }, 
                        StringSplitOptions.RemoveEmptyEntries);
                    int currentAmmount = int.Parse(data[0]);
                    int ammountToWithdraw = int.Parse(data[1]);

                    clients.AddClient(new Client(bankAccountNumber, currentAmmount, ammountToWithdraw));
                }
            }
        }

        /// <summary>
        /// creates BanknoteCount object for each banknote in the data arrays
        /// 
        /// EXAMPLE DATA ARRAYS:
        /// banknoteType: ["500", "200", "100"]
        /// banknoteCount: ["10", "7", "15"]
        /// this means there are 10 banknotes worth 500, 7 banknotes worth 200 and 15 banknotes worth 100
        /// </summary>
        /// <param name="banknoteType">array containing banknote wealth values</param>
        /// <param name="banknoteCount">array containing banknote count values</param>
        /// <param name="banknotes">BanknoteContainer to store created BanknoteCount objects in</param>
        static void CreateBankotes(string[] banknoteType, string[] banknoteCount, BanknoteContainer banknotes) {
            for (int i = 0; i < NumberOfBanknoteTypes; i++) {
                banknotes.AddBanknote(new BanknoteCount(int.Parse(banknoteType[i]),
                    int.Parse(banknoteCount[i])));
            }
        }
    }


    /// <summary>
    /// ATM class containing banknotes and performing withdraw operations
    /// </summary>
    class ATM {

        public BanknoteContainer Banknotes { get; set; } // banknotes in the ATM

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="banknotes">BanknoteContainer containing all banknotes in the ATM</param>
        public ATM(BanknoteContainer banknotes) {
            Banknotes = banknotes;
        }

        /// <summary>
        /// tries to withdraw money from a client's account
        /// </summary>
        /// <param name="client">Client object to take data from (client's current ammount, ammount of money
        /// the client wants to withdraw and the client's bank account number)</param>
        public void WithdrawMoney(Client client) {
            // client does not have enough money, cancel withdrawing
            if (client.AmmountToWithdraw > client.CurrentAmmount) {
                client.WithdrawOperationString = string.Format("{0} {1} {2} Negalima išmokėti",
                    client.BankAccountNumber, client.CurrentAmmount, client.AmmountToWithdraw);
                return;
            }

            // client has more money than the ammount to be withdrawn
            else {
                if (PossibleToWithdraw(client.AmmountToWithdraw)) {
                    bool success = true;
                    BanknoteContainer banknotes = GetBanknotes(client.AmmountToWithdraw, out success);

                    RemoveBanknotes(banknotes);
                    client.CurrentAmmount -= client.AmmountToWithdraw;
                    client.WithdrawOperationString = FormSuccessfullWithdrawingOperationString(client, banknotes);
                } else {
                    client.WithdrawOperationString = string.Format("{0} {1} {2} Negalima išmokėti",
                        client.BankAccountNumber, client.CurrentAmmount, client.AmmountToWithdraw);
                }
            }
        }

        /// <summary>
        /// creates information string from a successfull withdraw operation:
        /// client's bank account number, banknotes the client just received and the ammount of money client has left
        /// </summary>
        /// <param name="client">Client who performed withdraw operation</param>
        /// <param name="banknotes">banknotes the client received from the ATM</param>
        /// <returns>formatted withdrawing operation text</returns>
        private string FormSuccessfullWithdrawingOperationString(Client client, BanknoteContainer banknotes) {
            string operation = string.Format("{0} {1}",
                client.BankAccountNumber, client.CurrentAmmount + CalculateMoney(banknotes), client.AmmountToWithdraw);

            for (int i = 0; i < banknotes.Count; i++) {
                BanknoteCount banknote = banknotes.GetBanknote(i) as BanknoteCount;
                operation = string.Format("{0}-{1}*{2}", operation, banknote.Wealth, banknote.Count);
            }

            operation = string.Format("{0} = {1}", operation, client.CurrentAmmount);

            return operation;
        }

        /// <summary>
        /// calculates the ammount of money in BanknoteContainer
        /// </summary>
        /// <param name="banknotes">BanknoteContainer to take data from</param>
        /// <returns>the ammount of money from the banknotes</returns>
        public int CalculateMoney(BanknoteContainer banknotes) {
            int ammount = 0;

            for (int i = 0; i < banknotes.Count; i++) {
                BanknoteCount banknote = banknotes.GetBanknote(i) as BanknoteCount;
                ammount += banknote.Count * banknote.Wealth;
            }

            return ammount;
        }

        /// <summary>
        /// removes banknotes from the ATM
        /// called only after a successfull withdraw operation
        /// </summary>
        /// <param name="banknotes">BanknoteContainer containing BanknoteCount objects with the number of 
        /// banknotes to remove from the ATM</param>
        private void RemoveBanknotes(BanknoteContainer banknotes) {
            for (int i = 0; i < banknotes.Count; i++) {
                Banknote banknote = banknotes.GetBanknote(i);
                int banknoteIndex = Banknotes.BanknoteIndex(banknote);
                
                Banknotes.InsertBanknote(banknoteIndex,
                    (Banknotes.GetBanknote(banknoteIndex) as BanknoteCount) - (banknote as BanknoteCount));
            }
        }

        /// <summary>
        /// checks if it is possible to withdraw given ammount of money from the ATM
        /// withdrawing operation could be impossible if there are not enough money in the ATM or there are no
        /// right banknotes left
        /// </summary>
        /// <param name="ammount">ammount of money to withdraw</param>
        /// <returns>true if it is possible to withdraw, false otherwise</returns>
        public bool PossibleToWithdraw(int ammount) {
            if (ammount % 10 != 0) {
                return false;
            } else {
                bool canWithdraw = false;
                GetBanknotes(ammount, out canWithdraw);

                return canWithdraw;
            }
        }

        /// <summary>
        /// tries to get enough right banknotes for a withdraw operation
        /// this method does not remove any banknotes from the ATM, it only creates copies of the banknotes
        /// to withdraw
        /// </summary>
        /// <param name="ammount">ammount of money to withdraw</param>
        /// <param name="success">boolean indicating wether there was enough right banknotes to withdraw</param>
        /// <returns>BanknoteContainer of banknotes to be withdrawn (will be used only is success = true)</returns>
        private BanknoteContainer GetBanknotes(int ammount, out bool success) {
            BanknoteContainer banknotes = new BanknoteContainer(Program.NumberOfBanknoteTypes);
            
            for (int i = 0; i < Banknotes.Count; i++) {
                BanknoteCount banknote = Banknotes.GetBanknote(i) as BanknoteCount;

                int numberOfBanknotesRequired = ammount / banknote.Wealth;
                int availableBanknotes = (banknote.Count >= numberOfBanknotesRequired)
                    ? numberOfBanknotesRequired : banknote.Count;

                banknotes.AddBanknote(new BanknoteCount(banknote.Wealth, availableBanknotes));
                ammount -= availableBanknotes * banknote.Wealth;
            }

            success = ammount == 0;

            return banknotes;
        }

        /// <summary>
        /// formats the ammount of money in the ATM
        /// </summary>
        /// <returns>formatted string of the money in the ATM</returns>
        public override string ToString() {
            string atmString = "";
            int ammountOfMoney = 0;

            for (int i = 0; i < Banknotes.Count; i++) {
                BanknoteCount banknote = Banknotes.GetBanknote(i) as BanknoteCount;
                atmString = string.Format("{0}{1}*{2}+", atmString, banknote.Wealth, banknote.Count);
                ammountOfMoney += banknote.Wealth * banknote.Count;
            }

            atmString = atmString.Substring(0, atmString.Length - 1);
            atmString = string.Format("{0} = {1}", atmString, ammountOfMoney);

            return atmString;
        }
    }


    /// <summary>
    /// client class containing client information
    /// </summary>
    class Client {

        public string BankAccountNumber { get; set; }
        public int CurrentAmmount { get; set; }
        public int AmmountToWithdraw { get; set; }
        public string WithdrawOperationString { get; set; }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="bankAccountNumber">client's bank account number</param>
        /// <param name="currentAmmount">the ammount of money the client currently has</param>
        /// <param name="ammountToWithdraw">the ammount of money the client wants to withdraw</param>
        public Client(string bankAccountNumber, int currentAmmount, int ammountToWithdraw) {
            BankAccountNumber = bankAccountNumber;
            CurrentAmmount = currentAmmount;
            AmmountToWithdraw = ammountToWithdraw;
            WithdrawOperationString = "";
        }

        /// <summary>
        /// formats the client information
        /// </summary>
        /// <returns>formatted string with client data</returns>
        public override string ToString() {
            return string.Format("{0, -20} {1, 20} {2, 20}", 
                BankAccountNumber, CurrentAmmount, AmmountToWithdraw);
        }
    }


    /// <summary>
    /// client container
    /// </summary>
    class ClientContainer {

        private Client[] Clients;
        public int Count { get; private set; }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="maxSize">maximum number of clients the container can store</param>
        public ClientContainer(int maxSize) {
            Clients = new Client[maxSize];
        }

        /// <summary>
        /// adds a client to the end of the container
        /// </summary>
        /// <param name="client">Client to add</param>
        public void AddClient(Client client) {
            Clients[Count++] = client;
        }

        /// <summary>
        /// inserts a client at a given index in the container
        /// overrides existing client at that index
        /// </summary>
        /// <param name="index">index to insert client to</param>
        /// <param name="client">Client to insert</param>
        public void InsertClient(int index, Client client) {
            Clients[index] = client;
        }

        /// <summary>
        /// returns the client at specified index
        /// </summary>
        /// <param name="index">the index to take client from</param>
        /// <returns>Client object</returns>
        public Client GetClient(int index) {
            return Clients[index];
        }
    }


    /// <summary>
    /// banknote container
    /// </summary>
    class BanknoteContainer {

        private Banknote[] Banknotes;
        public int Count { get; private set; }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="maxSize">maximum number of banknotes the container can store</param>
        public BanknoteContainer(int maxSize) {
            Banknotes = new Banknote[maxSize];
        }

        /// <summary>
        /// adds a banknote to the end of the container
        /// </summary>
        /// <param name="banknote">Banknote to add</param>
        public void AddBanknote(Banknote banknote) {
            Banknotes[Count++] = banknote;
        }

        /// <summary>
        /// inserts a banknote at a given index
        /// </summary>
        /// <param name="index">index to insert banknote to</param>
        /// <param name="banknote">Banknote to insert</param>
        public void InsertBanknote(int index, Banknote banknote) {
            Banknotes[index] = banknote;
        }

        /// <summary>
        /// returns the banknote at the specified index
        /// </summary>
        /// <param name="index">index to take banknote from</param>
        /// <returns>Banknote object</returns>
        public Banknote GetBanknote(int index) {
            return Banknotes[index];
        }

        /// <summary>
        /// removes a banknote from the container
        /// </summary>
        /// <param name="index">index to remove banknote from</param>
        public void RemoveBanknote(int index) {
            for (int i = index + 1; i < Count; i++) {
                Banknotes[i - 1] = Banknotes[i];
            }

            Count--;
        }

        /// <summary>
        /// gets the index in the container of a given banknote
        /// </summary>
        /// <param name="banknote">Banknote to check for</param>
        /// <returns>the index of the given Banknote, -1 if the banknote was not found in the container</returns>
        public int BanknoteIndex(Banknote banknote) {
            for (int i = 0; i < Count; i++) {
                if (Banknotes[i] == banknote) {
                    return i;
                }
            }

            return -1;
        }
    }


    /// <summary>
    /// banknote class containing wealth of the banknote
    /// </summary>
    class Banknote {

        public int Wealth { get; set; }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="wealth">wealth of the banknote</param>
        public Banknote(int wealth) {
            Wealth = wealth;
        }

        /// <summary>
        /// checks if the banknote is equal to a given banknote
        /// (banknotes are considered equal if their wealth matches)
        /// </summary>
        /// <param name="obj">banknote object to check for equality</param>
        /// <returns>true if banknotes are equal, false otherwise</returns>
        public override bool Equals(object obj) {
            return (obj as Banknote).Wealth == Wealth;
        }

        /// <summary>
        /// gets hash code of the banknote
        /// </summary>
        /// <returns>hash code integer</returns>
        public override int GetHashCode() {
            return ToString().GetHashCode();
        }

        /// <summary>
        /// formats the banknote information
        /// </summary>
        /// <returns>formatted string containing banknote wealth</returns>
        public override string ToString() {
            return string.Format("{0, 20}", Wealth);
        }

        /// <summary>
        /// operator to check for equality of two banknotes
        /// </summary>
        /// <param name="lhs">first banknote of the equality check</param>
        /// <param name="rhs">second banknote of the equality check</param>
        /// <returns>true if banknotes are equal, false otherwise</returns>
        public static bool operator == (Banknote lhs, Banknote rhs) {
            return lhs.Equals(rhs);
        }

        /// <summary>
        /// operator to check for inequality of two banknotes
        /// </summary>
        /// <param name="lhs">first banknote of the inequality check</param>
        /// <param name="rhs">second banknote of the inequality check</param>
        /// <returns>true if banknotes are not equal, false otherwise</returns>
        public static bool operator != (Banknote lhs, Banknote rhs) {
            return !lhs.Equals(rhs);
        }
    }


    /// <summary>
    /// banknote count class containing the number of specific wealth banknotes
    /// </summary>
    class BanknoteCount : Banknote {

        public int Count { get; set; }

        /// <summary>
        /// class constructor
        /// </summary>
        /// <param name="wealth">wealth of the banknote</param>
        /// <param name="count">number of banknotes</param>
        public BanknoteCount(int wealth, int count) : base(wealth) {
            Count = count;
        }

        /// <summary>
        /// operator to add two BanknoteCount objects
        /// </summary>
        /// <param name="lhs">first banknote of the addition operation</param>
        /// <param name="rhs">second banknote of the addition operation</param>
        /// <returns>BanknoteCount containing the same wealth as the parameter objects and
        /// the new number of banknotes</returns>
        public static BanknoteCount operator + (BanknoteCount lhs, BanknoteCount rhs) {
            return new BanknoteCount(lhs.Wealth, lhs.Count + rhs.Count);
        }

        /// <summary>
        /// operator to subtract two BanknoteCount objects
        /// </summary>
        /// <param name="lhs">first banknote of the subtraction operation</param>
        /// <param name="rhs">second banknote of the subtraction operation</param>
        /// <returns>BanknoteCount containing the same wealth as the parameter objects and
        /// the new number of banknotes</returns>
        public static BanknoteCount operator - (BanknoteCount lhs, BanknoteCount rhs) {
            return new BanknoteCount(lhs.Wealth, lhs.Count - rhs.Count);
        }

        /// <summary>
        /// formats banknote information
        /// </summary>
        /// <returns>formatted string containing banknote wealth and number of banknotes</returns>
        public override string ToString() {
            return string.Format("{0} {1, 20}", base.ToString(), Count);
        }
    }
}
