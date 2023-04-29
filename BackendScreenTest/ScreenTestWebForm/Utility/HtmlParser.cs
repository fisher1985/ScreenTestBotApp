using ScreenTestWebForm.CarModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScreenTestWebForm.Utility
{
    public class HtmlParser
    {
        public List<CarModel> TOCarModelList(String html)
        {
            List<CarModel> carModels = new List<CarModel>();
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(html);
            
            

            return carModels;

        }
        public CarModel GetCarInfo(String html)
        {
            CarModel carModel = new CarModel();
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(html);
            foreach (var h1 in document.DocumentNode.Descendants("h1"))
            {
                if (h1.Attributes["class"] != null &&
                    h1.Attributes["class"].Value.Equals("listing-title"))
                {
                    
                    carModel.CarInfo = h1.InnerText.Trim();
                    
                }

            }
            foreach (var div in document.DocumentNode.Descendants("div"))
            {
                if (div.Attributes["class"] != null &&
                   div.Attributes["class"].Value.Equals("listing-mileage") && div.InnerText.Contains("mi."))
                {
                    carModel.Mileage=div.InnerText.Trim();

                }
                //
                if (div.Attributes["class"] != null &&
                  div.Attributes["class"].Value.Equals("basics-content-wrapper"))
                {
                    carModel.ExteriorColor = div.Descendants("dd").ElementAt(0).InnerText.Trim();
                    carModel.InteriorColor = div.Descendants("dd").ElementAt(1).InnerText.Trim();
                    carModel.Drivetrain = div.Descendants("dd").ElementAt(2).InnerText.Trim();
                    carModel.MPG = div.Descendants("dd").ElementAt(3).InnerText.Trim();
                    carModel.FuelType = div.Descendants("dd").ElementAt(4).InnerText.Trim();
                    carModel.Transmission = div.Descendants("dd").ElementAt(5).InnerText.Trim();
                    carModel.Engine = div.Descendants("dd").ElementAt(6).InnerText.Trim();
                    carModel.VIN = div.Descendants("dd").ElementAt(6).InnerText.Trim();
                    carModel.Stock = div.Descendants("dd").ElementAt(7).InnerText.Trim();
                    carModel.Mileage = div.Descendants("dd").ElementAt(8).InnerText.Trim();
                }
                if (div.Attributes["class"] != null &&
                  div.Attributes["class"].Value.Equals("dealer-phone"))
                {
                    carModel.Phone=div.InnerText.Replace("Call","").Trim();
                }
            }
            foreach (var span in document.DocumentNode.Descendants("span"))
            {
                if (span.Attributes["class"] != null &&
                   span.Attributes["class"].Value.Equals("primary-price") && span.InnerText.Contains("$"))
                {
                    carModel.Price = span.InnerText;

                }
                if (span.Attributes["class"] != null &&
                   span.Attributes["class"].Value.Equals("js-estimated-monthly-payment-formatted-value-with-abr") && span.InnerText.Contains("mo"))
                {
                    carModel.PriceMonth = span.InnerText;

                }
            }
            return carModel;
        }
       
        public List<String> GetAHref(string html)
        {
            HtmlAgilityPack.HtmlDocument document = new HtmlAgilityPack.HtmlDocument();
            document.LoadHtml(html);
            List<String> lstLink = new List<string>();
            foreach (var item in document.DocumentNode.Descendants("a"))
            {
                if (item.Attributes["href"]!=null&& item.Attributes["href"].Value!=null&&
                    item.Attributes["href"].Value.Contains("vehicledetail"))
                {
                    lstLink.Add(item.Attributes["href"].Value);
                }
               
            } 
            return lstLink;
        }
    }
}
