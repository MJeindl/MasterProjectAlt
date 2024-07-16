import numpy as np
from matplotlib import pyplot as plt
from matplotlib.animation import FuncAnimation
from celluloid import Camera
from IPython import display
import plotHelperLatex
plotHelperLatex.setMatplotSettings()
from matplotlib.animation import ImageMagickWriter

def gaussian(x, sig = 1, mu = 0):
    return 1/(np.sqrt(2*np.pi)*sig)*np.exp(-(x-mu)**2/(2*sig**2))

def gaussProd(x, sig1, sig2, mu1, mu2):
    mu_n = (mu1*sig2**2 + mu2*sig1**2)/(sig1**2 + sig2**2)
    sig_n = sig1*sig2/np.sqrt(sig1**2 + sig2**2)
    return 1/(np.sqrt(2*np.pi)*sig_n)*np.exp(-(x-mu_n)**2/(2*sig_n**2))

def cFactor(sigPump, sig2, mu1, mu2):
    return np.sqrt((sigPump**2 + sig2**2)/sigPump**2)*np.exp((mu1-mu2)**2/(2*(sigPump**2+sig2**2)))



def plotVisGaussians(x, sig2, dmu, norm = True, ax=None, colors = ["blue", "royalblue"]):

    #probe
    #ax.plot(x, gaussian(x, 1, -dmu/2), label = r"probe: $\sigma$ = 1; $\mu = %.1f$" %(-dmu/2))
    #pump r"pump: $\sigma$ = %d; $\mu = %.1f$" %(sig2, dmu)
    #ax.plot(x, gaussian(x, sig2, dmu), label = r"pump", color = colors[0], linestyle="dotted")
    #overlap
    if norm == True:
        ax.plot(x, gaussProd(x, 1, sig2, -dmu, 0), "r--", label = r"overlap")
    elif norm == False:
        relativeGaussiantoProbe = gaussian(x,1,0)*gaussian(x, sig2, dmu)/max(gaussian(x, sig2, +dmu))
        #plt.plot(x, relativeGaussiantoProbe, "r--", label = r"overlap")
        ax.fill_between(x, relativeGaussiantoProbe, hatch='/', color=colors[1], facecolor="None")
    #ax.legend()
    #ax.set_xlabel(r"x / $\sigma_{probe}$")
    #ax.set_ylabel(r"I / a.u.")
    #ax.set_title(r"$\frac{\sigma_{pump}}{\sigma_{probe}} = %.1d$, $\Delta\mu$ = %.1f $\sigma_{probe}$,  C = %.2f" %(sig2,dmu, cFactor(sig2, 1, dmu,0)), pad=8)
    #plt.show()
    return ax

##PUMP SHIFT
if 0:
    x = np.linspace(-3, 3, 200)
    fig, ax = plt.subplots(1, 1, figsize=plotHelperLatex.figSizer(1), dpi = 288)
    ax.set_ylim((0,0.45))
    ax.set_xlim((-3,3))
    camera = Camera(fig)
    ax.set_ylabel('intensity / a.u.')
    ax.set_xlabel(r'x / $\sigma$')
    plt.tight_layout()
    probeLine = ax.plot(x, gaussian(x, 1, 0), label = "pump", color="royalblue")
    pumpLine = ax.plot(x, gaussian(x, 1, 0), label="probe", linestyle="dotted", color="orange")
    ax.legend()
    #probe
    for mu_p in np.linspace(-3,3, 40):
        probeLine = ax.plot(x, gaussian(x, 1, 0), label = r"pump", color="royalblue")
        pumpLine, = ax.plot(x, gaussian(x, 1, mu_p), label=r"probe", linestyle="dotted", color="orange")
        #fillCollection = ax.fill_between(x, np.zeros(np.shape(x)), hatch='/', color="orange", facecolor="None")
        fillCollection  = ax.fill_between(x, gaussian(x,1,0)*gaussian(x, 1, mu_p)/max(gaussian(x, 1, mu_p)),hatch='/', color="orange", facecolor="None")
        stringy = r"$\Delta \mu_{probe}$ = %.1f" %abs(mu_p)
        ax.text(-2.5,0.4, stringy)
        camera.snap()

    animation = camera.animate()
    #print(type(animation))
    animation.save(r"C:\Users\M\Documents\Books\masterprojectinformation\images\Presentation"+r"\PumpShiftOverlapInverse.gif", fps=4, dpi=288)



##PUMP SIZE CHANGE
if 1:
    x = np.linspace(-3, 3, 200)
    fig, ax = plt.subplots(1, 1, figsize=plotHelperLatex.figSizer(1), dpi = 288)
    ax.set_ylim((0,0.45))
    ax.set_xlim((-3,3))
    camera = Camera(fig)
    ax.set_ylabel('intensity / a.u.')
    ax.set_xlabel(r'x / $\sigma$')
    
    plt.tight_layout()
    #ax.set_title(r"$\frac{\sigma_\text{Pump}}{\sigma_\text{Probe}}$")
    #probe
    probeLine = ax.plot(x, gaussian(x, 1, 0), label = "pump", color="royalblue")
    pumpLine = ax.plot(x, gaussian(x, 1, 0), label="probe", linestyle="dotted", color="orange")
    ax.legend()
    for sig_pump in np.linspace(0.3,3, 40):
        probeLine = ax.plot(x, gaussian(x, 1, 0), label = r"pump", color="royalblue")
        pumpLine = ax.plot(x, gaussian(x, sig_pump, 0), label=r"probe", linestyle="dotted", color="orange")
        #fillCollection = ax.fill_between(x, np.zeros(np.shape(x)), hatch='/', color="orange", facecolor="None")
        fillCollection  = ax.fill_between(x, gaussian(x,1,0)*gaussian(x, sig_pump, 0)/max(gaussian(x, 1, 0)),hatch='/', color="orange", facecolor="None")
        #stringy = r"$\frac{\sigma_{Pump}}{\sigma_{Probe}}$ = %.1f" %sig_pump
        stringy = r"$\frac{\sigma_{probe}}{\sigma_{pump}}$ = %.1f" %sig_pump
        ax.text(-2.5,0.4, stringy)
        camera.snap()
    
        

    animation = camera.animate()
    #print(type(animation))
    animation.save(r"C:\Users\M\Documents\Books\masterprojectinformation\images\Presentation"+r"\PumpSizeOverlapInverse.gif", fps=4, dpi=288)

def init():
    ax.set_ylim((0,0.45))
    ax.set_xlim((-3,3))
    #fillCollection = ax.fill_between(x, np.zeros(np.shape(x)), hatch='/', color="orange", facecolor="None")
    #return fillCollection
def updateLateral(muPump):
    pumpLine.set_data(x, gaussian(x,1,muPump))
    relativeGaussiantoProbe = gaussian(x,1,0)*gaussian(x, 1, muPump)/max(gaussian(x, 1, +muPump))
    #fig.canvas.restore_region
    #print(fillCollection)
    #print(muPump)
    fillCollection.remove()
    fillCollection = ax.fill_between(x, relativeGaussiantoProbe, hatch='/', color="orange", facecolor="None")
    return fillCollection
#print(fillCollection)
#ani = FuncAnimation(fig, updateLateral, frames = np.linspace(-3,3, 20), init_func=init,blit=False)
    

