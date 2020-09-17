using System;
using System.Linq;
using HtmlAgilityPack;

namespace WebsiteParser
{
  public class WebsiteScrapperTags
  {
    public const string ITEM_INDEX = "ITEM_INDEX";
    public const string PAGE_INDEX = "pageIndex";
  }
  public class BaseWebsiteScrapper
  {
    protected HtmlWeb web = new HtmlAgilityPack.HtmlWeb();

    #region GetStringValueOfHtmlElement

    public string GetStringValueOfHtmlElement(string xPath, HtmlDocument doc)
    {
      var nodes = doc.DocumentNode.SelectNodes(xPath);

      if (nodes != null)
      {
        string htmlValue = nodes[0]?.InnerHtml;

        return htmlValue;
      }

      return null;
    }

    #endregion

    #region GetInt32ValueOfHtmlElement

    public int? GetInt32ValueOfHtmlElement(string xPath, HtmlDocument doc)
    {
      var nodes = doc.DocumentNode.SelectNodes(xPath);

      if (nodes != null)
      {
        string htmlText = nodes[0]?.InnerText;

        if (!string.IsNullOrEmpty(htmlText))
        {
          return int.Parse(htmlText);
        }
      }

      return null;
    }

    #endregion

    #region GetDecimalValueOfHtmlElement

    public decimal? GetDecimalValueOfHtmlElement(string xPath, HtmlDocument doc)
    {
      var nodes = doc.DocumentNode.SelectNodes(xPath);

      if (nodes != null)
      {
        string htmlText = nodes[0]?.InnerHtml;

        if (!string.IsNullOrEmpty(htmlText))
        {
          return Convert.ToDecimal(htmlText);
        }
      }

      return null;
    }

    #endregion

    #region GetFinalXPath

    public string GetItemFinalXPath(string defaltItemXPath, int index)
    {
      var split = defaltItemXPath.Split(WebsiteScrapperTags.ITEM_INDEX);

      var finalXPath = split[0] + index + split[1];

      return finalXPath;
    }

    #endregion

    #region GetImageSrc

    public string GetImageSrc(string xPath, HtmlDocument doc)
    {
      var nodes = doc.DocumentNode.SelectNodes(xPath);

      if (nodes != null)
      {
        var htmlValue = nodes[0].Attributes.FirstOrDefault(x => x.Name == "src");

        return htmlValue.Value;
      }

      return null;
    }

    #endregion

    #region GetDataOriginal

    public string GetDataOriginal(string xPath, HtmlDocument doc)
    {
      var nodes = doc.DocumentNode.SelectNodes(xPath);

      if (nodes != null)
      {
        var htmlValue = nodes[0].Attributes["data-original"];

        return htmlValue.Value;
      }

      return null;
    }

    #endregion

    #region GetCurrencyPrice

    public decimal GetCurrencyPrice(string xPath, HtmlDocument doc)
    {
      var stringValue = GetStringValueOfHtmlElement(xPath, doc);
      var split = stringValue.Split(" ");

      return Convert.ToDecimal(split[0].Replace(".", ","));
    }

    #endregion

    #region AllHaveValue

    public bool AllHaveValue(params object[] objects)
    {
      foreach (var @object in objects)
      {
        if (@object == null)
          return false;
      }

      return true;
    }

    #endregion

    #region GetRefLink

    public string GetRefLink(string xPath, HtmlDocument doc)
    {
      var nodes = doc.DocumentNode.SelectNodes(xPath);

      if (nodes != null)
      {
        var htmlValue = nodes[0].Attributes.FirstOrDefault(x => x.Name == "href");

        return htmlValue.Value;
      }

      return null;
    }

    #endregion
  }
}