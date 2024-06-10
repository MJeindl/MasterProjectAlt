from fileParsingMethods import parseSummaryFileToArray as toOD
from fileParsingMethods import removeBackground, numberFileGenerator
from matplotlib import pyplot as plt
import sys
sys.path.insert(1, r"C:\Users\M\Documents\phdmatlab\sqib-pmma-probe-wavelength\UV_Setup\new_parallel_pol_pump653nm")
from ShowDelayScan import fromRawData
import numpy as np
plt.rcParams.update({'font.size':12})
path = r"C:\Users\M\Documents\phdmatlab\sqib4-amplitudes\point0\650udb-ldb"
files = []
files += numberFileGenerator(6825,6836)

wavs = np.arange(680, 744, 5)

tValues, delay, _ = toOD(files, path, False)
odValues = np.zeros(np.shape(tValues))
for i in range(np.shape(tValues)[0]):
    _, odValues[i], _ = removeBackground(np.array([tValues[i]]))

n_files = np.shape(tValues)[0]
fig, ax = plt.subplots(1,1, figsize = (6,3), dpi=288)

#colors according to colormap
#colors = plt.cm.cool(np.linspace(0,1,n_files))
indices = [0,9,10,11]
if np.ndim(odValues) == 1:
    ax.plot(delay*1e-3, odValues)
else:
    for i in indices:
        ax.plot(delay*1e-3, odValues[i,:], label = str(wavs[i]) + r" nm")
    #plot mean
    if 0 == True:
        ax.plot(delay*1e-3, np.mean(data, axis = 0), 'k')
ax.set_ylabel(r'$\Delta A$ / mOD')
ax.set_xlabel('time delay / ps')
ax.legend()
ax.set_xlim((-30, 70))
plt.tight_layout()
plt.show()

