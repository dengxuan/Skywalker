using Cronos;
using Skywalker.Scheduler.Abstractions;
using System;
using System.Runtime.CompilerServices;

namespace Skywalker.Scheduler.ConTrigger
{
    public class CronTrigger : ITrigger, IEquatable<ITrigger>
    {
        public static readonly ITriggerBuilder Builder = new CronTriggerBuilder();

        private readonly CronExpression _expression;

        private CronTrigger(string expression, DateTime? expiration = null, DateTime? effective = null)
        {
            _expression = CronExpression.Parse(expression, CronFormat.IncludeSeconds);

            Expression = _expression.ToString();
            ExpirationTime = expiration;
            EffectiveTime = effective;
        }

        /// <summary>
        /// 获取触发器的Cron表达式。
        /// </summary>
        public string Expression { get; }

        /// <summary>
        /// 获取或设置触发器的生效时间。
        /// </summary>
        public DateTime? EffectiveTime { get; set; }

        /// <summary>
        /// 获取或设置触发器的截止时间。
        /// </summary>
        public DateTime? ExpirationTime { get; set; }

        public DateTime? GetNextOccurrence(bool inclusive = false)
        {
            var now = Now();

            if (EffectiveTime.HasValue && now < EffectiveTime.Value)
            {
                return null;
            }
            if (ExpirationTime.HasValue && now > ExpirationTime.Value)
            {
                return null;
            }

            return _expression.GetNextOccurrence(now, inclusive);
        }

        public DateTime? GetNextOccurrence(DateTime origin, bool inclusive = false)
        {
            var now = Now(origin);

            if (EffectiveTime.HasValue && now < EffectiveTime.Value)
            {
                return null;
            }
            if (ExpirationTime.HasValue && now > ExpirationTime.Value)
            {
                return null;
            }

            return _expression.GetNextOccurrence(Now(origin), inclusive);
        }


        public bool Equals(ITrigger other)
        {
            return other is CronTrigger cron &&
                cron._expression.Equals(_expression) &&
                EffectiveTime == other.EffectiveTime &&
                ExpirationTime == other.ExpirationTime;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj as CronTrigger);
        }

        public override int GetHashCode()
        {
            var code = _expression.GetHashCode();

            if (EffectiveTime.HasValue)
            {
                code ^= EffectiveTime.Value.GetHashCode();
            }
            if (ExpirationTime.HasValue)
            {
                code ^= ExpirationTime.Value.GetHashCode();
            }
            return code;
        }

        public override string ToString()
        {
            if (EffectiveTime == null && ExpirationTime == null)
            {
                return "Cron: " + _expression.ToString();
            }
            else
            {
                return "Cron: " + _expression.ToString() + " (" +
                    (EffectiveTime.HasValue ? EffectiveTime.ToString() : "?") + " ~ " +
                    (ExpirationTime.HasValue ? ExpirationTime.ToString() : "?") + ")";
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private DateTime Now(DateTime? timestamp = null)
        {
            return new DateTime(timestamp.HasValue ? timestamp.Value.Ticks : DateTime.Now.Ticks, DateTimeKind.Utc);
        }

        private class CronTriggerBuilder : ITriggerBuilder
        {
            public ITrigger Build(string expression, DateTime? expiration = null, DateTime? effective = null)
            {
                if (string.IsNullOrWhiteSpace(expression))
                {
                    throw new ArgumentNullException(nameof(expression));
                }

                return new CronTrigger(expression, expiration, effective);
            }
        }
    }
}
