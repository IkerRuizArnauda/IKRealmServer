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
using System.IO;
using System.Text;
using System.Linq;
using IKLogonServer.Realm;
using IKLogonServer.Network;
using IKLogonServer.Accounts;
using IKLogonServer.Extensions;
using System.Collections.Generic;
using IKLogonServer.Cryptography;

namespace IKLogonServer.Enums
{
    public static class AuthenticationManager
    {
        public static Logger Logger = LogManager.CreateLogger();

        private static readonly BigInteger bi_g = 7;
        private static readonly BigInteger bi_k = 3;
        private static readonly BigInteger bi_N = new BigInteger("894B645E89E1535BBDAD5B8B290650530801B18EBFBF5E8FAB3C82872A3E9BB7", 16);
        private static readonly BigInteger bi_b = BigInteger.genPseudoPrime(160, 5, new Random(0));

        public static PacketStatus Process(AuthClient client, byte[] data)
        {
            int received = data.Length;
            Logger.Trace("Recieved {0} bytes from client {1}", received, client.RemoteEndPoint);

            //Retrieve the packet header.
            var MSGTYPE = (PacketHeader)(int)data[0];
            Logger.Trace("MSGTYPE: " + MSGTYPE);
            client.LogonStep = MSGTYPE;

            PacketStatus STATUS;
            switch (MSGTYPE)
            {
                case PacketHeader.MSG_AUTH_LOGON_RECODE_CHALLENGE:
                case PacketHeader.MSG_AUTH_LOGON_CHALLENGE:
                    STATUS = HandleClientLogonChallenge(client, data);
                    return STATUS;
                case PacketHeader.MSG_AUTH_LOGON_RECODE_PROOF:
                case PacketHeader.MSG_AUTH_LOGON_PROOF:
                    STATUS = HandleClientLogonProof(client, data);
                    return STATUS;
                case PacketHeader.MSG_REALM_LIST:

                    STATUS = GetRealmList(client);
                    return STATUS;
                default:
                    throw new Exception("Unhandled packet!");
            }
        }

        private static PacketStatus GetRealmList(AuthClient client)
        {
            var packet = new RealmList();
            client.Send(sendPacket(packet));
            return PacketStatus.MSG_SUCCESS;
        }

        private static byte[] sendPacket(ServerPacket packet)
        {
            return sendPacket((byte)packet.Opcode, packet.Packet);
        }

        private static byte[] sendPacket(byte opcode, byte[] data)
        {
            BinaryWriter writer = new BinaryWriter(new MemoryStream());
            writer.Write(opcode);
            writer.Write((ushort)data.Length);
            writer.Write(data);

            Logger.Trace("Server -> Client [" + (LoginOpcodes)opcode + "] [0x" + opcode.ToString("X") + "]");

            var buf = ((MemoryStream)writer.BaseStream).ToArray();
            byte[] buffer = new byte[buf.Length];
            Buffer.BlockCopy(buf, 0, buffer, 0, buf.Length);
            return buffer;
        }

        private static PacketStatus HandleClientLogonProof(AuthClient client, byte[] data)
        {
            BigInteger bi_A;
            BigInteger bi_M1;
            using (MemoryStream packetMs = new MemoryStream(data))
            {
                using (BinaryReader br = new BinaryReader(packetMs))
                {
                    var opcode = br.ReadBytes(1); //Read opcode! meh..
                    bi_A = new BigInteger(br.ReadBytes(32).Reverse());
                    bi_M1 = new BigInteger(br.ReadBytes(20).Reverse());
                }
            }

            byte[] u = H(bi_A.getBytes().Reverse().Concat(client.bi_B.getBytes().Reverse()));
            var bi_u = new BigInteger(u.Reverse());

            BigInteger bi_Temp2 = (bi_A * client.bi_v.modPow(bi_u, bi_N)) % bi_N; // v^u
            BigInteger bi_S = bi_Temp2.modPow(bi_b, bi_N); // (Av^u)^b

            byte[] S = bi_S.getBytes().Reverse();
            var S1 = new byte[16];
            var S2 = new byte[16];

            for (int i = 0; i < 16; i++)
            {
                S1[i] = S[i * 2];
                S2[i] = S[i * 2 + 1];
            }

            var SS_Hash = new byte[40];
            byte[] S1_Hash = H(S1);
            byte[] S2_Hash = H(S2);
            for (int i = 0; i < 20; i++)
            {
                SS_Hash[i * 2] = S1_Hash[i];
                SS_Hash[i * 2 + 1] = S2_Hash[i];
            }

            byte[] N_Hash = H(bi_N.getBytes().Reverse());
            byte[] G_Hash = H(bi_g.getBytes().Reverse());
            for (int i = 0; (i < 20); i++)
            {
                N_Hash[i] ^= G_Hash[i];
            }

            Logger.Trace(client.Account.Username);
            byte[] userHash = H(Encoding.UTF8.GetBytes(client.Account.Username));

            IEnumerable<byte> temp = N_Hash
               .Concat(userHash)
               .Concat(client.bi_s.getBytes().Reverse())
               .Concat(bi_A.getBytes().Reverse())
               .Concat(client.bi_B.getBytes().Reverse())
               .Concat(SS_Hash);

            var biM1Temp = new BigInteger(H(temp).Reverse());
            if (biM1Temp != bi_M1)
                return PacketStatus.MSG_NO_ACCOUNT;

            temp = bi_A.getBytes().Reverse()
               .Concat(biM1Temp.getBytes().Reverse());
            temp = temp.Concat(SS_Hash);
            byte[] M2 = H(temp);

            client.Send(GetLogonProof(M2));
            return PacketStatus.MSG_SUCCESS;
        }

