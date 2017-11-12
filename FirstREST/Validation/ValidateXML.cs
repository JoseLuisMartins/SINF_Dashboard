using System;
using System.Xml;
using Saxon.Api;
using System.Collections;
using System.IO;

namespace FirstREST.Validation { 
    class XMLValidation
    {
        public static void validation()
        {
            Saxon.Api.Processor proc = new Processor(true);

            SchemaManager schemaManager = proc.SchemaManager;
           
            FileStream xsdFs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "Validation\\SAFTPT1.04_01.xsd", FileMode.Open);
            XmlReader validationXml = XmlReader.Create(xsdFs);

            Boolean bSchema = schemaManager == null;
            Boolean bXml = validationXml == null;
            //throw new Exception("debug: schema: " + bSchema + " xml: " + bXml);

           // XmlReaderSettings settings = new XmlReaderSettings();
           // settings.ValidationType = ValidationType.Schema;

            schemaManager.Compile(validationXml);
            SchemaValidator schemaValidator = schemaManager.NewSchemaValidator();

            FileStream xmlFs = new FileStream(AppDomain.CurrentDomain.BaseDirectory + "Assets\\SAFT_DEMOSINF_01-01-2016_31-12-2016.xml", FileMode.Open);

            schemaValidator.SetSource(XmlReader.Create(xmlFs));

            schemaValidator.Run();
         
           
        }

     
    }
}
