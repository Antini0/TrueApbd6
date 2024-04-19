using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using proba5.Models;
using proba5.Models.DTOs;

namespace proba5.Controllers;


[ApiController]
//[Route("api/animals")]
[Route("api/[controller]")]
public class AnimalsController : ControllerBase
{

    private readonly IConfiguration _configuration;
    public AnimalsController(IConfiguration configuration)
    {
        _configuration = configuration;
    }
    
    [HttpGet]
    public IActionResult GetAnimals(string orderBy = "name")
{
    // Otwieramy połączenie
    using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
    {
        connection.Open();

        // Definiujemy commanda
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = $"SELECT * FROM Animal ORDER BY {OrderByColumn(orderBy)} ASC";

        // Wykonanie commanda
        using (var reader = command.ExecuteReader())
        {
            List<Animal> animals = new List<Animal>();

            int idAnimalOrdinal = reader.GetOrdinal("IdAnimal");
            int nameOrdinal = reader.GetOrdinal("Name");
            int descriptionOrdinal = reader.GetOrdinal("Descritpion");
            int categoryOrdinal = reader.GetOrdinal("Category");
            int areaOrdinal = reader.GetOrdinal("Area");

            while (reader.Read())
            {
                animals.Add(new Animal()
                {
                    IdAnimal = reader.GetInt32(idAnimalOrdinal),
                    name = reader.GetString(nameOrdinal),
                    description = reader.GetString(descriptionOrdinal),
                    category = reader.GetString(categoryOrdinal),
                    area = reader.GetString(areaOrdinal)
                });
            }

            return Ok(animals);
        }
    }
}

private string OrderByColumn(string orderBy)
{
    // Domyślne sortowanie po kolumnie name
    string column = "Name";

    // Sprawdzamy wartość parametru orderBy i ustalamy kolumnę sortowania
    switch (orderBy.ToLower())
    {
        case "name":
            column = "Name";
            break;
        case "description":
            column = "Description";
            break;
        case "category":
            column = "Category";
            break;
        case "area":
            column = "Area";
            break;
        default:
            break;
    }

    return column;
}

    [HttpPost]
    public IActionResult AddAnimal(AddAnimal animal)
    {

        /*using ()
        {
            
        }

        try
        {



        }

        finally
        {

            connection.Dispose();

        }*/
        //Otwieramy polaczenie
        SqlConnection connection=new SqlConnection(_configuration.GetConnectionString("Default"));
        connection.Open();
        
        
        
        //Definiujemy commanda
        SqlCommand command = new SqlCommand();
        command.Connection = connection;
        command.CommandText = "INSERT INTO Animal VALUES (@animalName, @animalDescription, @animalCategory, @animalArea)";
        command.Parameters.AddWithValue("@animalName", animal.name);
        command.Parameters.AddWithValue("@animalDescription", animal.description);
        command.Parameters.AddWithValue("@animalCategory", animal.category);
        command.Parameters.AddWithValue("@animalArea", animal.area);
        command.ExecuteNonQuery();
        
        
        return Created("", null);
    }
    [HttpPut("{animalName}")]
    public IActionResult UpdateAnimal(string animalName, Animal animal)
    {
        // Otwieramy połączenie
        using (SqlConnection connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();

            // Definiujemy komendę
            using (SqlCommand command = new SqlCommand())
            {
                command.Connection = connection;
                command.CommandText = "UPDATE Animal SET Name = @animalName, Descritpion = @animalDescription, Category = @animalCategory, Area = @animalArea WHERE Name = @originalName";

                // Dodajemy parametry
                command.Parameters.AddWithValue("@animalName", animal.name);
                command.Parameters.AddWithValue("@animalDescription", animal.description);
                command.Parameters.AddWithValue("@animalCategory", animal.category);
                command.Parameters.AddWithValue("@animalArea", animal.area);
                command.Parameters.AddWithValue("@originalName", animalName);

                // Wykonujemy zapytanie
                int rowsAffected = command.ExecuteNonQuery();

                // Sprawdzamy czy zwierzę zostało zaktualizowane
                if (rowsAffected == 0)
                {
                    return NotFound($"Animal with name '{animalName}' not found.");
                }
            }
        }

        return NoContent();
    }
    [HttpDelete("{idAnimal}")]
    public IActionResult DeleteAnimal(int idAnimal)
    {
        using (var connection = new SqlConnection(_configuration.GetConnectionString("Default")))
        {
            connection.Open();

            var commandText = "DELETE FROM Animal WHERE IdAnimal = @IdAnimal";
            using (var command = new SqlCommand(commandText, connection))
            {
                command.Parameters.AddWithValue("@IdAnimal", idAnimal);

                int rowsAffected = command.ExecuteNonQuery();
                if (rowsAffected == 0)
                {
                    return NotFound($"Animal with ID {idAnimal} not found.");
                }
            }
        }

        return Ok($"Deleted animal with ID={idAnimal}");
    }
}