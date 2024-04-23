using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using APBD_05;

namespace APBD_05.Services
{
    public class AnimalService : IAnimalService
    {
        private readonly string _connectionString;

        public AnimalService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<Animal>> GetAnimalsAsync(string orderBy = "name")
        {
            var animals = new List<Animal>();

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand($"SELECT * FROM ANIMAL ORDER BY {orderBy} ASC", connection))
                {
                    using (var reader = await command.ExecuteReaderAsync())
                    {
                        while (await reader.ReadAsync())
                        {
                            animals.Add(new Animal
                            {
                                IdAnimal = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Description = reader.IsDBNull(2) ? null : reader.GetString(2),
                                Category = reader.GetString(3),
                                Area = reader.GetString(4)
                            });
                        }
                    }
                }
            }

            return animals;
        }

        public async Task<Animal> AddAnimalAsync(Animal animal)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("INSERT INTO ANIMAL (Name, Description, Category, Area) OUTPUT INSERTED.IdAnimal VALUES (@Name, @Description, @Category, @Area)", connection))
                {
                    command.Parameters.AddWithValue("@Name", animal.Name);
                    command.Parameters.AddWithValue("@Description", animal.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Category", animal.Category);
                    command.Parameters.AddWithValue("@Area", animal.Area);

                    animal.IdAnimal = (int)await command.ExecuteScalarAsync();
                }
            }

            return animal;
        }

        public async Task UpdateAnimalAsync(Animal animal)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("UPDATE ANIMAL SET Name = @Name, Description = @Description, Category = @Category, Area = @Area WHERE IdAnimal = @IdAnimal", connection))
                {
                    command.Parameters.AddWithValue("@IdAnimal", animal.IdAnimal);
                    command.Parameters.AddWithValue("@Name", animal.Name);
                    command.Parameters.AddWithValue("@Description", animal.Description ?? (object)DBNull.Value);
                    command.Parameters.AddWithValue("@Category", animal.Category);
                    command.Parameters.AddWithValue("@Area", animal.Area);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }

        public async Task DeleteAnimalAsync(int id)
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();

                using (var command = new SqlCommand("DELETE FROM ANIMAL WHERE IdAnimal = @IdAnimal", connection))
                {
                    command.Parameters.AddWithValue("@IdAnimal", id);

                    await command.ExecuteNonQueryAsync();
                }
            }
        }
    }
}
