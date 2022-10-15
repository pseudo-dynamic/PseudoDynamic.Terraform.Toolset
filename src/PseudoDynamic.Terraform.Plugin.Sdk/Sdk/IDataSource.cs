namespace PseudoDynamic.Terraform.Plugin.Sdk
{
    public interface IDataSource<Schema>
    {
        // ISSUE: should be constructor
        //Task Configure();
        Task Read();
        Task Validate();
        Task Plan();
        Task Create();
        Task Update();
        Task Delete();
    }
}
