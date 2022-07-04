using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.Data.Sqlite;

namespace Habit_Tracker
{
    class Program
    {
        static string connectionString = @"Data Source=habit-Tracker.db";
        static void Main(string[] args)
        {


            CreateDatabase();

            void CreateDatabase()
            {
                /*Creating a connection passing the connection string as an argument
                This will create the database for you, there's no need to manually create it.
                And no need to use File.Create().*/
                using (var connection = new SqliteConnection(connectionString))
                {
                    //Creating the command that will be sent to the database
                    using (var tableCmd = connection.CreateCommand())
                    {
                        connection.Open();
                        //Declaring what is that command (in SQL syntax)
                        tableCmd.CommandText =
                            @"CREATE TABLE IF NOT EXISTS yourHabit (
                    Id INTEGER PRIMARY KEY AUTOINCREMENT,
                    Date TEXT,
                    Quantity INTEGER
                    )";

                        // Executing the command, which isn't a query, it's not asking to return data from the database.
                        tableCmd.ExecuteNonQuery();
                    }
                    // We don't need to close the connection or the command. The 'using statement' does that for us.
                }

                /* Once we check if the database exists and create it (or not),
                we will call the next method, which will handle the user's input. Your next step is to create this method*/
                //GetUserInput();
            }
            MainMenu();
        }
        static void MainMenu()
        {
            Console.Clear();
            Console.WriteLine("Please select an option");
            Console.WriteLine("Enter 1 to create an entry.");
            Console.WriteLine("Enter 2 to display all entries.");
            Console.WriteLine("Enter 3 to delete an entry");
            Console.WriteLine("Enter 4 to see all entries");

            string optionSelected = Console.ReadLine();
            bool closeApp = false;
            while (closeApp == false)
            {
                switch (optionSelected)
                {
                    case "0": closeApp = true; break;
                    case "1": CreateEntry(); break;
                    case "2": DisplayAll(); break;
                    default:
                        break;
                }
            }
        }
        public static void CreateEntry()
        {
            string date = GetDateInput();
            int quantity = GetQuantityInput();
            using (var connection = new SqliteConnection(connectionString))
            {

                using (var tableCmd = connection.CreateCommand())
                {
                    connection.Open();

                    tableCmd.CommandText =
                        $"INSERT INTO yourHabit (Date, Quantity) VALUES('{date}',{quantity})";

                    tableCmd.ExecuteNonQuery();
                    connection.Close();
                }
                MainMenu();
            }
        }
        static string GetDateInput()
        {
            Console.WriteLine("Please enter the date in format DD MMMM YYYY. Enter 0 for main menu");
            string dateInput = Console.ReadLine();
            if (dateInput == "0") MainMenu();
            return dateInput;
        }

        static int GetQuantityInput()
        {
            Console.WriteLine("Please enter the number of glasses of water you drank e.g 4 (no decimals). Enter 0 for main menu.");
            string quantityInput = Console.ReadLine();
            if (quantityInput == "0") MainMenu();
            int convertedQuantityInput = Convert.ToInt32(quantityInput);
            return convertedQuantityInput;
        }
        public static void DisplayAll()
        {

            using (var connection = new SqliteConnection(connectionString))
            {

                    var tableCmd = connection.CreateCommand();
                    connection.Open();
                    tableCmd.CommandText =
                        $"SELECT * FROM yourHabit";

                    List<DrinkingWater> tableData = new();
                    SqliteDataReader reader = tableCmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            tableData.Add(
                                new DrinkingWater
                                {
                                    Id = reader.GetInt32(0),
                                    Date = DateTime.ParseExact(reader.GetString(1), "dd MMMM yyyy", CultureInfo.InvariantCulture),
                                    
                                    Quantity = reader.GetInt32(2)
                                }); 
                        }
                        
                    }
                    else
                    {
                        Console.WriteLine("No entries found");
                    }

                    connection.Close();
                    

                    foreach (var item in tableData)
                    {
                       Console.WriteLine($"Id: {item.Id} Date:{item.Date} Quantity: {item.Quantity}");
                        
                    }

                
                
            }
        }
    }

    public class DrinkingWater
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public int Quantity { get; set; }
    }
}
