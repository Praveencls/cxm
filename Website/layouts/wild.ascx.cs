using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Xml.Serialization;
using Newtonsoft.Json;
using Website.Models;








namespace Website.layouts
{
    public class City
    {

        public string Name { get; set; }

        public string State { get; set; }

        public string County { get; set; }

        public bool IsCountyCenter { get; set; }

        public bool IsStateCapital { get; set; }

        public int Population { get; set; }

    }



    /// <summary>

    /// Company info

    /// </summary>

    public class Company
    {

        public string Title { get; set; }

        public List<Employee> Employees { get; set; }

    }



    /// <summary>

    /// Employee info

    /// </summary>

    public class Employee
    {

        public string Name { get; set; }

        public EmployeeType EmployeeType { get; set; }

    }



    /// <summary>

    /// Types of employees

    /// (just to se how JSON deals with enums!)

    /// </summary>

    public enum EmployeeType
    {

        CEO,

        Developer

    }



    [JsonObject(MemberSerialization.OptIn)]

    class Person
    {

        [JsonProperty("forename")]

        public string forename { get; set; }

        [JsonProperty("surname")]

        public string surname { get; set; }

        [JsonProperty("age")]

        public int age { get; set; }

    }

 
    

    public partial class wild : System.Web.UI.UserControl
    {
        /// <summary>

        /// Function that use upper classes to simulate some data

        /// </summary>

        /// <returns></returns>

        public static Company GetData()
        {

            return new Company()

            {

                Title = "Company Ltd",

                Employees = new List<Employee>()

            {

                new Employee(){ Name = "Mark CEO", EmployeeType = EmployeeType.CEO },

                new Employee(){ Name = "Matija Božičević", EmployeeType = EmployeeType.Developer },

                new Employee(){ Name = "Steve Developer", EmployeeType = EmployeeType.Developer}

            }

            };

        }

        public static T DeserializeJSon<T>(string jsonString)
        {
            DataContractJsonSerializer ser = new DataContractJsonSerializer(typeof(T));
            MemoryStream stream = new MemoryStream(Encoding.UTF8.GetBytes(jsonString));
            T obj = (T)ser.ReadObject(stream);
            return obj;
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            string jsonString = @"[{""forename"" : ""Phil"", ""surname"" : ""Curnow"", ""age"" : 41},{""forename"" : ""Lorna"", ""surname"" : ""Curnow"", ""age"" : 44}]";

            dynamic obj = JsonConvert.DeserializeObject<List<Person>>(jsonString);



            // Load object with some sample data

            Company company = GetData();

            string json1 = Newtonsoft.Json.JsonConvert.SerializeObject(company);

            //var obj = DeserializeJSon<Person>(jsonString);



            City city = new City();

            city.Name = "Independence";

            city.State = "KS";

            city.County = "Montgomery";

            city.IsCountyCenter = true;

            city.IsStateCapital = false;

            city.Population = 10000;



            XmlSerializer serializer = new XmlSerializer(city.GetType());

            StreamWriter writer = new StreamWriter(@"D:\Temporary\myfile.xml");

            serializer.Serialize(writer.BaseStream, city);



            City city1 = new City();



            XmlSerializer serializer1 = new XmlSerializer(city1.GetType());



            StreamReader reader = new StreamReader(@"D:\Temporary\myfile1.xml");

            object deserialized = serializer1.Deserialize(reader.BaseStream);



            city1 = (City)deserialized;

            HttpWebRequest request = (HttpWebRequest)WebRequest.Create("http://dms72/-/item/v1/?sc_itemid={74516918-5271-43E2-9CC8-3A4060C9156B}&sc_database=master");
            var LastUrlString = Sitecore.Web.WebUtil.GetUrlName(0);

            Response.Write(String.Format("Last url value is wild card:  {0}  <br/>", LastUrlString));

            request.Headers["X-Scitemwebapi-Username"] = @"sitecore\admin";
            request.Headers["X-Scitemwebapi-Password"] = "b";

            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            Response.Write(String.Format("Content length is {0}", response.ContentLength));
            Response.Write(String.Format("Content type is {0}", response.ContentType));
            // Get the stream associated with the response.
            Stream receiveStream = response.GetResponseStream();

            // Pipes the stream to a higher level stream reader with the required encoding format. 
            StreamReader readStream = new StreamReader(receiveStream, Encoding.UTF8);

            Response.Write("<br /> Response stream received. <br />");
           
            string json = readStream.ReadToEnd();
            var recs = JsonConvert.DeserializeObject<Record>(json);

            if (recs != null)
            {
                foreach (var item in recs.Result.Items) {
                    Response.Write(item.DisplayName + "<br/>");
                }
            }
            
        }
    }
}