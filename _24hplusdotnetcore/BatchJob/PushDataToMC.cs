using _24hplusdotnetcore.Services.CRM;
using _24hplusdotnetcore.Services.MC;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.BatchJob
{
    public class PushDataToMC : HostedService
    {
        private readonly ILogger<PushDataToMC> _logger;
        private readonly MCService _mcService;
        public PushDataToMC(ILogger<PushDataToMC> logger, MCService mCService)
        {
            _logger = logger;
            _mcService = mCService;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            await _mcService.PushDataToMCAsync();
            await Task.Delay(TimeSpan.FromMinutes(30), cancellationToken);
        }
    }
}