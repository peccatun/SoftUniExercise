namespace PetStore.Services.Implementations
{
    using Data;
    using System.Linq;

    public class CategoryService : ICategoryService
    {
        private readonly PetStoreDbContext data;

        public CategoryService(PetStoreDbContext data)
        {
            this.data = data;
        }

        public bool Exists(int categoryId)
        {
            return this.data.Categories.Any(c => c.Id == categoryId);
        }
    }
}
