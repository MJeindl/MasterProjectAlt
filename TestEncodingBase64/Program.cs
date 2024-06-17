using System.Xml;
using System.Text;
using System.IO;
using System.Buffers.Text;

string filepath = "C:\\Users\\TAMINATOR\\phdmatlab\\sqib-pmma-probe-wavelength\\UV_Setup\\new_parallel_pol_pump653nm\\ArtrayCalib2024.01.16\\Artray2024.01.17\\ArtrayCheck653PumpCalib.txt";
byte[] byteMap = File.ReadAllBytes(filepath);
byte[] truncArray = new byte[24];
Array.Copy(byteMap, truncArray, truncArray.Length);

File.WriteAllBytes("C:\\Users\\TAMINATOR\\Documents\\masterprojectinformation\\ArtrayControlC#wip\\Base64ConversionTest_Binary.txt", truncArray);

Console.WriteLine(BitConverter.ToString(truncArray));
Console.WriteLine(Encoding.UTF8.GetString(truncArray));
string base64string = Convert.ToBase64String(truncArray);
Console.WriteLine(base64string);
File.WriteAllText("C:\\Users\\TAMINATOR\\Documents\\masterprojectinformation\\ArtrayControlC#wip\\Base64ConversionTest_Base64.txt", base64string);
//now do the reverse
Console.WriteLine(BitConverter.ToString(Convert.FromBase64String(base64string)));
Console.WriteLine(Encoding.UTF8.GetString(Convert.FromBase64String(base64string)));
