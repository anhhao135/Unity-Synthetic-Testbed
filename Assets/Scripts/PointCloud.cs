using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

public class PointCloud : MonoBehaviour
{
    public int totalPixels;
    [SerializeField] Color[] pix;
    public Texture2D tex2d;
    public GameObject depthCameraObject;
    public Camera depthCamera;
    public float verticalFOV;
    public float horizontalFOV;
    public float camAspect;
    public MeshFilter mf;
    private int iteration = 0;
    public float depthMultiplier = 50f;
    public bool assumeRangeMap = false;
    public float planeOffset = 0f;
    private float initialNearPlane;
    private float initialFarPlane;
    private int totalPoints;
    public int cumIndex = 0;
    public bool SLAM = false;
    private Vector3 depthCamInitPos;
    public Vector3 originChange;
    public int pointsCount;
    public int indeciesCount;


    List<Vector3> points = new List<Vector3>();
    List<int> indecies = new List<int>();
    List<Color> colors = new List<Color>();


    // Use this for initialization
    void Start()
    {
        mf = GetComponent<MeshFilter>();

        depthCamera = depthCameraObject.GetComponent<Camera>();

        camAspect = depthCamera.aspect;

        pix = null;

        initialFarPlane = depthCamera.farClipPlane;
        initialNearPlane = depthCamera.nearClipPlane;

        depthCamInitPos = transform.InverseTransformPoint(depthCameraObject.transform.position);

        mf.mesh = new Mesh();

        depthMultiplier = initialFarPlane;







    }

