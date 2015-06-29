using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Security.Cryptography.Xml;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;

namespace OpenSslGost
{
    class Program
    {
        static void Main(string[] args)
        {
            //VerifyXmlFile("data.xml");
            //VerifyXmlFileRsa("doc-signed.xml");
            //SignXml("RsaXmlAttachment_V.xml");

            
            //SignXmlFile("PresentedForm_Template.xml", "PresentedForm_Template_sign.xml");
            //Console.WriteLine(VerifyXmlFile("PresentedForm_Template_sign.xml"));
            Console.WriteLine(VerifyXmlFile("PresentedForm_Template_sign(2).xml"));
            Console.ReadKey();
        }

        // Sign an XML file and save the signature in a new file. This method does not  
        // save the public key within the XML file.  This file cannot be verified unless  
        // the verifying code has the key with which it was signed.
        public static void SignXmlFile(string fileName, string signedFileName)
        {
            //var cspParams = new CspParameters { KeyContainerName = "XML_DSIG_RSA_KEY" };
            //var key = new RSACryptoServiceProvider(cspParams);
            //var blob = key..ExportCspBlob(true);
            //var cert = new X509Certificate2(blob);
            //File.WriteAllBytes("Hello.cer", cert.Export(X509ContentType.Cert));


            var cert = new X509Certificate2("certificate.pfx");
            var privateKey = cert.PrivateKey as RSACryptoServiceProvider;

            // Create a new XML document.
            XmlDocument doc = new XmlDocument();

            // Load the passed XML file using its name.
            doc.Load(new XmlTextReader(fileName));

            // Create a SignedXml object.
            SignedXml signedXml = new SignedXml(doc);

            // Add the key to the SignedXml document. 
            signedXml.SigningKey = privateKey;

            // Create a reference to be signed.
            Reference reference = new Reference();
            reference.Uri = "";

            // Add an enveloped transformation to the reference.
            XmlDsigEnvelopedSignatureTransform env = new XmlDsigEnvelopedSignatureTransform();
            reference.AddTransform(env);

            // Add the reference to the SignedXml object.
            signedXml.AddReference(reference);

            var keyInfo = new KeyInfo();
            keyInfo.AddClause(new RSAKeyValue(cert.PublicKey.Key as RSACryptoServiceProvider));
            signedXml.KeyInfo = keyInfo;   

            // Compute the signature.
            signedXml.ComputeSignature();

            // Get the XML representation of the signature and save
            // it to an XmlElement object.
            XmlElement xmlDigitalSignature = signedXml.GetXml();

            // Append the element to the XML document.
            doc.DocumentElement.AppendChild(doc.ImportNode(xmlDigitalSignature, true));

            if (doc.FirstChild is XmlDeclaration)
            {
                doc.RemoveChild(doc.FirstChild);
            }

            // Save the signed XML document to a file specified
            // using the passed string.
            XmlTextWriter xmltw = new XmlTextWriter(signedFileName, new UTF8Encoding(false));
            doc.WriteTo(xmltw);
            xmltw.Close();
        }

        // Verify the signature of an XML file against an asymetric 
        // algorithm and return the result.
        public static Boolean VerifyXmlFile(String name)
        {
            //var cert = new X509Certificate2("certificate.pfx");
            //var key = cert.PublicKey.Key as RSACryptoServiceProvider;

            // Create a new XML document.
            XmlDocument xmlDocument = new XmlDocument();

            // Load the passed XML file into the document. 
            xmlDocument.Load(name);

            // Create a new SignedXml object and pass it
            // the XML document class.
            SignedXml signedXml = new SignedXml(xmlDocument);

            // Find the "Signature" node and create a new
            // XmlNodeList object.
            XmlNodeList nodeList = xmlDocument.GetElementsByTagName("Signature");

            // Load the signature node.
            signedXml.LoadXml((XmlElement)nodeList[0]);

            // Check the signature and return the result.
            return signedXml.CheckSignature();
        }

        //static void SignXml(string signedFileName)
        //{
        //    var xmlDoc = new XmlDocument { PreserveWhitespace = true };
        //    xmlDoc.Load(signedFileName);
        //    var cspParams = new CspParameters { KeyContainerName = "XML_DSIG_RSA_KEY" };
        //    var rsaKey = new RSACryptoServiceProvider(cspParams);

        //    var signedXml = new SignedXml(xmlDoc) { SigningKey = rsaKey };

        //    var reference = new Reference { Uri = "#presented-form" };

