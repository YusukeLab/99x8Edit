using System;
using System.Collections.Generic;
using System.Text;

namespace _99x8Edit
{
    // Mementos for undo/redo actions
    class Memento
    {
        // [For editor] When other data sources are needed, add here
        public Machine vdpData;
    }
    class MementoCaretaker
    {
        private static MementoCaretaker _singleInstance = new MementoCaretaker();
        private List<Memento> mementoList = new List<Memento>();
        private List<Memento> mementoRedo = new List<Memento>();
        private MainWindow UI = null;
        private Machine vdpData;
        public static MementoCaretaker Instance
        {
            get
            {
                return _singleInstance;
            }
        }
        public void Initialize(MainWindow ui, Machine vdp)
        {
            UI = ui;
            // [For editor] When other data sources are needed, add here
            vdpData = vdp;
        }
        public void Push()
        {
            if(vdpData == null)
            {
                // Not initialized
                return;
            }
            Memento m = new Memento();
            {
                // [For editor] When other data sources are needed, add here
                m.vdpData = vdpData.CreateCopy();
            }
            mementoList.Add(m);
            if(mementoList.Count > 256)
            {
                mementoList.RemoveAt(0);
            }
            mementoRedo.Clear();
            UI.UndoEnable = true;
            UI.RedoEnable = false;
        }
        public void Undo()
        {
            if(mementoList.Count == 0)
            {
                return;
            }
            // Set current status for future redo action
            mementoRedo.Add(this.CreateCurrentMemento());
            // Pop one status from memento list
            Memento m = mementoList[mementoList.Count - 1];
            mementoList.RemoveAt(mementoList.Count - 1);
            {
                // [For editor] When other data sources are needed, add here
                vdpData.SetAllData(m.vdpData);
            }
            if (mementoList.Count == 0)
            {
                UI.UndoEnable = false;
            }
            UI.RedoEnable = true;
        }
        public void Redo()
        {
            if(mementoRedo.Count == 0)
            {
                return;
            }
            // Push current status to undo list
            mementoList.Add(this.CreateCurrentMemento());
            if (mementoList.Count > 32)
            {
                mementoList.RemoveAt(0);
            }
            // Pop one status from redo list
            Memento m = mementoRedo[mementoRedo.Count - 1];
            mementoRedo.RemoveAt(mementoRedo.Count - 1);
            {
                // [For editor] When other data sources are needed, add here
                vdpData.SetAllData(m.vdpData);
            }
            if (mementoRedo.Count == 0)
            {
                UI.RedoEnable = false;
            }
            UI.UndoEnable = true;
        }
        public void Clear()
        {
            mementoList.Clear();
            mementoRedo.Clear();
            UI.UndoEnable = false;
            UI.RedoEnable = false;
        }
        // Internal utility
        private Memento CreateCurrentMemento()
        {
            Memento current = new Memento();
            {
                // [For editor] When other data sources are needed, add here
                current.vdpData = vdpData.CreateCopy();
            }
            return current;
        }
    }
}
