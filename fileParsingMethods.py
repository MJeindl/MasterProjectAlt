import numpy as np
from scipy.io import loadmat
import os


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
    backgroundLevel = np.mean(T[:,:numEntries])
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