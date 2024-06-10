import pandas as pd
import numpy as np
from matplotlib import pyplot as plt
import scipy.constants as const
import plotHelperLatex
plotHelperLatex.setMatplotSettings()

#copied from above
def wavToEnergy(lamb):
    #Energy in eV, lamb in nm
    return const.h*const.c/lamb*1e9/const.elementary_charge

def energyToWav(E):
    #Energy in eV, lamb in nm
    return const.h*const.c/E*1e9/const.elementary_charge


def paramDictatTime(param_dict, time_fs):
    return param_dict['A1']*np.exp(-time_fs/param_dict['tau1']) + param_dict['A2']*np.exp(-time_fs/param_dict['tau2'])
#import data from excel (terrible choice I know, but I want the visualisation and comparability)

def ErrorCorrectionConvoluted(signal, correctionFactor, peakRadiance, dRelUmPerPx, dRelPower):
    '''correctionfactor is already divided with peak radiance\\
    dRelPower = dPower/Power'''
    dPeakRadiance = dRelPower*peakRadiance + 2*peakRadiance*dRelUmPerPx
    return abs(signal*correctionFactor/peakRadiance*dPeakRadiance)

def fitPeaks(x_data, y_data, N_gaussians, centers = [], sigma = [], amplitudes = [], positionBounds = [], ampBounds = [], function = None):
    from scipy.optimize import curve_fit
    startParameters = np.ones(3*N_gaussians)
    min_bounds = []
    max_bounds = []
    if len(centers) != N_gaussians:
        centers = np.linspace(min(x_data), max(x_data),N_gaussians)
    
    if len(amplitudes) != N_gaussians:
        amplitudes = max(np.abs(y_data))/N_gaussians*np.ones(N_gaussians)
    if len(sigma) != N_gaussians:
        sigma = np.ones(N_gaussians)*0.01

    for i in range(N_gaussians):
        startParameters[i*3+1] = sigma[i]
        startParameters[i*3] = centers[i]
        startParameters[i*3+2] = amplitudes[i]

    for i in range(N_gaussians):
        if len(positionBounds) == 0:
            min_bounds.append(min(x_data))
            max_bounds.append(max(x_data))
        else:
            min_bounds.append(positionBounds[i][0])
            max_bounds.append(positionBounds[i][1])
        min_bounds.append(0)
        max_bounds.append(np.inf)
        if len(ampBounds) == 0:
            min_bounds.append(-np.inf)
            max_bounds.append(np.inf)
        else:
            min_bounds.append(ampBounds[0])
            max_bounds.append(ampBounds[1])
    par_bounds = [tuple(min_bounds), tuple(max_bounds)]


    popt, pcov = curve_fit(function, x_data, y_data, 
            p0 = startParameters, 
            bounds = par_bounds,
            method="trf")
    return popt, pcov

def multiLorentzianAdditive(x_data, *parameters):
    r'''parameters are parameters[number of gaussian, center/gamma/amplitude]'''
    y_out = np.zeros(np.shape(x_data))
    for cauchyInd in range(0,len(parameters),3):
        #y_out += 1/(np.pi*parameters[cauchyInd+1]*(1+np.power((x_data-parameters[cauchyInd])/parameters[cauchyInd+1],2)))
        #removing normalisation constant, want it to be 1 in center position
        y_out += parameters[cauchyInd+2]/((1+np.power((x_data-parameters[cauchyInd])/parameters[cauchyInd+1],2)))
    return y_out

excel_path = r"C:\Users\M\Documents\phdmatlab\sqib-pmma-probe-wavelength\UV_Setup\new_parallel_pol_pump653nm\CorrectedPump653ProbeWavelengthScan_UV-extended.xlsx"
SHG_wav_timescan = pd.read_excel(excel_path, sheet_name = "2024WavelengthScanParameters", skiprows=[0,1])
SHG_main = pd.read_excel(excel_path, sheet_name='2024WavelengthScan', skiprows=[0,1])
#print(SHG_main)
#print(SHG_main.keys())


wavelengths = SHG_main['Probe wavelength / nm']
#simple correction without map
dOD_SHG_main = SHG_main['cor_dOD / (mOD*m^2/W)']
#correction with map
dOD_SHG_mapCorr = SHG_main['hypothetical correction']
dOD_map_reference = abs(SHG_main['mapReference'])
dPumpPower = SHG_main['dRelPumpPower']
peakRad = SHG_main['Pump power density / (W/m^2)']


scan_wavs = SHG_wav_timescan['Probe wavelength / nm']
scan_corrFactors = SHG_wav_timescan['Correction Factor / (W/m^2)^-1']
scan_params = {
    'A1' : SHG_wav_timescan['A1'],
    'A2' : SHG_wav_timescan['A2'],
    'tau1' : SHG_wav_timescan['tau1'],
    'tau2' : SHG_wav_timescan['tau2'],
}


