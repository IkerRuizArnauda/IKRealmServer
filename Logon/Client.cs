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
using System.Diagnostics;
using IKLogonServer.Logon;
using IKLogonServer.Accounts;

namespace IKLogonServer.Enums
{
    public class AuthClient
    {
        public delegate void DisconnectedEventHandler(AuthClient sender, EventArgs e);
        public event DisconnectedEventHandler OnDisconnected;

        public delegate void TimeOutEventHandler(AuthClient sender, EventArgs e);
        public event TimeOutEventHandler OnTimeOut;

        public EndPoint RemoteEndPoint { get; private set; }
        public Enums.PacketHeader LogonStep = Enums.PacketHeader.MSG_AUTH_LOGON_CHALLENGE;

        public BigInteger bi_B;
        public BigInteger bi_s = BigInteger.genPseudoPrime(256, 5, new Random(0));
        public BigInteger bi_v;

        public const int BufferSize = 1024;
        public byte[] Buffer = new byte[BufferSize];
        private readonly Socket socket;
        public Socket Socket { get { return socket; } }

        public Account Account { get; set; }


        //Client Logger
        Logger Logger = LogManager.CreateLogger();

        public void Listen()
        {
            Logger.Trace("Listening..");
            IAsyncResult result = Socket.BeginReceive(Buffer, 0, Buffer.Length, SocketFlags.None, DataRecieved, Socket);
            bool success = result.AsyncWaitHandle.WaitOne(5000, true);
            if (!success)
                TimeOut();
        }

        private void TimeOut()
        {
            OnTimeOut?.Invoke(this, null);
        }

        private void DataRecieved(IAsyncResult ar)
        {
            try
            {
                int bytesRead = (ar.AsyncState as Socket).EndReceive(ar);
                if (bytesRead > 0)
                {
                    Array.Resize(ref Buffer, bytesRead);
                    PacketStatus status = AuthenticationManager.Process(this, Buffer);
                    Array.Resize(ref Buffer, BufferSize);

                    if (status == PacketStatus.MSG_SUCCESS)
                        this.Listen();
                    else
                    {
                        var errorPacket = ErrorBuilder.BuildAuthError(this, status); //Build error pkg
                        Send(errorPacket); //Send
                        socket.Receive(Buffer); //0, we need this to make sure the client display the proper error.
                        Kill(); //Shutdown socket.
                    }
                }
                else
                    this.Kill();
            }
            catch
            {
                this.Kill();
            }
        }

        /// <param name="name"></param>
        /// <param name="socket"></param>
        public AuthClient(Socket clientSocket)
        {
            this.socket = clientSocket;
            this.socket.DontFragment = true;

            //this.socket.Blocking = false;
            this.socket.NoDelay = true;
            this.RemoteEndPoint = clientSocket.RemoteEndPoint;
        }

        public void Kill()
        {
            OnDisconnected?.Invoke(this, null);
        }

        public void Send(byte[] data)
        {
            try
            {
                Debug.WriteLine(data.Length);
                Socket.BeginSend(data, 0, data.Length, SocketFlags.None, delegate (IAsyncResult result) { }, null);
            }
            catch (SocketException)
            {
                Kill();
            }
            catch (NullReferenceException)
            {
                Kill();
            }
            catch (ObjectDisposedException)
            {
                Kill();
            }
        }
    }
}
