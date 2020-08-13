using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Web;
using AlkomatDomain.DomainClasses;
using HtmlAgilityPack;

namespace WebsiteParser
{
  public static class TescoProductsInfomation
  {
    public const string NUMBER_OF_PAGES = "/html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[5]/nav/ul/li[6]/a/span";

    public const string NAME = "//html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[productIndex]/div/div/div/div/div[1]/div[1]/h3/a";
    public const string PRODUCT_PRICE = "/html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[productIndex]/div/div/div/div/div[2]/form/div/div[1]/div[1]/div/div/span/span[1]";
    public const string UNIT_PRICE = "/html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[productIndex]/div/div/div/div/div[2]/form/div/div[1]/div[2]/span[1]/span[1]";

  }

  public class ParsedStoreProduct : StoreProduct
  {
    public string Name { get; set; }
  }

  public class TescoParser : BaseWebsiteScrapper
  {
    HtmlWeb web = new HtmlAgilityPack.HtmlWeb();
    private string siteUrl = "https://potravinydomov.itesco.sk/groceries/sk-SK/shop/alkohol/all?page=1";
    private int maximumNumberOfProductsOnPage = 25;

    public TescoParser()
    {

    }


    #region GetProducts

    public IEnumerable<ParsedStoreProduct> GetStoreProducts(int? maxNumberOfPages = null)
    {
      HtmlDocument doc = web.Load(siteUrl);
      var numberOfPages = GetInt32ValueOfHtmlElement(TescoProductsInfomation.NUMBER_OF_PAGES, doc);

      List<ParsedStoreProduct> storeProducts = new List<ParsedStoreProduct>();

      if (maxNumberOfPages != null)
      {
        numberOfPages = maxNumberOfPages + 1;
      }

      for (int i = 1; i < numberOfPages; i++)
      {
        var page = "https://potravinydomov.itesco.sk/groceries/sk-SK/shop/alkohol/all?page=" + i;

        if (i != 1)
          doc = web.Load(page);

        for (int j = 1; j < maximumNumberOfProductsOnPage + 1; j++)
        {
          try
          {
            var product = GetProduct(j, doc);

            storeProducts.Add(product);
            Console.WriteLine($"{storeProducts.Count}. Got product : {product.Name}");
          }
          catch (Exception ex)
          {
            Debug.Write(ex);
          }
        }
      }

      return storeProducts;
    }

    #endregion

    #region GetProduct

    private ParsedStoreProduct GetProduct(int index, HtmlDocument doc)
    {
      ParsedStoreProduct product = null;

      var id = GetProductId(index,doc);
      var name = GetStringValueOfHtmlElement(GetFinalXPath(TescoProductsInfomation.NAME, index), doc);
      var price = GetDecimalValueOfHtmlElement(GetFinalXPath(TescoProductsInfomation.PRODUCT_PRICE, index), doc);
      var salePrice = GetProductSale(index, doc);
      var unitPrice = GetProductUnitPrice(index, doc);

      if (AllHaveValue(id, name, price))
      {
        product = new ParsedStoreProduct()
        {
          Name = name,
          Price = price.Value,
          IdInStore = id.Value,
        };
      }

      if (product != null)
      {
        if (salePrice != null)
        {
          product.StoreProductSales = new List<StoreProductSale>();

          product.StoreProductSales.Add(salePrice);
        }

        if (unitPrice != null)
        {
          product.StoreProductUnitPrices = new List<StoreProductUnitPrice>();

          product.StoreProductUnitPrices.Add(unitPrice);
        }
      }

      if (product == null)
      {
        throw new Exception("Cannot get product data");
      }

      return product;
    }

    #endregion

   

  

    #region GetProductId

    ///html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[1]/div/div/div/div/div[1]/div[1]/h3/a
    ///html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[2]/div/div/div/div/div[1]/div[1]/h3/a
    ///html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[{index}]/div/div/div/div/div[1]/div[1]/h3/a
    ///  
    private long? GetProductId(int index, HtmlDocument doc)
    {
      var xPathName = $"/html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[{index}]/div/div/div/div/div[1]/div[1]/h3/a";

      var nodes = doc.DocumentNode.SelectNodes(xPathName);

      if (nodes != null)
      {
        var attributes = nodes[0]?.Attributes;

        if (attributes != null && attributes.Count > 0)
        {
          var split = attributes[0].Value.Split("/");

          return Convert.ToInt64(split.Last());
        }
      }

      return null;
    }

    #endregion

    #region GetProductUnitPrice

