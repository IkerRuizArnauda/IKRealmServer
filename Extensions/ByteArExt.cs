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

using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace IKLogonServer.Extensions
{
    public static class ByteArExt
    {
        public static byte[] Reverse(this byte[] from)
        {
            var res = new byte[from.Length];
            int i = 0;
            for (int t = from.Length - 1; t >= 0; t--)
            {
                res[i++] = from[t];
            }
            return res;
        }

        public static bool Equals(byte[] left, byte[] right)
        {
            if (left.Length != right.Length)
            {
                return false;
            }
            return !left.Where((t, i) => right[i] != t).Any();
        }
    }
}
