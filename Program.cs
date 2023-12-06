using System;
using System.Data.SqlClient;
using System.Data;

namespace Labb1Sql;
class Program
{
    static string connectionString = "Data Source=(localdb)\\.;Initial Catalog=SchoolDatabase;Integrated Security=True";

    static void Main()
    {
        bool exit = false;

        while (!exit)
        {
            Console.Clear();
            Console.WriteLine("1. Get all students");
            Console.WriteLine("2. Get all students in a specific class");
            Console.WriteLine("3. Add new staff member");
            Console.WriteLine("4. Get staff members");
            Console.WriteLine("5. Get all grades set in the last month");
            Console.WriteLine("6. Get average, highest, and lowest grades per course");
            Console.WriteLine("7. Add new student");
            Console.WriteLine("0. Exit");

            Console.Write("Choose a function (enter a number): ");
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    DisplayAllStudents();
                    break;
                case "2":
                    DisplayStudentsInClass();
                    break;
                case "3":
                    AddNewStaff();
                    break;
                case "4":
                    GetStaffMembers();
                    break;
                case "5":
                    GetGradesInLastMonth();
                    break;
                case "6":
                    GetCourseStatistics();
                    break;
                case "7":
                    AddNewStudent();
                    break;
                case "0":
                    exit = true;
                    break;
                default:
                    Console.WriteLine("Invalid choice. Please try again.");
                    break;
            }

            if (!exit)
            {
                Console.WriteLine("Press Enter to return to the main menu.");
                Console.ReadLine();
                Console.Clear();
            }
        }
    }

    static void DisplayAllStudents()
    {
        Console.Clear();
        Console.WriteLine("Choose sorting order:");
        Console.WriteLine("1. First name ascending");
        Console.WriteLine("2. First name descending");
        Console.WriteLine("3. Last name ascending");
        Console.WriteLine("4. Last name descending");

        Console.Write("Enter the number for the sorting option: ");
        string sortChoice = Console.ReadLine();

        string orderBy = "";

        switch (sortChoice)
        {
            case "1":
                orderBy = "ORDER BY FirstName ASC";
                break;
            case "2":
                orderBy = "ORDER BY FirstName DESC";
                break;
            case "3":
                orderBy = "ORDER BY LastName ASC";
                break;
            case "4":
                orderBy = "ORDER BY LastName DESC";
                break;
            default:
                Console.WriteLine("Invalid choice for sorting option.");
                return;
        }

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = $"SELECT * FROM Students {orderBy}";
            SqlCommand command = new SqlCommand(query, connection);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                Console.WriteLine("Students:");
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["StudentID"]}. {reader["FirstName"]} {reader["LastName"]}");
                }
            }
        }
    }

    static void DisplayStudentsInClass()
    {
        Console.Clear();
        Console.WriteLine("Choose a class:");

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

      
            string classQuery = "SELECT * FROM Classes";
            SqlCommand classCommand = new SqlCommand(classQuery, connection);

            using (SqlDataReader classReader = classCommand.ExecuteReader())
            {
                Console.WriteLine("Classes:");
                while (classReader.Read())
                {
                    Console.WriteLine($"{classReader["ClassID"]}. {classReader["ClassName"]}");
                }
            }

         
            Console.Write("Enter the number for the class: ");
            int chosenClassID;

            if (int.TryParse(Console.ReadLine(), out chosenClassID))
            {
              
                string studentQuery = $"SELECT * FROM Students WHERE ClassID = {chosenClassID}";
                SqlCommand studentCommand = new SqlCommand(studentQuery, connection);

                using (SqlDataReader studentReader = studentCommand.ExecuteReader())
                {
                    Console.WriteLine($"Students in Class {chosenClassID}:");
                    while (studentReader.Read())
                    {
                        Console.WriteLine($"{studentReader["StudentID"]}. {studentReader["FirstName"]} {studentReader["LastName"]}");
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid input for class number.");
            }
        }
    }

    static void AddNewStaff()
    {
        Console.Clear();
        Console.WriteLine("Enter details for the new staff member:");

        Console.Write("Name: ");
        string staffName = Console.ReadLine();

        Console.Write("Category ( (1) for Teacher, (2) for Administrator, (3) for Principal ): ");
        string staffCategoryString = Console.ReadLine();

        if (int.TryParse(staffCategoryString, out int staffCategory))
        { 
            if (staffCategory >= 1 && staffCategory <= 3)
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    connection.Open();

                    string query = "INSERT INTO Staff (Name, CategoryID) VALUES (@Name, @CategoryID)";
                    SqlCommand command = new SqlCommand(query, connection);
                    command.Parameters.AddWithValue("@Name", staffName);
                    command.Parameters.AddWithValue("@CategoryID", staffCategory);

                    int rowsAffected = command.ExecuteNonQuery();

                    if (rowsAffected > 0)
                    {
                        Console.WriteLine("New staff member added successfully.");
                    }
                    else
                    {
                        Console.WriteLine("Error adding staff member. Please try again.");
                    }
                }
            }
            else
            {
                Console.WriteLine("Invalid category number. Please enter a number between 1 and 3.");
            }
        }
        else
        {
            Console.WriteLine("Invalid input for category. Please enter a valid number.");
        }
    }
    static void GetStaffMembers()
    {
        Console.Clear();
        Console.WriteLine("Choose an option:");
        Console.WriteLine("1. Get all staff members");
        Console.WriteLine("2. Get staff members by category");

        Console.Write("Enter the number for the option: ");
        string optionChoice = Console.ReadLine();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            switch (optionChoice)
            {
                case "1":
                    DisplayAllStaffMembers(connection);
                    break;
                case "2":
                    DisplayStaffMembersByCategory(connection);
                    break;
                default:
                    Console.WriteLine("Invalid choice for the option.");
                    break;
            }
        }
    }

    
    static void DisplayAllStaffMembers(SqlConnection connection)
    {
        Console.Clear();
        string query = "SELECT * FROM Staff";
        SqlCommand command = new SqlCommand(query, connection);

        using (SqlDataReader reader = command.ExecuteReader())
        {
            Console.WriteLine("All Staff Members:");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["StaffID"]}. {reader["Name"]} - {reader["CategoryId"]}");
            }
        }
    }

    static void DisplayStaffMembersByCategory(SqlConnection connection)
    {
        Console.Write("Enter the category ( (1) For Teacher, (2) For Administrator, (3) For Principal): ");
        int category = int.Parse(Console.ReadLine());

        string query = "SELECT * FROM Staff WHERE CategoryID = @CategoryID";
        SqlCommand command = new SqlCommand(query, connection);
        command.Parameters.AddWithValue("@CategoryID", category);

        using (SqlDataReader reader = command.ExecuteReader())
        {
            Console.WriteLine($"Staff Members in Category {category}:");
            while (reader.Read())
            {
                Console.WriteLine($"{reader["StaffID"]}. {reader["Name"]}");
            }
        }
    }


    static void GetGradesInLastMonth()
    {
        Console.Clear();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT Students.FirstName AS StudentFirstName, Students.LastName AS StudentLastName, Courses.CourseName, Grades.Grade " +
                            "FROM Grades " +
                            "JOIN Students ON Grades.StudentID = Students.StudentID " +
                            "JOIN Courses ON Grades.CourseID = Courses.CourseID " +
                            "WHERE Grades.DateSet >= DATEADD(month, DATEDIFF(month, 0, GETDATE()) - 1, 0) " +
                            "AND Grades.DateSet < DATEADD(month, DATEDIFF(month, 0, GETDATE()), 0)";

            SqlCommand command = new SqlCommand(query, connection);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                Console.WriteLine("Grades set in the last month:");
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["StudentFirstName"]} - {reader["StudentLastName"]} - {reader["CourseName"]} - Grade: {reader["Grade"]}");
                }
            }
        }
    }

    static void GetCourseStatistics()
    {
        Console.Clear();

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = "SELECT Courses.CourseName, " +
                           "AVG(Grades.Grade) AS AverageGrade, " +
                           "MAX(Grades.Grade) AS HighestGrade, " +
                           "MIN(Grades.Grade) AS LowestGrade " +
                           "FROM Courses " +
                           "LEFT JOIN Grades ON Courses.CourseID = Grades.CourseID " +
                           "GROUP BY Courses.CourseID, Courses.CourseName";

            SqlCommand command = new SqlCommand(query, connection);

            using (SqlDataReader reader = command.ExecuteReader())
            {
                Console.WriteLine("Course Statistics:");
                while (reader.Read())
                {
                    Console.WriteLine($"{reader["CourseName"]} - " +
                                      $"Average Grade: {reader["AverageGrade"]}, " +
                                      $"Highest Grade: {reader["HighestGrade"]}, " +
                                      $"Lowest Grade: {reader["LowestGrade"]}");
                }
            }
        }
    }

    static void AddNewStudent()
    {
        Console.Clear();
        Console.WriteLine("Enter details for the new student:");

        Console.Write("First Name: ");
        string firstName = Console.ReadLine();

        Console.Write("Last Name: ");
        string lastName = Console.ReadLine();

        Console.Write("ClassID: ");
        int classID = int.Parse(Console.ReadLine());

        using (SqlConnection connection = new SqlConnection(connectionString))
        {
            connection.Open();

            string query = "INSERT INTO Students (FirstName, LastName, ClassID) VALUES (@FirstName, @LastName, @ClassID)";
            SqlCommand command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@FirstName", firstName);
            command.Parameters.AddWithValue("@LastName", lastName);
            command.Parameters.AddWithValue("@ClassID", classID);

            int rowsAffected = command.ExecuteNonQuery();

            if (rowsAffected > 0)
            {
                Console.WriteLine("New student added successfully.");
            }
            else
            {
                Console.WriteLine("Error adding student. Please try again.");
            }
        }
    }
}