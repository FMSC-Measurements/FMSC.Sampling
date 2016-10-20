using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FMSC.Sampling
{
    /// <summary>
    /// Class that acts a container for creating and maintaining a block sampling set.
    /// the block and be itterated over using block[index], 
    /// </summary>
    public class BlockState
    {
        private struct IntInterval
        {
            public int Start;
            public int End;
        }

        private int subBlockHeight = 2;
        private int numSubBlocks = 5;
        private int blockSize = -1;
        //private IFrequencyBasedSelecter master = null;
        private List<SampleItem> selectionList = null;//current working block

        //constructer
        public BlockState()
        {

        }


        //public Block(IFrequencyBasedSelecter master)
        //{
        //    if (master != null)
        //    {
        //        this.Master = master;
        //    }
        //}

        //indexer
        [XmlIgnore]
        public boolItem this[int index] {
            get
            {
                if (SelectionList != null &&
                    SelectionList.Count > 0 &&
                    index < BlockSize &&
                    index >= 0)
                {
                    boolItem item = new boolItem(index);
                    item = (boolItem)this.SelectionList.Find(
                        ((SampleItem)item).Equals);
                    return item;
                }
                else
                {
                    return null;
                }
            }
        }

        //properties 

        //[XmlIgnore]
        //public IFrequencyBasedSelecter Master 
        //{
        //    get 
        //    { return master; }
        //    set
        //    {
        //        if (master == null)
        //        {
        //            master = value;
        //        }
        //        else
        //        {
        //            throw new ArgumentException("master can't be reset once it has been set");
        //        }
        //    }
        //}

        [XmlAttribute]
        public int SubBlockHeight { 
            get { return subBlockHeight; }
            set { subBlockHeight = (value > 0) ? value : -1; }
        }

        [XmlAttribute]
        public int NumSubBlocks {
            get { return numSubBlocks; }
            set { numSubBlocks = (value > 0) ? value : -1; } 
        }

        /// <summary>
        /// gets and sets the list of items that represent selected
        /// items in the block. can't be set unless it is null. 
        /// </summary>
        [XmlArray("SelectionList")]
        [XmlArrayItem(typeof(boolItem))]
        public List<SampleItem> SelectionList
        {
            get { return this.selectionList; }
            set
            {
                if (value != null && this.selectionList == null)
                {
                    this.selectionList = value;
                }
                else
                {
                    throw new System.ArgumentException("selectionList can't be set once it is set, or can't be set to null");
                }
            }
        }

        [XmlIgnore]
        public int BlockSize { get; protected set; }



        //Methods

        public void initBlock(IFrequencyBasedSelecter master)
        {
            if (this.SelectionList == null)
            {
                this.SelectionList = new List<SampleItem>();
            }
            else
            {
                SelectionList.Clear();
            }

            if (master == null)
            {
                throw new System.ArgumentNullException("master can't be null");
            }

            calcBlockSize(master);

            boolItem nextSelection; //temp value
            IntInterval subBlock;//interval from the start to the end of the current subblock

            //select selection numbers for each subBlock
            for (int i = 0; i < this.NumSubBlocks; i++)
            {
                //calculate the range of the current subblock
                subBlock.Start = i * (master.Frequency * this.SubBlockHeight);
                subBlock.End = (i + 1) * (master.Frequency * this.SubBlockHeight);

                //get a random item in the range of the current subblock
                nextSelection = new boolItem(master.Rand.Next(subBlock.Start, subBlock.End));

                if (master.IsSelectingITrees)
                {
                    if (master.InsuranceCounter.Next())
                    {
                        nextSelection.IsInsuranceItem = true;
                    }
                    else
                    {
                        nextSelection.IsSelected = true;
                    }
                }
                else
                {
                    nextSelection.IsSelected = true;
                }

                this.selectionList.Add(nextSelection);
            }

            //select remaining selection numbers
            for (int i = 0; i < this.NumSubBlocks; i++)
            {
                nextSelection = new boolItem();
                //keep requesting new selection nubers untill you get one 
                //that you dont already have
                do
                {
                    nextSelection.Index = master.Rand.Next(0, this.NumSubBlocks * this.SubBlockHeight * master.Frequency);
                } while (this.selectionList.Contains(nextSelection));

                if (master.IsSelectingITrees)
                {
                    if (master.InsuranceCounter.Next())
                    {
                        nextSelection.IsInsuranceItem = true;
                    }
                    else
                    {
                        nextSelection.IsSelected = true;
                    }
                }
                else
                {
                    nextSelection.IsSelected = true;
                }

                this.selectionList.Add(nextSelection);
            }

            if (master.IsSelectingITrees)
            {
                int i = 0;
                boolItem item = new boolItem(i);
                //loop though the intire block
                for (; i < BlockSize; i++)
                {

                    //if nothing already exists at that block index
                    if (!selectionList.Contains(item))
                    {
                        //see if it might be an insurance item
                        if (master.InsuranceCounter.Next())
                        {
                            item.IsInsuranceItem = true;
                            SelectionList.Add(item);
                            item = new boolItem();
                        }
                    }
                    item.Index = i + 1;
                }
            }



            this.selectionList.Sort();
        }

        internal void calcBlockSize(IFrequencyBasedSelecter master)
        {
            if (master.Frequency != -1 && SubBlockHeight != -1 && NumSubBlocks != -1)
            {
                BlockSize = master.Frequency * SubBlockHeight * NumSubBlocks;
            }
        }


    }

    

}
