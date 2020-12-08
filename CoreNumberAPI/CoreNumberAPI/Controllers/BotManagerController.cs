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
    public class BotManagerController : ControllerBase
    {
        private readonly ILogger<BotManagerController> _logger;
        private readonly IBotProcessManager _botProcessManager;

        public BotManagerController(ILogger<BotManagerController> logger, IBotProcessManager botProcessManager)
        {
            _logger = logger;
            _botProcessManager = botProcessManager;
        }

        [HttpPost]
        [Route("CreateBot")]
        public string CreateBot(CreateBotRequestModel createBot)
        {
            var botId =_botProcessManager.CreateBot(createBot.BotName, createBot.ExchangeName, createBot.Key, createBot.Secret, createBot.Subaccount);
            return botId;
        }

        [HttpPut]
        [Route("ExecuteBot")]
        public ActionResult ExecuteBot(string botId)
        {
            _botProcessManager.ExecuteBot(botId);
            return Ok();
        }

        [HttpPut]
        [Route("ExecuteAllBots")]
        public ActionResult ExecuteAllBots()
        {
            _botProcessManager.ExecuteAllAlgos();
            return Ok();
        }

        [HttpPut]
        [Route("StartProcessingBots")]
        public ActionResult StartProcessingBots()
        {
            _botProcessManager.StartProcessingBots();
            return Ok();
        }

        [HttpPut]
        [Route("StopProcessingBots")]
        public ActionResult StopProcessingBots()
        {
            _botProcessManager.StopProcessingBots();
            return Ok();
        }

        [HttpPut]
        [Route("StartBotInstance")]
        public ActionResult StartBotInstance(string instanceId)
        {
            _botProcessManager.StartBot(instanceId);
            return Ok();
        }

        [HttpPut]
        [Route("StopBotInstance")]
        public ActionResult StopBotInstance(string instanceId)
        {
            _botProcessManager.StopBot(instanceId);
            return Ok();
        }

        [HttpPut]
        [Route("ShutDownInstance")]
        public ActionResult ShutdownBotInstance(string instanceId)
        {
            _botProcessManager.ShutdownBot(instanceId);
            return Ok();
        }
    }
}
