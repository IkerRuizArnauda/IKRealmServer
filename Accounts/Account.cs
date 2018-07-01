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
using System.Linq;
using System.Text;
using IKLogonServer.Extensions;
using IKLogonServer.Cryptography;
using System.Collections.Generic;

namespace IKLogonServer.Accounts
{
    public class Account
    {
        private readonly BigInteger bi_g = 7;
        private readonly BigInteger bi_N = new BigInteger("894B645E89E1535BBDAD5B8B290650530801B18EBFBF5E8FAB3C82872A3E9BB7", 16);

        public virtual int Id { get; set; }
        public virtual string Username { get; set; }
        public virtual int Expansion { get; set; }
        public virtual byte[] PasswordSalt { get; set; }
        public virtual byte[] PasswordVerifier { get; set; }
        public virtual byte[] SessionKey { get; set; }

        public void SetPassword(string password)
        {
            BigInteger bi_s = BigInteger.genPseudoPrime(256, 5, new Random(0));
            PasswordSalt = bi_s.getBytes().Reverse();
            byte[] pHash = Tools.SHA1.ComputeHash(Encoding.UTF8.GetBytes((Username + ":" + password).ToUpper()));
            byte[] x = H(bi_s.getBytes().Reverse().Concat(pHash));
            var bi_x = new BigInteger(x.Reverse());
            BigInteger bi_v = bi_g.modPow(bi_x, bi_N);
            PasswordVerifier = bi_v.getBytes().Reverse();
        }

        private byte[] H(IEnumerable<byte> bytes)
        {
            return Tools.SHA1.ComputeHash(bytes.ToArray());
        }
    }
}
