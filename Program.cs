/*
 * Date Published: 1/3/2019
 * This program scrapes a shopify product url's .xml extension to find variants and display them
 * Add to cart options are also available as they can be used to directly add the item to cart and go to checkout
 * 
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Xml;
using System.Net;
using System.Web;



namespace VariantScraper
{
    class Program
    {
        //Class variables
        static bool atcLinks;
        static string domain;
        static string url;

        static void userInput()
        {
            //Initial input
            Console.Write("Please enter URL of product: ");
            url = Console.ReadLine();

            //Input validation
            while(string.IsNullOrEmpty(url))
            {
                Console.Write("Please enter a valid product URL: ");
                url = Console.ReadLine();
            }
            url += ".xml";

            //Secondary input
            Console.Write("Do you want add to cart links (Y/N): ");
            string cartLinks = Console.ReadLine().ToLower();

            //Input validation
            while(cartLinks != "y" && cartLinks != "n")
            {
                Console.Write("Please enter Y/N: ");
                cartLinks = Console.ReadLine().ToLower();

            }


            if (cartLinks == "y")
            {
                atcLinks = true;

                Console.Write("Please enter domain name (Ex: kith.com): ");
                domain = Console.ReadLine();

            }
                
            else if (cartLinks == "n")
                atcLinks = false;
          
        }

        static void request()
        {
            //Initiating client
            using (HttpClient client = new HttpClient())
            {
                //Header
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/68.0.3440.106 Safari/537.36");

                //Sending get request
                using (HttpResponseMessage response = client.GetAsync(url).Result)
                {
                    //Getting page content
                    using (HttpContent content = response.Content)
                    {
                        string myContent = content.ReadAsStringAsync().Result;


                        // Convert string to XML file 
                        XmlDocument xmlDoc = new XmlDocument();
                        xmlDoc.LoadXml(myContent);

                        //Parse XML
                        XmlNode fullTag = xmlDoc.SelectSingleNode("hash/variants");

                        //Formatting output
                        Console.WriteLine();
                        var sb = new StringBuilder();
                        if (atcLinks)
                            sb.Append(string.Format("{0, 6} {1,15} {2, 19} \n\n", "Size", "Variant", "ATC Link"));
                        else
                            sb.Append(string.Format("{0, 6} {1,15}\n\n", "Size", "Variant"));

                        //For every variant append to string builder
                        foreach (XmlNode element in fullTag)
                        {
                            var variantScraped = element.SelectSingleNode("id").InnerText;
                            var size = element.SelectSingleNode("option1").InnerText;



                            if (atcLinks)
                                sb.AppendFormat("{0, 6} {1, 22}     https://{2}/cart/{3}:1\n", size, variantScraped, domain, variantScraped);
                            else
                                sb.AppendFormat("{0, 6} {1, 22}\n", size, variantScraped);
                        }

                        //Final output
                        Console.WriteLine(sb);

                    }

                }


            }


        }


        static void Main(string[] args)
        {
            string resp;
            //Loop until user quits
            while(true)
            {

                userInput();


                try
                {
                    request();
                }
                catch
                {
                    Console.WriteLine("Error scraping data. Invalid URL may be the cause. Please try again. ");
                }

                //User prompt to continue
                Console.Write("Do you want to continue(Y/N): ");
                resp = Console.ReadLine().ToLower();


                //Input validation
                while(resp != "y" && resp != "n")
                {
                    Console.Write("Please enter Y/N: ");
                    resp = Console.ReadLine().ToLower();
                }


                if(resp == "n")
                {
                    Console.WriteLine("Program exiting...");
                    return;
                }

            }
            
           


            
            
        }
    }
}
