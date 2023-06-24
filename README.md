# PointCloud To Mesh Algorithms

This is an example project made in Unity that visualizes some techniques and algorithms that are used to generate meshes from point cloud data.

## Delaunay triangulation
Open Scene: Assets/Scenes/Delaunay and hit Play

<p align="center">
    <img width="auto" height="200px" src="https://i.imgur.com/vtFH7X4.png" alt="Image 1"> <img width="auto" height="200px" src="https://i.imgur.com/HD89h4p.png" alt="Image 2">
</p>
You can also change the predefined points or add more points just by clicking anywhere on the screen.

## Marching cubes
Open Scene: Assets/Scenes/Marching Cubes and hit Play

You can change the parameters in the Visualizer Game Object. The point cloud data is generated from the Donut Game Object. From the scene view you can see the marching cubes algorithm in action and all of the points that are generated in a grid.

<p align="center">
    <img width="auto" height="250px" src="https://i.imgur.com/lOJy143.png" alt="Image 1"> <img width="auto" height="250px" src="https://i.imgur.com/AqfQjOh.png" alt="Image 2">
</p>
And this is the final product:
<p align="center">
  <img width="auto" height="300px" src="https://i.imgur.com/eI0jjjt.png">
</p>

## References
[1] [Habrador/Computational-geometry: Computational Geometry Unity library with implementations of intersection algorithms, triangulations like delaunay, voronoi diagrams, polygon clipping, bezier curves, ear clipping, convex hulls, mesh simplification, etc (github.com)](https://github.com/Habrador/Computational-geometry)

[2] [Marching Cubes Part 1: Explaining marching cubes · Poly Coding](https://polycoding.net/marching-cubes/part-1/)
