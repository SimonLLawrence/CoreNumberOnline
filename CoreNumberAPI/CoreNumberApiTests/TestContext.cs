using System;
using System.Collections.Generic;
using CoreNumberAPI.Factory;
using CoreNumberAPI.Model;
using CoreNumberAPI.Processors;
using CoreNumberAPI.Repository;
using CoreNumberAPI.Services;
using Moq;

namespace CoreNumberApiTests
{
    public class TestContext
    {
        public IBotProcessManager BotProcessManager { get; set; }
        public ISecretDataRepository SecretDataRepository { get; set; } = new MemorySecretDataRepository(null);
        public IBotInstanceDataRepository BotInstanceDataRepository { get; set; } = new MemoryBotInstanceDataRepository();
        public IExchangeFactory ExchangeFactory { get; set; } = Mock.Of<IExchangeFactory>();
        public IBotProcessorFactory BotProcessorFactory { get; set; } = Mock.Of<IBotProcessorFactory>();
        public ITradingViewAlertService TradingViewAlertServiceMock { get; set; } = Mock.Of<ITradingViewAlertService>();
        public IInstanceConfigurationService InstanceConfigurationServiceMock { get; set; } = Mock.Of<IInstanceConfigurationService>();

        public TestContext()
        {
        }

        public IBotProcessManager GetBotProcessManager()
        {
            var exchanges = new List<IExchange> { new BinanceService(SecretDataRepository) };
            ExchangeFactory = new ExchangeFactory(exchanges);
            var processors = new List<IBotProcessor> { new CoreNumberProcessor(ExchangeFactory, BotInstanceDataRepository)};
            BotProcessorFactory = new BotProcessorFactory(processors);
            BotProcessManager = new BotProcessManager(ExchangeFactory, BotProcessorFactory, BotInstanceDataRepository, SecretDataRepository, TradingViewAlertServiceMock, InstanceConfigurationServiceMock); ;
            return BotProcessManager;
        }
    }
}
