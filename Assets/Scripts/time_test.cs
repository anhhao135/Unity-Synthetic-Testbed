using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Text;
using System.Xml;

public class time_test : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

        XmlWriterSettings settings = new XmlWriterSettings();


        settings.Encoding = Encoding.UTF8;
        settings.Indent = true;

        XmlWriter xmlWriter = XmlWriter.Create("test.xml", settings);

        xmlWriter.WriteStartDocument(true);

        xmlWriter.WriteDocType("boost_serialization",null,null,null);


        xmlWriter.WriteStartElement("boost_serialization");
        xmlWriter.WriteAttributeString("signature", "serialization::archive");
        xmlWriter.WriteAttributeString("version", "9");

        xmlWriter.WriteStartElement("users");

        xmlWriter.WriteStartElement("user");
        xmlWriter.WriteAttributeString("age", "42");
        xmlWriter.WriteString("John Doe");
        xmlWriter.WriteEndElement();

        xmlWriter.WriteStartElement("user");
        xmlWriter.WriteAttributeString("age", "39");
        xmlWriter.WriteString("Jane Doe");
        xmlWriter.WriteEndElement();

        xmlWriter.WriteEndElement();

        xmlWriter.WriteEndElement();

        xmlWriter.WriteEndDocument();
        xmlWriter.Close();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
