using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using VCore.WPF.Controls;

namespace VCore.WPF.Helpers
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

    public static TChild FindChildByName<TChild>(this DependencyObject reference, string childName) where TChild : DependencyObject
    {
      return FindChildByNameRec<TChild>(reference, childName, null);
    }

    public static DependencyObject FindChildByName(this DependencyObject reference, string childName)
    {
      if (!string.IsNullOrEmpty(childName))
      {
        if (reference != null)
        {
          int childrenCount = VisualTreeHelper.GetChildrenCount(reference);
          for (int i = 0; i < childrenCount; i++)
          {
            var child = VisualTreeHelper.GetChild(reference, i);

            var frameworkElement = child as FrameworkElement;

            if (frameworkElement != null && frameworkElement.Name == childName)
            {
              return child;
            }
            else
            {
              var result = FindChildByName(child, childName);

              if (result != null)
              {
                return result;
              }
            }
          }

          return null;
        }
      }

      return null;
    }

    private static TChild FindChildByNameRec<TChild>(DependencyObject reference, string childName, TChild foundChild) where TChild : DependencyObject
    {
      if (foundChild != null)
      {
        return foundChild;
      }

      if (reference != null)
      {
        int childrenCount = VisualTreeHelper.GetChildrenCount(reference);
        for (int i = 0; i < childrenCount; i++)
        {
          var child = VisualTreeHelper.GetChild(reference, i);
          // If the child is not of the request child type child
          if (!(child is TChild))
          {
            // recursively drill down the tree
            foundChild = FindChildByNameRec<TChild>(child, childName, foundChild);
          }
          else if (!string.IsNullOrEmpty(childName))
          {
            var frameworkElement = child as FrameworkElement;
            // If the child's name is set for search
            if (frameworkElement != null && frameworkElement.Name == childName)
            {
              // if the child's name is of the request name
              foundChild = (TChild)child;
              break;
            }
          }
          else
          {
            // child element found.
            foundChild = (TChild)child;

            break;
          }
        }
      }

      return foundChild;
    }


  }
}