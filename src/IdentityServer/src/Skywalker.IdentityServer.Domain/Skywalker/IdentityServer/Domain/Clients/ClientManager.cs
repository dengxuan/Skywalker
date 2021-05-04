using Skywalker.Caching.Abstractions;
using Skywalker.Domain.Repositories;
using Skywalker.Domain.Services;
using Skywalker.IdentityServer.Models;
using System.Threading.Tasks;

namespace Skywalker.IdentityServer.Domain.Clients
{
    public class ClientManager : DomainService
    {
        private readonly IRepository<Client> _clients;

        public ClientManager(IRepository<Client> clients)
        {
            _clients = clients;
        }

        /// <summary>
        /// Finds a client by id
        /// </summary>
        /// <param name="clientId">The client id</param>
        /// <returns>
        /// The client
        /// </returns>
        public async Task<Client?> FindClientByIdAsync(string clientId)
        {
            return await _clients.FindAsync(predicate => predicate.ClientId == clientId);
        }
    }
}
