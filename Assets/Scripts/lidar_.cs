using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using UnityEngine;

//lidar in unity world spins clockwise when viewed top down

public class lidar_ : MonoBehaviour
{
    private float physicsUpdatePeriod; // 1/240 means physics updates 240 times a second or 240hz; this number is in SECONDS
    public float veloRPS; //lidar revolutions per second
    public float veloRotationIncrement; //lidar rotation amount in degrees per fixed update (physics update) frame
    public float veloRange; //lidar range; HDL-64 has up to 120m
    private float currentRevolution; //keep track of lidar rotation in terms of revolution; starts from intial with 0 revolution; half way revolution is 0.5 etc.
    private float currentGameTime; //keep track of gametime; time in seconds since the start of the unity game; time is synced to in game time
    private StringBuilder veloCSV; //space delimited csv storing all points collected in play session
    private string veloCSVName; //name of saved points csv
    private DirectoryInfo sessionDir; //session's directory
    private float revolutionTime; //how long it takes for the lidar to do one rev in SECS
    public float verticalAngularResolution;
    public int aboveHorizonChannels;
    public int belowHorizonChannels;
    private List<lidarChannel> lidarChannels = new List<lidarChannel>();
    public List<Camera> RGB_Cameras = new List<Camera>();
    public float rangeAccuracy; //accuracy +- in meters; hdl-64e has 2cm, so 0.02
    public Matrix4x4 lidarTRS;
    public Transform lidarBaseTransform;
    private List<RenderTexture> camRenderTextures = new List<RenderTexture>();
    public int currentRevolutionFloored;
    private int revolutionImageTaken;

    public int captureFrames; //user specified
    public int capturedFrames;

    private string velodyneDirName;

    public List<DirectoryInfo> imageDirectories = new List<DirectoryInfo>();

    public bool enable180Lidar;
    public string lidarName;

    private List<ImageSynthesis> cameraISList = new List<ImageSynthesis>();

    public bool toggleXVIZFormat = false;
    public bool toggleRGBDCameraCapture = false;

    private IDictionary<int, List<float[]>> perRevolutionVelodynePoints = new Dictionary<int, List<float[]>>(); // this dictionary will store the csv string builder for each revolution floored

    private TimeSpan startTime;
    private float startTimeMilliseconds;

    private List<StringBuilder> cameraTimestampsStringBuilder = new List<StringBuilder>();
    private List<StringBuilder> lidarTimestampsStringBuilder = new List<StringBuilder>();

    public IDictionary<int, List<object[]>> trackletDynamic = new Dictionary<int, List<object[]>>(); //store tracklets info per revolution
    //[tx,ty,tz,rx,ry,rz,2,0,0,1,0,0,1,-1]

    public IDictionary<int, object[]> trackletStatic = new Dictionary<int, object[]>(); //store tracklets static info ie every object will have one object[]
                                                                                        //[objecttype,h,w,l,firstframe]

    public IDictionary<int, object[]> perRevolutionTracklets = new Dictionary<int, object[]>(); //store tracklets unique per revolution to then be stored in trackletDynamic; this is cleared every new revolution

    private struct lidarChannel
    {
        public RaycastHit laserHit;
        public float horizonAngularOffset; //angular position from horizon degree = 0; so 0.4 degrees would be first lidar channel above horizon

        public Vector3 laserDirection()
        {
            if (horizonAngularOffset == 0)
            {
                return Vector3.forward;
            }
            else
            {
                float theta = Mathf.Abs(horizonAngularOffset);
                float y = Mathf.Tan(theta * Mathf.Deg2Rad);

                if (horizonAngularOffset < 0)
                {
                    y = -y;
                }

                Vector3 directionVector = Vector3.forward + new Vector3(0f, y, 0f);
                return Vector3.Normalize(directionVector);
            }
        }
    }

    public Matrix4x4 calculateLidarToCamMatrix(Camera RGBCam)
    {
        Matrix4x4 RGBCamTRS = Matrix4x4.TRS(RGBCam.transform.position, RGBCam.transform.rotation, RGBCam.transform.lossyScale);
        return RGBCamTRS.inverse * lidarTRS;
    }

