using _24hplusdotnetcore.Services.CRM;
using _24hplusdotnetcore.Services.MA;
using _24hplusdotnetcore.Settings;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace _24hplusdotnetcore.BatchJob
{
    public class PushCustomerCRMToMA : HostedService
    {
        private const string query = "SELECT * FROM Potentials WHERE cf_1254 ='Đủ điều kiện-Chuyển NTC' and cf_1178 LIKE ('%MIRAE ASSET%') and leadsource = 'Telesales 24hPlus -2020' and cf_1206 = 0";
        private readonly CRMServices _cRMServices;
        private readonly MAService _mAService;
        private readonly MAConfig _mAConfig;

        public PushCustomerCRMToMA(CRMServices cRMServices, MAService mAService, IOptions<MAConfig> mAConfig)
        {
            _cRMServices = cRMServices;
            _mAService = mAService;
            _mAConfig = mAConfig.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken cancellationToken)
        {
            while (!cancellationToken.IsCancellationRequested)
            {
                await _cRMServices.GetCustomerFromCrmAsync(query);
                await _mAService.PublishAsync();
                await _cRMServices.AddingDataToCRM();

                await Task.Delay(_mAConfig.MillisecondsDelay, cancellationToken);
            }
        }
    }
}
