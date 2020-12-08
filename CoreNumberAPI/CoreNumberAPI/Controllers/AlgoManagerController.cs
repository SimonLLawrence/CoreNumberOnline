using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Model;
using CoreNumberAPI.Processors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace CoreNumberAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AlgoManagerController : ControllerBase
    {
        private readonly ILogger<AlgoManagerController> _logger;
        private readonly IAlgoProcessManager _algoProcessManager;

        public AlgoManagerController(ILogger<AlgoManagerController> logger, IAlgoProcessManager algoProcessManager)
        {
            _logger = logger;
            _algoProcessManager = algoProcessManager;
        }

        [HttpPost]
        [Route("CreateAlgo")]
        public string CreateAlgo(CreateAlgoRequestModel createAlgo)
        {
            var algoId =_algoProcessManager.CreateAlgo(createAlgo.AlgoName, createAlgo.ExchangeName, createAlgo.Key, createAlgo.Secret, createAlgo.Subaccount);
            return algoId;
        }

        [HttpPut]
        [Route("ExecuteAlgo")]
        public ActionResult ExecuteAlgo(string algoId)
        {
            _algoProcessManager.ExecuteAlgo(algoId);
            return Ok();
        }

        [HttpPut]
        [Route("ExecuteAlgoAll")]
        public ActionResult ExecuteAlgo()
        {
            _algoProcessManager.ExecuteAllAlgos();
            return Ok();
        }

        [HttpPut]
        [Route("StartProcessingAlgos")]
        public ActionResult StartProcessingAlgos()
        {
            _algoProcessManager.StartProcessingAlgos();
            return Ok();
        }

        [HttpPut]
        [Route("StopProcessingAlgos")]
        public ActionResult StopProcessingAlgos()
        {
            _algoProcessManager.StopProcessingAlgos();
            return Ok();
        }

        [HttpPut]
        [Route("StarAlgoInstance")]
        public ActionResult StartAlgoInstance(string instanceId)
        {
            _algoProcessManager.StartAlgo(instanceId);
            return Ok();
        }

        [HttpPut]
        [Route("StopAlgoInstance")]
        public ActionResult StopAlgoInstance(string instanceId)
        {
            _algoProcessManager.StopAlgo(instanceId);
            return Ok();
        }

        [HttpPut]
        [Route("ShutDownInstance")]
        public ActionResult ShutdownAlgoInstance(string instanceId)
        {
            _algoProcessManager.ShutdownAlgo(instanceId);
            return Ok();
        }
    }
}
