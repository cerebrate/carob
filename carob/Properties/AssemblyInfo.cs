#region header

// carob - AssemblyInfo.cs
// 
// Alistair J. R. Young
// Arkane Systems
// 
// Copyright Arkane Systems 2015.  All rights reserved.
// 
// Created: 2015-02-09 5:09 PM

#endregion

#region using

using System.Reflection ;
using System.Runtime.InteropServices ;

#endregion

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.

[assembly: AssemblyTitle ("carob")]
[assembly: AssemblyDescription ("An admin-promotion wrapper for Chocolatey.")]

#if DEBUG

[assembly: AssemblyConfiguration ("Debug")]
#else
[assembly: AssemblyConfiguration("Release")]
#endif

[assembly: AssemblyCompany ("Arkane Systems")]
[assembly: AssemblyProduct ("carob")]
[assembly: AssemblyCopyright ("Copyright © Arkane Systems 2015")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.

[assembly: ComVisible (false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM

[assembly: Guid ("acadb46b-a95b-4265-a1ce-657c23f4b7c8")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion("1.0.*")]

[assembly: AssemblyVersion ("1.1.*")]
[assembly: AssemblyFileVersion ("1.1.0.0")]
