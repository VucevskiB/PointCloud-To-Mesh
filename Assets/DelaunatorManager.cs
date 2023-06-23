using DelaunatorSharp.Unity.Extensions;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DelaunatorManager : MonoBehaviour
{
    public GameObject pointPrefab;
    public DelaunatorPreview preview;
    public GameObject parentOfPoints;
    public Vector2[] Points;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(AddRandomPoints());
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator AddRandomPoints()
    {
        foreach(var point in Points)
        {
            Instantiate(pointPrefab, new Vector3(point.x, point.y, 0),Quaternion.identity); 
            yield return new WaitForSeconds(0.2f);
            preview.AddPont(point);
            yield return new WaitForSeconds(0.5f);
        }


        yield return null;
    }
}