    ///html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[1]/div/div/div/div/div[2]/form/div/div[1]/div[2]/span[1]/span[1]
    ///html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[2]/div/div/div/div/div[2]/form/div/div[1]/div[2]/span[1]/span[1]
    ///html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[{index}]/div/div/div/div/div[2]/form/div/div[1]/div[2]/span[1]/span[1]


    ///html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[1]/div/div/div/div/div[2]/form/div/div[1]/div[2]/span[2]
    ///html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[2]/div/div/div/div/div[2]/form/div/div[1]/div[2]/span[2]
    ///html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[{index}]/div/div/div/div/div[2]/form/div/div[1]/div[2]/span[2]


    private StoreProductUnitPrice GetProductUnitPrice(int index, HtmlDocument doc)
    {
      var xPathPrice = $"/html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[{index}]/div/div/div/div/div[2]/form/div/div[1]/div[2]/span[1]/span[1]";
      var xPathUnit = $"/html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[{index}]/div/div/div/div/div[2]/form/div/div[1]/div[2]/span[2]";

      var priceNodes = doc.DocumentNode.SelectNodes(xPathPrice);
      var unitNodes = doc.DocumentNode.SelectNodes(xPathUnit);

      if (priceNodes != null && unitNodes != null)
      {
        var unit = unitNodes[0]?.InnerHtml.Remove(0, 1);
        var price = GetDecimalValueOfHtmlElement(GetFinalXPath(TescoProductsInfomation.UNIT_PRICE, index), doc);

        if (price != null && !string.IsNullOrEmpty(unit))
        {
          var decimalPrice = Convert.ToDecimal(price);

          return new StoreProductUnitPrice()
          {
            Price = decimalPrice,
            UnitPriceType = new UnitPriceType()
            {
               Name = "l"
            }
          };
        }

      }

      return null;
    }

    #endregion

    #region GetProductSale

    ///html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[5]/div/div[1]/div/div[1]/div[1]/div[2]/div/ul/li/a/div/span[1]
    ///html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[12]/div/div[1]/div/div[1]/div[1]/div[2]/div/ul/li/a/div/span[1]
    ///html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[{index}]/div/div[1]/div/div[1]/div[1]/div[2]/div/ul/li/a/div/span[1]
    ///
    ///
    ///
    //html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[3]/div/div[1]/div/div[1]/div[1]/div[2]/div/ul/li/a/div/span[2]


    private StoreProductSale GetProductSale(int index, HtmlDocument doc)
    {
      var xPathSaleDescription = $"/html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[{index}]/div/div[1]/div/div[1]/div[1]/div[2]/div/ul/li/a/div/span[1]";
      var xPathSaleDate = $"/html/body/div[1]/div/div/div[2]/div[1]/div/div[1]/div[1]/div[2]/div[4]/div[2]/div/div/div/ul/li[{index}]/div/div[1]/div/div[1]/div[1]/div[2]/div/ul/li/a/div/span[2]";

      var nodesDescription = doc.DocumentNode.SelectNodes(xPathSaleDescription);
      var nodesDate = doc.DocumentNode.SelectNodes(xPathSaleDate);

      if (nodesDate != null && nodesDescription != null)
      {
        string saleDescription = nodesDescription[0]?.InnerHtml;
        string saleDate = nodesDate[0]?.InnerHtml;

        if (!string.IsNullOrEmpty(saleDescription) && !string.IsNullOrEmpty(saleDescription))
        {
          var split = saleDescription.Split(',');
          var percentage = double.Parse(split[0].Substring(0, split[0].Length - 1).Substring(1));

          var oldPriceInteger = int.Parse(split[1].Split(" ")[2]);
          var oldPriceDecimal = int.Parse(split[2].Split(" ")[0]) / 100.0;

          var oldPrice = Convert.ToDecimal(oldPriceInteger + oldPriceDecimal);

          var newPriceInteger = int.Parse(split[3].Split(" ")[2]);
          var newPriceDecimal = int.Parse(split[4].Split(" ")[0]) / 100.0;

          var newPrice = Convert.ToDecimal(newPriceInteger + newPriceDecimal);

          var dateText = "Cena je platná pri dodaní do ";
          var toDate = Convert.ToDateTime(saleDate.Substring(dateText.Length - 1));

          return new StoreProductSale()
          {
            NewPrice = newPrice,
            OldPrice = oldPrice,
            SalePercentage = percentage,
            To = toDate
          };
        }
      }

      return null;
    }

    #endregion

    
  }
}
