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

namespace Logging
{
    /// <summary>
    /// Holds configuration info for log manager.
    /// </summary>
    public sealed class LogConfig
    {
        /// <summary>
        /// Gets or sets the logging root.
        /// </summary>
        public string LoggingRoot
        {
            get { return AppDomain.CurrentDomain.BaseDirectory; }
            set { LoggingRoot = AppDomain.CurrentDomain.BaseDirectory; }
        }

        /// <summary>
        /// Available log target configs.
        /// </summary>
        public LogTargetConfig[] Targets = new[]
        {
            new LogTargetConfig("ConsoleLog"), 
            //new LogTargetConfig("ServerLog"), 
            //new LogTargetConfig("PacketLog")
        };

        /// <summary>
        /// Creates a new log config.
        /// </summary>
        private LogConfig() //: base("Logging") //
        { }

        /// <summary>
        /// Singleton instance.
        /// </summary>
        public static LogConfig Instance { get { return _instance; } }

        /// <summary>
        /// The internal instance pointer.
        /// </summary>
        private static readonly LogConfig _instance = new LogConfig();
    }

    /// <summary>
    /// Holds configuration of a log target.
    /// </summary>
    public class LogTargetConfig
    {
        /// <summary>
        /// Is enabled?
        /// </summary>
        public bool Enabled = true; //{ get; set; }

        /// <summary>
        /// Target type. Valid values are file and console.
        /// </summary>
        public string Target
        {
            get { return "Console"; }
            set { Target = "Console"; }
        }

        /// <summary>
        /// Include timestamps in logs?
        /// </summary>
        public bool IncludeTimeStamps
        {
            get { return true; }
            set { IncludeTimeStamps = true; }
        }

        /// <summary>
        /// Filename if logtarget is a file based one.
        /// </summary>
        public string FileName
        {
            get { return "Log.txt"; }
            set { FileName = "Log.txt"; }
        }

        /// <summary>
        /// Minimum level of messages to emit.
        /// </summary>
        public Logger.Level MinimumLevel
        {
            get { return (Logger.Level)0; }
            set { MinimumLevel = 0; }
        }

        /// <summary>
        /// Maximum level of messages to emit
        /// </summary>
        public Logger.Level MaximumLevel
        {
            get { return (Logger.Level)7; }
            set { MaximumLevel = (Logger.Level)7; }
        }

        /// <summary>
        /// Reset log file on startup?
        /// </summary>
        public bool ResetOnStartup
        {
            get { return false; }
            set { ResetOnStartup = false; }
        }

        public LogTargetConfig(string loggerName) 
        { }
    }
}
