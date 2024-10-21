namespace FruitsAndVegetables
{
    using System;
    using System.Data;
    using Microsoft.Data.SqlClient;

    class Program
    {
        private static string connectionString = "Server=DESKTOP-9PK656A\\SQLEXPRESS;Database=FruitsAndVegetables;Trusted_Connection=True;TrustServerCertificate=True";

        static void Main()
        {
            while (true)
            {
                Console.Clear();
                Console.WriteLine("1. Подключиться к базе данных");
                Console.WriteLine("2. Отключиться от базы данных");
                Console.WriteLine("3. Отобразить всю информацию");
                Console.WriteLine("4. Отобразить названия овощей и фруктов");
                Console.WriteLine("5. Отобразить все цвета");
                Console.WriteLine("6. Показать максимальную калорийность");
                Console.WriteLine("7. Показать минимальную калорийность");
                Console.WriteLine("8. Показать среднюю калорийность");
                Console.WriteLine("9. Показать количество овощей");
                Console.WriteLine("10. Показать количество фруктов");
                Console.WriteLine("11. Показать количество овощей и фруктов заданного цвета");
                Console.WriteLine("12. Показать количество овощей и фруктов каждого цвета");
                Console.WriteLine("13. Показать продукты с калорийностью ниже указанной");
                Console.WriteLine("14. Показать продукты с калорийностью выше указанной");
                Console.WriteLine("15. Показать продукты с калорийностью в диапазоне");
                Console.WriteLine("16. Показать продукты с красным или жёлтым цветом");
                Console.WriteLine("0. Выход");
                Console.Write("Выберите действие: ");
                string choice = Console.ReadLine();

                switch (choice)
                {
                    case "1": Connect(); break;
                    case "2": Disconnect(); break;
                    case "3": DisplayAll(); break;
                    case "4": DisplayNames(); break;
                    case "5": DisplayColors(); break;
                    case "6": ShowMaxCalories(); break;
                    case "7": ShowMinCalories(); break;
                    case "8": ShowAverageCalories(); break;
                    case "9": ShowVegetableCount(); break;
                    case "10": ShowFruitCount(); break;
                    case "11": ShowCountByColor(); break;
                    case "12": ShowCountByEachColor(); break;
                    case "13": ShowBelowCalories(); break;
                    case "14": ShowAboveCalories(); break;
                    case "15": ShowInRangeCalories(); break;
                    case "16": ShowRedOrYellow(); break;
                    case "0": return;
                    default: Console.WriteLine("Неверный выбор."); break;
                }

                Console.WriteLine("\nНажмите любую клавишу для продолжения...");
                Console.ReadKey();
            }
        }

        static void Connect()
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                try
                {
                    connection.Open();
                    Console.WriteLine("Подключение успешно.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка подключения: {ex.Message}");
                }
            }
        }

        static void Disconnect()
        {
            Console.WriteLine("Отключение от базы данных выполнено.");
        }

        static void DisplayAll()
        {
            ExecuteQuery("SELECT * FROM Products", reader =>
            {
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Id"]}. {reader["Name"]} - {reader["Type"]} - {reader["Color"]} - {reader["Calories"]} ккал");
                }
            });
        }

        static void DisplayNames()
        {
            ExecuteQuery("SELECT Name FROM Products", reader =>
            {
                while (reader.Read())
                {
                    Console.WriteLine(reader["Name"]);
                }
            });
        }

        static void DisplayColors()
        {
            ExecuteQuery("SELECT DISTINCT Color FROM Products", reader =>
            {
                while (reader.Read())
                {
                    Console.WriteLine(reader["Color"]);
                }
            });
        }

        static void ShowMaxCalories() => ExecuteScalar("SELECT MAX(Calories) FROM Products");
        static void ShowMinCalories() => ExecuteScalar("SELECT MIN(Calories) FROM Products");
        static void ShowAverageCalories() => ExecuteScalar("SELECT AVG(Calories) FROM Products");
        static void ShowVegetableCount() => ExecuteScalar("SELECT COUNT(*) FROM Products WHERE Type = 'Овощ'");
        static void ShowFruitCount() => ExecuteScalar("SELECT COUNT(*) FROM Products WHERE Type = 'Фрукт'");

        static void ShowCountByColor()
        {
            Console.Write("Введите цвет: ");
            string color = Console.ReadLine();
            ExecuteScalar($"SELECT COUNT(*) FROM Products WHERE Color = '{color}'");
        }

        static void ShowCountByEachColor()
        {
            ExecuteQuery("SELECT Color, COUNT(*) AS Count FROM Products GROUP BY Color", reader =>
            {
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["Color"]}: {reader["Count"]}");
                }
            });
        }

        static void ShowBelowCalories()
        {
            Console.Write("Введите значение калорий: ");
            int calories = int.Parse(Console.ReadLine());
            ExecuteQuery($"SELECT Name FROM Products WHERE Calories < {calories}", reader =>
            {
                while (reader.Read())
                {
                    Console.WriteLine(reader["Name"]);
                }
            });
        }

        static void ShowAboveCalories()
        {
            Console.Write("Введите значение калорий: ");
            int calories = int.Parse(Console.ReadLine());
            ExecuteQuery($"SELECT Name FROM Products WHERE Calories > {calories}", reader =>
            {
                while (reader.Read())
                {
                    Console.WriteLine(reader["Name"]);
                }
            });
        }

        static void ShowInRangeCalories()
        {
            Console.Write("Введите минимальное значение калорий: ");
            int min = int.Parse(Console.ReadLine());
            Console.Write("Введите максимальное значение калорий: ");
            int max = int.Parse(Console.ReadLine());
            ExecuteQuery($"SELECT Name FROM Products WHERE Calories BETWEEN {min} AND {max}", reader =>
            {
                while (reader.Read())
                {
                    Console.WriteLine(reader["Name"]);
                }
            });
        }

        static void ShowRedOrYellow()
        {
            ExecuteQuery("SELECT Name FROM Products WHERE Color IN ('Красный', 'Желтый')", reader =>
            {
                while (reader.Read())
                {
                    Console.WriteLine(reader["Name"]);
                }
            });
        }

        static void ExecuteQuery(string query, Action<SqlDataReader> action)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    action(reader);
                }
            }
        }

        static void ExecuteScalar(string query)
        {
            using (SqlConnection connection = new SqlConnection(connectionString))
            {
                SqlCommand command = new SqlCommand(query, connection);
                connection.Open();
                object result = command.ExecuteScalar();
                Console.WriteLine(result);
            }
        }
    }

}
