using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficController : MonoBehaviour
{


    public GameObject northCarBlock;
    public GameObject southCarBlock;
    public GameObject westCarBlock;
    public GameObject eastCarBlock;
    public GameObject EW_PedBlock1;
    public GameObject EW_PedBlock2;
    public GameObject NS_PedBlock1;
    public GameObject NS_PedBlock2;

    public GameObject N_TrafficLight;
    public GameObject S_TrafficLight;

    public GameObject N_Green;
    public GameObject N_Yellow;
    public GameObject N_Red;
    public GameObject N_Go_Ped;
    public GameObject N_Stop_Ped;


    public GameObject S_Green;
    public GameObject S_Yellow;
    public GameObject S_Red;
    public GameObject S_Go_Ped;
    public GameObject S_Stop_Ped;

    public float greenLightPeriod = 20f;
    public float yellowLightPeriod = 5f;



    public float TimeCurrent;
    // Start is called before the first frame update
    void Start()
    {
        TimeCurrent = 0f;

        for (int i = 0; i < N_TrafficLight.transform.childCount; i++)
        {
            if (N_TrafficLight.transform.GetChild(i).gameObject.name == "Red_Lights")
            {
                N_Red = N_TrafficLight.transform.GetChild(i).gameObject;
            }

            if (N_TrafficLight.transform.GetChild(i).gameObject.name == "Yellow_Lights")
            {
                N_Yellow = N_TrafficLight.transform.GetChild(i).gameObject;
            }

            if (N_TrafficLight.transform.GetChild(i).gameObject.name == "Green_Lights")
            {
                N_Green = N_TrafficLight.transform.GetChild(i).gameObject;
            }

            if (N_TrafficLight.transform.GetChild(i).gameObject.name == "Go_PedXing_Lights")
            {
                N_Go_Ped = N_TrafficLight.transform.GetChild(i).gameObject;
            }

            if (N_TrafficLight.transform.GetChild(i).gameObject.name == "Stop_PedXing_Lights")
            {
                N_Stop_Ped = N_TrafficLight.transform.GetChild(i).gameObject;
            }
        }

        for (int i = 0; i < S_TrafficLight.transform.childCount; i++)
        {
            if (S_TrafficLight.transform.GetChild(i).gameObject.name == "Red_Lights")
            {
                S_Red = S_TrafficLight.transform.GetChild(i).gameObject;
            }

            if (S_TrafficLight.transform.GetChild(i).gameObject.name == "Yellow_Lights")
            {
                S_Yellow = S_TrafficLight.transform.GetChild(i).gameObject;
            }

            if (S_TrafficLight.transform.GetChild(i).gameObject.name == "Green_Lights")
            {
                S_Green = S_TrafficLight.transform.GetChild(i).gameObject;
            }

            if (S_TrafficLight.transform.GetChild(i).gameObject.name == "Go_PedXing_Lights")
            {
                S_Go_Ped = S_TrafficLight.transform.GetChild(i).gameObject;
            }

            if (S_TrafficLight.transform.GetChild(i).gameObject.name == "Stop_PedXing_Lights")
            {
                S_Stop_Ped = S_TrafficLight.transform.GetChild(i).gameObject;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {

        if (TimeCurrent < greenLightPeriod)
        {
            NS_Phase();
        }

        if (TimeCurrent >= greenLightPeriod && TimeCurrent < greenLightPeriod + yellowLightPeriod)
        {
            AllBlock_Phase();
        }

        if (TimeCurrent >= greenLightPeriod + yellowLightPeriod && TimeCurrent < greenLightPeriod + yellowLightPeriod + greenLightPeriod)
        {
            EW_Phase();
        }

        if (TimeCurrent >= greenLightPeriod + yellowLightPeriod + greenLightPeriod && TimeCurrent < greenLightPeriod + yellowLightPeriod + greenLightPeriod + yellowLightPeriod)
        {
            AllBlock_Phase();
        }

        if (TimeCurrent >= greenLightPeriod + yellowLightPeriod + greenLightPeriod + yellowLightPeriod)
        {
            TimeCurrent = 0;
        }

        TimeCurrent += Time.deltaTime;
        
    }

    void NS_Phase()
    {
        northCarBlock.SetActive(false);
        southCarBlock.SetActive(false);
        westCarBlock.SetActive(true);
        eastCarBlock.SetActive(true);
        NS_PedBlock1.SetActive(true);
        NS_PedBlock2.SetActive(true);
        EW_PedBlock1.SetActive(false);
        EW_PedBlock2.SetActive(false);

        N_Green.SetActive(true);
        N_Yellow.SetActive(false);
        N_Red.SetActive(false);
        S_Green.SetActive(true);
        S_Yellow.SetActive(false);
        S_Red.SetActive(false);

        N_Go_Ped.SetActive(true);
        S_Go_Ped.SetActive(true);
        N_Stop_Ped.SetActive(false);
        S_Stop_Ped.SetActive(false);
    }

    void EW_Phase()
    {
        northCarBlock.SetActive(true);
        southCarBlock.SetActive(true);
        westCarBlock.SetActive(false);
        eastCarBlock.SetActive(false);
        NS_PedBlock1.SetActive(false);
        NS_PedBlock2.SetActive(false);
        EW_PedBlock1.SetActive(true);
        EW_PedBlock2.SetActive(true);

        N_Green.SetActive(false);
        N_Yellow.SetActive(false);
        N_Red.SetActive(true);
        S_Green.SetActive(false);
        S_Yellow.SetActive(false);
        S_Red.SetActive(true);

        N_Go_Ped.SetActive(false);
        S_Go_Ped.SetActive(false);
        N_Stop_Ped.SetActive(true);
        S_Stop_Ped.SetActive(true);
    }

    void AllBlock_Phase()
    {
        northCarBlock.SetActive(true);
        southCarBlock.SetActive(true);
        westCarBlock.SetActive(true);
        eastCarBlock.SetActive(true);
        NS_PedBlock1.SetActive(true);
        NS_PedBlock2.SetActive(true);
        EW_PedBlock1.SetActive(true);
        EW_PedBlock2.SetActive(true);

        N_Green.SetActive(false);
        N_Yellow.SetActive(true);
        N_Red.SetActive(false);
        S_Green.SetActive(false);
        S_Yellow.SetActive(true);
        S_Red.SetActive(false);

        N_Go_Ped.SetActive(false);
        S_Go_Ped.SetActive(false);
        N_Stop_Ped.SetActive(false);
        S_Stop_Ped.SetActive(false);

    }

}
