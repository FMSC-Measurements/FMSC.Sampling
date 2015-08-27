using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FMSC.Sampling
{
    public class ListSelecter : SampleSelecter
    {
        //Fields
        private int populationSize = -1;
        private List<ListItem> deck = null;
        private IEnumerator<ListItem> deckEnum = null;
        //private Utility.SystematicCounter insuranceCounter = null;

        //Properties

        [XmlAttribute]
        public int PopulationSize
        {
            get { return this.populationSize; }
            set { this.populationSize = (value > 0) ? value : -1; }
        }

        [XmlArray]
        [XmlArrayItem(typeof(ListItem))]
        public List<ListItem> Deck
        {
            get{ return deck;}
            set { deck = value; }
        }
                

        //[XmlElementAttribute(ElementName = "insuranceCounter",
        //    IsNullable = false, Type = typeof(Utility.SystematicCounter))]
        //public Utility.SystematicCounter InsuranceCounter
        //{
        //    get { return insuranceCounter; }
        //    set
        //    {
        //        if (insuranceCounter == null && value != null)
        //        {
        //            insuranceCounter = value;
        //        }
        //        else if (value == null)
        //        {
        //            throw new System.ArgumentNullException("can't set insuranceCounter to null");
        //        }
        //        else if( insuranceCounter != null )
        //        {
        //            throw new System.InvalidOperationException("can't reset insuranceCounter once it has been initialized");
        //        }
        //    }

        //}

        //constructers
        public ListSelecter()
            : base()
        {
            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="iTreeFrequency">if the number of items you wish is known
        /// use an iTreeFrequency = (populationSize / numberOfSamples) * iTreeFrequency</param>
        public ListSelecter(
            int populationSize,
            int iTreeFrequency)
            : base( iTreeFrequency)
        {
            this.Deck = new List<ListItem>();
            this.PopulationSize = populationSize;

            if (base.IsSelectingITrees)
            {
                InsuranceCounter =
                    new SystematicCounter(base.ITreeFrequency,
                        SystematicCounter.CounterType.ON_RANDOM, this.Rand);
            }

            PopulateDeck();
            ShuffleDeck();

        }


        //methods
        private void PopulateDeck()
        {
            if (base.IsSelectingITrees)
            {
                Deck.Clear();
                for (int i = 0; i < PopulationSize; i++)
                {
                    Deck.Add(new ListItem(i, base.InsuranceCounter.Next()));
                }
            }
            else
            {
                Deck.Clear();
                for (int i = 0; i < PopulationSize; i++)
                {
                    Deck.Add(new ListItem(i));
                }
            }
        }

        private void ShuffleDeck()
        {
            int destIndex; //destination index
            int upperBounds = PopulationSize - 1;
            for (int i = 0; i + 1 < upperBounds; i++)
            {
                destIndex = Rand.Next((i + 1), upperBounds);
                //TODO learn how to do a proper swap function
                ListItem temp = Deck[i];
                Deck[i] = Deck[destIndex];
                Deck[destIndex] = temp;
            }

            deckEnum = Deck.GetEnumerator();
            deckEnum.Reset();
        }

        //no workie
        //private void Swap(out SampleListItems.ListItem uno,out SampleListItems.ListItem dos)
        //{
        //    SampleListItems.ListItem tres = uno;
        //    uno = dos;
        //    dos = tres;
        //}



        public override SampleItem NextItem()
        {
            if (deckEnum.MoveNext())
            {
                base.Count++;
                return deckEnum.Current;
            }
            else
            {
                PopulateDeck();
                ShuffleDeck();
                return NextItem();
            }          
        }

        public override bool Ready(bool throwException)
        {

            if (base.Count < this.PopulationSize)
            {
                return true;
            }
            else
            {
                if (throwException)
                {
                    throw new System.InvalidOperationException("list sample selecter is not ready");
                }
                else
                {
                    return false;
                }
            }
        }
    }
}
