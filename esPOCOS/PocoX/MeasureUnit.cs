using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using eStore.Utilities;

namespace eStore.POCOS
{
    /// <summary>
    /// Default dimension unit is US system
    /// </summary>
    public class MeasureUnit
    {
        private const decimal kg2lb = 2.205m;
        private const decimal cm2inch = 0.394m;

        /// <summary>
        /// US type adopts inch and lb
        /// SI type adopts cm and kg
        /// </summary>
        public enum UnitType { METRICS, IMPERIAL };

        public MeasureUnit()
        { }

        /// <summary>
        /// EN, International standart, uses CM and KGS, 
        /// US, US system, uses IN and LBS
        /// </summary>
        /// <param name="unicode"></param>
        public MeasureUnit(Decimal length, Decimal width, Decimal height, Decimal weight, UnitType unicode)
        {
            this.UnitCode = unicode;
            this.Height = height;
            this.Length = length;
            this.Width = width;
            this.Weight = weight;
        }
       

        public void Convert(UnitType newType)
        {
            if (this.UnitCode == newType)
                return; //no convertion is needed

            if (this.UnitCode == UnitType.METRICS && newType == UnitType.IMPERIAL)
                ConvertMetrics2Imperial();
            else if (this.UnitCode == UnitType.IMPERIAL && newType == UnitType.METRICS)
                ConvertImperial2Metrics();
            else
            {
                //not supported measurement unit
                eStoreLoger.Warn("Not supported measurement unit convertion", this.UnitCode.ToString(), newType.ToString());
                return;
            }

        }

        private UnitType _unitCode = UnitType.METRICS;  //default value
        public UnitType UnitCode
        {
            get { return _unitCode;}

            set
            {
                _unitCode = value;
                //auto to determine the unit type, if provide unit type
                if (_unitCode == UnitType.IMPERIAL)
                {
                    DimensionUnit = "IN";
                    WeightUnit = "LBS";
                }
                else if (_unitCode == UnitType.METRICS)
                {
                    DimensionUnit = "CM";
                    WeightUnit = "KGS";
                }
            }
        }

        /// <summary>
        /// US used LBS, SI(International standard) used KG
        /// </summary>
        public string WeightUnit
        {
            get;
            set;
        }

        /// <summary>
        /// US used IN, SI(International standard) used CM
        /// </summary>
        public string DimensionUnit
        {
            get;
            set;
        }

        public decimal Length
        {
            get;
            set;
        }

        public decimal Width
        {
            get;
            set;
        }

        public decimal Height
        {
            get;
            set;
        }

        public decimal Weight
        {
            get;
            set;
        }

        /// <summary>
        /// US dimension converts to EN type, BUT WEIGHT DOESN'T CONVERT!!!
        /// </summary>
        /// <param name="_orig"></param>
        /// <returns></returns>
        private void ConvertMetrics2Imperial()
        {
            Length = (decimal)Math.Round(Length * cm2inch, 3);
            Width = (decimal)Math.Round(Width * cm2inch, 3);
            Height = (decimal)Math.Round(Height * cm2inch, 3);
            Weight = (decimal)Math.Round(Weight * kg2lb, 3);
            UnitCode = UnitType.IMPERIAL;
        }

        /// <summary>
        /// EN dimension converts to US type,  BUT WEIGHT DOESN'T CONVERT!!!
        /// </summary>
        /// <param name="_orig"></param>
        /// <returns></returns>
        private void ConvertImperial2Metrics()
        {
            Length = (decimal)Math.Round(Length / cm2inch, 3);
            Width = (decimal)Math.Round(Width / cm2inch, 3);
            Height = (decimal)Math.Round(Height / cm2inch, 3);
            Weight = (decimal)Math.Round(Weight / kg2lb, 3);
            UnitCode = UnitType.METRICS;
        }

        public static Decimal convertKG2LB(decimal weight)
        {
            return weight * kg2lb;
        }

        public static Decimal convertLB2KG(decimal weight)
        {
            return weight / kg2lb;
        }

        public static Decimal convertCM2IN(decimal length)
        {
            return length * cm2inch;
        }

        public static decimal convertIN2CM(decimal length)
        {
            return length / cm2inch;
        }
    }
}
