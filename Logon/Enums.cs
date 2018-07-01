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

namespace IKLogonServer.Enums
{
    public enum LoginOpcodes : byte
    {
        AUTH_LOGIN_CHALLENGE = 0x00,
        AUTH_LOGIN_PROOF = 0x01,
        AUTH_RECONNECT_CHALLENGE = 0x02,
        AUTH_RECONNECT_PROOF = 0x03,
        REALM_LIST = 0x10,
        XFER_INITIATE = 0x30,
        XFER_DATA = 0x31,
        XFER_ACCEPT = 0x32,
        XFER_RESUME = 0x33,
        XFER_CANCEL = 0x34,
    }

    public enum PacketHeader
    {
        MSG_AUTH_LOGON_CHALLENGE = 0x00, //Initiate handshake
        MSG_AUTH_LOGON_PROOF = 0x01, //Handshake
        MSG_AUTH_LOGON_RECODE_CHALLENGE = 0x02, //Ignored for now.
        MSG_AUTH_LOGON_RECODE_PROOF = 0x03, //Ignored for now.
        MSG_REALM_LIST = 0x10, //Retrieve available realms
        MSG_XFER_INITIATE = 0x30,
        MSG_XFER_DATA = 0x31,
        MSG_XFER_ACCEPT = 0x32,
        MSG_XFER_RESUME = 0x33,
        MSG_XFER_CANCEL = 0x34
    }

    public enum PacketStatus
    {
        MSG_SUCCESS = 0x00,
        MSG_IPBAN = 0x01,
        MSG_ACCOUNT_CLOSED = 0x03, //"This account has been closed and is no longer in service -- Please check the registered email address of this account for further information.";
        MSG_NO_ACCOUNT = 0x04, //(5)The information you have entered is not valid.  Please check the spelling of the account name and password.  If you need help in retrieving a lost or stolen password and account
        MSG_ACCOUNT_IN_USE = 0x06, //This account is already logged in.  Please check the spelling and try again.
        MSG_PREORDER_TIME_LIMIT = 0x07,
        MSG_SERVER_FULL = 0x08, //Could not log in at this time.  Please try again later.
        MSG_WRONG_BUILD_NUMBER = 0x09, //Unable to validate game version.
        MSG_UPDATE_CLIENT = 0x0a,
        MSG_ACCOUNT_FREEZED = 0x0c //Account freezed due on-going investigation. (TOS)
    }
}
