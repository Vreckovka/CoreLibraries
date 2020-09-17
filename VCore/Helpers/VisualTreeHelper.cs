using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using VCore.Common;
using VCore.Controls;

namespace VCore.Helpers
{
  public static class VisualTreeExtentions
  {

    public static IEnumerable<PlayableWrapPanelItem> GetListViewItemsFromList(this PlayableWrapPanel lv)
    {
      return FindChildrenOfType<PlayableWrapPanelItem>(lv);
    }

    public static IEnumerable<T> FindChildrenOfType<T>(this DependencyObject ob) where T : DependencyObject
    {
      foreach (var child in GetChildren(ob))
      {
        T castedChild = child as T;
        if (castedChild != null)
        {
          yield return castedChild;
        }
        else
        {
          foreach (var internalChild in FindChildrenOfType<T>(child))
          {
            yield return internalChild;
          }
        }
      }
    }

    public static IEnumerable<DependencyObject> GetChildren(this DependencyObject ob)
    {
      int childCount = System.Windows.Media.VisualTreeHelper.GetChildrenCount(ob);

      for (int i = 0; i < childCount; i++)
      {
        yield return System.Windows.Media.VisualTreeHelper.GetChild(ob, i);
      }
    }

    public static T GetFirstChildOfType<T>(this DependencyObject dependencyObject) where T : DependencyObject
    {
      if (dependencyObject == null)
      {
        return null;
      }

      for (var i = 0; i < VisualTreeHelper.GetChildrenCount(dependencyObject); i++)
      {
        var child = VisualTreeHelper.GetChild(dependencyObject, i);

        var result = (child as T) ?? GetFirstChildOfType<T>(child);

        if (result != null)
        {
          return result;
        }
      }

      return null;
    }

    public static T GetFirstParentOfType<T>(this DependencyObject dependencyObject) where T : DependencyObject
    {
      if (dependencyObject == null)
      {
        return null;
      }

      T result = null;


      do
      {
        var parent = VisualTreeHelper.GetParent(dependencyObject);

        if (parent == null)
        {
          break;
        }

        result = (parent as T) ?? GetFirstParentOfType<T>(parent);

      } while (result == null);

      return result;
    }
  }

  public static class DisposableExtentions
  {
    public static void DisposeWith(this IDisposable disposable, VDisposableObject disposableObject)
    {
      disposableObject.AddAutoDisposeObject(disposable);
    }
  }
}