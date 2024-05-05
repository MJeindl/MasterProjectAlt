using System.Xml;
using Encoding.UTF8;
using System.Text;
using System.IO;

string filepath  = "C:\Users\M\Documents\Books\masterprojectinformation\ArtrayControlC#wip\ArtrayBackground_10.10.2023_exp15ms.txt";
byte[] byteMap  = File.ReadAllBytes(filepath);
byte[24] truncArray = new byte[24];
Array.Copy(byteMap, truncArray, truncArray.Length);

File.WriteAllBytes(truncArray, "Base64ConversionTest_Binary.txt");

Console.WriteLine(BitConverter.ToString(truncArray));
Console.WriteLine(Encoding.UTF8.GetString(truncArray));
string base64string = Convert.ToBase64String(truncArray);
Console.WriteLine(base64string);
File.WriteAllBytes(base64, "Base64ConversionTest_Base64.txt")
//now do the reverse
Console.WriteLine(BitConverter.ToString(Convert.FromBase64String(base64string)));
Console.WriteLine(Encoding.UTF8.GetString(Convert.FromBase64String(base64string)));
