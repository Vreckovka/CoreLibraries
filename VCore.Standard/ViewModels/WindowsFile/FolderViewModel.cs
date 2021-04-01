using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text.RegularExpressions;
using VCore.Standard.Factories.ViewModels;
using VCore.Standard.Helpers;
using VCore.Standard.ViewModels.TreeView;

namespace VCore.Standard.ViewModels.WindowsFile
{
  public class FolderViewModel : WindowsItem<DirectoryInfo>
  {
    protected readonly IViewModelsFactory viewModelsFactory;
    private bool foldersLoaded;
    public FolderViewModel(DirectoryInfo directoryInfo, IViewModelsFactory viewModelsFactory) : base(directoryInfo)
    {
      this.viewModelsFactory = viewModelsFactory;

      CanExpand = true;

      
    }

    #region Properties

    #region IsRoot

    private bool isRoot;

    public bool IsRoot
    {
      get { return isRoot; }
      set
      {
        if (value != isRoot)
        {
          isRoot = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region FolderType

    private FolderType folderType;

    public FolderType FolderType
    {
      get { return folderType; }
      set
      {
        if (value != folderType)
        {
          folderType = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #region View

    private List<TreeViewItemViewModel> view;

    public List<TreeViewItemViewModel> View
    {
      get { return view; }
      set
      {
        if (value != view)
        {
          view = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    #endregion

    #region Methods

    #region GetFolderInfo

    public void GetFolderInfo()
    {
      SubItems = new List<TreeView.TreeViewItemViewModel>();

      string[] soundExtentions = new string[] { ".mp3", ".flac" };
      string[] videoExtentions = new string[] { ".mkv", ".avi", ".mp4" };

      FileInfo[] allFiles = Model.GetFiles();
      FileInfo[] soundFiles = allFiles.Where(f => soundExtentions.Contains(f.Extension.ToLower())).ToArray();
      FileInfo[] videoFiles = allFiles.Where(f => videoExtentions.Contains(f.Extension.ToLower())).ToArray();

      if (soundFiles.Length > 0 && videoFiles.Length > 0)
      {
        FolderType = FolderType.Mixed;
      }
      else if (soundFiles.Length > 0)
      {
        FolderType = FolderType.Sound;
      }
      else if (videoFiles.Length > 0)
      {
        FolderType = FolderType.Video;
      }

      var files = new List<FileInfo>();

      files.AddRange(soundFiles);
      files.AddRange(videoFiles);

      SubItems.AddRange(allFiles.Select(CreateNewFileItem));

      if (allFiles.Length == 0)
      {
        CanExpand = false;
      }

      View = SubItems;

      RaisePropertyChanged(nameof(SubItems));
      OnGetFolderInfo();
    }

    #endregion

    #region OnGetFolderInfo

    protected virtual void OnGetFolderInfo()
    {

    }

    #endregion

    #region OnExpanded

    protected override void OnExpanded(bool isExpandend)
    {
      if (isExpandend && !foldersLoaded)
      {
        LoadSubItems();
      }

      if (isFiltered)
      {
        View = SubItems;
      }
    }

    #endregion

    #region LoadSubItems

    private void LoadSubItems()
    {
      var directories = Model.GetDirectories();

      var direViewModels = directories.Select(x => CreateNewFolderItem(x)).ToList();

      SubItems.AddRange(direViewModels);

      foreach (var dir in direViewModels)
      {
        dir.GetFolderInfo();
      }


      foldersLoaded = true;
      RaisePropertyChanged(nameof(SubItems));
    }

    #endregion

    #region LoadSubFolders

    public void LoadSubFolders(FolderViewModel folderViewModel)
    {
      if (!folderViewModel.foldersLoaded)
        folderViewModel.LoadSubItems();

      foreach (var directory in folderViewModel.SubItems.OfType<FolderViewModel>())
      {
        directory.LoadSubFolders(directory);
      }
    }

    #endregion

    #region CreateNewFolderItem

    protected virtual FolderViewModel CreateNewFolderItem(DirectoryInfo directoryInfo)
    {
      return viewModelsFactory.Create<FolderViewModel>(directoryInfo);
    }

    #endregion

    #region CreateNewFileItem

    protected virtual FileViewModel CreateNewFileItem(FileInfo fileInfo)
    {
      return viewModelsFactory.Create<FileViewModel>(fileInfo);
    }

    #endregion

  
    #region ChunkSimilarity

    private string startHighlight = "|~S~|";
    private string endHighlight = "|~E~|";
    public string ToHighlitedText(string original, string predicate)
    {

      if (original.Length > predicate.Length && original.ToLower().Contains(predicate.ToLower()))
      {
        var without = original.ToLower().Split(predicate.ToLower());

        var firstPart = original.Substring(0, without[0].Length);

        var originalPredictate = original.Substring(without[0].Length, predicate.Length);

        var secondPart = original.Substring(predicate.Length + without[0].Length , without[1].Length);

        var final = firstPart + startHighlight + originalPredictate + endHighlight + secondPart;

        return final;

      }
      else if (original.ToLower() == predicate.ToLower())
      {
        return startHighlight + original + endHighlight;
      }
      else
      {
        return original;
      }

    }

    #endregion

    #region Filter

    private bool isFiltered;
    private bool wasExpandedByFilter;

    public void Filter(string predicated)
    {
      if (!string.IsNullOrEmpty(predicated) && !predicated.All(x => char.IsWhiteSpace(x)))
      {
        isFiltered = true;

        var viewItems = SubItems.Where(x => x.Name.ToLower().Contains(predicated) || x.Name.ChunkSimilarity(predicated) > 0.70).ToList();

        var folders = SubItems.OfType<FolderViewModel>().ToList();

        foreach (var folder in folders)
        {
          LoadSubFolders(folder);

          folder.Filter(predicated);
        }

        var childs = folders.Where(x => x.View.Count > 0);

        foreach (var item in viewItems)
        {
          item.HighlitedText = ToHighlitedText(item.Name, predicated);
        }

        foreach (var item in childs)
        {
          if (!viewItems.Contains(item))
          {
            viewItems.Add(item);
          }

          if (!item.IsExpanded)
          {
            item.wasExpandedByFilter = true;
            item.IsExpanded = true;
          }
        }



        View = new List<TreeViewItemViewModel>(viewItems);
      }
      else if (isFiltered)
      {
        ResetFilter();
      }
    }

    #endregion

    #region ResetFilter

    public void ResetFilter()
    {
      if (isFiltered)
      {
        var folders = SubItems.OfType<FolderViewModel>().ToList();

        folders.ForEach(x => { x.ResetFilter(); });

        if (wasExpandedByFilter)
        {
          wasExpandedByFilter = false;
          IsExpanded = false;
        }

        View = SubItems;

        SubItems.ForEach(x => x.HighlitedText = x.Name);
        isFiltered = false;
      }
    }

    #endregion

    #endregion
  }
}