    private void CamCapture(Camera Cam, DirectoryInfo imageDir)
    {
        RenderTexture currentRT = RenderTexture.active;
        RenderTexture.active = Cam.targetTexture;

        Cam.Render();

        Texture2D Image = new Texture2D(Cam.targetTexture.width, Cam.targetTexture.height);
        Image.ReadPixels(new Rect(0, 0, Cam.targetTexture.width, Cam.targetTexture.height), 0, 0);
        Image.Apply();
        RenderTexture.active = currentRT;

        var Bytes = Image.EncodeToJPG();
        Destroy(Image); //important to save on memory leaks!!!

        File.WriteAllBytes(Path.Combine(imageDir.FullName, Cam.gameObject.name + "_" + currentRevolutionFloored + ".jpg"), Bytes);
    }

    private string formatMatrixtoString(Matrix4x4 lidartoCamMatrix)
    {
        string lidarToCamMatrixString = string.Format("{0},{1},{2},{3}\n{4},{5},{6},{7}\n{8},{9},{10},{11}\n{12},{13},{14},{15}", lidartoCamMatrix[0].ToString(), lidartoCamMatrix[4].ToString(), lidartoCamMatrix[8].ToString(), lidartoCamMatrix[12].ToString(), lidartoCamMatrix[1].ToString(), lidartoCamMatrix[5].ToString(), lidartoCamMatrix[9].ToString(), lidartoCamMatrix[13].ToString(), lidartoCamMatrix[2].ToString(), lidartoCamMatrix[6].ToString(), lidartoCamMatrix[10].ToString(), lidartoCamMatrix[14].ToString(), lidartoCamMatrix[3].ToString(), lidartoCamMatrix[7].ToString(), lidartoCamMatrix[11].ToString(), lidartoCamMatrix[15].ToString()); //format matrix to csv bruh

        return lidarToCamMatrixString;
    }

    private string BoundingBoxPointsToString(Vector3[] pts3D, Transform localSpace)
    {
        string outputString = "";

        foreach (Vector3 pt in pts3D)
        {
            Vector3 pt_tran = localSpace.InverseTransformPoint(pt);

            outputString += pt_tran.x + ",";
            outputString += pt_tran.y + ",";
            outputString += pt_tran.z + ",";
        }

        return outputString;
    }

    private Vector3 relativeRotation(Transform parentObject, Transform targetObject)
    {
        Quaternion relative = Quaternion.Inverse(parentObject.rotation) * targetObject.rotation;
        return relative.eulerAngles;
    }

    private Vector3 relativePosition(Transform parentObject, Vector3 targetObject)
    {
        return parentObject.InverseTransformPoint(targetObject);
    }

