using System;
using System.Collections.Generic;
using System.Text;
using System.Xml.Serialization;

namespace FMSC.Sampling
{
    public abstract class SampleItem : IEquatable<Object>, IComparable<SampleItem>
    {
        public const String INSURANCE_TAG = " <I>";

        [XmlAttribute]
        public int Index { get; set; }

        [XmlAttribute]
        public bool IsInsuranceItem { get; set; }

        public bool IsSelected { get; set; }

        public abstract int getValue();

        public abstract override String ToString();

        public abstract Object Clone();

        public override bool Equals(Object other)
        {
            if (this.Index == ((SampleItem)other).Index)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int CompareTo(SampleItem other)
        {
            if (other.Index > this.Index)
            {
                return -1;
            }
            else if (other.Index == this.Index)
            {
                return 0;
            }
            else
            {
                return 1;
            }
        }
    }

    public class ThreePItem : SampleItem
    {
        [XmlAttribute]
        public int KPI { get; set; }

        public ThreePItem()
        {
            base.Index = -1;
            base.IsInsuranceItem = false;
            this.KPI = -1;
        }

        public ThreePItem(int index, int KPI)
        {
            base.Index = index;
            this.KPI = KPI;
        }

        public ThreePItem(int index, int KPI, bool isInsuranceItem)
        {
            base.Index = index;
            this.KPI = KPI;
            base.IsInsuranceItem = isInsuranceItem;
        }

        public override object Clone()
        {
            return new ThreePItem(this.Index, this.KPI, this.IsInsuranceItem);
        }

        public override int getValue()
        {
            return KPI;
        }

        public override string ToString()
        {
            return String.Format("{0:d}:", this.KPI);
        }
    }

    public class boolItem : SampleItem
    {
        public boolItem()
        {
            base.Index = -1;
            IsInsuranceItem = false;
        }

        public boolItem(int index)
        {
            base.Index = index;
            IsInsuranceItem = false;
        }

        public boolItem(int index, bool isInsuranceItem, bool isSelected)
        {
            base.Index = index;
            IsInsuranceItem = isInsuranceItem;
            IsSelected = isSelected;
        }

        public override object Clone()
        {
            return new boolItem(this.Index, this.IsInsuranceItem, this.IsSelected);
        }

        public override int getValue()
        {
            return 1;
        }

        public override string ToString()
        {
            String insuranceIndicator;
            if (IsInsuranceItem)
            {
                insuranceIndicator = INSURANCE_TAG;
            }
            else
            {
                insuranceIndicator = "";
            }
            return "$ " + insuranceIndicator;
        }
    }

    //public class ListItem : boolItem//, IEquatable<SampleItem>
    //{
    //    public ListItem() : base()
    //    {
    //    }

    //    public ListItem(int index) : base(index)
    //    {
    //    }

    //    public ListItem(int index, bool isInsuranceItem) : base(index)
    //    {
    //        base.IsInsuranceItem = isInsuranceItem;
    //    }

    //    public override bool Equals(Object other)
    //    {
    //        if (this.Index == ((ListItem)other).Index)
    //        {
    //            return true;
    //        }
    //        else
    //        {
    //            return false;
    //        }
    //    }

    //    public override object Clone()
    //    {
    //        return new ListItem(this.Index, this.IsInsuranceItem);
    //    }

    //    public override int getValue()
    //    {
    //        return base.Index;
    //    }

    //    public override string ToString()
    //    {
    //        String insuranceIndicator;
    //        if (base.IsInsuranceItem)
    //        {
    //            insuranceIndicator = INSURANCE_TAG;
    //        }
    //        else
    //        {
    //            insuranceIndicator = "";
    //        }
    //        return (base.Index + 1).ToString() + insuranceIndicator;
    //    }
    //}
}