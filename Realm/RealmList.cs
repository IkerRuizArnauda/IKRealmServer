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

using IKLogonServer.Enums;
using IKLogonServer.Network;
using IKLogonServer.Extensions;
using IKLogonServer.Components.Realm;

namespace IKLogonServer.Realm
{
   class RealmList : ServerPacket
   {
      public RealmList() : base(LoginOpcodes.REALM_LIST)
      {
         Write((uint)0x0000);
         Write((byte)1);
         Write((uint)RealmType.Normal);
         Write((byte)0);
         this.WriteCString("Iker");
         this.WriteCString("127.0.0.1" + ":" + "120");
         Write((float)0); // Pop
         Write((byte)1); // Chars
         Write((byte)1); // time
         Write((byte)1); // time
         Write((ushort)0x0002);
      }
   }
}
