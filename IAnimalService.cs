namespace APBD_05
{
    public interface IAnimalService
    {
        Task<IEnumerable<Animal>> GetAnimalsAsync(string orderBy = "name");
        Task<Animal> AddAnimalAsync(Animal animal);
        Task UpdateAnimalAsync(Animal animal);
        Task DeleteAnimalAsync(int id);
    }
}