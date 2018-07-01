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
using System.Collections.Generic;
using ICSharpCode.SharpZipLib.Zip.Compression;

namespace IKLogonServer.Network
{
    public class Packet
    {
        private Packet()
        { }

        public Packet(byte[] data)
        {
            Reader = new PacketReader(data);
        }

        public Packet(PacketHeaderType header, byte opCode)
        {
            HeaderType = header;
            Writer = new PacketWriter(header);
            // Keep in mind; depending on the type of packet header passed,
            // this will write either a byte, or a ushort.
            Writer.WritePacketHeader(opCode);
        }

        #region Compress/Decompress

        public byte[] GetCompressedOutPacket(int offset, int length)
        {
            var deflater = new Deflater();
            deflater.SetInput(Data, offset, length);
            deflater.Finish();

            var compBuffer = new byte[1024];
            var ret = new List<byte>();

            while (!deflater.IsFinished)
            {
                try
                {
                    deflater.Deflate(compBuffer);
                    ret.AddRange(compBuffer);
                    Array.Clear(compBuffer, 0, compBuffer.Length);
                }
                catch (Exception ex)
                {
                    return null;
                }
            }
            deflater.Reset();
            return ret.ToArray();
        }

        public byte[] GetCompressedOutPacket()
        {
            return GetCompressedOutPacket(0, Data.Length);
        }

        #endregion

        public ushort OpCode { get; private set; }

        public byte[] Data
        {
            get
            {
                // How do you like THAT!?
                return Writer != null
                           ? (Writer.BaseStream as MemoryStream).ToArray()
                           : (Reader != null ? (Reader.BaseStream as MemoryStream).ToArray() : null);
            }
        }

        public PacketWriter Writer { get; private set; }
        public PacketReader Reader { get; private set; }

        protected PacketHeaderType HeaderType { get; set; }
    }
}
