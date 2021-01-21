using Skywalker.Scheduler.Abstractions;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Skywalker.Scheduler
{
    public static class Trigger
    {
        private static readonly ConcurrentDictionary<string, ITrigger> _triggers = new ConcurrentDictionary<string, ITrigger>(StringComparer.OrdinalIgnoreCase);

        public static IDictionary<string, ITriggerBuilder> Builders { get; } = new Dictionary<string, ITriggerBuilder>(StringComparer.OrdinalIgnoreCase);

        public static ITrigger Cron(string expression, DateTime? expiration = null, DateTime? effective = null)
        {
            if (string.IsNullOrWhiteSpace(expression))
            {
                return null;
            }

            return Get("cron", expression, expiration, effective);
        }

        public static ITrigger Get(string scheme, string expression, DateTime? expiration = null, DateTime? effective = null)
        {
            if (string.IsNullOrWhiteSpace(scheme))
                throw new ArgumentNullException(nameof(scheme));

            scheme = scheme.Trim();

            var key = scheme + ":" + expression + "|" + (expiration.HasValue ? expiration.Value.Ticks.ToString() : "?") + "-" + (effective.HasValue ? effective.Value.Ticks.ToString() : "?");

            return _triggers.GetOrAdd(key, _ =>
            {
                if (Builders.TryGetValue(scheme, out var builder))
                {
                    return builder.Build(expression, expiration, effective);
                }

                throw new InvalidProgramException($"The '{scheme}' trigger builder not found.");
            });
        }
    }
}
