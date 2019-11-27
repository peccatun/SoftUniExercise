namespace PetStore.Services
{
    public interface IBreedService
    {
        void AddBreed(string name);

        bool Exists(int breedId);
    }
}
