using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CoreNumberAPI.Factory;
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
        private readonly IExchangeFactory _exchangeFactory;
        private readonly IBotProcessorFactory _botProcessorFactory;

        public BotManagerController(ILogger<BotManagerController> logger, IBotProcessManager botProcessManager , IExchangeFactory exchangeFactory, IBotProcessorFactory botProcessorFactory)
        {
            _logger = logger;
            _botProcessManager = botProcessManager;
            _exchangeFactory = exchangeFactory;
            _botProcessorFactory = botProcessorFactory;
        }

        [HttpPost]
        [Route("CreateBot")]
        public string CreateBot(CreateBotRequestModel createBot)
        {
            var botId =_botProcessManager.CreateBot(createBot.BotProcessorName, createBot.ExchangeName, createBot.Key, createBot.Secret, createBot.Subaccount);
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

        [HttpGet]
        [Route("SupportedExchanges")]
        public List<string> GetSupportedExchanges()
        {
            return _exchangeFactory.GetSupportedExchanges();
        }

        [HttpGet]
        [Route("SupportedProcessors")]
        public List<string> GetSupportedProcessors()
        {
            return _botProcessorFactory.GetSupportedProcessors();
        }

        [HttpPost]
        [Route("TradingViewAlert")]
        public ActionResult GetSupportedProcessors(string botInstanceId, Dictionary<string,string> payload)
        {
            _botProcessManager.AddTradingViewUpdate(botInstanceId, payload);
            return Ok();
        }

        [HttpPost]
        [Route("SetConfiguration")]
        public ActionResult SetVariables(string botInstanceId, Dictionary<string, string> payload)
        {
            _botProcessManager.SetConfiguration(botInstanceId, payload);
            return Ok();
        }

        [HttpGet]
        [Route("GetConfiguration")]
        public Dictionary<string,string> GetVariables(string botInstanceId)
        {
            return _botProcessManager.GetConfiguration(botInstanceId);
        }
    }
}
