using System.Reflection;
using System.Runtime.CompilerServices;

//
// General information for assembly are controlled by following set of attribute value. 
// To change information related to assembly, modify those attribute values. 
// 
//
[assembly: AssemblyTitle("")]
[assembly: AssemblyDescription("")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("")]
[assembly: AssemblyProduct("")]
[assembly: AssemblyCopyright("")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]		

//
// Version information of assembly consists of following 4 attribute values: :
//
//      Major version
//      Minor version 
//      Build number
//      Revision
//
// You can set values using '*' as below. 
// You can set "Build" and "Revision number" as set value. :

[assembly: AssemblyVersion("1.0.*")]

//
// To sign assembly, you need to appoint key to be used.  
// For details regarding assembly sign, refer to Microsoft .NET Framework documents.
//
// Control keys for sign with following attributes:  
//
// Memo:
//   (*)Assembly will not be signed unless key is appointed.
//   (*) KeyName represents code service privider (CSP) that is installed in computer.
//        KeyFile is a file that includes key.
//       
//   (*) When both KeyFile and KeyName values are appointed, following process will be done: 
//       
//       (1)When CSP finds KeyName, that key is used.
//       (2) When only KeyFile exists, the key in KeyFile is installed in CSP and be used. 
//           
//   (*) To creat KeyFile, use "sn.exe" (strict name) utility.
//       When appointing KeyFile, it needs to be relative path to the directory of project output.
//       
//       Path is %Project Directory%/obj/<configuration>.
//       For example, when KeyFile is in project directory, appoint [assembly: AssemblyKeyFile("..//..//mykey.snk")]. 
//       
//   (*) Delay sign is an advanced option.
//       Please refer to Microsoft .NET Framework Document for the detail.
//
[assembly: AssemblyDelaySign(false)]
[assembly: AssemblyKeyFile("")]
[assembly: AssemblyKeyName("")]
