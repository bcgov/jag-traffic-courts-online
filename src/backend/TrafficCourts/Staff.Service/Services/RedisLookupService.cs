﻿using TrafficCourts.Staff.Service.Models;
using System.Globalization;
using StackExchange.Redis;
using TrafficCourts.Staff.Service.Utils;

namespace TrafficCourts.Staff.Service.Services;

/// <summary>
/// This is an initial simple implementation of the ILookupService that pulls data from a flat file (csv).
/// </summary>
public class RedisLookupService : ILookupService
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisLookupService> _logger;

    public RedisLookupService(IConnectionMultiplexer redis, ILogger<RedisLookupService> logger)
    {
        _redis = redis ?? throw new ArgumentNullException(nameof(redis));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public IEnumerable<Statute> GetStatutes(string? section)
    {
        IList<Statute> statutes = new List<Statute>();

        try
        {
            IDatabase db = _redis.GetDatabase();

            RedisValue[] redisValues;
            if (String.IsNullOrEmpty(section))
            {
                // Get all Statute Keys
                redisValues = db.SetMembers(new RedisKey("statute"));
            }
            else
            {
                // Get all Statute Keys that match the given section text
                section = section.ToLower().Trim();
                redisValues = db.SetCombine(SetOperation.Intersect, new RedisKey("statute:section:" + section), new RedisKey("statute:section:" + section));
            }

            foreach (RedisValue redisValue in redisValues)
            {
                HashEntry[] fields = db.HashGetAll(new RedisKey("statute:" + redisValue.ToString()));
                Statute statute = RedisUtils.ToStatute(fields);
                statutes.Add(statute);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Could not retrieve Statutes from Redis cache.");
            throw;
        }

        return statutes;
    }
}
