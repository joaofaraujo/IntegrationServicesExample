using ProcessDomain.Domain;
using ProcessDomain.Interfaces.Repositories;
using ProcessDomain.Interfaces.Services;
using System.Collections.Generic;

namespace ProcessDomain.Services
{
    public class ProcessFileService : IProcessFileService
    {
        private readonly IExecucaoDtsxRepository _execucaoDtsxRepository;
        private readonly PackageIntegrationServices _packageIntegrationServices;

        public ProcessFileService(IExecucaoDtsxRepository execucaoDtsxRepository, PackageIntegrationServices packageIntegrationServices)
        {
            this._execucaoDtsxRepository = execucaoDtsxRepository;
            this._packageIntegrationServices = packageIntegrationServices;
        }

        public void ProcessArquivo()
        {
            //executo o meu package
            var execution_id = _execucaoDtsxRepository.ExecPackageFromCatalog( _packageIntegrationServices.Packagename, _packageIntegrationServices.Foldername, 
                                                            _packageIntegrationServices.Projectname, _packageIntegrationServices.Environmentname);

            List<string> errors;
            bool success = _execucaoDtsxRepository.ExecutionIsError(execution_id, _packageIntegrationServices.Packagename, out errors);

            if (!success)
            {
                //get messages
            }
        }
    }
}
