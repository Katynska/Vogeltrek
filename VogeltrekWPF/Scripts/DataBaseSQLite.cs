using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SQLite;
using System.Windows.Controls;
using System.Windows.Markup;

namespace VogeltrekWPF.Scripts
{
    internal class DataBaseSQLite
    {
        //Загружает данные в ListBox и ComboBox
        public static void LoadDataFromDB(ListBox listBox, ComboBox comboBox)
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
                            // Привязка списка к ComboBox
                            comboBox.ItemsSource = cityNames;
                    }
                    }

                    connection.Close();
                }
            }


        //Загружает на карту отметки выбранные в ListBox
        public static (double latitude, double longitude) GetCityCoordinates(string selectedCity)
        {
            double latitude = 0.0;
            double longitude = 0.0;

            // Подключение к базе данных SQLite
            string connectionString = $"Data Source=|DataDirectory|\\Resources\\DBRussianCities.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Выбираем координаты выбранного города из таблицы "city_filtered"
                string query = "SELECT geo_lat, geo_lon FROM city_filtered WHERE city = @City";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@City", selectedCity);
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        // Если удалось получить координаты, устанавливаем их
                        if (reader.Read())
                        {
                            latitude = Convert.ToDouble(reader["geo_lat"]);
                            longitude = Convert.ToDouble(reader["geo_lon"]);
                        }
                    }
                }

                connection.Close();
            }

            return (latitude, longitude);
        }
    }
}
