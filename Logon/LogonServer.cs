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
using System.Net;
using System.Net.Sockets;

namespace IKLogonServer.Enums
{
    public class LogonServer
    {
        private Socket Listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        public ClientHandler ClientManager = new ClientHandler();
        private Logger Logger = LogManager.CreateLogger();

        public LogonServer()
        {
            TempAccounts.Accounts.Add("wesko", "33551247795907624939556164795507472559891219400522266034252381968576134918169");
            StartServer();
        }

        private void StartServer()
        {
            IPEndPoint ipEndPoint = new IPEndPoint(IPAddress.Any, 3724);
            Listener.Bind(ipEndPoint);
            if (Listener.IsBound)
            {
                Logger.Info("is running and waiting for connections...");
                Listener.Listen(100); //Queue 100.
                Listener.BeginAccept(AcceptClient, Listener);
            }
        }

        private void AcceptClient(IAsyncResult ar)
        {
            var clientSocket = Listener.EndAccept(ar);

            if (ClientManager.HandleNewClient(clientSocket))
            {
                Listener.BeginAccept(AcceptClient, Listener);
            }
            else
            {
                Logger.Error("Error while handling client, terminating socket: {0}", clientSocket.RemoteEndPoint);
                clientSocket.Shutdown(SocketShutdown.Both);
            }

            Listener.BeginAccept(AcceptClient, Listener);
        }
    }
}
