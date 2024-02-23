import numpy as np
from matplotlib import pyplot as plt
from numba import jit, float32, int32

#x = sin(theta)cos(phi)r
#y = sin(theta)sin(phi)r
#z = cos(theta)r

timesteps = int(1e3)
tau = 1e8
n_molecules = int(1e4)

amplitude_vector_parallel = np.zeros((n_molecules, timesteps))
amplitude_vector_orthogonal = np.zeros((n_molecules, timesteps))
amplitude_vector_magic = np.zeros((n_molecules, timesteps))
vector = np.random.rand(n_molecules, 3)
vector[:,2] = 1
rotation = np.random.rand(n_molecules, 2)*1e-3
#pump beam: z direction
pumpVec = np.array([0,0,1], dtype=float)
orthVec = np.array([1/2,0,1], dtype = float)
magicVec = np.array([54.7/np.pi/2,0,1], dtype = float)


@jit(nopython=True, parallel=True)
def dotProd(vecMeas, vecSample, lensample):
    temp = np.zeros(lensample)
    for i in range(lensample):
        temp[i] = abs(vecMeas[2]*vecSample[i,2]*(np.sin(vecMeas[0]*np.pi)*np.sin(vecSample[i,0]*np.pi)*np.cos((vecMeas[1]-vecSample[i,1])*np.pi*2)+ np.cos(vecMeas[0]*np.pi)*np.cos(np.pi*vecSample[i,0])))
    return temp


#vec[0] is theta, vec[1] is phi, vec[2] is amplitude or r
vector[:,0] = vector[:,0]
vector[:,1] = vector[:,1]
vector[:,2] = dotProd(pumpVec, vector, n_molecules)

amplitude_vector_parallel[:,0] = vector[:,2]
amplitude_vector_parallel[:,0] = dotProd(orthVec, vector, n_molecules)
amplitude_vector_magic[:,0] = dotProd(magicVec, vector, n_molecules)

for t in range(1,timesteps,1):
    vector[:,0] = np.mod(rotation[:,0]+vector[:,0], 1)
    amplitude_vector_parallel[:,t] = dotProd(pumpVec, vector, n_molecules)
    amplitude_vector_orthogonal[:,t] = dotProd(orthVec, vector, n_molecules)
    amplitude_vector_magic[:,t] = dotProd(magicVec, vector, n_molecules)


#print(np.sum(amplitude_vector_parallel, axis = 0)/np.sum(amplitude_vector_parallel[:,0])
#plt.figure()
amplitude_vector_parallel = np.sum(amplitude_vector_parallel, axis = 0)
amplitude_vector_orthogonal = np.sum(amplitude_vector_orthogonal, axis = 0)
amplitude_vector_magic = np.sum(amplitude_vector_magic, axis = 0)
fig, axs = plt.subplots(3,1)
axs[0].plot(amplitude_vector_parallel/max(amplitude_vector_parallel), color='r')
axs[1].plot(amplitude_vector_orthogonal/max(amplitude_vector_orthogonal), color='b')
axs[2].plot(amplitude_vector_magic/max(amplitude_vector_magic), color='g')
plt.show()