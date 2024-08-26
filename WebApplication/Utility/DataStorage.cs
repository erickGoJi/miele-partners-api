using Microsoft.Extensions.Configuration;
using PDF_Generator.Models;
using System.Collections.Generic;
using WebApplication.Models;

namespace PDF_Generator.Utility
{
    public class DataStorage
    {
        public IConfiguration _config;
        public readonly MieleContext _context;

        public DataStorage(IConfiguration config, MieleContext context)
        {
            _config = config;
            _context = context;
        }


        public static List<Employee> GetAllEmployess()
        {
            return new List<Employee>
            {
                new Employee { Name="Mike", LastName="Turner", Age=35, Gender="Male"},
                new Employee { Name="Sonja", LastName="Markus", Age=22, Gender="Female"},
                new Employee { Name="Luck", LastName="Martins", Age=40, Gender="Male"},
                new Employee { Name="Sofia", LastName="Packner", Age=30, Gender="Female"},
                new Employee { Name="John", LastName="Doe", Age=45, Gender="Male"}
            };
        }
    }
}
