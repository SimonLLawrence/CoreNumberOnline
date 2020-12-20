using System;
using CoreNumberAPI.Factory;
using CoreNumberAPI.Processors;
using Xunit;

namespace CoreNumberApiTests
{
    public class BotProcessManagerTests
    {
        [Fact]
        public void ProcessManagerStartedInInitialised()
        {
            var testContext = new TestContext();
            var botProcessManager = testContext.GetBotProcessManager();
            Assert.Equal("INITIALIZED", botProcessManager.State);
        }

        [Fact]
        public void CreateBotTestWhenCalledPopulatesData()
        {
            var testContext = new TestContext();
            var botProcessManager = testContext.GetBotProcessManager();
            var botId = botProcessManager.CreateBot("CoreNumberCompound", "BINANCE", "key", "secret", "main");
            var intsanceData = testContext.BotInstanceDataRepository.GetBotInstanceData(botId);
            Assert.NotNull(intsanceData.ExchangeID);
            Assert.NotNull(intsanceData.ProcessorID);
            Assert.NotNull(intsanceData.SecretID);
            Assert.NotEqual(intsanceData.Id,Guid.Empty);
        }

        [Fact]
        public void CreateBotTestWhenCalledPopulatesProcessor()
        {
            var testContext = new TestContext();
            var botProcessManager = testContext.GetBotProcessManager();
            var botId = botProcessManager.CreateBot("CoreNumberCompound", "BINANCE", "key", "secret", "main");
            var intsanceData = testContext.BotInstanceDataRepository.GetBotInstanceData(botId);
            var botProcessor = testContext.BotProcessorFactory.GetBotProcessor(intsanceData.ProcessorID);
            Assert.NotNull(botProcessor);
            Assert.Equal(botProcessor.BotProcessorName, intsanceData.ProcessorID);
        }

        [Fact]
        public void CreateBotTestWhenCalledPopulatesSecret()
        {
            var testContext = new TestContext();
            var botProcessManager = testContext.GetBotProcessManager();
            var botId = botProcessManager.CreateBot("CoreNumberCompound", "BINANCE", "key1a", "secret1a", "main1a");
            var instanceData = testContext.BotInstanceDataRepository.GetBotInstanceData(botId);
            var botSecret = testContext.SecretDataRepository.GetApiSecret(instanceData.SecretID);
            Assert.NotNull(botSecret);
            Assert.Equal("key1a",botSecret.Key);
            Assert.Equal("secret1a", botSecret.Secret);
            Assert.Equal("main1a", botSecret.SubaccountName);
            Assert.NotEqual(Guid.Empty.ToString(), botSecret.SecretId);
        }

        [Fact]
        public void CreateBotTestWhenCalledPopulatesExchange()
        {
            var testContext = new TestContext();
            var botProcessManager = testContext.GetBotProcessManager();
            var botId = botProcessManager.CreateBot("CoreNumberCompound", "BINANCE", "key1a", "secret1a", "main1a");
            var instanceData = testContext.BotInstanceDataRepository.GetBotInstanceData(botId);

            var botExchange = testContext.ExchangeFactory.GetExchange(instanceData.ExchangeID);
            Assert.NotNull(botExchange);
            Assert.Equal("BINANCE", botExchange.ExchangeName);
        }

    }
}
