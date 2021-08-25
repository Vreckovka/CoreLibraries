using System.Reactive.Linq;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Threading.Tasks;
using VCore.ItemsCollections;
using VCore.Standard.Factories.ViewModels;
using VCore.Standard.Helpers;
using VCore.Standard.ViewModels.TreeView;
using VCore.Standard.ViewModels.WindowsFile;
using VCore.WPF.ItemsCollections;
using System;
using System.Windows;


namespace VCore.WPF.ViewModels.WindowsFiles
{
  public abstract class FolderViewModel<TFileViewModel> : FolderHierarchyItemViewModel<FolderInfo>
  where TFileViewModel : FileViewModel
  {
    protected readonly IViewModelsFactory viewModelsFactory;
    private bool foldersLoaded;
    private Subject<bool> isLoadedSubject = new Subject<bool>();

    public FolderViewModel(FolderInfo folderInfo, IViewModelsFactory viewModelsFactory) : base(folderInfo)
    {
      this.viewModelsFactory = viewModelsFactory;

      CanExpand = true;

      Name = folderInfo.Name;

      isLoadedSubject.Throttle(TimeSpan.FromMilliseconds(150)).ObserveOn(Application.Current.Dispatcher).Subscribe(x =>
      {
        IsLoading = x;
      }).DisposeWith(this);
    }

    #region Properties

    #region ParentFolder

    private FolderViewModel<TFileViewModel> parentFolder;

    public FolderViewModel<TFileViewModel> ParentFolder
    {
      get { return parentFolder; }
      set
      {
        if (value != parentFolder)
        {
          parentFolder = value;
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

    #region WasLoaded

    private bool wasLoaded;

    public bool WasLoaded
    {
      get { return wasLoaded && foldersLoaded; }
    }

    #endregion

    #region IsLoading

    private bool isLoading;

    public bool IsLoading
    {
      get { return isLoading; }
      set
      {
        if (value != isLoading)
        {
          isLoading = value;
          RaisePropertyChanged();
        }
      }
    }

    #endregion

    public virtual bool LoadSubItemsWhenExpanded { get; } = true;

    #endregion

    #region Methods

    public abstract Task<IEnumerable<FileInfo>> GetFiles();
    public abstract Task<IEnumerable<FolderInfo>> GetFolders();

    #region LoadFolder

    public async Task LoadFolder()
    {
      SubItems = new ItemsViewModel<TreeViewItemViewModel>();

      if (Model.Name != "System Volume Information")
      {
        var allFiles = (await GetFiles()).ToList();

        SubItems.AddRange(allFiles.Select(CreateNewFileItem));

        var directories = (await GetFolders()).ToList();

        if (allFiles.Count == 0 && directories.Count == 0)
        {
          CanExpand = false;
        }

        RefreshType();
        RaisePropertyChanged(nameof(SubItems));

        OnGetFolderInfo();
      }
      else
      {
        CanExpand = false;
      }

      wasLoaded = true;
      RaisePropertyChanged(nameof(WasLoaded));
    }

    #endregion

    #region OnGetFolderInfo

    protected virtual void OnGetFolderInfo()
    {

    }

    #endregion

    #region OnExpanded

    protected override async void OnExpanded(bool isExpandend)
    {
      if (isExpandend)
      {
        try
        {
          isLoadedSubject.OnNext(true);

          if (!WasLoaded)
            await LoadFolder();

          if (!foldersLoaded)
            await LoadFolders();
        }
        finally
        {
          isLoadedSubject.OnNext(false);
        }
      }
    }

    #endregion

    #region LoadFolders

    private async Task LoadFolders()
    {
      if (Model.Name != "System Volume Information")
      {
        var directories = await GetFolders();

        var direViewModels = directories.Select(x => CreateNewFolderItem(x)).ToList();

        SubItems.AddRange(direViewModels);

        foreach (var dir in direViewModels)
        {
          if (LoadSubItemsWhenExpanded)
          {
            await dir.LoadFolder();
          }

          dir.ParentFolder = this;
        }

        OnLoadSubItems();
        RefreshType();

        foldersLoaded = true;

        RaisePropertyChanged(nameof(SubItems));
        RaisePropertyChanged(nameof(WasLoaded));
      }
    }

    #endregion

    #region OnLoadSubItems

    protected virtual void OnLoadSubItems()
    {

    }

    #endregion

    #region LoadSubFolders

    public async Task LoadSubFolders(FolderViewModel<TFileViewModel> folderViewModel)
    {
      if (!folderViewModel.WasLoaded)
        await LoadFolder();

      if (!folderViewModel.foldersLoaded)
        await LoadFolders();

      foreach (var directory in folderViewModel.SubItems.ViewModels.OfType<FolderViewModel<TFileViewModel>>())
      {
        await directory.LoadSubFolders(directory);
      }
    }

    #endregion

    #region CreateNewFolderItem

    protected virtual FolderViewModel<TFileViewModel> CreateNewFolderItem(FolderInfo directoryInfo)
    {
      return viewModelsFactory.Create<FolderViewModel<TFileViewModel>>(directoryInfo);
    }

    #endregion

    #region CreateNewFileItem

    protected virtual TFileViewModel CreateNewFileItem(FileInfo fileInfo)
    {
      return viewModelsFactory.Create<TFileViewModel>(fileInfo);
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

        var folders = SubItems.ViewModels.OfType<FolderViewModel<TFileViewModel>>().ToList();

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
        var folders = SubItems.ViewModels.OfType<FolderViewModel<TFileViewModel>>().ToList();

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

    public virtual void RefreshType()
    {
      var videos = SubItems.View.OfType<FolderViewModel<TFileViewModel>>().Any(x => x.FolderType == FolderType.Video) ||
                   SubItems.View.OfType<TFileViewModel>().Any(x => x.FileType == FileType.Video);


      var sounds = SubItems.View.OfType<FolderViewModel<TFileViewModel>>().Any(x => x.FolderType == FolderType.Sound) ||
                   SubItems.View.OfType<TFileViewModel>().Any(x => x.FileType == FileType.Sound);


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

      ParentFolder?.RefreshType();
    }

    #endregion

    #endregion
  }
}
