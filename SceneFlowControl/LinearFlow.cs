using System;
using UnityEngine;

namespace VNTags.SceneFlowControl
{
    [Serializable]
    public class LinearFlowItem
    {
        public FlowItem          Item;
        public VNTransitionData EndTransition;
    }
    
    [CreateAssetMenu(fileName = "LinearFlow", menuName = "VNTags/Flow/LinearFlow")]
    [Serializable]
    public class LinearFlow : Flow
    {
        // Data variables
        [SerializeField]
        public LinearFlowItem[] items;
        public VNTransitionData defaultTransition;


       // Session Variables
       [NonSerialized] private LinearFlowItem _current = null;
       
       protected override void Start()
       {
           GetCurrent();

           if (_current != null)
           {
               _current.Item.onEnd += _onItemEnd;
               
               _current.Item?.Start();
               onStart?.Invoke();
           }
       }

       private void _onItemEnd()
       {
           var next = MoveNext();
           if (next != null)
           {
               next.Start();
           }
       }
       
       public override void Resume(IFlowSafeState safeState)
       {
           GetCurrent();
           _current.Item?.Resume(safeState);
           onStart?.Invoke();
       }
       public override bool End()
       {
           GetCurrent();
           bool finished = true;
           _current.Item?.End(out finished);
           if (finished)
           {
               onEnd?.Invoke();
           }
           return finished;
       }

       public override FlowItem GetCurrent()
       {
           if (_current != null)
           {
               return _current.Item;
           }

           if (items == null || items.Length <= 0)
           {
               Debug.LogError("LinearFlow: GetCurrent: this flow does not contain any items, returning null");
               return null;
           }

           _current = items[0];
           return  _current.Item;
       }

       private LinearFlowItem _peekNext()
       {
           var index = Array.IndexOf(items, _current);

           if (index + 1 >= items.Length)
           {
               return null;
           }

           return items[index + 1];
       }
       
       public override FlowItem PeekNext()
       {
           var item = _peekNext();

           return item?.Item;
       }
       
       public override FlowItem MoveNext()
       {
           var next = _peekNext();

           if (next != null)
           {
               ResetCurrent();
               _current = next;
           }
           onNext?.Invoke(next?.Item);

           return next?.Item;
       }
       
       public override IFlowSafeState GetSafeState()
       {
          return  _current.Item?.Save();
       }
       
       private  void ResetCurrent()
       {
           if (_current != null && _current.Item != null)
           {
               _current.Item.onStart = null;
               _current.Item.onEnd   = null;
           }
       }
    }
}