    void Update()
    {


        depthCamera.nearClipPlane = initialNearPlane + planeOffset;
        depthCamera.farClipPlane = initialFarPlane + planeOffset;

        originChange = depthCamInitPos - depthCameraObject.transform.position;


        if (assumeRangeMap == false && SLAM == false)
        {

            verticalFOV = depthCamera.fieldOfView;
            horizontalFOV = Camera.VerticalToHorizontalFieldOfView(verticalFOV, camAspect);

            tex2d = depthCamera.GetComponent<Render2D>().tex2d;
            pix = tex2d.GetPixels();

            totalPixels = pix.Length;

            //color is RGBA, 0001 is black, 1111 is white

            mf.mesh = new Mesh();

            Vector3[] points = new Vector3[totalPixels];
            int[] indecies = new int[totalPixels];
            Color[] colors = new Color[totalPixels];

            for (int i = 0; i < totalPixels; ++i)
            {


                Color currentPixel = pix[i];

                if (currentPixel.r != 1)
                {
                    int currentColumn;

                    if (i < tex2d.width)
                    {
                        currentColumn = i + 1;
                    }

                    else
                    {
                        currentColumn = (i % tex2d.width) + 1;
                    }

                    float depthZ = currentPixel.r * depthMultiplier;


                    float alphaHorizontal = (180 - horizontalFOV) / 2;
                    float gammaHorizontal = alphaHorizontal + (currentColumn * horizontalFOV) / (tex2d.width);
                    float deltaX = (depthZ) / (Mathf.Tan(gammaHorizontal * Mathf.Deg2Rad));



                    float currentRow = Mathf.Ceil((i + 1) / tex2d.width);

                    float gammaVertical = (180 - verticalFOV) / 2 + (verticalFOV / tex2d.height) * currentRow;
                    float deltaY = (depthZ) / (Mathf.Tan(gammaVertical * Mathf.Deg2Rad));




                    points[i] = new Vector3(-deltaX, -deltaY, depthZ);
                    indecies[i] = i;
                    //colors[i] = new Color(1 - currentPixel.r, currentPixel.r, 0, 1);
                    colors[i] = Color.white;
                }



            }



            mf.mesh.vertices = points;
            mf.mesh.colors = colors;
            mf.mesh.SetIndices(indecies, MeshTopology.Points, 0);



            iteration++;

        }



        if (assumeRangeMap == false && SLAM == true)
        {

            verticalFOV = depthCamera.fieldOfView - 10;
            horizontalFOV = Camera.VerticalToHorizontalFieldOfView(verticalFOV, camAspect) - 10;

            tex2d = depthCamera.GetComponent<Render2D>().tex2d;
            pix = tex2d.GetPixels();

            totalPixels = pix.Length;

            //color is RGBA, 0001 is black, 1111 is white



            //Vector3[] points = new Vector3[totalPixels];
            //int[] indecies = new int[totalPixels];
            //Color[] colors = new Color[totalPixels];





            for (int i = 0; i < totalPixels; ++i)
            {


                Color currentPixel = pix[i];

                if (currentPixel.r != 1)
                {
                    int currentColumn;

                    if (i < tex2d.width)
                    {
                        currentColumn = i + 1;
                    }

                    else
                    {
                        currentColumn = (i % tex2d.width) + 1;
                    }

                    float depthZ = currentPixel.r * depthMultiplier;


                    float alphaHorizontal = (180 - horizontalFOV) / 2;
                    float gammaHorizontal = alphaHorizontal + (currentColumn * horizontalFOV) / (tex2d.width);
                    float deltaX = (depthZ) / (Mathf.Tan(gammaHorizontal * Mathf.Deg2Rad));



                    float currentRow = Mathf.Ceil((i + 1) / tex2d.width);

                    float gammaVertical = (180 - verticalFOV) / 2 + (verticalFOV / tex2d.height) * currentRow;
                    float deltaY = (depthZ) / (Mathf.Tan(gammaVertical * Mathf.Deg2Rad));

                    Vector3 relative = transform.InverseTransformPoint(depthCameraObject.transform.position);
                    Vector3 delta = relative - depthCamInitPos;

                    Vector3 rawPoint = new Vector3(-deltaX, -deltaY, depthZ);
                    Vector3 adjustedPoint = rawPoint + delta;



                    points.Add (adjustedPoint);
                    indecies.Add (cumIndex);
                    colors.Add(Color.white);

                    pointsCount = points.Count;
                    indeciesCount = indecies.Count;


                    cumIndex++;

                }



            }

            //string[] arrayOfStrings = listOfStrings.ToArray();

            mf.mesh.vertices = points.ToArray();
            mf.mesh.colors = colors.ToArray();
            mf.mesh.SetIndices(indecies.ToArray(), MeshTopology.Points, 0);

            iteration++;

            if (cumIndex > 65534)
            {
                points.Clear();
                colors.Clear();
                indecies.Clear();
                cumIndex = 0;
            }

        }




        if (assumeRangeMap == true && SLAM == false)
        {

            verticalFOV = depthCamera.fieldOfView;
            horizontalFOV = Camera.VerticalToHorizontalFieldOfView(verticalFOV, camAspect);

            tex2d = depthCamera.GetComponent<Render2D>().tex2d;
            pix = tex2d.GetPixels();

            totalPixels = pix.Length;

            //color is RGBA, 0001 is black, 1111 is white

            mf.mesh = new Mesh();

            Vector3[] points = new Vector3[totalPixels];
            int[] indecies = new int[totalPixels];
            Color[] colors = new Color[totalPixels];

            for (int i = 0; i < totalPixels; ++i)
            {
                Color currentPixel = pix[i];
                int currentColumn;

                if (i < tex2d.width)
                {
                    currentColumn = i + 1;
                }

                else
                {
                    currentColumn = (i % tex2d.width) + 1;
                }

                float depthZ = currentPixel.r * depthMultiplier;


                float alphaHorizontal = (180 - horizontalFOV) / 2;
                float gammaHorizontal = alphaHorizontal + (currentColumn * horizontalFOV) / (tex2d.width);
                float deltaX = (depthZ) * (Mathf.Cos(gammaHorizontal * Mathf.Deg2Rad));



                float currentRow = Mathf.Ceil((i + 1) / tex2d.width);

                float gammaVertical = (180 - verticalFOV) / 2 + (verticalFOV / tex2d.height) * currentRow;
                float deltaY = (depthZ) * (Mathf.Cos(gammaVertical * Mathf.Deg2Rad));

                float deltaZ = Mathf.Sqrt(Mathf.Pow(depthZ, 2f) - Mathf.Pow(deltaY, 2f));




                points[i] = new Vector3(-deltaX, -deltaY, deltaZ);
                indecies[i] = i;


                //colors[i] = new Color(1 - currentPixel.r, currentPixel.r, 0, 1);
                colors[i] = Color.white;


            }



            mf.mesh.vertices = points;
            mf.mesh.colors = colors;
            mf.mesh.SetIndices(indecies, MeshTopology.Points, 0);

            iteration++;


        }


    }


    void OnDrawGizmos()
    {
        var p1 = transform.position;
        var p2 = transform.position + transform.right*5;
        var thickness = 10;
        Handles.DrawBezier(p1, p2, p1, p2, Color.red, null, thickness);
        Handles.Label(transform.position + transform.right*6, "X");

        p1 = transform.position;
        p2 = transform.position + transform.up * 5;
        thickness = 10;
        Handles.DrawBezier(p1, p2, p1, p2, Color.green, null, thickness);
        Handles.Label(transform.position + transform.up *6, "Y");

        p1 = transform.position;
        p2 = transform.position + transform.forward * 5;
        thickness = 10;
        Handles.DrawBezier(p1, p2, p1, p2, Color.blue, null, thickness);
        Handles.Label(transform.position + transform.forward*6, "Z");



    }


}
