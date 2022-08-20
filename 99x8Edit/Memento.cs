using System;
using System.Collections.Generic;

namespace _99x8Edit
{
    // Mementos for undo/redo actions
    // Expected to be used as:
    //  Initialization - MementoCaretaker.Instance.AddTarget(object_to_be_managed);
    //  For each operation - MementoCaretaker.Instance.Push();
    //  Undo - MementoCaretaker.Instance.Undo();
    internal interface IMementoTarget
    {
        // Object to be managed should implement these interfaces
        internal IMementoTarget CreateCopy();
        internal void Restore(IMementoTarget m);
    }
    internal class Memento
    {
        // One record for each action
        private List<IMementoTarget> _objs = new List<IMementoTarget>();
        internal void Add(IMementoTarget s)
        {
            _objs.Add(s);
        }
        internal IMementoTarget See(int index)
        {
            return _objs[index];
        }
    }
    internal class MementoCaretaker
    {
        // Singleton class for memento management
        private static MementoCaretaker _singleInstance = new MementoCaretaker();
        private List<IMementoTarget> _targetList = new List<IMementoTarget>();
        private List<Memento> _undoList = new List<Memento>();
        private List<Memento> _redoList = new List<Memento>();
        private Action _stateChanged;
        internal static MementoCaretaker Instance
        {
            get
            {
                return _singleInstance;
            }
        }
        internal bool UndoEnable
        {
            get;
            set;
        } = false;
        internal bool RedoEnable
        {
            get;
            set;
        } = false;
        // For initialization
        internal void SetCallback(Action callback)
        {
            // Called when UndoEnable/RndoEnable have changed
            _stateChanged = callback;
        }
        internal void AddTarget(IMementoTarget target)
        {
            // Add data source to be managed
            _targetList.Add(target);
        }
        // For undo/redo actions
        internal void Push()
        {
            // Push current status for further undo
            _undoList.Add(this.CreateCurrentMemento());
            if (_undoList.Count > 256)
            {
                _undoList.RemoveAt(0);
            }
            _redoList.Clear();
            this.UndoEnable = true;
            this.RedoEnable = false;
            _stateChanged?.Invoke();
        }
        internal void Undo()
        {
            // Undo action
            if (_undoList.Count == 0)
            {
                return;
            }
            // Set current status for future redo action
            _redoList.Add(this.CreateCurrentMemento());
            // Pop one status from memento list
            Memento m = _undoList[_undoList.Count - 1];
            _undoList.RemoveAt(_undoList.Count - 1);
            // Restore current status from memento
            this.RestoreFromMemento(m);
            // State changed
            if (_undoList.Count == 0)
            {
                this.UndoEnable = false;
            }
            this.RedoEnable = true;
            _stateChanged?.Invoke();
        }
        internal void Redo()
        {
            // Redo action
            if (_redoList.Count == 0)
            {
                return;
            }
            // Push current status to undo list
            _undoList.Add(this.CreateCurrentMemento());
            if (_undoList.Count > 32)
            {
                _undoList.RemoveAt(0);
            }
            // Pop one status from redo list and restore
            Memento m = _redoList[_redoList.Count - 1];
            _redoList.RemoveAt(_redoList.Count - 1);
            this.RestoreFromMemento(m);
            // State changed
            if (_redoList.Count == 0)
            {
                this.RedoEnable = false;
            }
            this.UndoEnable = true;
            _stateChanged?.Invoke();
        }
        internal void Clear()
        {
            _undoList.Clear();
            _redoList.Clear();
            this.UndoEnable = false;
            this.RedoEnable = false;
            _stateChanged?.Invoke();
        }
        // Private
        private Memento CreateCurrentMemento()
        {
            Memento current = new Memento();
            for (int i = 0; i < _targetList.Count; ++i)
            {
                current.Add(_targetList[i].CreateCopy());
            }
            return current;
        }
        private void RestoreFromMemento(Memento m)
        {
            for (int i = 0; i < _targetList.Count; ++i)
            {
                _targetList[i].Restore(m.See(i));
            }
        }
    }
}
