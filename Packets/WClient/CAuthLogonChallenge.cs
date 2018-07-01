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
using System.IO;
using System.Net;

namespace IKLogonServer.Packets.WClient
{
    public class CAuthLogonChallenge
    {
        byte Command;
        byte Error;
        UInt16 Size;
        string GameName;
        byte Version1;
        byte Version2;
        byte Version3;
        UInt16 Build;
        string Platform;
        string Os;
        string Country;
        UInt32 Region;
        IPAddress ClientIp;
        byte Account_len;
        public string Account_name;

        public CAuthLogonChallenge(byte[] data)
        {
            using (MemoryStream ms = new MemoryStream(data))
            using (BinaryReader reader = new BinaryReader(ms))
            {
                Command = reader.ReadByte();
                Error = reader.ReadByte();
                Size = reader.ReadUInt16();
                GameName = new string(reader.ReadChars(4));
                Version1 = reader.ReadByte();
                Version2 = reader.ReadByte();
                Version3 = reader.ReadByte();
                Build = reader.ReadUInt16();
                Platform = new string(reader.ReadChars(4));
                Os = new string(reader.ReadChars(4));
                Country = new string(reader.ReadChars(4));
                Region = reader.ReadUInt32();
                ClientIp = new IPAddress(reader.ReadUInt32());
                Account_len = reader.ReadByte();
                Account_name = new string(reader.ReadChars(Account_len));
            }
        }

        public override string ToString()
        {
            return
               (int)Command
               + "\n" + (int)Error
               + "\n" + Size
               + "\n" + GameName
               + "\n" + (int)Version1
               + "\n" + (int)Version2
               + "\n" + (int)Version3
               + "\n" + Build.ToString()
               + "\n" + Platform
               + "\n" + Os
               + "\n" + Country
               + "\n" + Region
               + "\n" + ClientIp.ToString()
               + "\n" + (int)Account_len
               + "\n" + Account_name;
        }
    }
}
