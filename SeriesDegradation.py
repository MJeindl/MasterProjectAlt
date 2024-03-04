import pandas as pd
from matplotlib import pyplot as plt
from scipy import constants as const
from scipy.io import loadmat
import os
import numpy as np
from datetime import timedelta
import json
from scipy.optimize import curve_fit

import sys
import plotHelperLatex
plotHelperLatex.setMatplotSettings()
sys.path.insert(1, r"C:\Users\M\Documents\phdmatlab\sqib-pmma-probe-wavelength\UV_Setup\new_parallel_pol_pump653nm")
from ShowDelayScan import main

from argparse import ArgumentParser



import plotHelperLatex
plotHelperLatex.setMatplotSettings()

#so all of these functions should take the filename without leading \ if you use a dirPath
#if they don't I am sorry and it slipped by me in one of the many revisions
#need to fix it so it gives the full filepath instead of something without directory
def parseTime(time):
    '''takes time in hh:mm:ss and returns a value in s'''
    if type(time) == str():
        times = np.zeros(len(time), dtype = int)
        hh, mm, ss = float(str(time[0]).split(":"))
    else:
        times = np.zeros(np.shape(time)[0])
        hh = time[:,0]
        mm = time[:,1]
        ss = time[:,2]

    times[0] = int(hh[0])*3600+int(mm[0])*60+int(ss[0])
    for i in range(1,np.shape(time)[0],1):
        #hh, mm, ss = str(time[i+1]).split(":")
        times[i] = int(hh[i])*3600+int(mm[i])*60+int(ss[i]) - times[0]

    times[0] = 0
    return times


def parseFilenames(filenameArray, directoryPath):
    if directoryPath != r"":
        pathInsert = r"\\"[0]
    else:
        pathInsert = r""
    filenames = [] 

    if type(filenameArray) != type(str()):
        
        #parsing for summary files
        for file in filenameArray:
            tempfile = loadmat(directoryPath + pathInsert+ file)
            if 'delay' not in tempfile.keys():
                #only works in same directory, multiple summary files
                file = tempfile['filenames'][0]
                for subFile in file:
                    #filenames.append(directoryPath + r"\\"[0] + subFile[0] + ".mat")
                    filenames.append(pathInsert + subFile[0] + ".mat")
            else:
                #TA_fourier style file
                filenames.append(pathInsert + file + ".mat")
    else:
        #if single saturation file
        summaryfile = loadmat(directoryPath + pathInsert + filenameArray)
        directoryPath = os.path.dirname(os.path.abspath(filenameArray))
        for fname in summaryfile['filenames'][0,:]:
            if isinstance(fname, np.ndarray):
                fname = fname[0]
            filenames.append(r"\\"[0] + fname)

    for i in range(len(filenames)):
        filenames[i] = directoryPath + filenames[i]
    return filenames

def removeBackground(T, numEntries: int = 20) -> float:
    '''returns T, A and Background levels\\numEntries is how many of the points are to be taken as background'''
    backgroundLevel = np.mean(T[:numEntries])
    T += 1 - backgroundLevel
    A = -1000 * np.log10(T)
    return T, A, backgroundLevel
        
def parseSummaryFileToArray(filenameArray, directoryPath=r""):
    '''takes or filearray and returns T, delay'''

    filenames = parseFilenames(filenameArray, directoryPath)

    for ind, file in enumerate(filenames):
        #if os.path.isfile(directoryPath + r'\\' + file[0]) == False:
        #    continue

        if isinstance(file, np.ndarray):
            file = file[0]
        tempFile = loadmat(file)
        delay = tempFile['delay'][0]
        if ind == 0:
            datArray = np.zeros((len(filenames), len(delay)))
        datArray[ind, :] = removeBackground(tempFile['d_vec'], 10)[0]


    return datArray, delay  


        
def getTimes(filenameArray, dirPath=r""):
    '''moved out of degradationCompensation for general use'''
    filenames = parseFilenames(filenameArray, dirPath)
    #test if file has keys for single TA measurement
    subTimes = []
    for ind, filename in enumerate(filenames):
        tempLoad = loadmat(filename)
        if 'delay' in tempLoad.keys() and 'd_vec' in tempLoad.keys():
            #this part can easily be broken by a matlab update
            #load time
            timeString = str(tempLoad['__header__']).split("Created on: ")[1]
            timeString = timeString.split(' ')[3]

            tSplit = np.array(timeString.split(':')[:3], dtype=float)
            subTimes.append([tSplit[0], tSplit[1], tSplit[2]])

        elif 'dates' in tempLoad.keys():
            subDates = tempLoad['dates'][0]

            for ind, timeArray in enumerate(subDates):
                timeArray = np.array(timeArray[0][3:6], dtype=float)
                subTimes.append([timeArray[0], timeArray[1], timeArray[2]])
                #subTimes[ind,0] = timeArray[0]#hh
                #subTimes[ind,1] = timeArray[1]##mm
                #subTimes[ind,2] = timeArray[2]##ss with milisecond
    return np.array(subTimes, dtype = float)
    

def degradationCompensation(degConstant, times, decaysteps, powerDensities=1):
    '''returns correction factor for each measurement\\
        not sure what to do with powerDensities, got to try a few things and see if they work but also read up on it
        '''

    meanTime = 0
    for i in range(len(times)-1):
        meanTime = times[i+1]-times[i]
        meanTime = meanTime/(len(times)-1)
        
    Correction = np.zeros((len(times), decaysteps))

    if type(powerDensities) == type(int()):
        powerDensities = np.ones(np.shape(times)[0])
    #should also correct for within a measurement, though that part is more estimate (should be fine for long decay times)
    Correction = np.zeros((len(times), decaysteps))
    for i in range(len(times)):
        Correction[i] = np.exp((times[i]+meanTime*np.linspace(0,1, decaysteps))*powerDensities[i]/degConstant)

    return Correction



