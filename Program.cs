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

using Logging;
using System;
using System.Reflection;
using IKLogonServer.Enums;

namespace IKLogonServer
{
   public class CobolWoW
   {
      /// <summary>
      /// Used for uptime calculations.
      /// </summary>
      public static readonly DateTime StartupTime = DateTime.Now;
      public static LogonServer Server;
      private static readonly Logger Logger = LogManager.CreateLogger();

      public static void Main(string[] args)
      {
         Console.SetWindowSize(100, 30);
         AppDomain.CurrentDomain.UnhandledException += UnhandledExceptionHandler; // Watch for any unhandled exceptions.

         // Initialize Logging facility.
         InitLoggers();

         Logger.Info("LogonServer v{0} warming-up...", Assembly.GetExecutingAssembly().GetName().Version);

         // Initialize Server
         StartServer();

         //Todo: Handle commands.
         while (Console.ReadKey().Key != ConsoleKey.Escape)
         {
            Console.ReadLine();
         }
      }

      /// <summary>
      /// Inits logging facility and loggers.
      /// </summary>
      private static void InitLoggers()
      {
         //Enable logger by default.
         LogManager.Enabled = true;

         foreach (var targetConfig in LogConfig.Instance.Targets)
         {
            if (!targetConfig.Enabled)
               continue;

            LogTarget target = null;

            switch (targetConfig.Target.ToLower())
            {
               case "console":
                  target = new ConsoleTarget(targetConfig.MinimumLevel, targetConfig.MaximumLevel,
                                             targetConfig.IncludeTimeStamps);
                  break;
               case "file":
                  target = new FileTarget(targetConfig.FileName, targetConfig.MinimumLevel,
                                          targetConfig.MaximumLevel, targetConfig.IncludeTimeStamps,
                                          targetConfig.ResetOnStartup);
                  break;
            }

            if (target != null)
               LogManager.AttachLogTarget(target);
         }
      }

      private static void UnhandledExceptionHandler(object sender, UnhandledExceptionEventArgs e)
      {
         var ex = e.ExceptionObject as Exception;

         if (e.IsTerminating)
            Logger.FatalException(ex, "LogonServer terminating because of unhandled exception.");
         else
            Logger.ErrorException(ex, "Caught unhandled exception.");

         Console.ReadLine();
      }

      public static bool StartServer()
      {
         if (Server != null)
            return false;

         Server = new LogonServer();
         return true;
      }
   }
}