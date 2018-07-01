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

using System.IO;
using IKLogonServer.Enums;

namespace IKLogonServer.Network
{
    public class ServerPacket : BinaryWriter
    {
        public int Opcode;

        public ServerPacket(int opcode) : base(new MemoryStream())
        {
            Opcode = opcode;
        }

        public ServerPacket(LoginOpcodes opcode) : this((byte)opcode) { }

        public byte[] Packet
        {
            get { return (BaseStream as MemoryStream).ToArray(); }
        }
    }
}
