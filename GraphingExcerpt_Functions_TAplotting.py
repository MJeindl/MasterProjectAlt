import numpy as np
from scipy import constants as const

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

def ErrorCorrectionSimple(signal, correctionFactor, peakRadiance, dRelPower):
    dPeakRadiance = dRelPower*peakRadiance
    return abs(signal*correctionFactor/peakRadiance*dPeakRadiance)

def fitPeaks(x_data, y_data, N_gaussians, centers = [], sigma = [], amplitudes = [], positionBounds = [], ampBounds = [], sigBounds = [], function = None):
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
        if len(sigBounds)==0:
            min_bounds.append(0)
            max_bounds.append(np.inf)
        else:
            min_bounds.append(sigBounds[i][0])
            max_bounds.append(sigBounds[i][1])
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


def multiGaussianAdditive(x_data, *parameters):
    r'''parameters are parameters[number of gaussian, center/sig/amplitude]'''
    y_out = np.zeros(np.shape(x_data))
    for gaussInd in range(0,len(parameters),3):
        y_out += parameters[gaussInd+2]*np.exp(-np.power(x_data-parameters[gaussInd+0], 2)/(2*np.power(parameters[gaussInd+1],2)))
    return y_out

def multiLorentzianAdditive(x_data, *parameters):
    r'''parameters are parameters[number of gaussian, center/gamma/amplitude]'''
    y_out = np.zeros(np.shape(x_data))
    for cauchyInd in range(0,len(parameters),3):
        #y_out += 1/(np.pi*parameters[cauchyInd+1]*(1+np.power((x_data-parameters[cauchyInd])/parameters[cauchyInd+1],2)))
        #removing normalisation constant, want it to be 1 in center position
        y_out += parameters[cauchyInd+2]/((1+np.power((x_data-parameters[cauchyInd])/parameters[cauchyInd+1],2)))
    return y_out