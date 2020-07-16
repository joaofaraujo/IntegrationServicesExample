using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using ProcessDomain.Interfaces.Repositories;

namespace ProcessData.Repositories.SSIS
{
    public class ExecucaoDtsxRepository : IExecucaoDtsxRepository
    {
        private readonly ContextoSSISDB _contexto;

        public ExecucaoDtsxRepository(ContextoSSISDB contexto)
        {
            this._contexto = contexto;
        }

        public long ExecPackageFromCatalog(string package_name, string folder_name, string project_name, string environment_name)
        {
            bool use32bitruntime = false;
            long? reference_id = null;

            Int16 object_type = 50;
            string parameter_name = "SYNCHRONIZED";
            Int16 parameter_value = 1;

            if (!string.IsNullOrEmpty(environment_name))
            {
                reference_id = ObterReferenceIdPorEnvironmentName(folder_name, environment_name);
            }
            
            long execution_id = ExecCreateExecution(folder_name, project_name, package_name, reference_id, use32bitruntime);
            ExecSetExecutionParameterValue(execution_id, object_type, parameter_name, parameter_value);

            _contexto.Database.ExecuteSqlRaw("EXEC [SSISDB].[catalog].[start_execution] @execution_id", new SqlParameter("@execution_id", execution_id));

            return execution_id;
        }

        private long ExecCreateExecution(string folder_name, string project_name, string package_name, long? reference_id, bool use32bitruntime)
        {
            var p1 = new SqlParameter("@folder_name", folder_name);
            var p2 = new SqlParameter("@project_name", project_name);
            var p3 = new SqlParameter("@package_name", package_name);
            var p4 = new SqlParameter("@reference_id", (object)reference_id ?? DBNull.Value);
            var p5 = new SqlParameter("@use32bitruntime", use32bitruntime);
            var p6 = new SqlParameter("@execution_id", SqlDbType.BigInt);
            p6.Direction = ParameterDirection.Output;

            
            _contexto.Database.ExecuteSqlRaw("EXEC [SSISDB].[catalog].[create_execution] @folder_name, @project_name, @package_name, @reference_id, " + "@use32bitruntime, @execution_id OUT", p1, p2, p3, p4, p5, p6);
            return (long)p6.Value;
        }

        private void ExecSetExecutionParameterValue(long execution_id, Int16 object_type, string parameter_name, Int16 parameter_value)
        {
            var p1 = new SqlParameter("@execution_id", execution_id);
            var p2 = new SqlParameter("@object_type", object_type);
            var p3 = new SqlParameter("@parameter_name", parameter_name);
            var p4 = new SqlParameter("@parameter_value", parameter_value);

            _contexto.Database.ExecuteSqlRaw("EXEC [SSISDB].[catalog].[set_execution_parameter_value] @execution_id, @object_type, @parameter_name, @parameter_value", p1, p2, p3, p4);
        }

        public bool ExecutionIsError(long executionId, string packagName, out List<string> messages)
        {
            messages = null;
            var listaStatus = SelecionarStatusExecucao(executionId);
            bool comErro = listaStatus.Any(x => x != 7);

            if (comErro)
            {
                messages = SelecionarMensagensExecucao(executionId);
            }

            return comErro;
        }

        private List<string> SelecionarMensagensExecucao(long executionId)
        {
            var p1 = new SqlParameter("@pExecutionId", executionId);

            List<string> messages = _contexto.Model.FromSqlRaw(@"SELECT em.message FROM catalog.event_messages em 
                                        WHERE em.operation_id = @pExecutionId", p1).Select(x => x.message).ToList();

            return messages;

        }

        private List<int> SelecionarStatusExecucao(long executionId)
        {
            var p1 = new SqlParameter("@pExecutionId", executionId);

            List<int> status = _contexto.Model.FromSqlRaw(@"SELECT e.status FROM catalog.executions e 
                      WHERE e.execution_id = @pExecutionId", p1).Select(x => x.status).ToList();

            return status;

        }

        private long? ObterReferenceIdPorEnvironmentName(string folderName, string environmentName)
        {
            var p1 = new SqlParameter("@pEnvironmentName", environmentName);
            var p2 = new SqlParameter("@pFolderName", folderName);

            long? referenceId = _contexto.Model.FromSqlRaw(@"   SELECT er.reference_id FROM catalog.folders f
                                                                        INNER JOIN catalog.projects p ON p.folder_id = f.folder_id
                                                                        INNER JOIN catalog.environment_references er ON er.project_id = p.project_id
                                                                        WHERE er.environment_name = @pEnvironmentName AND f.name = @pFolderName", p1, p2).Select(x => x.reference_id).SingleOrDefault();

            return referenceId;

        }
    }
}