        //    var env = new XmlDsigEnvelopedSignatureTransform();
        //    reference.AddTransform(env);
        //    signedXml.AddReference(reference);
        //    signedXml.ComputeSignature();
        //    var xmlDigitalSignature = signedXml.GetXml();
        //    xmlDoc.DocumentElement.AppendChild(xmlDoc.ImportNode(xmlDigitalSignature, true));

        //    xmlDoc.Save(string.Format("{0}_signed_rsa.xml", Path.GetFileName(signedFileName)));
        //}

        //static void VerifyXmlFileRsa(string signedFileName)
        //{
        //    var xmlDoc = new XmlDocument { PreserveWhitespace = true };
        //    xmlDoc.Load(signedFileName);

        //    var signedXml = new SignedXml(xmlDoc);
        //    var dsig = xmlDoc.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl)[0];
        //    signedXml.LoadXml((XmlElement)dsig);
        //}

        //static void VerifyXmlFile(string signedFileName)
        //{
        //    // Создаем новый документ XML.
        //    var xmlDocument = new XmlDocument {PreserveWhitespace = true};
        //    xmlDocument.Load(signedFileName);

        //    var signature = xmlDocument.GetElementsByTagName("Signature", SignedXml.XmlDsigNamespaceUrl)[0];
        //    var signedXml = new SmevSignedXml(xmlDocument);

        //    signedXml.LoadXml((XmlElement) signature);

        //    //загрузим сертификат
        //    var cert = new X509Certificate2(Convert.FromBase64String(signedXml.KeyInfo.GetXml().GetElementsByTagName("X509Certificate")[0].InnerText));

        //    var result = signedXml.CheckSignature(cert.PublicKey.Key);
        //    Console.WriteLine(result ? "Подпись верна." : "Подпись не верна.");

        //    //for (int curSignature = 0; curSignature < nodeList.Count; curSignature++)
        //    //{
        //    //    var signedXml = new SmevSignedXml(xmlDocument);

        //    //    signedXml.LoadXml((XmlElement)nodeList[curSignature]);

        //    //    var referenceList = signedXml.GetXml().GetElementsByTagName("Reference");
        //    //    if (referenceList.Count == 0)
        //    //        throw new XmlException("Не удалось найти ссылку на сертификат");

        //    //    var binaryTokenReference = ((XmlElement)referenceList[0]).GetAttribute("URI");

        //    //    if (string.IsNullOrEmpty(binaryTokenReference) || binaryTokenReference[0] != '#')
        //    //        throw new XmlException("Не удалось найти ссылку на сертификат");

        //    //    //var binaryTokenElement = xmlDocument.GetElementById(binaryTokenReference.Substring(1));
        //    //    //if (binaryTokenElement == null)
        //    //    //    throw new XmlException("Не удалось найти сертификат");

        //    //    var cert = new X509Certificate2(Convert.FromBase64String(signedXml.KeyInfo.GetXml().GetElementsByTagName("X509Certificate")[0].InnerText));

        //    //    // Проверяем подпись.
        //    //    // ВНИМАНИЕ! Проверка сертификата в данном примере не осуществляется. Её необходимо
        //    //    // реализовать самостоятельно в соответствии с требованиями к подписи проверяемого
        //    //    // типа сообщения СМЭВ.
        //    //    bool result = signedXml.CheckSignature(cert.PublicKey.Key);

        //    //    // Выводим результат проверки подписи в консоль
        //    //    if (result)
        //    //    {
        //    //        Console.WriteLine("Подпись №{0} верна.", curSignature + 1);
        //    //    }
        //    //    else
        //    //    {
        //    //        Console.WriteLine("Подпись №{0} не верна.", curSignature + 1);
        //    //    }
        //    //}
        //}

        //class SmevSignedXml : SignedXml
        //{
        //    public SmevSignedXml(XmlDocument document)
        //        : base(document)
        //    {
        //    }

        //    public override XmlElement GetIdElement(XmlDocument document, string idValue)
        //    {
        //        XmlNamespaceManager nsmgr = new XmlNamespaceManager(document.NameTable);
        //        nsmgr.AddNamespace("wsu", WSSecurityWSUNamespaceUrl);
        //        return document.SelectSingleNode("//*[@wsu:Id='" + idValue + "']", nsmgr) as XmlElement;
        //    }

        //    public const string WSSecurityWSSENamespaceUrl = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";
        //    public const string WSSecurityWSUNamespaceUrl = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-utility-1.0.xsd";
        //}
    }
}
