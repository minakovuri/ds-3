using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text;
using StackExchange.Redis;
using Grpc.Core;
using NATS.Client;
using Microsoft.Extensions.Logging;

namespace BackendApi.Services
{
    public class JobService : Job.JobBase
    {
        private readonly static Dictionary<string, string> _jobs = new Dictionary<string, string>();
        private readonly ILogger<JobService> _logger;
        private readonly ConnectionMultiplexer _redis;

        private readonly IConnection _nats;

        private readonly string _natsUrl = "nats://" + Environment.GetEnvironmentVariable("NATS_HOST") + ":" + Environment.GetEnvironmentVariable("NATS_PORT");

        private readonly string _redisUrl = Environment.GetEnvironmentVariable("REDIS_HOST") + ":" + Environment.GetEnvironmentVariable("REDIS_PORT");

        public JobService(ILogger<JobService> logger)
        {
            _logger = logger;
            _redis = ConnectionMultiplexer.Connect(_redisUrl);
            _nats = new ConnectionFactory().CreateConnection(_natsUrl);
        }

        async public override Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            string id = Guid.NewGuid().ToString();
            var resp = new RegisterResponse
            {
                Id = id
            };
            _jobs[id] = request.Description;

            await SendMessageToRedis(id, request.Description);

            SendMessageToNats(id);

            return await Task.FromResult(resp);
        }

        async private Task SendMessageToRedis(string id, string description)
        {
            IDatabase db = _redis.GetDatabase();

            await db.StringSetAsync(id, description);
        }

        private void SendMessageToNats(string id)
        {
            string message = $"JobCreated|{id}";
            byte[] payload = Encoding.Default.GetBytes(message);

           _nats.Publish("events", payload);
        }
    }
}