    public void Start()
    {
        capturedFrames = 0;



        velodyneDirName = "velodyne_points";
        startTime = new TimeSpan(DateTime.Now.Hour, DateTime.Now.Minute, DateTime.Now.Second); //log time of data collection start
        startTimeMilliseconds = (float)startTime.TotalMilliseconds; //convert time to ms

        lidarName = transform.parent.name;
        lidarBaseTransform = transform.parent; //static base transform instead of spinning one.

        //Time.captureDeltaTime = 0.01f;
        Time.timeScale = 1f; //controls in game fictional time scale. can force physics engine to go slower or faster, independent of real life time
        physicsUpdatePeriod = 1f / 10000f; //240hz
        sessionDir = Directory.CreateDirectory(string.Format("Session_{0:yyyy-MM-dd_hh-mm-ss-tt}", DateTime.Now) + "_" + lidarName); //create session directory unique to start system time
        veloRotationIncrement = veloRPS * physicsUpdatePeriod * 360f; //convert revolution to full rotation degrees of 360. this is azimuth angular resolution

        Debug.Log(veloRotationIncrement);
        Time.fixedDeltaTime = physicsUpdatePeriod; //set fixedupdate period
        currentRevolution = 0; //initally revolution is 0
        currentGameTime = 0;

        currentRevolutionFloored = 0;
        revolutionImageTaken = -1;

        veloCSV = new StringBuilder(); //initialized csv line builder
        veloCSVName = "points.csv";

        revolutionTime = 1f / veloRPS;

        for (int i = 0; i < aboveHorizonChannels; i++)
        {
            lidarChannel newLidarChannel = new lidarChannel();
            newLidarChannel.horizonAngularOffset = (i + 1) * verticalAngularResolution;
            lidarChannels.Add(newLidarChannel);
        } //add above horizon channels

        for (int i = 0; i < belowHorizonChannels; i++)
        {
            lidarChannel newLidarChannel = new lidarChannel();
            newLidarChannel.horizonAngularOffset = (i + 1) * -verticalAngularResolution;
            lidarChannels.Add(newLidarChannel);
        } //add below horizon channels

        lidarChannel trueHorizonChannel = new lidarChannel();
        trueHorizonChannel.horizonAngularOffset = 0f;
        lidarChannels.Add(trueHorizonChannel); //finally add true horizon channel

        foreach (lidarChannel channel in lidarChannels)
        {
            Debug.Log(channel.horizonAngularOffset);
            Debug.Log(channel.laserDirection());
        }


        if (toggleXVIZFormat == false)
        {
            lidarTRS = Matrix4x4.TRS(lidarBaseTransform.position, lidarBaseTransform.rotation, lidarBaseTransform.lossyScale); //lidar transformation from model space to world space; TRSlidar * local lidar = world space lidar)

            System.IO.File.WriteAllText(Path.Combine(sessionDir.FullName, "lidarTRSMatrix.csv"), formatMatrixtoString(lidarTRS)); //save cam projection matrix
        }


        //Debug.Log(lidarTRS);

        foreach (Camera cam in RGB_Cameras)
        {
            DirectoryInfo imageDir = Directory.CreateDirectory(Path.Combine(sessionDir.Name, cam.gameObject.name));
            imageDirectories.Add(imageDir);

            DirectoryInfo imageDirData = Directory.CreateDirectory(Path.Combine(imageDir.FullName, "data"));

            RenderTexture rt = new RenderTexture(1920, 1080, 0);
            //camRenderTextures.Add(rt);
            cam.targetTexture = rt; //create render texture instance for each cam

            Matrix4x4 lidartoCamMatrix = calculateLidarToCamMatrix(cam);

            //System.IO.File.WriteAllText(Path.Combine(imageDir.FullName, "lidarToCamMatrix.csv"), formatMatrixtoString(lidartoCamMatrix)); //save lidartocam matrix

            Matrix4x4 camProjectionMatrix = cam.projectionMatrix;

            //System.IO.File.WriteAllText(Path.Combine(imageDir.FullName, "camProjectionMatrix.csv"), formatMatrixtoString(camProjectionMatrix)); //save cam projection matrix

            cameraISList.Add(cam.GetComponent<ImageSynthesis>()); //add camera IS component to list to take ground truths

            cameraTimestampsStringBuilder.Add(new StringBuilder()); //add stringbuilder for each camera for their timestamps
        } //create directory for each camera angle

        for (int i = 0; i < 3; i++)
        {
            lidarTimestampsStringBuilder.Add(new StringBuilder()); //add 3 stringbuilders to lidar's; this represents timestamps.txt | timestamps_start.txt | timestamps_end.txt
        }
    }

    public void Update()
    {
    }

