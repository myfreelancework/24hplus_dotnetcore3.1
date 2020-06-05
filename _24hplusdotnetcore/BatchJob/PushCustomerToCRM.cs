using _24hplusdotnetcore.Services.CRM;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.BatchJob
{
    public class PushCustomerToCRM : HostedService
    {
        private readonly ILogger<AddNewCustomerFromCRM> _logger;
        private readonly CRMServices _crmServices;
        public PushCustomerToCRM(ILogger<AddNewCustomerFromCRM> logger, CRMServices crmServices)
        {
            _logger = logger;
            _crmServices = crmServices;
        }
        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                _crmServices.AddingDataToCRM();
                await Task.Delay(TimeSpan.FromHours(4), cancellationToken);
            }
        }
    }
}
