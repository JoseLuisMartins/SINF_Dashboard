using System;
using System.Xml;
using System.Xml.Schema;
using System.Xml.XPath;

class XValidation
{
    public static XmlDocument validation()
    {
        XmlReaderSettings settings = new XmlReaderSettings();
        settings.Schemas.Add(null, AppDomain.CurrentDomain.BaseDirectory + "Validation\\SAFTPT1.04_01.xsd");
        settings.ValidationType = ValidationType.Schema;

        XmlReader reader = XmlReader.Create(AppDomain.CurrentDomain.BaseDirectory + "Assets\\SAFT_DEMOSINF_01-01-2016_31-12-2016.xml", settings);
        XmlDocument document = new XmlDocument();
        document.Load(reader);

        ValidationEventHandler eventHandler = new ValidationEventHandler(ValidationEventHandler);

        document.Validate(eventHandler);
     
        return document;
    }

    static void ValidationEventHandler(object sender, ValidationEventArgs e)
    {
        switch (e.Severity)
        {
            case XmlSeverityType.Error:
                Console.WriteLine("Error: {0}", e.Message);
                throw new Exception("Error:" + e.Message);
                break;
            case XmlSeverityType.Warning:
                Console.WriteLine("Warning {0}", e.Message);
                throw new Exception("warning:" + e.Message);
                break;
        }

    }
}
