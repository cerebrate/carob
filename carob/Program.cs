#region header

// carob - Program.cs
// 
// Alistair J. R. Young
// Arkane Systems
// 
// Copyright Arkane Systems 2012-2013.  All rights reserved.
// 
// Created: 2015-02-09 5:09 PM

#endregion

#region using

using System ;
using System.Diagnostics ;
using System.Linq ;
using System.Runtime.InteropServices ;
using System.Security.Principal ;
using System.Threading ;

#endregion

namespace ArkaneSystems.Carob
{
    public static class Program
    {
        [DllImport("kernel32.dll", SetLastError = true)]
        static extern bool AttachConsole (uint dwProcessId);

        [DllImport("kernel32.dll", SetLastError = true, ExactSpelling = true)]
        static extern bool FreeConsole ();

        private static string chocolateyLocation ;

        private static int Main (string[] args)
        {
            // Get and parse the command line.
            var pargs = Environment.CommandLine ;
            if (pargs[0] == '"')
            {
                // Cut one character beyond second doublequote.
                pargs = pargs.Substring (pargs.IndexOf ('"', 1) + 1) ;
            }
            else
            {
                // Cut at first space, if it exists; if not, return empty string.
                var i = pargs.IndexOf (' ') ;
                pargs = i >= 0 ? pargs.Substring (i) : string.Empty ;
            }

            // Then, find chocolatey.
            chocolateyLocation = FindChocolatey () ;

            // Then, check if we have administrative rights.
            var identity = WindowsIdentity.GetCurrent () ;
            var principal = new WindowsPrincipal (identity) ;

            var areAdmin = principal.IsInRole (WindowsBuiltInRole.Administrator) ;

            // Then, check if this is a self-invoke.
            var parent = ParentProcessUtilities.GetParentProcess () ;

            bool isUs = parent.ProcessName == @"carob";

            if (parent.MainModule.FileName != Process.GetCurrentProcess ().MainModule.FileName)
                isUs = false ;

            if (!isUs)
            {
                if (!areAdmin)
                {
                    // User is NOT an administrator.
                    Console.WriteLine ("carob: administrative rights not present. checking further.") ;

                    // Is user in Administrators group? (i.e., has installation rights.)
                    if (principal.Claims.Any (c => c.Value == "S-1-5-32-544"))
                    {
                        Console.WriteLine ("carob: elevation to admin possible. invoking wrapper.") ;

                        // Self-invoke as a wrapper.
                        var psi = new ProcessStartInfo
                                  {
                                    FileName = Process.GetCurrentProcess ().MainModule.FileName,
                                    Arguments = pargs,
                                    WindowStyle = ProcessWindowStyle.Hidden,

                                      // Force UAC invoke.
                                    Verb = "runas"
                                  } ;

                        // Run wrapper and wait for it to exit.
                        var selfprocess = Process.Start (psi) ;

                        Debug.Assert (selfprocess != null, "selfprocess != null") ;
                        selfprocess.WaitForExit () ;

                        return selfprocess.ExitCode ;
                    }
                    Console.WriteLine ("carob: this user cannot elevate to admin. fail.") ;
                    return 1 ;
                }
                // User is already an administrator. Pass straight through to chocolatey.
                Console.WriteLine ("carob: administrative rights present. passthrough.") ;
                var chocprocess = InvokeChocolatey (pargs) ;

                return chocprocess.ExitCode ;
            }
            // We are a self-invoke (as admin).
            if (!areAdmin)
                throw new InvalidOperationException ("carob: not admin on self-invoke. fatal error. epic fail.") ;

            // Switch output to parent's console.
            FreeConsole () ;
            AttachConsole ((uint)parent.Id) ;

            // Invoke chocolatey.
            var process = InvokeChocolatey (pargs) ;

            //// Wait to be dismissed.
            //Console.WriteLine ("\ncarob: waiting for user. review chocolatey output and press any key.") ;

            //// Clear the buffer.
            //while (Console.KeyAvailable)
            //    Console.ReadKey () ;

            //Console.ReadKey (true) ;

            return process.ExitCode ;
        }

        private static Process InvokeChocolatey (string pargs)
        {
            // Without shell-execute, this will launch in the same command-line window.
            var psi = new ProcessStartInfo {FileName = chocolateyLocation, Arguments = pargs, UseShellExecute = false} ;

            // Run chocolatey and wait for it to exit.
            var process = Process.Start (psi) ;

            Debug.Assert (process != null, "process != null") ;
            process.WaitForExit () ;
            return process ;
        }

        public static string FindChocolatey ()
        {
            var retval = Environment.ExpandEnvironmentVariables (@"%ChocolateyInstall%\choco.exe") ;
            if (retval.Length <= 10)
            {
                Console.WriteLine ("carob: chocolatey not found, aborting.") ;
                Environment.Exit (130) ;
            }

            return retval ;
        }
    }
}
