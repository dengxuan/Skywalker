// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Skywalker.Domain.Entities;
using System;

namespace Skywalker.IdentityServer.Domain.PersistedGrants
{
    /// <summary>
    /// A model for a persisted grant
    /// </summary>
    public class PersistedGrant : AggregateRoot<string>
    {
        /// <summary>
        /// Gets the type.
        /// </summary>
        /// <value>
        /// The type.
        /// </value>
        public string Type { get; set; }

        /// <summary>
        /// Gets the subject identifier.
        /// </summary>
        /// <value>
        /// The subject identifier.
        /// </value>
        public string SubjectId { get; set; }

        /// <summary>
        /// Gets the session identifier.
        /// </summary>
        /// <value>
        /// The session identifier.
        /// </value>
        public string SessionId { get; set; }

        /// <summary>
        /// Gets the client identifier.
        /// </summary>
        /// <value>
        /// The client identifier.
        /// </value>
        public string ClientId { get; set; }

        /// <summary>
        /// Gets the description the user assigned to the device being authorized.
        /// </summary>
        /// <value>
        /// The description.
        /// </value>
        public string Description { get; set; }

        /// <summary>
        /// Gets or sets the expiration.
        /// </summary>
        /// <value>
        /// The expiration.
        /// </value>
        public DateTime? Expiration { get; set; }

        /// <summary>
        /// Gets or sets the consumed time.
        /// </summary>
        /// <value>
        /// The consumed time.
        /// </value>
        public DateTime? ConsumedTime { get; set; }

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        public string Data { get; set; }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        private PersistedGrant() { }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        public PersistedGrant(string id, string type, string subjectId, string sessionId, string clientId, string description, DateTime? expiration, DateTime? consumedTime, string data)
        {
            Id = id;
            Type = type;
            SubjectId = subjectId;
            SessionId = sessionId;
            ClientId = clientId;
            Description = description;
            Expiration = expiration;
            ConsumedTime = consumedTime;
            Data = data;
        }
    }
}