    public void FixedUpdate()
    {
        float updatedGameTimeInMS = startTimeMilliseconds + currentGameTime * 1000f; //absolute game time for kitti in ms
        TimeSpan t = TimeSpan.FromMilliseconds(updatedGameTimeInMS);
        string kittiTimeStamp = string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}",
                                t.Hours,
                                t.Minutes,
                                t.Seconds,
                                t.Milliseconds.ToString().PadLeft(3, '0').PadRight(9, '0'));
        string kittiDateTime = DateTime.Now.ToString("yyyy-MM-dd");
        string frameTimeStamp = string.Format("{0} {1}", kittiDateTime, kittiTimeStamp); //keeps kitti format timestamp for current physics frame

        //lidarBaseTransform = transform.parent; //static base transform instead of spinning one.

        float currentRevolutionDecimals = (float)(currentRevolution - Math.Truncate(currentRevolution)); //get decimal part of current revolution

        if (!enable180Lidar || (currentRevolutionDecimals <= 0.25f || currentRevolutionDecimals >= 0.75f))
        {
            for (int i = 0; i < lidarChannels.Count; i++)
            {
                lidarChannel currentChannel = lidarChannels[i];

                if (Physics.Raycast(transform.position, transform.TransformDirection(currentChannel.laserDirection()), out currentChannel.laserHit, veloRange))
                {
                    string className = "NoLabel";
                    string uniqueID = "NoID";
                    Vector3[] pts3D = new Vector3[8];

                    string boundingBoxCoordsString = "0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,0,"; //zero padding column in csv

                    RootObject rootObject = currentChannel.laserHit.collider.gameObject.GetComponentInParent<RootObject>();

                    if (rootObject != null)
                    {
                        className = rootObject.className;
                        uniqueID = rootObject.uniqueID.ToString();
                        int uniqueID_int = rootObject.uniqueID;
                        pts3D = rootObject.BoundingBox_WorldCoord; //these points are still in world coords

                        if (trackletStatic.ContainsKey(uniqueID_int) == false)
                        {
                            trackletStatic.Add(new KeyValuePair<int, object[]>(uniqueID_int, new object[5]));

                            object[] tempArray = { className, rootObject.boundingBoxYSize, rootObject.boundingBoxXSize, rootObject.boundingBoxZSize, currentRevolutionFloored};  //[objecttype,h,w,l,firstframe]

                            trackletStatic[uniqueID_int] = tempArray;
                        }

                        if (perRevolutionTracklets.ContainsKey(uniqueID_int) == false)
                        {
                            perRevolutionTracklets.Add(new KeyValuePair<int, object[]>(uniqueID_int, new object[15])); //each revolution will contain unique ids of the first instancce of tracklets

                            Vector3 t_ = relativePosition(lidarBaseTransform, rootObject.boundingBoxCenterWorldSpace);
                            Vector3 r_ = relativeRotation(lidarBaseTransform, rootObject.transform);

                            float tx = t_.z;
                            float ty = -t_.x;
                            float tz = t_.y; //convert to kitti axis

                            tz = tz - rootObject.boundingBoxYSize / 2f;

                            /*

                            float rx = r_.z;
                            float ry = r_.x;
                            float rz = -r_.y; //convert to kitti axis


                            */



                            float rx = 0f;
                            float ry = 0f;

                            float rz = r_.y;

                            if (rz > 0 && rz <= 180f)
                            {
                                rz = -rz;
                            }

                            if (rz > 180f && rz < 360f)
                            {
                                rz = 360f - rz;
                            }

                            rz = rz * Mathf.Deg2Rad;

                            object[] tempArray = { tx, ty, tz, rx, ry, rz, 2, 0, 0, 1, 0, 0, 1, -1, -1, currentRevolutionFloored };
                            //[tx,ty,tz,rx,ry,rz,2,0,0,1,0,0,1,-1]

                            perRevolutionTracklets[uniqueID_int] = tempArray;
                        }

                        boundingBoxCoordsString = BoundingBoxPointsToString(pts3D, lidarBaseTransform); //this function also turns world coords to lidar base space

                        /*

                        6 - - - - - - - - - 2
                        | \                 | \
                        |   \               |   \
                        |     \             |     \
                        |       \           |       \
                        |         \         |         \
                        |           \       |           \
                        |             \     |             \
                        |               \   |               \
                        |                 \ |                 \
                        |                   7 - - - - - - - - - 3
                        |                   |                   |
                        4 - - - - - - - - - -0                   |
                          \                 | \                 |
                            \               |   \               |
                              \             |     \             |
                                \           |       \           |
                                  \         |         \         |
                                    \       |           \       |
                                      \     |             \     |
                                        \   |               \   |
                                          \ |                 \ |
                                            5 - - - - - - - - - 1
                                                                    \ forward direction
                                                                     V

                        */
                    }

                    Vector3 laserHitPoint = lidarBaseTransform.InverseTransformPoint(currentChannel.laserHit.point);
                    float distance = currentChannel.laserHit.distance;
                    float accuracyMargin = rangeAccuracy * (distance / veloRange); //further away means closer to accuracy max margin; farther away means more deviation from actual value
                    Vector3 randomAccuracyNoiseVector = new Vector3(UnityEngine.Random.Range(-accuracyMargin, accuracyMargin), UnityEngine.Random.Range(-accuracyMargin, accuracyMargin), UnityEngine.Random.Range(-accuracyMargin, accuracyMargin)); //generates noise vector in all three axis
                    laserHitPoint += randomAccuracyNoiseVector; //add noise to point

                    float x = laserHitPoint.z;
                    float y = -laserHitPoint.x;
                    float z = laserHitPoint.y; //convert to kitti axis

                    /*
                    float x = laserHitPoint.x;
                    float y = laserHitPoint.y;
                    float z = laserHitPoint.z; //no conversion

                    */

                    if (toggleXVIZFormat == false)
                    {
                        var newLine = string.Format("{0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", currentGameTime, currentRevolution, Mathf.Floor(currentRevolution), x, y, z, revolutionTime, currentChannel.horizonAngularOffset, className, uniqueID, boundingBoxCoordsString);
                        // store csv like: currentGameTime currentRevolution currentRevolutionFloored x y z revolutionTime lidarAngularoffset className uniqueID BoundingBoxCoord.ToString
                        veloCSV.AppendLine(newLine); //non xviz will store all points in one single csv
                    }
                    else
                    {
                        if (perRevolutionVelodynePoints.ContainsKey(currentRevolutionFloored) == false) //check if revolution is present in dictionary, if not add it first
                        {
                            perRevolutionVelodynePoints.Add(new KeyValuePair<int, List<float[]>>(currentRevolutionFloored, new List<float[]>())); //creates a new dictionary entry with new list of float arrays; each array stores 4 points: x y z reflectance
                        }

                        float[] tempArray = { x, y, z, 1f }; //creates temp array; reflectance by default is always 1
                        perRevolutionVelodynePoints[currentRevolutionFloored].Add(tempArray); //add float array
                    }

                    Debug.DrawLine(transform.position, currentChannel.laserHit.point, Color.green, physicsUpdatePeriod);

                    //Camera RGBCam = RGB_Cameras[0];
                    //Matrix4x4 RGBCamTRS = Matrix4x4.TRS(RGBCam.transform.position, RGBCam.transform.rotation, RGBCam.transform.lossyScale);

                    //Matrix4x4 LidartoCamMat = RGBCamTRS.inverse * lidarTRS;
                    //Vector4 localLidar = new Vector4(laserHitPoint.x, laserHitPoint.y, laserHitPoint.z, 1f);
                    //Vector4 localCam = LidartoCamMat.MultiplyPoint(localLidar);
                    //Debug.DrawLine(RGBCam.transform.position, RGBCamTRS.MultiplyPoint3x4(localCam), Color.yellow, physicsUpdatePeriod);

                    //Vector3 laserRelativeToCamera = lidarToCameraMatrix.MultiplyPoint3x4(laserHitPoint);
                    //Debug.DrawLine(RGBCam.transform.position, RGBCamTRS.MultiplyPoint3x4(localCam, Color.yellow, physicsUpdatePeriod);
                }
            }
        }

        transform.Rotate(0.0f, veloRotationIncrement, 0.0f, Space.Self); //rotate on local y axis by rotation increment
        currentRevolution += veloRotationIncrement / 360f; //updates revolution by adding rotation increment (degrees converted back to revolution)
        currentGameTime += physicsUpdatePeriod; //add physics period to game time;
        currentRevolutionFloored = (int)Mathf.Floor(currentRevolution);

        if (currentRevolutionFloored >= 0 && currentRevolutionFloored != revolutionImageTaken) //this will occur everytime a new revolution is reached
        {
            if (currentRevolutionFloored > 0) //only triggered once the first lidar spin is passed ie once the first .bin file is logged
            {
                foreach (KeyValuePair<int, object[]> revolution in perRevolutionTracklets) //iterate through per revolution tracklets
                {
                    if (trackletDynamic.ContainsKey(revolution.Key) == false)
                    {
                        trackletDynamic.Add(new KeyValuePair<int, List<object[]>>(revolution.Key, new List<object[]>()));
                    }

                    trackletDynamic[revolution.Key].Add(revolution.Value); //add dynamic array to list of array
                }

                perRevolutionTracklets.Clear(); //clear dict for next revolution
            }

            if (toggleRGBDCameraCapture == true)
            {
                for (int i = 0; i < RGB_Cameras.Count; i++)
                {
                    cameraISList[i].Save(currentRevolutionFloored.ToString().PadLeft(10, '0'), RGB_Cameras[i].pixelWidth, RGB_Cameras[i].pixelHeight, imageDirectories[i].FullName + "/data" ); //save ground truths for every camera when the lidar completes one full spin ; also captures on the start of the collection as 0 index
                }

                capturedFrames++; //just captured frame

                foreach (StringBuilder sb in cameraTimestampsStringBuilder)
                {
                    sb.AppendLine(frameTimeStamp); //add frame time stamp to each camera string builder
                }

                lidarTimestampsStringBuilder[0].AppendLine(frameTimeStamp); //add timestamp to timestamps.txt of lidar; this is when lidar "triggers" camera capture; this in kitti paper represents time when laser scanner is facing forward; incidentally, in unity this is also the beginning and end of the spin...

                lidarTimestampsStringBuilder[1].AppendLine(frameTimeStamp); //add timestamp to timestamps_start.txt

                if (currentRevolutionFloored != 0)
                {
                    lidarTimestampsStringBuilder[2].AppendLine(frameTimeStamp); //add timestamp to timestamps_end.txt after the 1st revolution is complete
                }

                if (capturedFrames == captureFrames)
                {
                    float lastLidarEndTimeStamp = updatedGameTimeInMS + revolutionTime * 1000f; //this will be the end time for the last lidar spin
                    TimeSpan t_ = TimeSpan.FromMilliseconds(lastLidarEndTimeStamp);
                    string kittiTimeStamp_ = string.Format("{0:D2}:{1:D2}:{2:D2}.{3:D3}",
                                            t_.Hours,
                                            t_.Minutes,
                                            t_.Seconds,
                                            t_.Milliseconds.ToString().PadLeft(3, '0').PadRight(9, '0'));
                    string frameTimeStamp_ = string.Format("{0} {1}", kittiDateTime, kittiTimeStamp_); //keeps kitti format timestamp for current physics frame
                    lidarTimestampsStringBuilder[2].AppendLine(frameTimeStamp_); //append the last end timestamp

                    UnityEditor.EditorApplication.isPlaying = false; //quit sim after enough captured frames
                }
            }

            /*
            for (int i = 0; i < RGB_Cameras.Count; i++)
            {
                CamCapture(RGB_Cameras[i], imageDirectories[i]);
            } //capture all cameras on a new revolution once

            */

            //lidarTRS = Matrix4x4.TRS(lidarBaseTransform.position, lidarBaseTransform.rotation, lidarBaseTransform.lossyScale); //lidar transformation from model space to world space; TRSlidar * local lidar = world space lidar)

            //System.IO.File.WriteAllText(Path.Combine(sessionDir.FullName, currentRevolutionFloored + "lidarTRSMatrix.csv"), formatMatrixtoString(lidarTRS)); //save cam projection matrix

            revolutionImageTaken = currentRevolutionFloored;
        }
    }

    /*
    public void Capture(Camera cam, DirectoryInfo dataset)
    {
        RenderTexture activeRenderTexture = RenderTexture.active;
        RenderTexture.active = cam.targetTexture;

        cam.Render();

        Texture2D image = new Texture2D(cam.targetTexture.width, cam.targetTexture.height);
        image.ReadPixels(new Rect(0, 0, cam.targetTexture.width, cam.targetTexture.height), 0, 0);
        image.Apply();
        RenderTexture.active = activeRenderTexture;

        byte[] bytes = image.EncodeToJPG();
        Destroy(image);

        File.WriteAllBytes(Path.Combine(dataset.ToString(), "sonia" + i.ToString().PadLeft(5, '0') + ".jpg"), bytes);
    }

    */

    public void OnApplicationQuit() //this is where all stringbuilders and files are saved
    {
        
        
        if (toggleXVIZFormat == false)
        {

            File.WriteAllText(Path.Combine(sessionDir.ToString(), veloCSVName), veloCSV.ToString()); //write points stringbuilder to one single csv name in session directory
        }
        
        

        DirectoryInfo velodyne_binaries_dir;

        velodyne_binaries_dir = Directory.CreateDirectory(Path.Combine(sessionDir.Name, velodyneDirName + "/data"));

        foreach (KeyValuePair<int, List<float[]>> revolution in perRevolutionVelodynePoints) //iterate through dictionary of points and save each list's arrays as binary
        {
            string fileName = Path.Combine(sessionDir.ToString(), velodyne_binaries_dir.FullName + "/" + revolution.Key.ToString().PadLeft(10, '0') + ".bin"); //total width is 10 to match kitti standard

            using (BinaryWriter binWriter = new BinaryWriter(File.Open(fileName, FileMode.Create)))
            {
                foreach (float[] floatArray in revolution.Value) //iterate through arrays of list
                {
                    foreach (float arrayValue in floatArray)
                    {
                        binWriter.Write(arrayValue); //iterate through each array value and write to binary
                    }
                }
            }
        }

        foreach (KeyValuePair<int, object[]> revolution in trackletStatic) //iterate through dictionary of points and save each list's arrays as binary
        {
            Debug.Log(revolution.Key);

            foreach (object item in revolution.Value)
            {
                Debug.Log(item.ToString());
            }
        }

        File.WriteAllText(Path.Combine(sessionDir.ToString(), velodyneDirName + "/timestamps.txt"), lidarTimestampsStringBuilder[0].ToString());
        File.WriteAllText(Path.Combine(sessionDir.ToString(), velodyneDirName + "/timestamps_start.txt"), lidarTimestampsStringBuilder[1].ToString());
        File.WriteAllText(Path.Combine(sessionDir.ToString(), velodyneDirName + "/timestamps_end.txt"), lidarTimestampsStringBuilder[2].ToString()); //save timestamps to velodyne directory

        for (int i = 0; i < cameraTimestampsStringBuilder.Count; i++)
        {
            File.WriteAllText(Path.Combine(imageDirectories[i].FullName, "timestamps.txt"), cameraTimestampsStringBuilder[i].ToString());
        } //save all camera string builders that output timestamps.txt to their respective directories




#region write xml
        //write xml file tracket_labels.xml

        XmlWriterSettings settings = new XmlWriterSettings();
        settings.Encoding = Encoding.UTF8;
        settings.Indent = true;
        XmlWriter xmlWriter = XmlWriter.Create(Path.Combine(sessionDir.FullName, "tracklet_labels.xml"), settings);
        xmlWriter.WriteStartDocument(true); xmlWriter.WriteDocType("boost_serialization", null, null, null);




        xmlWriter.WriteStartElement("boost_serialization"); xmlWriter.WriteAttributeString("signature", "serialization::archive"); xmlWriter.WriteAttributeString("version", "9");

        xmlWriter.WriteStartElement("tracklets"); xmlWriter.WriteAttributeString("class_id", "0"); xmlWriter.WriteAttributeString("tracking_level", "0"); xmlWriter.WriteAttributeString("version", "0");

        xmlWriter.WriteStartElement("count");
        xmlWriter.WriteString(trackletStatic.Count.ToString());
        xmlWriter.WriteEndElement();//end count

        xmlWriter.WriteStartElement("item_version");
        xmlWriter.WriteString("1");
        xmlWriter.WriteEndElement();//end item_version



        foreach (KeyValuePair<int, object[]> revolution in trackletStatic)
        {
            writeXMLTrackletsPoses(xmlWriter, revolution.Key);
        }




        xmlWriter.WriteEndElement();//end tracklet node



        xmlWriter.WriteEndElement();//end boost_serialization



        xmlWriter.WriteEndDocument();
        xmlWriter.Close();

#endregion





        Debug.Log("Time of session: " + Time.realtimeSinceStartup);
    }


    void writeXMLTrackletsPoses(XmlWriter xmlWriter, int key)
    {
        xmlWriter.WriteStartElement("item"); xmlWriter.WriteAttributeString("class_id", "1"); xmlWriter.WriteAttributeString("tracking_level", "0"); xmlWriter.WriteAttributeString("version", "1");


        xmlWriter.WriteStartElement("objectType");
        xmlWriter.WriteString(trackletStatic[key][0].ToString());
        xmlWriter.WriteEndElement();//end object_type

        xmlWriter.WriteStartElement("h");
        xmlWriter.WriteString(trackletStatic[key][1].ToString());
        xmlWriter.WriteEndElement();//end h

        xmlWriter.WriteStartElement("w");
        xmlWriter.WriteString(trackletStatic[key][2].ToString());
        xmlWriter.WriteEndElement();//end w

        xmlWriter.WriteStartElement("l");
        xmlWriter.WriteString(trackletStatic[key][3].ToString());
        xmlWriter.WriteEndElement();//end l

        xmlWriter.WriteStartElement("first_frame");
        xmlWriter.WriteString(trackletStatic[key][4].ToString());
        xmlWriter.WriteEndElement();//end first_frame




        xmlWriter.WriteStartElement("poses"); xmlWriter.WriteAttributeString("class_id", "2"); xmlWriter.WriteAttributeString("tracking_level", "0"); xmlWriter.WriteAttributeString("version", "0");


        xmlWriter.WriteStartElement("count");
        xmlWriter.WriteString(trackletDynamic[key].Count.ToString());
        xmlWriter.WriteEndElement();//end count

        xmlWriter.WriteStartElement("item_version");
        xmlWriter.WriteString("2");
        xmlWriter.WriteEndElement();//end item_version



        foreach (object[] item in trackletDynamic[key])
        {

            xmlWriter.WriteStartElement("item"); xmlWriter.WriteAttributeString("class_id", "3"); xmlWriter.WriteAttributeString("tracking_level", "0"); xmlWriter.WriteAttributeString("version", "2");


            xmlWriter.WriteStartElement("tx");
            xmlWriter.WriteString(item[0].ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("ty");
            xmlWriter.WriteString(item[1].ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("tz");
            xmlWriter.WriteString(item[2].ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("rx");
            xmlWriter.WriteString(item[3].ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("ry");
            xmlWriter.WriteString(item[4].ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("rz");
            xmlWriter.WriteString(item[5].ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("state");
            xmlWriter.WriteString(item[6].ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("occlusion");
            xmlWriter.WriteString(item[7].ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("occlusion_kf");
            xmlWriter.WriteString(item[8].ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("truncation");
            xmlWriter.WriteString(item[9].ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("amt_occlusion");
            xmlWriter.WriteString(item[10].ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("amt_occlusion_kf");
            xmlWriter.WriteString(item[11].ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("amt_border_1");
            xmlWriter.WriteString(item[12].ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("amt_border_r");
            xmlWriter.WriteString(item[13].ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteStartElement("amt_border_kf");
            xmlWriter.WriteString(item[14].ToString());
            xmlWriter.WriteEndElement();

            xmlWriter.WriteEndElement();//end item_version
        }




        xmlWriter.WriteEndElement();//end poses






        xmlWriter.WriteStartElement("finished");
        xmlWriter.WriteString("1");
        xmlWriter.WriteEndElement();//end finished








        xmlWriter.WriteEndElement(); //end item
    }
}