#high res zheng

zheng_high_res_path = r"C:\Users\M\Documents\Books\masterprojectinformation\images\Zheng2020_higherPrecisionRip.csv"
Zheng = pd.read_csv(zheng_high_res_path, sep=",")

ZhengDict = {}
ZhengDict['10ps'] = np.array([Zheng["10 ps"][1:], Zheng["Y10 ps"][1:]], dtype=float)
ZhengDict['200fs'] = np.array([Zheng["0.2 ps"][1:], Zheng["Y0.2 ps"][1:]], dtype=float)
ZhengDict['2500fs'] = np.array([Zheng["2.5 ps"][1:], Zheng["Y2.5 ps"][1:]], dtype=float)
ZhengDict['100ps'] = np.array([Zheng["100 ps"][1:], Zheng["Y100 ps"][1:]], dtype=float)
#print(np.array(ZhengDict["2500fs"][0,:],dtype=int))
#print(np.array(ZhengDict["2500fs"][0,:],dtype=int) == 680)
ZhengModifier = abs(ZhengDict["2500fs"][1,np.array(ZhengDict["2500fs"][0,:],dtype=int) == 679])

fig, axs = plt.subplots(1,1, figsize = plotHelperLatex.figSizer(1,2), dpi=144)


times = [2e2, 2.5e3, 1e4, 1e5]
colors = ['red', 'green', 'orange', 'royalblue']

indexMeasureModifier = np.array(scan_wavs, int) == 680
modifier_scan_params = {
    'A1' : SHG_wav_timescan['A1'][indexMeasureModifier],
    'A2' : SHG_wav_timescan['A2'][indexMeasureModifier],
    'tau1' : SHG_wav_timescan['tau1'][indexMeasureModifier],
    'tau2' : SHG_wav_timescan['tau2'][indexMeasureModifier],
}

#get rid of faulty measurement
dOD_map_reference[8] = np.nan

measurementModifier = abs(scan_corrFactors*dOD_map_reference[indexMeasureModifier]*paramDictatTime(modifier_scan_params, times[0]))[0]
#print((dOD_map_reference[indexMeasureModifier]*paramDictatTime(scan_params, time))[0])

sizeM = 2
for ind, key in enumerate(ZhengDict):
    axs.plot(ZhengDict[key][0,:], ZhengDict[key][1,:]/ZhengModifier, linestyle="None", marker = ".", markersize= sizeM, color=colors[ind])



for ind, time in enumerate(times):
    axs.plot(scan_wavs, scan_corrFactors*dOD_map_reference*paramDictatTime(scan_params, time)/measurementModifier, '1', color = colors[ind], label="%.1f ps" %(time*1e-3))
 
#plotting 310 and 330 by hand
axs.plot([310, 330], [0,0], '1', color = colors[-1], label="%.1f ps" %(times[-1]*1e-3))


#############plot the fitting of with cauchy functions##############################
#y_dataFit = scan_corrFactors*dOD_map_reference*paramDictatTime(scan_params, times[0])/measurementModifier
y_dataFit = scan_corrFactors*dOD_map_reference*paramDictatTime(scan_params, times[0])/measurementModifier
y_dataFit = y_dataFit[1:-2]
wav_dataFit = scan_wavs[1:-2]
bool_mask = np.invert(np.isnan(y_dataFit))
y_dataFit = np.array(y_dataFit[bool_mask])
wav_dataFit = wav_dataFit[bool_mask]

#do nan-check
#nanBool = np.isnan(y_dataFit)

