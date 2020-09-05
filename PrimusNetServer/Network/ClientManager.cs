using PrimusNetServer.Extensions;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace PrimusNetServer.Network
{
    public class ClientManager
    {
        private static Dictionary<Guid, PrimusClient> _clients = new Dictionary<Guid, PrimusClient>();
        public static ReadOnlyDictionary<Guid, PrimusClient> Clients => _clients.AsReadOnly();

        public static PrimusClient GetClient(Guid guid)
        {
            if(_clients.TryGetValue(guid, out PrimusClient client))
            {
                return client;
            }
            return null;
        }

        public static PrimusClient CreateClient()
        {
            var client = new PrimusClient(Guid.NewGuid());
            _clients.Add(client.Guid, client);

            return client;
        }

        public static bool RemoveClient(Guid guid)
        {
            return _clients.Remove(guid);
        }
    }
}
