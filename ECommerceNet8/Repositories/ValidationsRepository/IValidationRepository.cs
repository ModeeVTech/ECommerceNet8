namespace ECommerceNet8.Repositories.ValidationsRepository
{
    public interface IValidationRepository
    {
        public Task<bool> ValidateMaterial(string materialName);
    }
}
