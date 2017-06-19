using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FMSC.Sampling
{
    public class BlockSelecter : SampleSelecter, IFrequencyBasedSelecter
    {
        //fields

        private int frequency = -1;
        private BlockState block = null;

        //Properties

        [XmlAttribute]
        public int Frequency
        {
            get
            {
                return this.frequency;
            }
            set
            {
                this.frequency = (value > 0) ? value : -1;
            }
        }

        public override bool IsSelectingITrees
        {
            get
            {
                return Frequency > 1 && base.IsSelectingITrees;
            }
        }

        [XmlElement("Block", typeof(BlockState))]
        public BlockState Block
        {
            get { return block; }
            set
            {
                if (value == null) { throw new System.ArgumentNullException("Block can't be set to null"); }
                if (block != null) { throw new System.InvalidOperationException("block can not be changed after it has been set"); }
                if (block == null && value != null)
                {
                    block = value;
                }
            }
        }

        [XmlAttribute]
        public int BlockIndex { get; set; }

        #region Ctor

        protected BlockSelecter() // protected constructer for serialization
            : base()
        {
        }

        #endregion Ctor

        public BlockSelecter(int frequency,
            int iTreeFrequency)
            : base(iTreeFrequency)
        {
            this.Frequency = frequency;

            if (base.IsSelectingITrees)
            {
                //create a systematicCounter for insurance trees
                //that will select an insurance tree at random
                //with the probability of 1 / (frequency * iTreeFrequency)
                InsuranceCounter = new SystematicCounter(
                    (frequency * base.ITreeFrequency),
                    SystematicCounter.CounterType.ON_RANDOM, base.Rand);
                //alternative method where iTree is selected every nth tally
                //and frequency is n
                //InsuranceCounter = new Utility.SystematicCounter( n , Utility.SystematicCounter.CounterType.ON_RANDOM)
            }

            this.Block = new BlockState();
            Block.initBlock(this);
            BlockIndex = 0;
        }

        //methods
        public override SampleItem NextItem()
        {
            this.Ready(true);
            boolItem nextItem;

            if (this.block.BlockSize <= 0)
            {
                this.block.calcBlockSize(this);
            }
            if (this.block != null && this.BlockIndex < this.block.BlockSize)
            {
                //if a item exists at the current blockIndex
                //create a clone and change its Index to the current count
                nextItem = Block[BlockIndex];
                if (nextItem != null)
                {
                    nextItem = (boolItem)nextItem.Clone();
                    nextItem.Index = Count;
                }
                BlockIndex++;
                base.Count++;
                return nextItem;
            }
            else if (this.block == null)
            {
                //create and initalize the block
                //and call NextItem
                //Block = new Block(this);
                //Block.initBlock();
                //return NextItem();
                //TODO enable this for debuging
                throw new System.InvalidOperationException("no instance of block");
            }
            else if (this.BlockIndex == this.block.BlockSize)
            {
                //reinitialize block when the end has been reached
                BlockIndex = 0;
                Block.initBlock(this);
                return NextItem();
            }
            throw new Exception("code should be unreachable");
        }

        public override bool Ready(bool throwException)
        {
            if (Frequency == -1)
            {
                if (throwException)
                {
                    throw new System.InvalidOperationException("block sample not ready");
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return true;
            }
        }
    }
}