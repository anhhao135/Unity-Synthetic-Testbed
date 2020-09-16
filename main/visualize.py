import mayavi
import numpy as np
import os
import sys
import time
import mayavi.mlab
from numpy import genfromtxt
from mayavi import mlab
import open3d as o3d
from mpl_toolkits.mplot3d import Axes3D
import matplotlib.pyplot as plt



classDict = {
    "Unlabelled" : 1,
    "Person" : 2
}

def viz_mayavi(points, vals="distance"):
    x = points[:, 0]  # x position of point
    y = points[:, 1]  # y position of point
    z = points[:, 2]  # z position of point
    d = points[:, 3]
    # r = lidar[:, 3]  # reflectance value of point
    #d = np.sqrt(x ** 2 + y ** 2)  # Map Distance from sensor

    # Plot using mayavi -Much faster and smoother than matplotlib
    import mayavi.mlab

    

    if vals == "height":
        col = z
    else:
        col = d

    fig = mayavi.mlab.figure(bgcolor=(0, 0, 0), size=(640, 360))
    pts = mayavi.mlab.points3d(x, y, z, col,          # Values used for Color
                         mode="point",
                         colormap='bone', # 'bone', 'copper', 'gnuplot'
                         #color=(0, 1, 0),   # Used a fixed (r,g,b) instead
                         figure=fig,
                         #scale_factor=0.05
                         )
    #pts.glyph.glyph.clamping = False
    #pts.glyph.scale_mode = 'scale_by_vector'
    mayavi.mlab.show()

    #mayavi.mlab.show(stop=True)



if sys.argv[2] == "syn":

    points = genfromtxt(sys.argv[1], delimiter=',', names=True, dtype=None, encoding=None)
    currentRevolutions  = {}

    for row in points:
        currentGameTime, currentRevolution, currentRevolutionPeriod, x, y, z, revolutionTime, lidarAngularOffset, className = row
        if currentRevolutionPeriod not in currentRevolutions:
            currentRevolutions[currentRevolutionPeriod] = list()
        currentRevolutions[currentRevolutionPeriod].append((x, y, z, classDict[className]))


    print(currentRevolutions.keys())

    veloPoints = np.array(currentRevolutions[6])

    print(veloPoints)

    #fig = plt.figure()
    #ax = fig.add_subplot(111, projection='3d')

    # For each set of style and range settings, plot n random points in the box
    # defined by x in [23, 32], y in [0, 100], z in [zlow, zhigh].

    '''
    for c, m, zlow, zhigh in [('r', 'o', -50, -25), ('b', '^', -30, -5)]:
        xs = veloPoints[:, 0]
        ys = veloPoints[:, 1]
        zs = veloPoints[:, 2]
        ax.scatter(xs, ys, zs, c=c, marker=m)
    '''
    '''
    xs = veloPoints[:, 0]
    ys = veloPoints[:, 1]
    zs = veloPoints[:, 2]



    ax.scatter(xs, ys, zs, s=1)

    ax.set_xlabel('X Label')
    ax.set_ylabel('Y Label')
    ax.set_zlabel('Z Label')
    plt.gca().set_aspect('equal', adjustable='box')
    plt.show()

    '''

    #viz_mayavi(veloPoints, vals="distance")


    x = np.zeros(3)
    y = np.zeros(3)
    z = np.zeros(3)

    fig = mayavi.mlab.figure(bgcolor=(0, 0, 0), size=(640, 360))
    pts = mayavi.mlab.points3d(x, y, z,  # Values used for Color
                               mode="point",
                               colormap='spectral',  # 'bone', 'copper', 'gnuplot'
                               # color=(0, 1, 0),   # Used a fixed (r,g,b) instead
                               figure=fig,
                               # scale_factor=0.05
                               )


    @mlab.animate(delay=int(revolutionTime * 1000))
    def anim():
        for revolution in currentRevolutions:
            # print(len(currentRevolutions[revolution]))
            pointsArray = np.array(currentRevolutions[revolution])
            # print(pointsArray)
            print(pointsArray)
            pts.mlab_source.reset(x=pointsArray[:, 0], y=pointsArray[:, 1], z=pointsArray[:, 2])


            # print(revolution)

            #pts.mlab_source.reset()
            #pts.mlab_source.x = pointsArray[:, 0]
            #pts.mlab_source.y = pointsArray[:, 1]
            #pts.mlab_source.z = pointsArray[:, 2]
            #pts.mlab_source.d = pointsArray[:, 3]
            yield


    anim()
    mayavi.mlab.show()
    print(revolutionTime)






if sys.argv[2] == "kitti":

    points = genfromtxt(sys.argv[1], delimiter=' ')
    extracted_points = []

    for row in points:
        x, y, z, reflectance = row
        extracted_points.append((x,y,z))

    veloPoints = np.array(extracted_points)

    print(veloPoints)

    viz_mayavi(veloPoints, vals="distance")














