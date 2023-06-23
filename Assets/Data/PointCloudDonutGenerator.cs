using System.Collections.Generic;
using System.IO;
using UnityEngine;

    //public int pointCount = 1000; // Number of points to generate
    //public float innerRadius = 1f; // Inner radius of the donut shape
    //public float outerRadius = 3f; // Outer radius of the donut shape
    //public float heightScale = 1f; // Scale factor for the y-coordinate

    //public void GeneratePointCloudData()
    //{
    //    string filePath = "Assets/Data/pointcloud.txt";

    //    using (StreamWriter writer = new StreamWriter(filePath))
    //    {
    //        for (int i = 0; i < pointCount; i++)
    //        {
    //            // Generate random angle and radius values
    //            float angle = Random.Range(0f, Mathf.PI * 2f);
    //            float radius = Random.Range(innerRadius, outerRadius);

    //            // Calculate the point coordinates based on angle and radius
    //            float x = Mathf.Cos(angle) * radius;
    //            float y = Random.Range(0f, 1f) * heightScale; // Vary the y-coordinate within the specified height scale
    //            float z = Mathf.Sin(angle) * radius;

    //            // Normalize the coordinates to fit within the grid size (16x16x16)
    //            x = Mathf.Clamp01((x + outerRadius) / (2f * outerRadius));
    //            y = Mathf.Clamp01((y + outerRadius) / (2f * outerRadius));
    //            z = Mathf.Clamp01((z + outerRadius) / (2f * outerRadius));

    //            // Write the coordinates to the file
    //            writer.WriteLine($"{x},{y},{z}");
    //        }
    //    }

    //    Debug.Log("Point cloud data file generated: " + filePath);
    //}


public class PointCloudDonutGenerator : MonoBehaviour
{
    public int pointCount = 1; // Number of points to generate
    public float innerRadius = 3f; // Inner radius of the donut shape
    public float outerRadius = 8f; // Outer radius of the donut shape
    public float heightScale = 1f; // Scale factor for the y-coordinate
    public float heightSpread = 1f; // Controls the spread of the bell curve
    public float donutScale = 0.1f; // Scale factor for the donut shape (0.0 to 1.0)

    public List<Vector3> GeneratePointCloudData()
    {
        List<Vector3> pointCloud = new List<Vector3>();

        for (int i = 0; i < pointCount; i++)
        {
            // Generate angle value based on index
            float angle = (float)i / pointCount * 2f * Mathf.PI;

            // Calculate the point coordinates based on angle and radius
            float radius = Random.Range(innerRadius, outerRadius) * donutScale;
            float x = Mathf.Cos(angle) * radius;
            float z = Mathf.Sin(angle) * radius;

            // Generate the y-coordinate using a bell curve function
            float normalizedX = (x + outerRadius * donutScale) / (2f * outerRadius * donutScale) ; // Normalize x to [0, 1]
            float normalizedZ = (z + outerRadius * donutScale) / (2f * outerRadius * donutScale) ; // Normalize z to [0, 1]
            float y = heightScale * BellCurve(normalizedX, normalizedZ, heightSpread);

            // Normalize the coordinates to fit within the grid size (16x16x16)
            x = Mathf.Clamp(normalizedX, 0.0f, 1f);
            y = Mathf.Clamp01(y) - 0.4f;
            z = Mathf.Clamp(normalizedZ, 0.0f, 1f);

            // Add the point to the point cloud list
            pointCloud.Add(new Vector3(x, y, z));
            pointCloud.Add(new Vector3(x, 1-y, z));
        }

        return pointCloud;
    }

    private float BellCurve(float x, float z, float spread)
    {
        // Compute the bell curve value based on the x and z coordinates
        float centerX = 0.5f;
        float centerZ = 0.5f;
        float distanceX = Mathf.Abs(x - centerX);
        float distanceZ = Mathf.Abs(z - centerZ);
        float exponent = (distanceX * distanceX + distanceZ * distanceZ) / (-2f * spread * spread);
        return Mathf.Exp(exponent);
    }





private void Start()
    {
        GeneratePointCloudData();
    }
}

