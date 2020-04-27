using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using StackExchange.Redis;
using Grpc.Core;
using Microsoft.Extensions.Logging;

namespace BackendApi.Services
{
    public class JobService : Job.JobBase
    {
        private readonly static Dictionary<string, string> _jobs = new Dictionary<string, string>();
        private readonly ILogger<JobService> _logger;
        private readonly ConnectionMultiplexer _redisConnection;

        public JobService(ILogger<JobService> logger)
        {
            _logger = logger;

            string redisUrl = Environment.GetEnvironmentVariable("REDIS_HOST") + ":" + Environment.GetEnvironmentVariable("REDIS_PORT");
            Console.WriteLine(redisUrl);
            _redisConnection = ConnectionMultiplexer.Connect(redisUrl);
        }

        public override Task<RegisterResponse> Register(RegisterRequest request, ServerCallContext context)
        {
            string id = Guid.NewGuid().ToString();
            var resp = new RegisterResponse
            {
                Id = id
            };
            _jobs[id] = request.Description;

            SendMessageToRedis(id, request.Description);

            return Task.FromResult(resp);
        }

        private void SendMessageToRedis(string id, string description)
        {
            IDatabase db = _redisConnection.GetDatabase();
            db.StringSet(id, description);
        }
    }
}