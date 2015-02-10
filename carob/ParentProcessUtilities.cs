#region header

// carob - ParentProcessUtilities.cs
// 
// Alistair J. R. Young
// Arkane Systems
// 
// Copyright Arkane Systems 2015.  All rights reserved.
// 
// Created: 2015-02-09 6:11 PM

#endregion

#region using

using System ;
using System.ComponentModel ;
using System.Diagnostics ;
using System.Runtime.InteropServices ;

#endregion

namespace ArkaneSystems.Carob
{
    /// <summary>
    ///     A utility class to determine a process parent.
    /// </summary>
    [StructLayout (LayoutKind.Sequential)]
    public struct ParentProcessUtilities
    {
        // These members must match PROCESS_BASIC_INFORMATION
        internal IntPtr Reserved1 ;
        internal IntPtr PebBaseAddress ;
        internal IntPtr Reserved2_0 ;
        internal IntPtr Reserved2_1 ;
        internal IntPtr UniqueProcessId ;
        internal IntPtr InheritedFromUniqueProcessId ;

        [DllImport ("ntdll.dll")]
        private static extern int NtQueryInformationProcess (IntPtr processHandle,
                                                             int processInformationClass,
                                                             ref ParentProcessUtilities processInformation,
                                                             int processInformationLength,
                                                             out int returnLength) ;

        /// <summary>
        ///     Gets the parent process of the current process.
        /// </summary>
        /// <returns>An instance of the Process class.</returns>
        public static Process GetParentProcess ()
        {
            return GetParentProcess (Process.GetCurrentProcess ().Handle) ;
        }

        /// <summary>
        ///     Gets the parent process of a specified process.
        /// </summary>
        /// <param
        ///     name="handle">
        ///     The process handle.
        /// </param>
        /// <returns>An instance of the Process class.</returns>
        public static Process GetParentProcess (IntPtr handle)
        {
            var pbi = new ParentProcessUtilities () ;
            int returnLength ;
            var status = NtQueryInformationProcess (handle, 0, ref pbi, Marshal.SizeOf (pbi), out returnLength) ;
            if (status != 0)
                throw new Win32Exception (status) ;

            try
            {
                return Process.GetProcessById (pbi.InheritedFromUniqueProcessId.ToInt32 ()) ;
            }
            catch (ArgumentException)
            {
                // not found
                return null ;
            }
        }
    }
}