def plotTrend(filePaths, dirPath=r"", figsize=(8,4)):
    #need to switch getTimes to the same system as parseSummary to allow any type of input
    times = getTimes(filePaths, dirPath)
    dArray, delay = parseSummaryFileToArray(filePaths, dirPath)
    dArray, OD, _ = removeBackground(dArray, 10)
    #get delta t of times
    timesFromZero = parseTime(times)

    delays, measurementTimes = np.meshgrid(delay, timesFromZero)
    fig, ax = plt.subplots(1,1, figsize=(8,4), dpi = 200)
    map = ax.pcolor(delays, measurementTimes, OD, cmap="plasma")
    ax.set_xlabel('delay time / fs')
    ax.set_ylabel('total time ellapsed at start / s')
    fig.colorbar(map, ax=ax)
    plt.tight_layout()
    plt.show()

#degradationCompensation(1, [r"C:\Users\M\Documents\phdmatlab\sqib-pmma-probe-wavelength\UV_Setup\new_parallel_pol_pump653nm\Pump653Probe493_Degradation\saturation_2024-01-19_16-14.mat"], 1)
if __name__ == "__main__":
    """
    parser = ArgumentParser(description="Shows response trend over mulitple adjacent measurements")
    parser.add_argument("startFileNumber")
    parser.add_argument("endFileNumber")
    parser.add_argument("pathDir")
    
    args = parser.parse_args()

    start = int(args.startFileNumber)
    stop = int(args.endFileNumber)
    plotTrend(args.pathDir, start, stop)"""
    

def fitDegradation(inputArray, times, powerDensity=1, sliding_window_len = 5):
    '''only works for single series without interruptions'''
    #find the mean time to adjust for the final part of the measurement
    for i in range(len(times)-1):
        dtime = times[i+1]-times[i]
    dtime = dtime/(len(times)-1)
    times = np.array(times)

    working_array = np.zeros((len(times), np.shape(inputArray)[1]-sliding_window_len+1))
    for i in range(len(times)):
        working_array[i,:] = np.convolve(inputArray[i,:], np.ones(sliding_window_len)/sliding_window_len, mode = 'valid')


    def costFunction(times, tau):
        time_fit = lambda time: -time/tau
        residual = 0

        for delayInd in range(np.shape(working_array)[1]):
            residual += abs(np.sum(np.log(abs(working_array[:,delayInd]/working_array[0,delayInd]))-time_fit(times)))**2
        return residual

    popt, pcov = curve_fit(costFunction, times, np.zeros(np.shape(working_array[0,:]),dtype=float))

    return popt, pcov[0,0]




def ignore():
    #this is the constant pump power and probe power variation
    filenames=[]
    for x in range(6645,6654,1):
        filenames.append("TA_fourier_%4d" %(x))
    dirPath = r"C:\Users\M\Documents\phdmatlab\sqib-pmma-probe-wavelength\UV_Setup\new_parallel_pol_pump653nm\STD-680\2023.12.22_PumpPowerVar"


    dirPath = r"C:\Users\M\Documents\phdmatlab\sqib-pmma-probe-wavelength\UV_Setup\new_parallel_pol_pump653nm\Pump653Probe493_Degradation"
    filenames = [r"\saturation_2024-01-19_16-14"]
        
    dArray, delay = parseSummaryFileToArray(filenames, dirPath)

    for ind in range(len(filenames)):
        filenames[ind] = dirPath +r"\\"[0] + filenames[ind]
    dtime = getTimes(filenames, dirPath)
    dtime = parseTime(dtime)
    #dOD = powerVar['dOD / mOD']
    print(np.shape(dArray))
    dArray, aArray, _ = removeBackground(dArray, 20)
    popt, pcov = fitDegradation(aArray[:,:], dtime[:])

    print(popt)
    print(np.sqrt(pcov))



    file = open(r"C:\Users\M\Documents\phdmatlab\sqib-pmma-probe-wavelength\UV_Setup\new_parallel_pol_pump653nm\Pump653Probe493_Degradation\TAfitPump653Probe493_OUTPUT.JSON")
    entries = json.load(file)['entries']
    entries_files = []
    timeAt = np.array([0,1e3,5e3]) #fs
    dOD = np.zeros((len(entries), len(timeAt)))
    expDecay = lambda ampTau1, ampTau2, t: ampTau1[0]*np.exp(-t/ampTau1[1])+ampTau2[0]*np.exp(-t/ampTau2[1])

    fig, ax = plt.subplots(1,1, figsize=(plotHelperLatex.figSizer(2,3)), dpi = 144)

    indices = [20,25,40]
    print(np.shape(aArray))
    #print(aArray)
    for i in indices:
        ax.plot(dtime, aArray[:,i], label='std')
        ax.plot(dtime, aArray[:,i]*np.exp(dtime/popt))
    #print((1-degradationCompensation(popt, dtime, np.shape(aArray)[0])[:,0])/abs(aArray[:,0]))
    #print(np.exp(dtime/popt))
    #print(aArray)
    #ax.plot(dtime, aArray[:,50]*np.exp(dtime/popt))
    ax.legend()
    ax.set_xlabel('time / s')
    ax.set_ylabel('absorbance / mOD')

    plt.show()

def numberFileGenerator(start, stop, namePrefix=r"TA_fourier_", nameSuffix=r""):
    ''''generate TA_fourier_02364 style names in list form'''
    vec = np.arange(start, stop+1,1)
    filenames = []
    for number in vec:
        filenames.append(namePrefix + str(number)+ nameSuffix)
    return filenames


