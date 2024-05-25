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
    public class CityRating
    {
        public string CityName { get; set; }
        public int TotalScore { get; set; }
    }

    public class CityComparer : IComparer<CityRating>
    {
        public int Compare(CityRating x, CityRating y)
        {
            return x.TotalScore.CompareTo(y.TotalScore);
        }
    }

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

        //Загружает в список отсортированный рейтинг городов
        public static List<string> SortCitiesByRating(List<int> selectedAnswersFull)
        {
            List<CityRating> cityRatings = new List<CityRating>();

            // Получаем значения критериев из selectedAnswersFull
            int[] selectedCriteriaValues = selectedAnswersFull.ToArray();

            // Подключение к базе данных SQLite
            string connectionString = $"Data Source=|DataDirectory|\\Resources\\DBRussianCities.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Выбираем данные о городах из БД
                string query = "SELECT city, temperament, climate, infrastructure, workWages, housingAffordability, ecology, medicalLevel FROM city_filtered";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Получаем данные о городе из запроса
                            string cityName = reader["city"].ToString();
                            int[] cityCriteriaValues = new int[7];
                            for (int i = 0; i < 7; i++)
                            {
                                object value = reader[i + 1];
                                if (value != DBNull.Value)
                                {
                                    cityCriteriaValues[i] = Convert.ToInt32(value);
                                }
                                else
                                {
                                    // Обработка случая, когда значение равно DBNull
                                    cityCriteriaValues[i] = 0; // Пример присвоения нулевого значения
                                }
                            }

                            // Вычисляем общий балл для города
                            int totalScore = 0;
                            for (int i = 0; i < 7; i++)
                            {
                                totalScore += Math.Abs(selectedCriteriaValues[i] - cityCriteriaValues[i]);
                            }

                            // Добавляем город и его общий балл в список
                            cityRatings.Add(new CityRating { CityName = cityName, TotalScore = totalScore });
                        }
                    }
                }

                connection.Close();
            }

            // Сортируем города по общему баллу
            cityRatings.Sort(new CityComparer());

            // Возвращаем отсортированные города
            return cityRatings.Select(cityRating => cityRating.CityName).ToList();
        }


        //Полчение занчений количество населения городов из БД
        public static int GetCityPopulation(string cityName)
        {
            int population = 0;

            // Подключение к базе данных SQLite
            string connectionString = $"Data Source=|DataDirectory|\\Resources\\DBRussianCities.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Выбираем численность населения выбранного города из таблицы "city_filtered"
                string query = "SELECT population FROM city_filtered WHERE city = @City";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    command.Parameters.AddWithValue("@City", cityName);
                    object result = command.ExecuteScalar();
                    if (result != null && result != DBNull.Value)
                    {
                        population = Convert.ToInt32(result);
                    }
                }

                connection.Close();
            }

            return population;
        }


        public static List<(string cityName, double latitude, double longitude, int climate)> GetCityData()
        {
            List<(string cityName, double latitude, double longitude, int climate)> cityDataClimate = new List<(string cityName, double latitude, double longitude, int climate)>();

            // Подключение к базе данных SQLite
            string connectionString = $"Data Source=|DataDirectory|\\Resources\\DBRussianCities.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Выбираем данные о городах из БД
                string query = "SELECT city, geo_lat, geo_lon, climate FROM city_filtered";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Получаем данные о городе из запроса
                            string cityName = reader["city"].ToString();
                            double latitude = Convert.ToDouble(reader["geo_lat"]);
                            double longitude = Convert.ToDouble(reader["geo_lon"]);
                            int climateValue = 0; // Значение по умолчанию
                            if (!reader.IsDBNull(reader.GetOrdinal("climate"))) // Проверяем, не является ли значение DBNull
                            {
                                climateValue = Convert.ToInt32(reader["climate"]);
                            }

                            // Добавляем данные о городе в список
                            cityDataClimate.Add((cityName, latitude, longitude, climateValue));
                        }
                    }
                }

                connection.Close();
            }

            return cityDataClimate;
        }


        public static List<(string cityName, double latitude, double longitude, int ecology)> GetEcologicalData()
        {
            List<(string cityName, double latitude, double longitude, int ecology)> cityDataEcology = new List<(string cityName, double latitude, double longitude, int ecology)>();

            // Подключение к базе данных SQLite
            string connectionString = $"Data Source=|DataDirectory|\\Resources\\DBRussianCities.db;Version=3;";
            using (SQLiteConnection connection = new SQLiteConnection(connectionString))
            {
                connection.Open();

                // Выбираем данные о городах из БД
                string query = "SELECT city, geo_lat, geo_lon, ecology FROM city_filtered";
                using (SQLiteCommand command = new SQLiteCommand(query, connection))
                {
                    using (SQLiteDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            // Получаем данные о городе из запроса
                            string cityName = reader["city"].ToString();
                            double latitude = Convert.ToDouble(reader["geo_lat"]);
                            double longitude = Convert.ToDouble(reader["geo_lon"]);
                            int ecologyValue = 0; // Значение по умолчанию
                            if (!reader.IsDBNull(reader.GetOrdinal("ecology"))) // Проверяем, не является ли значение DBNull
                            {
                                ecologyValue = Convert.ToInt32(reader["ecology"]);
                            }

                            // Добавляем данные о городе в список
                            cityDataEcology.Add((cityName, latitude, longitude, ecologyValue));
                        }
                    }
                }

                connection.Close();
            }

            return cityDataEcology;
        }
    }
}