signalFit, signalCov = fitPeaks(wavToEnergy(wav_dataFit), y_dataFit, 3, [2.82, 2.48, 3.55], [0.1,0.05, 0.03], [0.4, 0.4, 0.1], positionBounds=[[2.76, 2.83], [2.3, 2.7], [3.00,3.60]], function=multiLorentzianAdditive)
timesFit = [1e2,2e3,8e3,1e4,3e4]
fig2, axc = plt.subplots(len(timesFit), dpi=144, sharex=True)
wav_range = np.linspace(350,550,500)
#timeComparisonFit = np.zeros((len(timesFit), 9))
timeComparisonFit = {}
timeComparisonFit['legend'] = ['center', 'sig', 'amplitude','center', 'sig', 'amplitude','center', 'sig', 'amplitude']
timeComparisonSTD = np.zeros((len(timesFit), 9))
wav_dataFit = scan_wavs[1:-2]
for timeInd, subTime in enumerate(timesFit):
    y_dataFit = scan_corrFactors*dOD_map_reference*paramDictatTime(scan_params, subTime)/measurementModifier
    y_dataFit = y_dataFit[1:-2]
    bool_mask = np.invert(np.isnan(y_dataFit))
    y_dataFit = np.array(y_dataFit[bool_mask])
    wav_dataFit = wav_dataFit[bool_mask]

    tempFit, tempCov = fitPeaks(wavToEnergy(wav_dataFit), y_dataFit, 3, [2.82, 2.48, 3.15], [0.1,0.04, 0.02], [0.7, 0.7, 0.2], positionBounds=[[2.76, 2.83], [2.3, 2.7], [2.95,3.60]], function=multiLorentzianAdditive, ampBounds=[0,np.inf])
    timeComparisonFit[str(subTime)] = tempFit
    timeComparisonSTD[timeInd,:] = np.sqrt(np.diag(tempCov))
    #plot sum

    axc[timeInd].plot(wav_range, multiLorentzianAdditive(wavToEnergy(wav_range), *tempFit), label=str(int(subTime/1e3))+" ps")
    for i in range(0,len(tempFit),3):
        axc[timeInd].plot(wav_range, multiLorentzianAdditive(wavToEnergy(wav_range), *tempFit[i:i+3]), label="fit "+str(int(i/3)), alpha=0.5,linestyle="-")
    axc[timeInd].plot(wav_dataFit, y_dataFit, linestyle="None", marker=".", markersize=2, color="r")
    axc[timeInd].set_ylim(0,1)
    axc[timeInd].set_xlim(350,550)
    axc[timeInd].set_ylabel("%.1f ps" %(subTime/1e3))
axc[-1].set_xlabel(r"$\lambda_{probe}$ / nm")

secax2 = axc[0].secondary_xaxis('top', functions=(wavToEnergy, energyToWav))
secax2.set_xticks([3.5, 3, 2.82, 2.48])
secax2.set_xlabel(r"$\mathrm{E_{probe} / eV}$")

#output the fits
import pandas as pd
timeComparisonFit = pd.DataFrame(timeComparisonFit)

print(timeComparisonFit.T.to_string())
#pprint.pprint(timeComparisonSTD)

wav_range = np.linspace(350,550,500)
axs.plot(wav_range, multiLorentzianAdditive(wavToEnergy(wav_range), *signalFit), label=r"$\sum$ fits", color="m", alpha=0.5,linestyle="-")
for i in range(0,len(signalFit),3):
    axs.plot(wav_range, multiLorentzianAdditive(wavToEnergy(wav_range), *signalFit[i:i+3]), label="fit "+str(int(i/3)), alpha=0.5,linestyle="-")



secax = axs.secondary_xaxis('top', functions=(wavToEnergy, energyToWav))
secax.set_xticks(np.round(wavToEnergy(np.array([440, 500, 653])), 2))
secax.set_xlabel(r"$\mathrm{E_{probe} / eV}$")




axs.set_xlabel('probe wavelength / nm')
axs.set_ylabel(r'relative $\Delta A$ / a.u.')
axs.grid(True)




#custom legend
from matplotlib.lines import Line2D
legend_items = [Line2D([0], [0], color = "black", label="Zheng et al.", marker=".", linestyle="None"),
                Line2D([0], [0], color = "black", label="SHG setup", marker="1",linestyle="None")]
for ind in range(len(times)):
    legend_items.append(Line2D([0], [0], color = colors[ind], label="%.1f ps" %(times[ind]*1e-3), marker="s", linestyle="None"))

axs.legend(handles=legend_items)

axs.set_ylim((0,1.5))
axs.set_xlim((300, 550))


#plt.grid(True)
#plt.show()
plt.tight_layout()

#attempt the alternative moving mean at a width of 10 nm aka +- 5 nm
gaussian = lambda wavs, centerwav: 1/np.sqrt(2*np.pi)/5*np.exp(-(centerwav-wavs)**2/(2*5**2))
ZhengOurWavelengths = np.zeros((len(ZhengDict.keys()), len(wavelengths)))
for wav_ind, wav in enumerate(wavelengths):
    for key_ind, key in enumerate(ZhengDict.keys()):
        #print(key)
        wav_bool_array = (wav- 30< ZhengDict[key][0,:]) & (wav +30 > ZhengDict[key][0,:])
        #bool_array = (delays[indWav,:] > plotDelay[indDelay]) & (delays[indWav,:] < plotDelay[indDelay] + 1e3/(n_subticks))
        #ZhengOurWavelengths[key_ind, wav_ind] = np.sum(ZhengDict[key][1,wav_bool_array])
        ZhengOurWavelengths[key_ind, wav_ind] = np.sum(gaussian(ZhengDict[key][0,wav_bool_array], wav)*ZhengDict[key][1,wav_bool_array])
        #print(gaussian(ZhengDict[key][0,:], wav)*ZhengDict[key][1,:])
plt.show()