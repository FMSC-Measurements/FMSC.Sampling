using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FMSC.Sampling
{
    [Serializable]
    public abstract class SampleSelecter
    {
        #region fields
        private static MersenneTwister  rand = null;

        private int                     iTreeFrequency = -1;
        private int                     count = -1;
        private SystematicCounter       insuranceCounter = null;
        #endregion

        #region Ctor
        protected SampleSelecter()
        {
            if (rand == null)
            {
                uint seed = (uint)Math.Abs(DateTime.Now.Ticks);//TODO test edge cases on seed value
                rand = new MersenneTwister(seed);
            }
            this.Count = 0;
        }


        protected SampleSelecter( int iTreeFrequency ): this()
        {
            this.ITreeFrequency = iTreeFrequency;
        }

        #endregion

        #region Public properties
        /// <summary>
        /// gets and sets ITreeFrequency. If value is not valid 
        /// isSelectingITrees is set to false and ITreeFrequency to -1
        /// </summary>
        [XmlAttribute]
        public int ITreeFrequency {
            get {return this.iTreeFrequency;}
            //sets iTreeFrequency if value is positive and non zero,
            //otherwise it is set to -1 and set isSelectingITrees to false
            set
            {
                this.iTreeFrequency = value;
            }
        }

        [XmlElementAttribute(ElementName = "insuranceCounter",
            IsNullable = false, Type = typeof(SystematicCounter))]
        public SystematicCounter InsuranceCounter
        {
            get { return insuranceCounter; }
            set
            {
                if (insuranceCounter == null && value != null)
                {
                    value.Rand = this.Rand;
                    insuranceCounter = value;
                }
                else if (value == null)
                {
                    throw new System.ArgumentNullException("can't set insuranceCounter to null");
                }
                else if (insuranceCounter != null)
                {
                    throw new System.InvalidOperationException("can't reset insuranceCounter once it has been initialized");
                }
            }

        }


        /// <summary>
        /// gets IsSelectingItrees. value is true if 
        /// ITreeFrequency is not -1.
        /// </summary>
        public bool IsSelectingITrees
        {
            get
            {
                return ITreeFrequency > 0;
            }
        }

        [XmlAttribute]
        public int Count {
            get { return this.count; }
            //sets count if value is positive, otherwise it is set to -1
            set { this.count = (value >= 0) ? value : -1; }

        }
        #endregion 

        #region protected Properties
        public MersenneTwister Rand
        {
            get { return rand; }
        }
        #endregion

        #region Methods

        /// <summary>
        /// Determins if the next tree is a sample and if IsSelectingITrees is true
        /// it determins if the next tree is a insurance tree. subclass should call
        /// subclass's overriden Ready()
        /// </summary>
        /// <returns>SampleItem object containing informating on the next tree</returns>
        public abstract SampleItem NextItem();

        /// <summary>
        /// Checks if the SampleSelecter is ready, and will optionaly throw an exception if not
        /// subclass should override. overridden method should call base's ready. 
        /// </summary>
        /// <param name="throwException">set to true if an exception should be thrown</param>
        /// <returns>true if SampleSelecter is ready, otherwise false</returns>
        public abstract bool Ready(bool throwException);

        /// <summary>
        /// Determins if the next tree is a sample
        /// </summary>
        /// <returns>true if the next tree is a sample</returns>
        public bool Next()
        {
            SampleItem item = this.NextItem();
            if (item != null)
            {
                if (item.IsInsuranceItem)
                {
                    return false;
                }
                else
                {
                    return true;
                }
            }
            else
            {
                return false;
            }
        }


        /// <summary>
        /// Determins if the next n trees are a sample 
        /// </summary>
        /// <param name="num">number of trees to check</param>
        /// <returns>Array of bools indicating if each tree is a sample</returns>
        public bool[] GetMany(int num)
        {
            List<bool> many = new List<bool>(num);
            for (int i = 0; i < num; i++)
            {
                many.Add(this.Next());
            }

            return many.ToArray();
        }

        public List<SampleItem> GetManyItems(int num)
        {
            List<SampleItem> manyItems = new List<SampleItem>(num);
            SampleItem nextItem = null;
            for (int i = 0; i < num; i++)
            {
                nextItem = this.NextItem();
                if (nextItem != null)
                {
                    manyItems.Add(nextItem);
                }
            }

            return manyItems;
        }
        #endregion
    }
}
