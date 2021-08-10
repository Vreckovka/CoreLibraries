using System.Collections.Generic;
using VCore.Modularity.RegionProviders;

namespace VCore.Modularity.Navigation
{
  public class NavigationSet
  {
    #region Fields

    private bool isInBackState;

    #endregion Fields

    #region Properties

    public LinkedListNode<IRegistredView> Actual { get; set; }
    public LinkedList<IRegistredView> Chain { get; } = new LinkedList<IRegistredView>();

    #endregion Properties

    #region Methods

    #region Add

    public void Add(IRegistredView registredView)
    {
      if (isInBackState)
      {
        RemoveAllNodesAfter(Actual);
        isInBackState = false;
      }

      Chain.AddLast(registredView);
      Actual = Chain.Last;
    }

    #endregion

    #region GetNext

    public IRegistredView GetNext()
    {
      if (Actual.Next == null)
        return null;

      Actual = Actual.Next;

      return Actual.Value;
    }

    #endregion

    #region GetPrevious

    public IRegistredView GetPrevious()
    {
      if (Actual.Previous == null)
        return null;

      if (Actual?.Value != null)
        Actual.Value.Deactivate();

      Actual = Actual.Previous;

      isInBackState = true;

      return Actual.Value;
    }

    #endregion

    #region RemoveAllNodesAfter

    public void RemoveAllNodesAfter(LinkedListNode<IRegistredView> node)
    {
      while (node.Next != null)
        Chain.Remove(node.Next);
    }

    #endregion

    #endregion 
  }
}