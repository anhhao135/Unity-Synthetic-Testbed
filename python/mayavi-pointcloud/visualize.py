import mayavi
import numpy as np
import os
import sys


from numpy import genfromtxt


def viz_mayavi(points, vals="distance"):
    x = points[:, 3]  # x position of point
    y = points[:, 4]  # y position of point
    z = points[:, 5]  # z position of point
    # r = lidar[:, 3]  # reflectance value of point
    d = np.sqrt(x ** 2 + y ** 2)  # Map Distance from sensor

    # Plot using mayavi -Much faster and smoother than matplotlib
    import mayavi.mlab

    if vals == "height":
        col = z
    else:
        col = d

    fig = mayavi.mlab.figure(bgcolor=(0, 0, 0), size=(640, 360))
    mayavi.mlab.points3d(x, y, z,
                         col,          # Values used for Color
                         mode="point",
                         colormap='spectral', # 'bone', 'copper', 'gnuplot'
                         # color=(0, 1, 0),   # Used a fixed (r,g,b) instead
                         figure=fig,
                         )
    mayavi.mlab.show()



points = genfromtxt(sys.argv[1], delimiter=' ')

'''
x = points[:, 2]  # x position of point
y = -points[:, 0]  # y position of point
z = points[:, 1]  # z position of point

new_points = np.zeros((len(points), 4))

new_points[:,0] = x
new_points[:,1] = y
new_points[:,2] = z
new_points[:,3] = 0

print(len(new_points))
print(new_points)


#np.save("my_data.npy", new_points)

'''

viz_mayavi(points, vals="distance")

