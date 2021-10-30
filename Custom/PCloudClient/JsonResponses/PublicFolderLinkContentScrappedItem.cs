using System;
using System.Collections.Generic;
using System.Text;

namespace PCloudClient.JsonResponses
{
  public class PublicFolderLinkContentScrappedItem
  {
    public class Content
    {
      public string name { get; set; }
      public string created { get; set; }
      public bool cancreate { get; set; }
      public bool thumb { get; set; }
      public string modified { get; set; }
      public bool canread { get; set; }
      public bool isfolder { get; set; }
      public long fileid { get; set; }
      public ulong hash { get; set; }
      public int category { get; set; }
      public bool candelete { get; set; }
      public string id { get; set; }
      public bool isshared { get; set; }
      public bool canmodify { get; set; }
      public string contenttype { get; set; }
      public int parentfolderid { get; set; }
      public int size { get; set; }
      public int icon { get; set; }
      public int folderid { get; set; }
      public List<Content> contents { get; set; }
    }

    public class Metadata
    {
      public bool isshared { get; set; }
      public string name { get; set; }
      public string created { get; set; }
      public bool cancreate { get; set; }
      public bool thumb { get; set; }
      public string modified { get; set; }
      public bool candelete { get; set; }
      public bool canread { get; set; }
      public string id { get; set; }
      public bool canmodify { get; set; }
      public int icon { get; set; }
      public bool isfolder { get; set; }
      public int folderid { get; set; }
      public List<Content> contents { get; set; }
    }

    public bool ownerispremium { get; set; }
    public bool canupload { get; set; }
    public string country { get; set; }
    public string downloadlink { get; set; }
    public int result { get; set; }
    public bool ownerisme { get; set; }
    public bool candownload { get; set; }
    public bool ownerisbusiness { get; set; }
    public string language { get; set; }
    public bool usercanupload { get; set; }
    public string code { get; set; }
    public Metadata metadata { get; set; }
  }
}
