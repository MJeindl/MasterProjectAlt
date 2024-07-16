
import sys
sys.path.insert(1, r"C:\Users\M\Documents\phdmatlab\sqib-pmma-probe-wavelength\UV_Setup\new_parallel_pol_pump653nm")
from ArtrayAnalysis import ArtFit
import base64
filepath = r"C:\Users\M\Documents\Books\masterprojectinformation\ArtrayControlC#wip\ArtrayCheck653PumpCalib.txt"

fileBin = open(filepath, "rb")
dataBin = fileBin.read()[:24]
fileBin.close()
print(dataBin)

filepath2 = r"C:\Users\M\Documents\Books\masterprojectinformation\ArtrayControlC#wip\Base64ConversionTest_Base64.txt"
fileBase64Decode = open(filepath2, "rt")
dataBase64Decode = fileBase64Decode.read()
print(dataBase64Decode)
#B64toBin = bytearray()
B64toBin = base64.b64decode(dataBase64Decode, validate = True)
#print(B64toBin)
#print(B64toBin == dataBin)
fileBase64Decode.close()
#check for output of xml
xmlPath = r"C:\Users\M\Documents\Books\masterprojectinformation\ArtrayControlC#wip\XML_template.xml"
xmlOutput, a, a, a, a, a = ArtFit.loadXMLimage(xmlPath)[:24]
print(xmlOutput)