/* Iker Ruiz Arnauda 2012
 * This program is free software; you can redistribute it and/or modify
 * it under the terms of the GNU General Public License as published by
 * the Free Software Foundation; either version 2 of the License, or
 * (at your option) any later version.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA
 */

using System;
using Logging;
using System.Threading;
using System.Net.Sockets;
using System.Collections.Generic;

namespace IKLogonServer.Enums
{
    public class ClientHandler
    {
        Logger Logger = LogManager.CreateLogger();
        private Dictionary<Socket, AuthClient> Clients = new Dictionary<Socket, AuthClient>();

        //Ctor
        public ClientHandler()
        { }

        public bool HandleNewClient(Socket socket)
        {
            if (Clients.ContainsKey(socket))
                return false;
            else
            {
                AuthClient client = new AuthClient(socket);
                Clients.Add(socket, client);
                //Listen disc event.
                client.OnDisconnected += ClientDisconnected;
                client.OnTimeOut += ClientTimeOut;
                //Listen for this client net messages.        
                Thread thread = new Thread(client.Listen);
                thread.Start();
                return true;
            }
        }

        public AuthClient GetClient(Socket client)
        {
            if (Clients.ContainsKey(client))
                return Clients[client];
            else
                return null;
        }

        private void ClientDisconnected(AuthClient client, EventArgs e)
        {
            Logger.Info("Client disconnected: " + client.RemoteEndPoint.ToString());
            client.OnDisconnected -= ClientDisconnected;
            client.OnTimeOut -= ClientTimeOut;
            Clients.Remove(client.Socket);
            client.Socket.Shutdown(SocketShutdown.Both);
        }
        private void ClientTimeOut(AuthClient client, EventArgs e)
        {
            Logger.Info("Client timeout: " + client.RemoteEndPoint.ToString());

            client.Send(new byte[] { 0 });
            client.OnDisconnected -= ClientDisconnected;
            client.OnTimeOut -= ClientTimeOut;
            Clients.Remove(client.Socket);
            client.Socket.Shutdown(SocketShutdown.Both);
        }

        //Checks for client within our collection
        public bool HasClient(Socket client)
        {
            return Clients.ContainsKey(client);
        }
    }
}
