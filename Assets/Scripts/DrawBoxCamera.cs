using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class DrawBoxCamera : MonoBehaviour
{
    public Camera cam;
    public RenderTexture camRenderTexture;
    private DirectoryInfo dataset;

    /*
    public IDictionary<string, int> classes = new Dictionary<string, int>()
                                            {
                                                {"Person", 0},
                                                {"Bicycle", 1},
                                                {"Car", 2},
                                                {"Motorbike", 3},
                                                {"Bus", 5},
                                                {"Truck", 7},
                                                {"Traffic Light", 9},
                                                {"Fire Hydrant", 10},
                                                {"Stop Sign", 11},
                                                {"Bench", 13},
                                                {"Umbrella", 25},
                                                {"Potted Plant", 58},
                                                {"Traffic Sign", 59}
                                            }; //MSCOCO classes
    */

    /*
    public IDictionary<string, int> classes = new Dictionary<string, int>()
                                            {
                                                {"Person", 0},
                                                {"Bicycle", 1},
                                                {"Car", 2},
                                                {"Motorbike", 3},
                                                {"Bus", 4},
                                                {"Truck", 5},
                                                {"Traffic Light", 6},
                                                {"Fire Hydrant", 7},
                                                {"Bench", 8},
                                                {"Umbrella", 9},
                                                {"Potted Plant", 10},
                                                {"Traffic Sign", 11},
                                                {"Train", 12}
                                            }; //custom for BDD

    */

    public IDictionary<string, int> classes = new Dictionary<string, int>()
                                            {
                                                {"Person", 0},
                                                {"Car", 1},
                                            }; //custom for BDD

    public float labellingPad = 0.1f; //padding around screen space to threshold if label will be YOLO valid.
    [SerializeField] private int frameCount;
    public LayerMask layerMask;
    public Collider[] hitColliders;
    public float radius = 20f;
    public bool toggleCapture = true;

    private string startTime;
    private string endTime;

    private string nameTime;

    private ImageSynthesis IS;

    private int cam_width;
    private int cam_height;

    int ISCaptureFrameCount = 0;

    private void Start()
    {
        IS = GetComponent<ImageSynthesis>();

        cam = this.GetComponent<Camera>();

        dataset = Directory.CreateDirectory(string.Format("DataSet_{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now));

        cam_width = cam.pixelWidth;
        cam_height = cam.pixelHeight;

        Debug.Log("created!");

        using (StreamWriter sw = File.CreateText(dataset.FullName + "/classes.txt"))
        {
            int i = 0;

            foreach (KeyValuePair<string, int> item in classes)
            {
                while (item.Value != i)
                {
                    i++;
                    sw.WriteLine("");
                }

                sw.WriteLine(item.Key);
                i++;
            }
        }

        using (StreamWriter sw = File.CreateText(dataset.FullName + "/obj.data"))
        {
            sw.WriteLine("classes = 13");
            sw.WriteLine("train = data/" + dataset.Name + "/train.txt");
            sw.WriteLine("valid = data/" + dataset.Name + "/test.txt");
            sw.WriteLine("names = data/bdd.names");
        }

        startTime = "start: " + DateTime.Now.ToString("h:mm:ss tt");

        nameTime = DateTime.Now.ToString("yyyy-MM-dd_hh-mm-ss-tt");






        Time.fixedDeltaTime = 1f / 30f;
    }

    public void Update()
    {

        /*
        if (Time.frameCount % 3 == 0)
        {
            IS.OnSceneChange();
            CaptureImageSynthesis();
        }
        */

        /*

        if (Time.frameCount % 10 == 0)
        {
            CaptureImageSynthesis();
        }

        */

                CaptureImageSynthesis();

        ISCaptureFrameCount++;

        Debug.Log("Captured Frames: " + ISCaptureFrameCount);
        
    }




    public void OnApplicationQuit()
    {
        endTime = "end: " + DateTime.Now.ToString("h:mm:ss tt");

        using (StreamWriter sw = File.CreateText(dataset.FullName + "/time.txt"))
        {
            sw.WriteLine(startTime);
            sw.WriteLine(endTime);
        }

        IS.SaveDictionaryAsText(dataset.FullName + "/dictionary.txt");
    }

    private void CaptureImageSynthesis()
    {
        IS.Save(cam.name + Time.frameCount.ToString().PadLeft(5,'0'), cam_width, cam_height, dataset.FullName);
    }

    public void CaptureFrame()
    {
        if (toggleCapture == false)
        {
            return;
        }

        List<Collider> frontalHitColliders = new List<Collider>();
        List<BoxCollider> frontalBoxColliders = new List<BoxCollider>();

        hitColliders = Physics.OverlapSphere(transform.position, radius, layerMask, QueryTriggerInteraction.Collide);

        foreach (Collider collider in hitColliders)
        {
            Vector3 screenPoint = GetComponent<Camera>().WorldToViewportPoint(collider.gameObject.transform.position);

            bool onScreen = screenPoint.z > -1f && screenPoint.x > 0f && screenPoint.x < 1f && screenPoint.y > 0f && screenPoint.y < 1f;

            if (onScreen == true)
            {
                frontalHitColliders.Add(collider);
            }
        }

        foreach (Collider collider in frontalHitColliders)
        {
            if (collider.GetType() == typeof(BoxCollider))
            {
                frontalBoxColliders.Add((BoxCollider)collider);
            }
        }

        using (StreamWriter sw = File.CreateText(dataset.FullName + "/" + Time.frameCount + nameTime + ".txt"))
        {
            foreach (BoxCollider col in frontalBoxColliders)
            {
                Vector3[] pts3D = new Vector3[8];

                var trans = col.transform;
                var min_ = col.center - col.size * 0.5f;
                var max_ = col.center + col.size * 0.5f;

                Bounds b = col.bounds;

                pts3D[0] = trans.TransformPoint(new Vector3(min_.x, min_.y, min_.z));
                pts3D[1] = trans.TransformPoint(new Vector3(min_.x, min_.y, max_.z));
                pts3D[2] = trans.TransformPoint(new Vector3(min_.x, max_.y, min_.z));
                pts3D[3] = trans.TransformPoint(new Vector3(min_.x, max_.y, max_.z));
                pts3D[4] = trans.TransformPoint(new Vector3(max_.x, min_.y, min_.z));
                pts3D[5] = trans.TransformPoint(new Vector3(max_.x, min_.y, max_.z));
                pts3D[6] = trans.TransformPoint(new Vector3(max_.x, max_.y, min_.z));
                pts3D[7] = trans.TransformPoint(new Vector3(max_.x, max_.y, max_.z));

                bool colliderRayCastSuccessful = false;

                foreach (Vector3 corner in pts3D)
                {
                    RaycastHit hit;

                    if (Physics.Raycast(transform.position, corner - transform.position, out hit, Mathf.Infinity, Physics.DefaultRaycastLayers, QueryTriggerInteraction.Collide))
                    {
                        if (hit.collider == col)
                        {
                            colliderRayCastSuccessful = true;
                        }
                    }
                }

                if (colliderRayCastSuccessful == false)
                {
                    continue;
                }

                for (int i = 0; i < pts3D.Length; i++)
                {
                    pts3D[i] = cam.WorldToScreenPoint(pts3D[i]);
                }

                /*

                for (int i = 0; i < 8; i++)
                {
                    pts3D[i].y = Screen.height - pts3D[i].y; //inverse y coordinates so y axis goes downwards; x goes rightwards
                }

                */

                Vector3 min = pts3D[0];
                Vector3 max = pts3D[0];

                for (int i = 1; i < pts3D.Length; i++)
                {
                    min = Vector3.Min(min, pts3D[i]);
                    max = Vector3.Max(max, pts3D[i]);
                }

                min.y = Screen.height - min.y;
                max.y = Screen.height - max.y;

                //Construct a rect of the min and max positions and apply some margin

                Rect r = Rect.MinMaxRect(min.x, min.y, max.x, max.y);

                float XCoord = (r.xMin + r.xMax) / 2;
                float YCoord = (r.yMin + r.yMax) / 2; // x and y center in screen space

                float height = Mathf.Abs(r.yMin - r.yMax);
                float width = Mathf.Abs(r.xMin - r.xMax); //height and width of box in screen space

                if (height < 12 || width < 12)
                {
                    continue;
                }

                XCoord = XCoord / Screen.width;
                YCoord = YCoord / Screen.height;
                height = height / Screen.height;
                width = width / Screen.width; //normalize ground truth to screen dimensions for YOLOv3 labelling format

                if (classes.ContainsKey(col.gameObject.tag) && XCoord > labellingPad / Screen.width && XCoord < 1 - labellingPad / Screen.width && YCoord > labellingPad / Screen.height && YCoord < 1 - labellingPad / Screen.height)
                {
                    sw.WriteLine(String.Format("{0} {1} {2} {3} {4}", classes[col.gameObject.tag], XCoord, YCoord, width, height));
                } //checks if center of box is within specified padded area. prevents bad labels (negative centers)
            }

            SaveTexture();
        }
    }

    /*

    frameCount = Time.frameCount;

    if (threeDimensionalBox == true)
    {
        boxPointsList.Clear();

        foreach (Transform child in transform.GetChild(0))
        {
            GameObject.Destroy(child.gameObject);
        }

        //frontalColliders = gameObject.GetComponent<CameraTriggerBoundingBox>().frontalHitColliders;
        rowDataTemp = new List<string>();
        rowDataTemp.Add(Time.frameCount.ToString());

        foreach (Collider collider in frontalColliders)
        {
            GameObject panel = Instantiate(boundingBoxPrefab) as GameObject;
            panel.transform.SetParent(transform.GetChild(0), false);
            panel.GetComponent<RectTransform>().localScale = new Vector3(1, 1, 1);
            panel.GetComponent<RectTransform>().localRotation = Quaternion.identity;

            b = collider.bounds;

            //All 8 vertices of the bounds
            pts[0] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
            pts[1] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
            pts[2] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
            pts[3] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));
            pts[4] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
            pts[5] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
            pts[6] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
            pts[7] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));

            //Get them in GUI space
            for (int i = 0; i < pts.Length; i++) pts[i].y = Screen.height - pts[i].y;

            //Calculate the min and max positions
            Vector3 min = pts[0];
            Vector3 max = pts[0];
            for (int i = 1; i < pts.Length; i++)
            {
                min = Vector3.Min(min, pts[i]);
                max = Vector3.Max(max, pts[i]);
            }

            //Construct a rect of the min and max positions and apply some margin
            Rect r = Rect.MinMaxRect(min.x, min.y, max.x, max.y);
            r.xMin -= margin;
            r.xMax += margin;
            r.yMin -= margin;
            r.yMax += margin;

            float XCoord = (r.xMin + r.xMax) / 2;
            float YCoord = -(r.yMin + r.yMax) / 2;
            Vector2 ScreenPos = new Vector2(XCoord, YCoord);

            float height = Mathf.Abs(r.yMin - r.yMax);
            float width = Mathf.Abs(r.xMin - r.xMax);

    panel.GetComponent<RectTransform>().anchoredPosition = ScreenPos;
    panel.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

    Vector3 localSpace = transform.InverseTransformPoint(collider.gameObject.transform.position);

    panel.transform.GetChild(0).gameObject.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, 40);
    panel.transform.GetChild(0).gameObject.GetComponent<Text>().fontSize = 15;
    panel.transform.GetChild(0).gameObject.GetComponent<Text>().text = "ID: " + collider.gameObject.name + "\nVelocity: " + collider.gameObject.GetComponent<Rigidbody>().velocity + "\nWorld position: " + collider.gameObject.transform.position + "\nLocal position: " + localSpace;

    Vector3[] pts3D = new Vector3[8];

            BoxCollider col = (BoxCollider)collider;

            rowDataTemp.Add((Mathf.Abs(col.gameObject.GetInstanceID())).ToString()); //add ID

            rowDataTemp.Add(col.gameObject.transform.position.x.ToString());
            rowDataTemp.Add(col.gameObject.transform.position.y.ToString());
            rowDataTemp.Add(col.gameObject.transform.position.z.ToString()); //add absolute position

            rowDataTemp.Add(col.gameObject.GetComponent<Rigidbody>().velocity.x.ToString());
            rowDataTemp.Add(col.gameObject.GetComponent<Rigidbody>().velocity.y.ToString());
            rowDataTemp.Add(col.gameObject.GetComponent<Rigidbody>().velocity.z.ToString()); //add velocity vectors

            var trans = col.transform;
            var min_ = col.center - col.size * 0.5f;
            var max_ = col.center + col.size * 0.5f;
            pts3D[0] = trans.TransformPoint(new Vector3(min_.x, min_.y, min_.z));
            pts3D[1] = trans.TransformPoint(new Vector3(min_.x, min_.y, max_.z));
            pts3D[2] = trans.TransformPoint(new Vector3(min_.x, max_.y, min_.z));
            pts3D[3] = trans.TransformPoint(new Vector3(min_.x, max_.y, max_.z));
            pts3D[4] = trans.TransformPoint(new Vector3(max_.x, min_.y, min_.z));
            pts3D[5] = trans.TransformPoint(new Vector3(max_.x, min_.y, max_.z));
            pts3D[6] = trans.TransformPoint(new Vector3(max_.x, max_.y, min_.z));
            pts3D[7] = trans.TransformPoint(new Vector3(max_.x, max_.y, max_.z));

            for (int i = 0; i < 8; i++)
            {
                Vector2 screenCood = cam.WorldToScreenPoint(pts3D[i]);
                screenCood.y = Screen.height - screenCood.y;
                rowDataTemp.Add(screenCood.x.ToString());
                rowDataTemp.Add(screenCood.y.ToString());
            }

            //boxPointsList.Add(pts3D);
        }

        rowData.Add(rowDataTemp.ToArray());

        foreach (Vector3[] boxPoints in boxPointsList)
        {
            foreach (Vector3 pt in boxPoints)
            {
                GameObject point = Instantiate(dotPrefab);
                point.transform.SetParent(transform.GetChild(0), false);
                point.GetComponent<RectTransform>().localScale = new Vector3(0.1f, 0.1f, 0.1f);
                point.GetComponent<RectTransform>().localRotation = Quaternion.identity;
                point.GetComponent<RectTransform>().localPosition = Vector3.zero;
                Vector2 screenCood = cam.WorldToScreenPoint(pt);
                screenCood.y = Screen.height - screenCood.y;
                screenCood.y = -screenCood.y;
                point.GetComponent<RectTransform>().anchoredPosition = screenCood;
            }
        }
    }
    else
    {
        frontalColliders = frontalHitColliders; //get colliders in view

        using (StreamWriter sw = File.CreateText(dataset.FullName + "/" + (Time.frameCount) + ".txt")){}
        {
            foreach (Collider collider in frontalColliders)
            {
                //b = collider.bounds;

                boxPointsList.Clear();

                Vector3[] pts3D = new Vector3[8];

                BoxCollider col = collider;

                var trans = col.transform;
                var min_ = col.center - col.size * 0.5f;
                var max_ = col.center + col.size * 0.5f;
                pts3D[0] = trans.TransformPoint(new Vector3(min_.x, min_.y, min_.z));
                pts3D[1] = trans.TransformPoint(new Vector3(min_.x, min_.y, max_.z));
                pts3D[2] = trans.TransformPoint(new Vector3(min_.x, max_.y, min_.z));
                pts3D[3] = trans.TransformPoint(new Vector3(min_.x, max_.y, max_.z));
                pts3D[4] = trans.TransformPoint(new Vector3(max_.x, min_.y, min_.z));
                pts3D[5] = trans.TransformPoint(new Vector3(max_.x, min_.y, max_.z));
                pts3D[6] = trans.TransformPoint(new Vector3(max_.x, max_.y, min_.z));
                pts3D[7] = trans.TransformPoint(new Vector3(max_.x, max_.y, max_.z));

                for (int i = 0; i < 8; i++)
                {
                    pts3D[i].y = Screen.height - pts3D[i].y;
                }

                boxPointsList.Add(pts3D);

                Vector3 min = pts3D[0];
                Vector3 max = pts3D[0];

                for (int i = 1; i < pts.Length; i++)
                {
                    min = Vector3.Min(min, pts[i]);
                    max = Vector3.Max(max, pts[i]);
                }

                Vector3[] pts = new Vector3[8];

                //All 8 vertices of the bounds
                pts[0] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
                pts[1] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
                pts[2] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
                pts[3] = cam.WorldToScreenPoint(new Vector3(b.center.x + b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));
                pts[4] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z + b.extents.z));
                pts[5] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y + b.extents.y, b.center.z - b.extents.z));
                pts[6] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z + b.extents.z));
                pts[7] = cam.WorldToScreenPoint(new Vector3(b.center.x - b.extents.x, b.center.y - b.extents.y, b.center.z - b.extents.z));

                //Get them in GUI space
                for (int i = 0; i < pts.Length; i++) pts[i].y = Screen.height - pts[i].y;

                //Calculate the min and max positions
                Vector3 min = pts[0];
                Vector3 max = pts[0];
                for (int i = 1; i < pts.Length; i++)
                {
                    min = Vector3.Min(min, pts[i]);
                    max = Vector3.Max(max, pts[i]);
                }

                //Construct a rect of the min and max positions and apply some margin

                Rect r = Rect.MinMaxRect(min.x, min.y, max.x, max.y);

                float XCoord = (r.xMin + r.xMax) / 2;
                float YCoord = (r.yMin + r.yMax) / 2; // x and y center in screen space

                float height = Mathf.Abs(r.yMin - r.yMax);
                float width = Mathf.Abs(r.xMin - r.xMax); //height and width of box in screen space

                XCoord = XCoord / Screen.width;
                YCoord = YCoord / Screen.height;
                height = height / Screen.height;
                width = width / Screen.width; //normalize ground truth to screen dimensions for YOLOv3 labelling format

                if (XCoord > labellingPad/Screen.width && XCoord < 1 - labellingPad/Screen.width && YCoord > labellingPad/Screen.height && YCoord < 1 - labellingPad / Screen.height)
                {
                    sw.WriteLine(String.Format("{0} {1} {2} {3} {4}", classes[collider.gameObject.tag], XCoord, YCoord, width, height));
                } //checks if center of box is within specified padded area. prevents bad labels (negative centers)

;
            }

            if (Time.frameCount > maxFrames)
            {
                EditorApplication.isPlaying = false;
            }
        }
    }

    */

    public void SaveTexture()
    {
        Texture2D myTex = toTexture2D(camRenderTexture);
        byte[] bytes = myTex.EncodeToJPG();
        System.IO.File.WriteAllBytes(dataset.FullName + "/" + Time.frameCount + nameTime + ".JPG", bytes);
        Destroy(myTex); // important for preventing memory leak
    }

    private Texture2D toTexture2D(RenderTexture rTex)
    {
        Texture2D tex = new Texture2D(camRenderTexture.width, camRenderTexture.height, TextureFormat.RGB24, false);
        RenderTexture.active = rTex;
        tex.ReadPixels(new Rect(0, 0, rTex.width, rTex.height), 0, 0);
        tex.Apply();
        UnityEngine.Object.Destroy(tex);
        return tex;
    }

    private string GetRelativePath(string filespec, string folder)
    {
        Uri pathUri = new Uri(filespec);
        // Folders must end in a slash
        if (!folder.EndsWith(Path.DirectorySeparatorChar.ToString()))
        {
            folder += Path.DirectorySeparatorChar;
        }
        Uri folderUri = new Uri(folder);
        return Uri.UnescapeDataString(folderUri.MakeRelativeUri(pathUri).ToString().Replace('/', Path.DirectorySeparatorChar));
    }
}