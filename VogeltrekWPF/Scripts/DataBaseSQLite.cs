﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Controls;

namespace VogeltrekWPF.Scripts
{
    internal class DataBaseSQLite
    {
            public static void LoadDataToListBox(ListBox listBox)
            {
            // Подключение к базе данных SQLite
            string connectionString = $"Data Source=|DataDirectory|\\Resources\\DBRussianCities.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
                {
                    connection.Open();

                    // Выборка данных из таблицы "city_filtered"
                    string query = "SELECT city FROM city_filtered";
                    using (SQLiteCommand command = new SQLiteCommand(query, connection))
                    {
                        using (SQLiteDataReader reader = command.ExecuteReader())
                        {
                            // Создание списка для хранения названий городов
                            List<string> cityNames = new List<string>();

                            // Чтение данных и добавление их в список
                            while (reader.Read())
                            {
                                string cityName = reader["city"].ToString();
                                cityNames.Add(cityName);
                            }

                            // Привязка списка к ListBox
                            listBox.ItemsSource = cityNames;
                        }
                    }

                    connection.Close();
                }
            }
    }
}