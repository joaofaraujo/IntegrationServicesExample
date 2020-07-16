using System.Collections.Generic;

namespace ProcessDomain.Interfaces.Repositories
{
    public interface IExecucaoDtsxRepository
    {
        long ExecPackageFromCatalog(string package_name, string folder_name, string project_name, string environment_name);
        bool ExecutionIsError(long executionId, string packagName, out List<string> messages);
    }
}
