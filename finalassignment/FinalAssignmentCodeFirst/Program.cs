using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Collections.Generic;

namespace FinalAssignmentCodeFirst
{
    class Program
    {
        static void Main(string[] args)// entry point of the application
        {
            using (var db = new StudentContext())           //
            {
                db.Database.EnsureCreated();// ensures that the database is created if it does not exist, and does nothing if it already exists
                //
                Console.WriteLine("Enter First Name:");// prompts the user to enter the first name of the student
                string firstName = Console.ReadLine() ?? "";///// to avoid null reference exception if user presses enter without typing anything

                Console.WriteLine("Enter Last Name:");     // prompts the user to enter the last name of the student
                string lastName = Console.ReadLine() ?? "";// to avoid null reference exception if user presses enter without typing anything

                if (!db.Students.Any(s => s.FirstName == firstName && s.LastName == lastName))      // checks if a student with the same first name and last name already exists in the databasem implemented using LINQ's Any method to query the Students DbSet for a matching record
                {
                    var student = new Student// creates a new instance of the Student class with the provided first name and last name
                    {

                        FirstName = firstName,      // sets the FirstName property of the student object to the value entered by the user
                        LastName = lastName// sets the LastName property of the student object to the value entered by the user
                    };
                    db.Students.Add(student);   // adds the new student object to the Students DbSet, marking it for insertion into the database when SaveChanges is called
                    db.SaveChanges();// saves the changes made to the database context, which includes inserting the new student record into the database

                    Console.WriteLine("Student added successfully!");// confirms to the user that the student was added successfully
                }
                else// if a student with the same first name and last name already exists in the database, it informs the user that the student already exists
                {
                    Console.WriteLine("Student already exists in the database.");// informs the user that a student with the same first name and last name already exists in the database
                }
            }
            Console.WriteLine("Press any key to exit...");// prompts the user to press any key to exit the application
            Console.ReadLine();// waits for the user to press a key before closing the console window, allowing them to see the output before the application exits
        }
    }
    public class Student// defines a Student class that represents the structure of the student entity in the database, with properties for Id, FirstName, and LastName
    {
        public int Id { get; set; }// defines an Id property of type int, which will serve as the primary key for the Student entity in the database
        public string FirstName { get; set; }// defines a FirstName property of type string, which will store the first name of the student
        public string LastName { get; set; }// defines a LastName property of type string, which will store the last name of the student
    }//

    public class StudentContext : DbContext// defines a StudentContext class that inherits from DbContext, which is used to interact with the database and manage the Student entities
    {
        public DbSet<Student> Students { get; set; }// defines a DbSet property for the Student entity, which allows for querying and saving instances of Student to the database
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)// overrides the OnConfiguring method to configure the database connection settings for the context
        {
            optionsBuilder.UseSqlServer(@"Server=(LocalDb)\MSSQLLocalDB;Database=StudentsDb; Trusted_Connection=True;");        // configures the context to use a SQL Server database with the specified connection string, which includes the server name, database name, and trusted connection settings
        }
    }
   
}