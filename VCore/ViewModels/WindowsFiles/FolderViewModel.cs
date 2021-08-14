using System.Collections.Generic;
using System.IO;
using System.Linq;
using VCore.ItemsCollections;
using VCore.Standard.Factories.ViewModels;
using VCore.Standard.Helpers;
using VCore.Standard.ViewModels.TreeView;
using VCore.Standard.ViewModels.WindowsFile;
using VCore.WPF.ItemsCollections;
using VCore.WPF.Managers;

namespace VCore.WPF.ViewModels.WindowsFiles
{
  public class FolderViewModel : WindowsItemViewModel<DirectoryInfo>
  {
    protected readonly IViewModelsFactory viewModelsFactory;
    private bool foldersLoaded;
    public FolderViewModel(
      DirectoryInfo directoryInfo, 
      IViewModelsFactory viewModelsFactory,
      IWindowManager windowManager) : base(directoryInfo, windowManager)
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

    #endregion

    #region Methods

    #region GetFolderInfo

    public void GetFolderInfo()
    {
      SubItems = new ItemsViewModel<TreeViewItemViewModel>();

      string[] soundExtentions = new string[] { ".mp3", ".flac" };
      string[] videoExtentions = new string[] { ".mkv", ".avi", ".mp4", ".ts" };

      if (Model.Name != "System Volume Information")
      {
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

        var directories = Model.GetDirectories();

        if (allFiles.Length == 0 && directories.Length == 0)
        {
          CanExpand = false;
        }

        RaisePropertyChanged(nameof(SubItems));

        OnGetFolderInfo();
      }
      else
      {
        CanExpand = false;
      }
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
    }

    #endregion

    #region LoadSubItems

    private void LoadSubItems()
    {
      if (Model.Name != "System Volume Information")
      {
        var directories = Model.GetDirectories();

        var direViewModels = directories.Select(x => CreateNewFolderItem(x)).ToList();

        SubItems.AddRange(direViewModels);

        foreach (var dir in direViewModels)
        {
          dir.GetFolderInfo();
        }

        foldersLoaded = true;

        OnLoadSubItems();
        RefreshType();

        RaisePropertyChanged(nameof(SubItems));
      }
    }

    #endregion

    #region OnLoadSubItems

    protected virtual void OnLoadSubItems()
    {

    }

    #endregion

    #region LoadSubFolders

    public void LoadSubFolders(FolderViewModel folderViewModel)
    {
      if (!folderViewModel.foldersLoaded)
        folderViewModel.LoadSubItems();

      foreach (var directory in folderViewModel.SubItems.ViewModels.OfType<FolderViewModel>())
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

        var secondPart = original.Substring(predicate.Length + without[0].Length, without[1].Length);

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

        var viewItems = SubItems.ViewModels.Where(x => x.Name.ToLower().Contains(predicated) || x.Name.ChunkSimilarity(predicated) > 0.70).ToList();

        var folders = SubItems.ViewModels.OfType<FolderViewModel>().ToList();

        foreach (var folder in folders)
        {
          LoadSubFolders(folder);

          folder.Filter(predicated);
        }

        var childs = folders.Where(x => x.SubItems.View.Count > 0);

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

        SubItems.View = new RxObservableCollection<TreeViewItemViewModel>(viewItems);
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
        var folders = SubItems.ViewModels.OfType<FolderViewModel>().ToList();

        folders.ForEach(x => { x.ResetFilter(); });

        if (wasExpandedByFilter)
        {
          wasExpandedByFilter = false;
          IsExpanded = false;
        }

        SubItems.ResetFilter();

        SubItems.ViewModels.ForEach(x => x.HighlitedText = x.Name);
        isFiltered = false;
      }
    }

    #endregion

    #region RefreshType

    private void RefreshType()
    {
      var videos = SubItems.View.OfType<FolderViewModel>().Any(x => x.FolderType == FolderType.Video) ||
                   SubItems.View.OfType<FileViewModel>().Any(x => x.FileType == FileType.Video); 


      var sounds = SubItems.View.OfType<FolderViewModel>().Any(x => x.FolderType == FolderType.Sound) ||
                   SubItems.View.OfType<FileViewModel>().Any(x => x.FileType == FileType.Sound);


      if (videos && sounds)
      {
        FolderType = FolderType.Mixed;
      }
      else if (sounds)
      {
        FolderType = FolderType.Sound;
      }
      else if (videos)
      {
        FolderType = FolderType.Video;
      }
      else
      {
        FolderType = FolderType.Other;
      }
    }

    #endregion

    #endregion
  }
}