        //AuthLogonProof_Server
        //Offset   Type Name  Description
        //0x0	      uint8    command	0x1
        //0x1	      uint8    error
        //0x2	      uint8[   20] m2
        //0x16	   uint32   unk
        private static byte[] GetLogonProof(byte[] m2)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((byte)PacketHeader.MSG_AUTH_LOGON_PROOF);

                bw.Write((byte)0);
                bw.Write(m2);
                bw.Write((UInt32)0);

                return ms.ToArray();
            }
        }

        private static byte[] H(IEnumerable<byte> buffer)
        {
            return Tools.SHA1.ComputeHash(buffer.ToArray());
        }

        //AuthLogonChallenge_Server
        //Offset   Type Name       Description
        //0x1	      uint8          command	0x0
        //0x2	      uint8          unk
        //0x3	      uint8          err
        //0x4	      char[32] B     SRP public server ephemeral
        //0x24	   uint8 g_len    SRP generator length
        //0x25	   uint8 g        SRP generator
        //0x26	   uint8 n_len    SRP modulus length
        //0x27	   char[32]       n  SRP modulus
        //0x47	   char[32]       srp_salt SRP user's salt
        //0x47	   char[16]       crc_salt A salt to be used in AuthLogonProof_Client.crc_hash
        private static PacketStatus HandleClientLogonChallenge(AuthClient client, byte[] data)
        {
            string Username;
            using (MemoryStream ms = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                reader.ReadBytes(33);
                Username = new string(reader.ReadChars(reader.ReadByte()));
            }

            //CAuthLogonChallenge pkg = new CAuthLogonChallenge(data);
            //TODO - LOOK FOR ACCOUNT HERE, use Wesko:test for now.
            client.Account = new Account(); //Create a new account.
            client.Account.Username = Username;
            client.Account.SetPassword("test");
            Logger.Trace("Created account with username {0}.", Username);

            client.bi_s = new BigInteger(client.Account.PasswordSalt.Reverse());
            client.bi_v = new BigInteger(client.Account.PasswordVerifier.Reverse());
            client.bi_B = (client.bi_v * bi_k + bi_g.modPow(bi_b, bi_N)) % bi_N;
            //if (!TempAccounts.Accounts.ContainsKey(Username.ToLower()))
            //    return PacketStatus.MSG_NO_ACCOUNT;
            //else
            //{
            var bi = new BigInteger(client.Account.PasswordVerifier.Reverse());
            if (bi.ToString() == TempAccounts.Accounts[Username.ToLower()])
            {
                Logger.Trace("Recieved correct authentication information for username: {0}", Username);
                client.Send(GetServerLogonChallenge(client));
                return PacketStatus.MSG_SUCCESS;
            }
            else
            {
                Logger.Error("Recieved incorrect authentication information for username: {0}", Username);
                return PacketStatus.MSG_NO_ACCOUNT;
            }

            //}
        }

        private static byte[] GetServerLogonChallenge(AuthClient client)
        {
            using (MemoryStream ms = new MemoryStream())
            using (BinaryWriter bw = new BinaryWriter(ms))
            {
                bw.Write((byte)PacketHeader.MSG_AUTH_LOGON_CHALLENGE);
                bw.Write((byte)0);
                bw.Write((byte)0);
                bw.Write(client.bi_B.getBytes().Reverse());
                bw.Write((byte)1);
                bw.Write(bi_g.getBytes().Reverse());
                bw.Write((byte)32);
                bw.Write(bi_N.getBytes().Reverse());
                bw.Write(client.bi_s.getBytes().Reverse());
                bw.Write(new byte[16]);
                bw.Write((byte)0);

                return ms.ToArray();
            }
        }
    }
}
