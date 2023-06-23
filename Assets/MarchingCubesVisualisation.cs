using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class MarchingCubesVisualisation : MonoBehaviour
{
	public PointCloudDonutGenerator pointCloudDonutGenerator;
	public int showOnlyPointCloudsData;
	public Vector3[] pointCloudData;

	[Header("Grid")]
	public int gridSize = 32;

	[Header("Marching Cubes")]
	public bool useMarchingCubes;
	public float isoLevel = 0.5f;
	public float noiseScale = 5f;
	public float noiseAmplitude = 5f;
	public bool useGroundLevel;
	[Range(0f, 1f)] public float groundLevel = 0.2f;

	[Header("Display")]
	public bool useMarchDelay;
	public float marchSpeedInSeconds = 0.5f;
	public MeshFilter MeshFilter;
	Mesh mesh;

	float[,,] pointValues;

	Vector3 currentCubePosition;

	List<Vector3> vertices = new List<Vector3>();
	List<int> triangles = new List<int>();


	private void Start()
    {
        pointCloudData = new Vector3[gridSize * gridSize * gridSize]; // gridSize * gridSize * gridSize = 256 points

		//GenerateSphereData();
		pointCloudData = pointCloudDonutGenerator.GeneratePointCloudData().ToArray();


		mesh = new Mesh();
        CreateGrid();
        StartCoroutine(March());
    }

    private void GenerateSphereData()
    {
        int index = 0;
        float radius = 0.4f; // Adjust the radius as needed

        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                for (int z = 0; z < gridSize; z++)
                {
                    // Calculate the position of the point within the grid
                    float posX = (x + 0.5f) / gridSize; // Adding 0.5f to center the points within each cell
                    float posY = (y + 0.5f) / gridSize;
                    float posZ = (z + 0.5f) / gridSize;

                    // Calculate the distance from the center of the grid to the current point
                    float distance = Vector3.Distance(new Vector3(posX, posY, posZ), new Vector3(0.5f, 0.5f, 0.5f));

                    // If the distance is less than the desired radius, consider it as part of the sphere
                    if (distance <= radius)
                    {
                        pointCloudData[index] = new Vector3(posX, posY, posZ);
                        index++;
                    }
                }
            }
        }

        // Resize the point cloud data array to fit the actual number of points detected
        System.Array.Resize(ref pointCloudData, index);
    }

    public string filePath = "Assets/Data/pointcloud.txt"; // Specify the file path of the point cloud data file

	public List<Vector3> LoadPointCloudData()
	{
		List<Vector3> pointCloudData = new List<Vector3>();

		if (File.Exists(filePath))
		{
			string[] lines = File.ReadAllLines(filePath);

			foreach (string line in lines)
			{
				string[] values = line.Split(',');

				if (values.Length >= 3)
				{
					float x, y, z;
					if (float.TryParse(values[0], out x) && float.TryParse(values[1], out y) && float.TryParse(values[2], out z))
					{
						Vector3 point = new Vector3(x, y, z);
						pointCloudData.Add(point);
					}
				}
			}
		}
		else
		{
			Debug.LogError("Point cloud data file not found: " + filePath);
		}

		return pointCloudData;
	}


	void CreateGrid() {
		pointValues = new float[gridSize, gridSize, gridSize];

		// Iterate through the point cloud data
		for (int i = 0; i < pointCloudData.Length; i++)
		{
			Vector3 point = pointCloudData[i];

			// Map the point coordinates to the grid indices
			int x = Mathf.Clamp(Mathf.FloorToInt(point.x * gridSize),0,gridSize-1) ;
			int y = Mathf.Clamp(Mathf.FloorToInt(point.y * gridSize),0,gridSize-1) ;
			int z = Mathf.Clamp(Mathf.FloorToInt(point.z * gridSize),0,gridSize-1) ;

			x = (int)(Mathf.Clamp01(point.x) * (gridSize - 2) + 1f);
			y = (int)(Mathf.Clamp01(point.y) * (gridSize - 2) + 1f);
			z = (int)(Mathf.Clamp01(point.z) * (gridSize - 2) + 1f);

			// Assign a scalar value to the corresponding grid cell
			pointValues[x, y, z] = 1;
		}
	}

	void AddTriangle(Vector3 a, Vector3 b, Vector3 c) {
		int triIndex = triangles.Count;
		vertices.Add(a);
		vertices.Add(b);
		vertices.Add(c);
		triangles.Add(triIndex);
		triangles.Add(triIndex + 1);
		triangles.Add(triIndex + 2);
	}

	Vector3 Interp(Vector3 edgeVertex1, float valueAtVertex1, Vector3 edgeVertex2, float valueAtVertex2) {
		return (edgeVertex1 + (isoLevel - valueAtVertex1) * (edgeVertex2 - edgeVertex1) / (valueAtVertex2 - valueAtVertex1));
	}

	IEnumerator March () {

		for (int x = 0; x < gridSize - 1; x++) {
			for (int y = 0; y < gridSize - 1; y++) {
				for (int z = 0; z < gridSize - 1; z++) {
					currentCubePosition = new Vector3(x + .5f, y + .5f, z + .5f);

					if (useMarchingCubes) {

						// Set values at the corners of the cube
						float[] cubeValues = new float[] {
							pointValues[x, y, z + 1],
							pointValues[x + 1, y, z + 1],
							pointValues[x + 1, y, z],
							pointValues[x, y, z],
							pointValues[x, y + 1, z + 1],
							pointValues[x + 1, y + 1, z + 1],
							pointValues[x + 1, y + 1, z],
							pointValues[x, y + 1, z]
						};

						// Find the triangulation index
						int cubeIndex = 0;
						if (cubeValues[0] < isoLevel) cubeIndex |= 1;
						if (cubeValues[1] < isoLevel) cubeIndex |= 2;
						if (cubeValues[2] < isoLevel) cubeIndex |= 4;
						if (cubeValues[3] < isoLevel) cubeIndex |= 8;
						if (cubeValues[4] < isoLevel) cubeIndex |= 16;
						if (cubeValues[5] < isoLevel) cubeIndex |= 32;
						if (cubeValues[6] < isoLevel) cubeIndex |= 64;
						if (cubeValues[7] < isoLevel) cubeIndex |= 128;

						// Get the intersecting edges
						int[] edges = MarchingCubesTables.triTable[cubeIndex];

						Vector3 worldPos = new Vector3(x, y, z);

						int triCount = triangles.Count;

						// Triangulate
						for (int i = 0; edges[i] != -1; i += 3) {
							int e00 = MarchingCubesTables.edgeConnections[edges[i]][0];
							int e01 = MarchingCubesTables.edgeConnections[edges[i]][1];

							int e10 = MarchingCubesTables.edgeConnections[edges[i + 1]][0];
							int e11 = MarchingCubesTables.edgeConnections[edges[i + 1]][1];

							int e20 = MarchingCubesTables.edgeConnections[edges[i + 2]][0];
							int e21 = MarchingCubesTables.edgeConnections[edges[i + 2]][1];

							Vector3 a = Interp(MarchingCubesTables.cubeCorners[e00], cubeValues[e00], MarchingCubesTables.cubeCorners[e01], cubeValues[e01]) + worldPos;
							Vector3 b = Interp(MarchingCubesTables.cubeCorners[e10], cubeValues[e10], MarchingCubesTables.cubeCorners[e11], cubeValues[e11]) + worldPos;
							Vector3 c = Interp(MarchingCubesTables.cubeCorners[e20], cubeValues[e20], MarchingCubesTables.cubeCorners[e21], cubeValues[e21]) + worldPos;

							AddTriangle(a, b, c);
						}

						// Only reset mesh when triangles have been added
						if (triCount != triangles.Count) {
							mesh.SetVertices(vertices);
							mesh.SetTriangles(triangles, 0);
							mesh.RecalculateNormals();
							MeshFilter.mesh = mesh;
						}
					}

					if (useMarchDelay) {
						yield return new WaitForSeconds(marchSpeedInSeconds);
                    }
                    else
                    {
						//yield return null;
                    }
				}
			}
		}
	}

	private void OnDrawGizmos() {
		if (pointValues == null) {
			return;
		}
		for (int x = 0; x < gridSize; x++) {
			for (int y = 0; y < gridSize; y++) {
				for (int z = 0; z < gridSize; z++) {
					Gizmos.color = Color.Lerp(new Color(0,0,0, showOnlyPointCloudsData), Color.white, pointValues[x, y, z]);
					Gizmos.DrawCube(new Vector3(x, y, z), Vector3.one * .1f);
				}
			}
		}

		Gizmos.color = Color.green;
		Gizmos.DrawWireCube(currentCubePosition, Vector3.one);
	}
}
