﻿using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Polly;
using Polly.Retry;
using RegistrationListenerService.Core.Interfaces;
using RegistrationListenerService.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace RegistrationListenerService.Core.Services {
    public class RepostingService : IRepostingService {
        private const int MaxRetries = 3;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ILogger<RepostingService> _logger;
        private readonly AsyncRetryPolicy<HttpResponseMessage> _retryPolicy;
                
        public RepostingService(IHttpClientFactory httpClientFactory, ILogger<RepostingService> logger) {
            this._httpClientFactory = httpClientFactory;
            this._logger = logger;

            this._retryPolicy = Policy.HandleResult<HttpResponseMessage>(result => !result.IsSuccessStatusCode)
                .Or<HttpRequestException>()
                .WaitAndRetryAsync(
                    retryCount: MaxRetries, 
                    sleepDurationProvider: times => TimeSpan.FromSeconds(times * 1),
                    onRetry: (outcome, timespan, retryAttempt, context) => {
                        _logger.LogWarning("Delaying for {delay}ms, then making retry {retry}.", timespan.TotalMilliseconds, retryAttempt);
                    });
        }

        public async Task<bool> RepostToEndpoint(RegistrationMessageRepost message, string endpointUrl) {

            var json = JsonConvert.SerializeObject(message);
            var postBody = new StringContent(json, Encoding.UTF8, "application/json");

            var client = _httpClientFactory.CreateClient();
            var response = await _retryPolicy.ExecuteAsync(async () => await client.PostAsync(new Uri(endpointUrl), postBody));

            if (response.IsSuccessStatusCode) {
                return true;
            }
            return false;
        }
    }
}
