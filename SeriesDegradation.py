import pandas as pd
from matplotlib import pyplot as plt
from scipy import constants as const
import numpy as np
from datetime import timedelta
import sys
from scipy import io
from argparse import ArgumentParser
sys.path.insert(1, r"C:\Users\M\Documents\phdmatlab\sqib-pmma-probe-wavelength\UV_Setup\new_parallel_pol_pump653nm")
from TA_EdgeFit import readData

#plt.rcParams['text.usetex'] = True
#font = {'size': 20}
#plt.rc('font', **font)

def parseTime(time):
    '''takes time in hh:mm:ss and returns a value in s'''
    times = np.zeros(len(time), dtype = int)
    hh, mm, ss = str(time[0]).split(":")
    times[0] = int(hh)*3600+int(mm)*60+int(ss)
    for i in range(len(time)-1):
        hh, mm, ss = str(time[i+1]).split(":")
        times[i+1] = int(hh)*3600+int(mm)*60+int(ss) - times[0]

    times[0] = 0
    return times

def getTimes(filepath):
    '''moved out of degradationCompensation for general use'''
    tempLoad = io.loadmat(filepath)
    #test if file has keys for single TA measurement

    if 'delay' in tempLoad.keys() and 'd_vec' in tempLoad.keys():
        #this part can easily be broken by a matlab update
        #load time
        timeString = str(tempLoad['__header__']).split("Created on: ")[1]
        timeString = timeString.split(' ')[3]
        #print(timeString)
        subTimes = np.zeros((1,3), dtype=float)
        subTimes[:] = timeString.split(':')[:3]

    elif 'dates' in tempLoad.keys():
        subDates = tempLoad['dates'][0]
        subTimes = np.zeros((len(subDates), 3), dtype=float)
        for ind, timeArray in enumerate(subDates):
            #print(timeArray[0][3:6])
            timeArray = timeArray[0][3:6]
            #print(timeArray)
            subTimes[ind,0] = timeArray[0]#hh
            subTimes[ind,1] = timeArray[1]##mm
            subTimes[ind,2] = timeArray[2]##ss with miliseconds
    return subTimes
    
def degradationCompensation(degradePerSecond, filepathArray, powerDensities):
    '''returns correction factor for each measurement\\
        degradePerSecond is a linear fit of degradation per second per W/m^2 peak\\
        filepathArray is ordered array of measurements after each other\\
        powerdensities is either a single number (int/float) or array of powerdensities of size of filepathArray\\
        not optimal but a linear approximation is about as good as I can do it with the data available'''
    from scipy import io 
    timesInSecond = np.zeros(len(filepathArray))
    AllTimes = [] #list so I can append
    for path in filepathArray:
        #need to differentiate between the summary of TA scans and TA scans
        #do not mix and match please
        #try:
        
        tempLoad = io.loadmat(path)
        #test if file has keys for single TA measurement

        if 'delay' in tempLoad.keys() and 'd_vec' in tempLoad.keys():
            #this part can easily be broken by a matlab update
            #load time
            timeString = str(tempLoad['__header__']).split("Created on: ")[1]
            timeString = timeString.split(' ')[3]
            #print(timeString)
            subTimes = np.zeros((1,3), dtype=float)
            subTimes[:] = timeString.split(':')[:3]

        elif 'dates' in tempLoad.keys():
            subDates = tempLoad['dates'][0]
            subTimes = np.zeros((len(subDates), 3), dtype=float)
            for ind, timeArray in enumerate(subDates):
                #print(timeArray[0][3:6])
                timeArray = timeArray[0][3:6]
                #print(timeArray)
                subTimes[ind,0] = timeArray[0]#hh
                subTimes[ind,1] = timeArray[1]##mm
                subTimes[ind,2] = timeArray[2]##ss with miliseconds

        
        AllTimes.append(subTimes)


        #except: 
        #    print("dooters")
    #print(AllTimes)
    timesFromZero = np.zeros(np.shape(AllTimes)[:2])
    degradationCorrection = np.zeros(np.shape(AllTimes)[:2])

    #grab the zero time
    zeroTime = np.array(AllTimes[0][0])
    
    if type(powerDensities) != type(np.ndarray):
        powerDensities = np.ones(np.shape(AllTimes)[0])
        

    for ind in range(np.shape(AllTimes)[0]):
        for subindex in range(np.shape(AllTimes[ind])[0]):
            #hours
            timesFromZero[ind,subindex] = (AllTimes[ind][subindex][0]-zeroTime[0])*3600
            #check for day-tickover
            if timesFromZero[ind,subindex] < 0:
                timesFromZero[ind,subindex] += 24*3600
            #minutes
            timesFromZero[ind,subindex] += (AllTimes[ind][subindex][1]-zeroTime[1])*60
            #seconds
            timesFromZero[ind,subindex] += AllTimes[ind][subindex][2]-zeroTime[2]
            degradationCorrection[ind, subindex] = 1/(1-degradePerSecond*timesFromZero[ind,subindex]*powerDensities[ind])
        
    return degradationCorrection, timesFromZero

def plotTrend(directoryPath, start, stop):
    #basepath = lambda num: r"C:\Users\M\Documents\phdmatlab\sqib-pmma-probe-wavelength\UV_Setup\new_parallel_pol_pump653nm\Pump653Probe493_Degradation\TA_fourier_"+str(num)+r".mat"
    basepath = lambda num: directoryPath + r"TA_fourier_" + str(num) + r".mat"

    start = int(start)
    stop = int(stop)

    file_vec = np.arange(start, stop+1, 1)
    OD = []
    times = np.zeros((stop-start+1, 3))
    #readData(r"C:\Users\M\Documents\phdmatlab\sqib-pmma-probe-wavelength\UV_Setup\new_parallel_pol_pump653nm\Pump653Probe493_Degradation\TA_fourier_6778.mat")
    for ind, number in enumerate(file_vec):
        delay, tempdata = readData(basepath(number))
        OD.append(tempdata)
        times[ind] = getTimes(basepath(number))
    OD = np.array(OD)

    #get delta t of times
    timesFromZero = np.zeros(np.shape(times)[0])
    zeroTime = times[0]
    for ind, time in enumerate(times):
        #hours
        timesFromZero[ind] = (times[ind][0]-zeroTime[0])*3600
        #check for day-tickover
        if timesFromZero[ind] < 0:
            timesFromZero[ind] += 24*3600
        #minutes
        timesFromZero[ind] += (times[ind][1]-zeroTime[1])*60
        #seconds
        timesFromZero[ind] += times[ind][2]-zeroTime[2]

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
    parser = ArgumentParser(description="Shows response trend over mulitple adjacent measurements")
    parser.add_argument("startFileNumber")
    parser.add_argument("endFileNumber")
    parser.add_argument("pathDir")
    
    args = parser.parse_args()

    start = int(args.startFileNumber)
    stop = int(args.endFileNumber)
    plotTrend(args.pathDir, start, stop)