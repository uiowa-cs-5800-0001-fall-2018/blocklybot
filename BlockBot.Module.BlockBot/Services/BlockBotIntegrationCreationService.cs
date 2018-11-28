using System;
using System.Collections.Generic;
using System.Text;
using BlockBot.Common.ServiceInterfaces;

namespace BlockBot.Module.BlockBot.Services
{
    public class BlockBotIntegrationCreationService : IIntegrationCreationService
    {
        /// <summary>
        /// Name of service, in lower case
        /// </summary>
        /// <returns></returns>
        public static string ServiceName() => "blockbot";
    }
}
