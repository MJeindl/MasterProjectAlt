#taken from Graphs and using interpolation
#Test binning of numerical correction factor
import numpy as np
from matplotlib import pyplot as plt
import plotHelperLatex
plotHelperLatex.setMatplotSettings()
from scipy.interpolate import RegularGridInterpolator as Interpol

def gaussian(x, sig = 1, mu = 0):
    return 1/(np.sqrt(2*np.pi)*sig)*np.exp(-(x-mu)**2/(2*sig**2))

def cFactor(sigPump, sig2, mu1, mu2):
    return np.sqrt((sigPump**2 + sig2**2)/sigPump**2)*np.exp((mu1-mu2)**2/(2*(sigPump**2+sig2**2)))

def binMap(map, xBinning, yBinning=0):
    '''N_binning means that N*N pixels are binned together\n
    only takes square z*z maps'''
    if yBinning == 0:
        yBinning = xBinning
    xBins = int(np.shape(map)[0]//xBinning) #along one axis
    yBins = int(np.shape(map)[1]//yBinning) #along one axis
    outmap = map.reshape(xBins, xBinning, yBins, yBinning).sum(3).sum(1)
    return outmap


#using identical gaussians for this
points_per_sig = 1024 #starting value 1000 points for 1 sig distance
sig = 1 #does not matter since I do binning in steps of sig at first
mu = 0
###----analytical cFactor ----###
cFactorAnalytic = cFactor(sig, sig, mu, mu)**2
print("Analytic correction factor:\n " + str(cFactorAnalytic))
###----numeric cFactor ----###
nsig = 5 # how far I measure to the sides
x_like = np.linspace(-nsig*sig,nsig*sig, 2*nsig*points_per_sig)
Xvec, Yvec = np.array(np.meshgrid(x_like,x_like))


gaussStd = gaussian(Xvec, sig, mu)*gaussian(Yvec, sig, mu)


n_steps = 11
binning_steps=np.zeros(n_steps, dtype=int)

for i in range(n_steps):
    binning_steps[i] = 2**i
#print(binning_steps)

#first without added noise
print("Correction Factor numeric, no noise:")
noNoiseGaussCorrection = np.zeros(len(binning_steps))
for ind in range(len(binning_steps)):
    gaussBinned = binMap(gaussStd, binning_steps[ind])
    if ind != 0:
        bin_vectorX = binMap(Xvec,2**(ind))[0,:]/(4**(ind))
        bin_vectorY = binMap(Yvec,2**(ind))[:,0]/(4**(ind))
    else:
        bin_vectorX = Xvec[0,:]
        bin_vectorY = Yvec[:,0]
    
    x_interpol = np.linspace(bin_vectorX[0], bin_vectorX[-1], 2*nsig*128)
    y_interpol = np.linspace(bin_vectorY[0], bin_vectorY[-1], 2*nsig*128)
    x_interpol, y_interpol = np.meshgrid(x_interpol,y_interpol)
    x_interpol = np.reshape(x_interpol,(-1))
    y_interpol = np.reshape(y_interpol, (-1))
    full_interpol = list(zip(x_interpol, y_interpol))
    pump = Interpol((bin_vectorX, bin_vectorY), gaussBinned)
    probe = Interpol((bin_vectorX, bin_vectorY), gaussBinned)
    print(np.shape(pump(full_interpol)))
    denominator = np.sum(np.multiply(pump(full_interpol), probe(full_interpol))[:])
    constantPumpIntensity = np.max(pump(full_interpol))
    #print(constantPumpIntensity)
    enumerator = np.sum(constantPumpIntensity*probe(full_interpol)[:])
    #this is missing various constant factors, since this is not using the gaussian shape, but is discretized
    noNoiseGaussCorrection[ind] = enumerator/denominator
    print("binning = 1/"+str(binning_steps[ind])+": " + str(noNoiseGaussCorrection[ind]))

plt.figure(dpi = 288, figsize=plotHelperLatex.figSizer(1,2))
plt.plot(np.linspace(n_steps-1,0,n_steps), noNoiseGaussCorrection, label = "no noise")
if False:
    #with added noise
    Noiselevels = [0.05,0.1,0.2]
    NoiseGaussCorrection = np.zeros((len(Noiselevels), len(binning_steps)))
    for indexSNR, SNR in enumerate(Noiselevels):

        gauss1 = gaussStd + (np.random.rand(np.shape(gaussStd)[0], np.shape(gaussStd)[1])-0.5)*np.max(gaussStd)*SNR
        gauss2 = gaussStd + (np.random.rand(np.shape(gaussStd)[0], np.shape(gaussStd)[1])-0.5)*np.max(gaussStd)*SNR
        
        for ind in range(len(binning_steps)):
            if (int(binning_steps[ind]/2)) != 0:
                current_vec = x_like[int(binning_steps[ind]/2):-1:binning_steps[ind]]
                x_interpol = x_like[int(binning_steps[ind]/2):-int(binning_steps[ind]*1.01/2)]
                #print(current_vec)
                pump = Interpol((current_vec, current_vec), binMap(gauss1, binning_steps[ind]))
                probe = Interpol((current_vec, current_vec), binMap(gauss2, binning_steps[ind]))
                denominator = np.sum(np.multiply(pump((x_interpol,x_interpol)), probe((x_interpol,x_interpol)))[:])
                constantPumpIntensity = np.max(pump((x_interpol,x_interpol)))
                enumerator = np.sum(constantPumpIntensity*probe((x_interpol,x_interpol))[:])
            else:
                current_vec = x_like
                denominator = np.sum(np.multiply(gauss1, gauss2)[:])
                constantPumpIntensity = np.max(gauss1)
                enumerator = np.sum((constantPumpIntensity*gauss2)[:])

            
            #this is missing various constant factors, since this is not using the gaussian shape, but is discretized
            NoiseGaussCorrection[indexSNR, ind] = enumerator/denominator
        plt.plot(np.linspace(n_steps-1,0,n_steps), NoiseGaussCorrection[indexSNR], label = "SNR = %.2f dB" %(10*np.log10(1/SNR)), linestyle='dashed')



plt.plot([0,n_steps-1], [2,2], label="analytical correction", linestyle="dotted", color = 'red')
plt.xlabel(r"$\mathrm{log_2}$ of discrete point density per $\sigma$ / 1")
plt.ylabel("correction factor / 1")
plt.ylim([1,2.5])
plt.legend(loc="lower right", fontsize = "small")
#plt.show()