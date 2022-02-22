using System;
using System.Threading.Tasks;

namespace Skywalker.AspNetCore.Authentication
{
    public class SkywalkerAuthenticationEvents
    {

        /// <summary>
        /// Invoked before a challenge is sent back to the caller.
        /// </summary>
        public Func<SkywalkerChallengeContext, Task> OnChallenge { get; set; } = context => Task.CompletedTask;

        public virtual Task Challenge(SkywalkerChallengeContext context) => OnChallenge(context);
    }
}
