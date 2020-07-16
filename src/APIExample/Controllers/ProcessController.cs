using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProcessDomain.Interfaces.Services;

namespace APIExample.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProcessController : ControllerBase
    {
        private readonly IProcessFileService _processFileService;

        public ProcessController(IProcessFileService processFileService)
        {
            this._processFileService = processFileService;
        }

        public IActionResult Get()
        {
            _processFileService.ProcessArquivo();
            return Ok();
        }
    }
}
