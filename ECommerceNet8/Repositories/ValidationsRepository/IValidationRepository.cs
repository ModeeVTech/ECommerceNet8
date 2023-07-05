namespace ECommerceNet8.Repositories.ValidationsRepository
{
    public interface IValidationRepository
    {
        public Task<bool> ValidateMaterial(string materialName);
        public Task<bool> ValidateMainCategory(string mainCategoryName);
        public Task<bool> ValidateProductColor(string productColorName);
    }
}
