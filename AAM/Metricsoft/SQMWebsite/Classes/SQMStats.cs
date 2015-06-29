using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SQM.Website
{
    // --------------------------------------------------------------------
    // ------------------- General Statistical Methods --------------------
    // --------------------------------------------------------------------

    public class NStatus
    {
 
    }

    class STStatus
    {
        public const long UNDEFINED = -1;
        public const long UNAVAILABLE = -2;
        public const long END = -5;
        public const long SUCCESS = 1;
        public const long NONE = 0;

        public const long E_UNDEFINED = -2100;
        public const long E_NULLDATASET = -2101;
        public const long E_NOVARIANCE = -2102;
        public const long E_BADVALUE = -2103;
        public const long E_NORESULT = -2111;
        public const long E_INCOMPLETE = -2222;
        public const double NOVALUE = -9999999;

        public const long MINCELLS = 3;
        public const long MAXCELLS = 30;

        public const long DIST_NORMAL = 8;
        public const double MAX_DEV = 27.0;
        public const double NN_PRCSN = .0000000001;

        public const long SORT_ASC = 1;
        public const long SORT_DSC = 2;
    }

    class Calcs
    {

        public static string ZeroFill(long value)
        {
            string segment = "";

            if (value < 0)
            {
                segment = value.ToString();
            }
            else
            {
                segment = "0" + value.ToString();
            }

            return segment;
        }

        public static string Pad(long value)
        {
            string segment = "";

            if (value >= 10)
            {
                segment = value.ToString();
            }
            else
            {
                segment = "0" + value.ToString();
            }

            return segment;
        }


        public static long PosNeg(double value)
        {
            long posOrneg = 0;

            if (value >= 0)
            {
                posOrneg = 1;
            }
            else
            {
                posOrneg = -1;
            }
            return posOrneg;
        }

        public static double Interpolate(double lwrIndex, double lwrValue,
            double uprIndex, double uprValue,
            double index)
        {
            double result, deltaIndex, deltaValue;

            deltaIndex = uprIndex - lwrIndex;
            deltaValue = uprValue - lwrValue;
            result = lwrValue + ((index - lwrIndex) / deltaIndex * deltaValue);

            return result;
        }

    }


    //
    // -------------------- basic statistical calculations ------------------
    // 

    public struct S_histogram
    {
        // histogram description structure 

        public long cellCount;
        public double cellWidth;
        public long[] a_cell;

        public S_histogram(long cellCount, double cellWidth)
        {
            this.cellCount = cellCount;
            this.cellWidth = cellWidth;
            this.a_cell = new long[STStatus.MAXCELLS + 1];
        }
    }

    /// <summary>
    /// Summary description for StatCalc.
    /// </summary>
    public class StatCalc
    {
        protected long dataType;
        protected long controlType;
        protected long status;
        protected string description;
        protected string uom;
        protected double lsl;
        protected long lslInd;
        protected double target;
        protected long targetInd;
        protected double usl;
        protected long uslInd;
        protected double val;
        protected double sizen;
        protected long observations;
        protected double cons;
        protected double sumz;
        protected double sumt;
        protected double sumx;
        protected double sumx2;
        protected double[] a_m = { 0, 0, 0, 0, 0 };
        protected double mean;
        protected double minimum;
        protected double maximum;
        protected double range;
        protected double stdDev;
        protected double variance;
        protected double stdErr;
        protected double coefVar;
        protected double skewness;
        protected double kurtosis;
        protected double KF;
        protected double Z1;
        protected double Z2;
        protected double lower3Sigma;
        protected double upper3Sigma;
        protected double cp;
        protected double cpk;
        protected double cpl;
        protected double cpu;
        protected double cpm;
        protected double cpk90;
        protected double percentOfSpec;
        protected double totalPercent;
        protected double percentBelow;
        protected double percentAbove;
        protected double cellWidth;
        protected double q1;
        protected double median;
        protected double q3;
        protected double statVal;
        protected long defaultHistCellCount;
        protected S_histogram hist;
        protected System.Collections.SortedList valList;



        public StatCalc(long dataType, string description,
            double lsl, long lslInd,
            double target, long targetInd,
            double usl, long uslInd)
        {
            this.dataType = dataType;
            this.description = description;
            this.lsl = lsl;
            this.lslInd = lslInd;
            this.target = targetInd;
            this.usl = usl;
            this.uslInd = uslInd;
            this.status = STStatus.E_NULLDATASET;
            this.observations = 0;
            this.sizen = 0;
            this.cons = 0;
            this.a_m[0] = this.a_m[1] = this.a_m[2] = this.a_m[3] = 0;
            this.sumx = this.sumx2 = this.sumz = this.sumt = 0;
            this.mean = this.range = this.stdDev = this.variance = 0;
            this.cp = this.cpk = this.cpk90 = this.cpl = this.cpu = STStatus.NOVALUE;
            this.defaultHistCellCount = 0;
            this.hist = new S_histogram(0, 0);
            this.valList = new System.Collections.SortedList();
        }

        protected StatCalc()
        {

        }

        public long Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
            }
        }

        public long DataType
        {
            get
            {
                return this.dataType;
            }
            set
            {
                this.dataType = value;
            }
        }

        public long ControlType
        {
            get
            {
                return this.controlType;
            }
            set
            {
                this.controlType = value;
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        public string UOM
        {
            get
            {
                return this.uom;
            }
            set
            {
                this.uom = value;
            }
        }

        public double LSL
        {
            get
            {
                return this.lsl;
            }
            set
            {
                this.lsl = value;
            }
        }
        public long LSLInd
        {
            get
            {
                return this.lslInd;
            }
            set
            {
                this.lslInd = value;
            }
        }

        public double Target
        {
            get
            {
                return this.target;
            }
            set
            {
                this.target = value;
            }
        }
        public long TargetInd
        {
            get
            {
                return this.targetInd;
            }
            set
            {
                this.targetInd = value;
            }
        }

        public double USL
        {
            get
            {
                return this.usl;
            }
            set
            {
                this.usl = value;
            }
        }
        public long USLInd
        {
            get
            {
                return this.uslInd;
            }
            set
            {
                this.uslInd = value;
            }
        }

        public double SizeN
        {
            get
            {
                return this.sizen;
            }
            set
            {
                this.sizen = value;
            }
        }

        public double Stat
        {
            get
            {
                return this.statVal;
            }
        }

        public long DefaultHistCellCount
        {
            get
            {
                return this.defaultHistCellCount;
            }
            set
            {
                this.defaultHistCellCount = value;
            }
        }

        public long HistCellCount
        {
            get
            {
                return this.hist.cellCount;
            }
        }
        public double HistCellWidth
        {
            get
            {
                return this.hist.cellWidth;
            }
        }

        public long HistCellObsv(long cellNo)
        {
            return this.hist.a_cell[cellNo];
        }
        public double HistCellPct(long cellNo)
        {
            return ((double)this.hist.a_cell[cellNo] / (double)this.observations) * 100.0;
        }
        public double HistCellMid(long cellNo)
        {
            // return (double)((cellNo+1) * this.hist.cellWidth + this.minimum);
            return (double)(cellNo * this.hist.cellWidth + this.minimum);
        }


        public void Reset()
        {
            this.status = STStatus.E_NULLDATASET;
            this.observations = 0;
            this.sizen = 0;
            this.cons = 0;
            this.a_m[0] = this.a_m[1] = this.a_m[2] = this.a_m[3] = 0;
            this.sumx = this.sumx2 = this.sumz = this.sumt = 0;
            this.mean = this.range = this.stdDev = this.variance = 0;
            this.cp = this.cpk = this.cpk90 = this.cpl = this.cpu = STStatus.NOVALUE;
            this.defaultHistCellCount = 0;
            this.hist = new S_histogram(0, 0);
            this.valList.Clear();
        }

        public void ClearValues()
        {
            this.valList.Clear();
        }

        public void CopySLFrom(StatCalc sts0)
        {
            this.dataType = sts0.DataType;
            this.controlType = sts0.ControlType;
            this.uom = sts0.UOM;
            this.lsl = sts0.LSL;
            this.lslInd = sts0.LSLInd;
            this.usl = sts0.USL;
            this.uslInd = sts0.USLInd;
            this.target = sts0.Target;
            this.targetInd = sts0.TargetInd;
        }

        public bool ValidStat(string statItem)
        {
            bool isValid = true;
            double val;

            val = GetStat(statItem);
            if (val == STStatus.NOVALUE)
            {
                isValid = false;
            }
            return isValid;
        }

        public double GetStat(string statItem)
        {
            double statVal = 0;

            // check if stats have been caclucated against this dataset 
            if (this.status == STStatus.E_NULLDATASET)
            {
                this.statVal = 0;
                return STStatus.NOVALUE;
            }

            switch (statItem)
            {
                case "STATUS": statVal = (double)this.status; break;
                case "MEAN": statVal = this.mean; break;
                case "OBSRV": statVal = (double)this.observations; break;
                case "MINIMUM": statVal = this.minimum; break;
                case "MAXIMUM": statVal = this.maximum; break;
                case "RANGE": statVal = this.range; break;
                case "STDDEV": statVal = this.stdDev; break;
                case "VARIANCE": statVal = this.variance; break;
                case "STDERR": statVal = this.stdErr; break;
                case "COEFVAR": statVal = this.coefVar; break;
                case "SUM": statVal = this.sumx; break;
                case "SUMX": statVal = this.sumx; break;
                case "SUMX2": statVal = this.sumx2; break;
                case "SIZEN": statVal = this.sizen; break;
                case "SUMZ": statVal = this.sumz; break;
                case "LSL": statVal = this.LSL; break;
                case "TARGET": statVal = this.USL; break;
                case "USL": statVal = this.USL; break;
                case "SUMT": statVal = this.sumt; break;
                case "SUMDEF": statVal = this.sumt; break;
                case "DEFPH": statVal = (this.sumt / this.sumz) * 100; break;
                case "DEFPT": statVal = (this.sumt / this.sumz) * 1000; break;
                case "DEFPM": statVal = (this.sumt / this.sumz) * 1000000; break;
                case "DEFPU": statVal = (this.sumt / this.sumz); break;
                default: statVal = STStatus.NOVALUE; break;
            }

            if (this.status != STStatus.E_NOVARIANCE)
            {
                switch (statItem)
                {
                    case "SKEWNESS": statVal = this.skewness; break;
                    case "KURTOSIS": statVal = this.kurtosis; break;
                    case "ZLOWER": statVal = this.Z1; break;
                    case "ZUPPER": statVal = this.Z2; break;
                    case "LOWER3S": statVal = this.lower3Sigma; break;
                    case "UPPER3S": statVal = this.upper3Sigma; break;
                    case "DISTYPE": statVal = this.KF; break;
                    case "Q1":
                    case "QUARTL1": statVal = this.q1; break;
                    case "MEDIAN": statVal = this.median; break;
                    case "Q3":
                    case "QUARTL3": statVal = this.q3; break;
                    case "CP": statVal = this.cp; break;
                    case "CPK": statVal = this.cpk; break;
                    case "CPL": statVal = this.cpl; break;
                    case "CPU": statVal = this.cpu; break;
                    case "CPK90": statVal = this.cpk90; break;
                    case "PCSPEC": statVal = this.percentOfSpec; break;
                    case "PCLOWER": statVal = this.percentBelow; break;
                    case "PCUPPER": statVal = this.percentAbove; break;
                    case "PCTOTAL": statVal = this.totalPercent; break;
                }
            }

            if (statVal == STStatus.NOVALUE)
            {
                this.statVal = 0;
            }
            else
            {
                this.statVal = statVal;
            }

            return statVal;
        }

        public long AddValue(double val, double sizen)
        {
            this.val = val;
            this.observations++;
            if (this.observations == 1)
            {
                this.cons = this.val;
                this.minimum = this.val;
                this.maximum = this.val;
            }

            this.a_m[0] += (this.val - this.cons);
            this.a_m[1] += Math.Pow((this.val - this.cons), 2);
            this.a_m[2] += Math.Pow((this.val - this.cons), 3);
            this.a_m[3] += Math.Pow((this.val - this.cons), 4);

            this.sumx += this.val;
            this.sumx2 += Math.Pow(this.val, 2);
            this.minimum = Math.Min(this.minimum, this.val);
            this.maximum = Math.Max(this.maximum, this.val);

            // accuumulate sample sizes if provided 
            this.sumz = this.sumz + Math.Max(1, sizen);

            // this.valList.Add(this.observations.ToString(), this.val);
            this.valList.Add(this.valList.Count, this.val);

            return this.observations;
        }

        public long AddAttrib(double tally)
        {
            // add attribute (defects) tally 
            this.sumt = this.sumt + tally;

            return this.observations;
        }


        public long Calculate(bool createHistogram, bool clearValues)
        {
            int n;
            double quantity = 0, temp, temp1, temp2;


            if ((quantity = this.observations) < 1)
            {
                return (this.status = STStatus.E_NULLDATASET);
            }

            this.mean = this.sumx / quantity;
            this.range = Math.Abs(this.maximum - this.minimum);

            // calculate variance estimates
            if (this.range == 0 || quantity < 2)
            {
                this.lower3Sigma = this.upper3Sigma = this.mean;
                return (this.status = STStatus.E_NOVARIANCE);
            }
            this.status = STStatus.SUCCESS;
            temp = this.sumx2 - Math.Pow(this.sumx, 2) / quantity;
            this.variance = temp / (quantity - 1);
            this.stdDev = Math.Sqrt(this.variance);
            this.stdErr = this.stdDev / Math.Sqrt(quantity);
            if (this.mean != 0)
            {
                this.coefVar = (this.stdDev * 100.0) / Math.Abs(this.mean);
            }

            // distribution moments (skewness & kurtosis)
            this.a_m[0] = this.a_m[0] / quantity;
            this.a_m[1] = this.a_m[1] / quantity;
            this.a_m[2] = this.a_m[2] / quantity;
            this.a_m[3] = this.a_m[3] / quantity;

            temp = this.a_m[2] - (3 * this.a_m[0]) * this.a_m[1];
            temp1 = (2 * Math.Pow(this.a_m[0], 3)) + temp;
            temp2 = Math.Pow(this.stdDev, 3);
            this.skewness = temp1 / temp2;
            if (Math.Abs(this.skewness) < .001) this.skewness = 0.0;

            temp = this.a_m[3] - (4 * this.a_m[0]) * this.a_m[2];
            temp1 = (6 * Math.Pow(this.a_m[0], 2) * this.a_m[1]) - (3 * Math.Pow(this.a_m[0], 4));
            temp2 = Math.Pow(this.stdDev, 4);
            this.kurtosis = ((temp + temp1) / temp2) - 3.0;
            if (Math.Abs(this.kurtosis) < .001) this.kurtosis = 0.0;

            // Calculate median and quartiles  
            double[] vals = new double[this.observations];

            for (n = 0; n < this.observations; n++)
            {
                vals[n] = (double)this.valList.GetByIndex(n);
            }
            Array.Sort(vals); // sort values from low to high

            if (observations > 2)
            {
                temp = (double)(this.observations + 1) * .25;
                n = Math.Max(0, (int)temp - 1);
                this.q1 = temp1 = vals[n];
                if (Math.Ceiling(temp) != Math.Floor(temp))
                {
                    this.q1 = (temp1 + vals[n + 1]) / 2;
                }

                temp = (double)this.observations * .50 + .5;
                n = Math.Max(0, (int)temp - 1);
                this.median = temp1 = vals[n];
                if (Math.Ceiling(temp) != Math.Floor(temp))
                {
                    this.median = (temp1 + vals[n + 1]) / 2;
                }

                temp = (double)(this.observations + 1) * .75;
                n = Math.Max(0, (int)temp - 1);
                this.q3 = temp1 = vals[n];
                if (Math.Ceiling(temp) != Math.Floor(temp))
                {
                    this.q3 = (temp1 + vals[n + 1]) / 2;
                }
            }

            // perform distibution assessments assuming normality
            NrmlCalc ncc = new NrmlCalc(this.observations, this.mean, this.stdDev);
            this.KF = ncc.DistType;
            // calculate +/- 3 sigma boundries
            if (ncc.P2X(.00135) == STStatus.SUCCESS)
            {
                this.lower3Sigma = this.mean - (ncc.XX * this.stdDev);
            }
            if (ncc.P2X(1 - .99865) == STStatus.SUCCESS)
            {
                this.upper3Sigma = this.mean + (ncc.XX * this.stdDev);
            }

            // calculate % beyond target specifications
            if (this.lslInd > 0)
            {
                if (ncc.X2P(this.lsl) == STStatus.SUCCESS)
                {
                    this.percentBelow = ncc.PR;
                }
                this.Z1 = (this.lsl - this.mean) / (Math.Abs(this.lower3Sigma - this.mean) / 3.0);
                this.cpk = this.cpl = (this.mean - this.lsl) / Math.Abs(this.mean - this.lower3Sigma);
                this.percentBelow *= 100.0;
                this.totalPercent = this.percentBelow;
            }
            if (this.lslInd > 0)
            {
                if (ncc.X2P(this.usl) == STStatus.SUCCESS)
                {
                    this.percentAbove = ncc.PR;
                }
                this.Z2 = (this.usl - this.mean) / (Math.Abs(this.upper3Sigma - this.mean) / 3.0);
                this.cpk = this.cpu = (this.usl - this.mean) / Math.Abs(this.upper3Sigma - this.mean);
                this.percentAbove = (1 - this.percentAbove) * 100.0;
                this.totalPercent += this.percentAbove;
            }

            // Capability indicies 
            if (this.lslInd > 0 && this.lslInd > 0)
            {
                this.cpk = Math.Min(this.cpl, this.cpu);
                this.cp = Math.Abs(this.usl - this.lsl) / Math.Abs(this.upper3Sigma - this.lower3Sigma);
            }

            if (this.cpk != STStatus.NOVALUE)
            {
                if ((temp = .6031 + .0639 * Math.Log((double)this.observations)) < 1.0)
                {
                    this.cpk90 = this.cpk * temp;
                }
                else
                {
                    this.cpk90 = this.cpk;
                }
            }
            // sanity checks (distribution and spec locations, etc...)
            if (this.lslInd > 0 && this.lsl > this.upper3Sigma)
                this.cp = STStatus.NOVALUE;
            if (this.uslInd > 0 && this.usl < this.lower3Sigma)
                this.cp = STStatus.NOVALUE;
            if (this.cp != STStatus.NOVALUE && this.cp != 0)
                this.percentOfSpec = 100.0 / this.cp;
            if (this.percentBelow == STStatus.NOVALUE && this.percentAbove == STStatus.NOVALUE)
                this.totalPercent = STStatus.NOVALUE;

            if (createHistogram)
            {
                this.CreateHistogram(this.defaultHistCellCount);
            }

            if (clearValues)
            {
                this.ClearValues();
            }

            return this.status;
        }

        public long CreateHistogram(long defaultCellCount)
        {
            int n, ncell;
            double val, cpos;

            // no variance - assume single cell
            if (this.range == 0)
            {
                this.hist.cellCount = 3;
                this.hist.cellWidth = this.mean / 3.0;
                this.hist.a_cell[0] = 0;
                this.hist.a_cell[1] = this.observations;
                this.hist.a_cell[2] = 0;
                return this.hist.cellCount;
            }

            // clear cell tally's in case this method is being rerun
            for (n = 0; n < STStatus.MAXCELLS; n++)
            {
                this.hist.a_cell[n] = 0;
            }

            // assign default cell count if supplied 
            if (defaultCellCount > 0 && defaultCellCount <= STStatus.MAXCELLS)
            {
                this.hist.cellCount = defaultCellCount;
            }
            else
            // calculate cell count based on # of observations (mil-std formula)
            {
                this.hist.cellCount = (long)Math.Round(Math.Pow((double)this.observations, .17) * 3.25, 0) + 1;
                if (this.hist.cellCount < STStatus.MINCELLS)
                    this.hist.cellCount = STStatus.MINCELLS;
                if (this.hist.cellCount > STStatus.MAXCELLS)
                    this.hist.cellCount = STStatus.MAXCELLS;
            }
            this.hist.cellWidth = Math.Abs(this.maximum - this.minimum) / (this.hist.cellCount - 1);
            if (this.range == 0 || this.hist.cellWidth <= 0)
                this.hist.cellWidth = this.mean;

            // walk the values list, calc cell position and increment cell tallys
            for (n = 0; n < this.observations; n++)
            {
                val = (double)this.valList.GetByIndex(n);
                cpos = (val - this.minimum) / this.hist.cellWidth;
                if (cpos < STStatus.MAXCELLS)
                {
                    ncell = (int)Math.Round(cpos, 0);
                    this.hist.a_cell[ncell]++;
                }
            }

            return this.hist.cellCount;
        }
    }

    //
    // --------------------- tests for normality  -----------------------
    // 
    /// Summary description for NrmlCalc.
    /// </summary>
    public class NrmlCalc
    {
        protected long distType;
        protected long observations;
        protected double mean;
        protected double stdDev;
        protected double xx;
        protected double pr;
        protected double tconfLevel;
        protected double tcalc;
        protected double ttable;
        protected double x2confLevel;
        protected double x2calc;
        protected double x2table;
        protected bool acceptHyp;


        public NrmlCalc(long observations, double mean, double stdDev)
        {
            this.distType = STStatus.DIST_NORMAL;
            this.observations = observations;
            this.mean = mean;
            this.stdDev = stdDev;
            this.pr = 0;
        }

        protected NrmlCalc()
        {

        }

        public long Observations
        {
            get
            {
                return this.observations;
            }
            set
            {
                this.observations = value;
            }
        }

        public double Mean
        {
            get
            {
                return this.mean;
            }
            set
            {
                this.mean = value;
            }
        }

        public double StdDev
        {
            get
            {
                return this.stdDev;
            }
            set
            {
                this.stdDev = value;
            }
        }

        public double XX
        {
            get
            {
                return this.xx;
            }
            set
            {
                this.xx = value;
            }
        }

        public double PR
        {
            get
            {
                return this.pr;
            }
            set
            {
                this.pr = value;
            }
        }

        public long DistType
        {
            get
            {
                return this.distType;
            }
            set
            {
                this.distType = value;
            }
        }

        public double TConfLevel
        {
            get
            {
                return this.tconfLevel;
            }
            set
            {
                this.tconfLevel = value;
            }
        }
        public double TCalc
        {
            get
            {
                return this.tcalc;
            }
            set
            {
                this.tcalc = value;
            }
        }
        public double TTable
        {
            get
            {
                return this.ttable;
            }
            set
            {
                this.ttable = value;
            }
        }

        public double Chi2ConfLevel
        {
            get
            {
                return this.x2confLevel;
            }
            set
            {
                this.x2confLevel = value;
            }
        }
        public double Chi2Calc
        {
            get
            {
                return this.x2calc;
            }
            set
            {
                this.x2calc = value;
            }
        }
        public double Chi2Table
        {
            get
            {
                return this.x2table;
            }
            set
            {
                this.x2table = value;
            }
        }
        public bool ChiHypAccept
        {
            get
            {
                return this.acceptHyp;
            }
            set
            {
                this.acceptHyp = value;
            }
        }

        public long X2P(double xx)
        {
            double z, d = 0, pr = 0;
            bool inverse = false;

            if (this.stdDev == 0)
            {
                return STStatus.E_NOVARIANCE;
            }

            // calculate z's
            this.xx = xx;
            z = (this.xx - this.mean) / this.stdDev;
            if (z < 0)
            {
                z = z * (-1);
                inverse = false;
            }
            else
            {
                inverse = true;
            }

            // check for trivial results
            if (z <= STStatus.NN_PRCSN)
            {
                this.pr = .50;
                return STStatus.SUCCESS;
            }
            if (z >= STStatus.MAX_DEV)
            {
                this.pr = 0.0;
                return STStatus.SUCCESS;
            }

            // calculate the probabilty-central area
            d = .5 * z * z;

            if (z <= 1.37)
            {
                pr = .50 - z * (0.39894228044400 - 0.39990343850400 * d /
                    (d + 5.7588548045800 - 29.821355780800 /
                    (d + 2.6243312167900 + 48.695993069200 /
                    (d + 5.9288572443800))));

                /*  calculate the probability-tail area */
            }
            else
            {
                pr = 0.3989422803850 * Math.Exp(-d) /
                    (z - 3.8052e-8 + 1.00000615302 /
                    (z + 3.98064794e-4 + 1.98615381364 /
                    (z - 0.151679116635 + 5.29330324926 /
                    (z + 4.8385912808 - 15.1508972451 /
                    (z + 0.742380924027 + 30.789933034 /
                    (z + 3.99019417011))))));
            }

            if (inverse)
            {
                this.pr = 1.0 - pr;
            }
            else
            {
                this.pr = pr;
            }

            return STStatus.SUCCESS;
        }


        public long P2X(double pr)
        {
            double t, u1, u2;
            double c0 = 2.515517;
            double c1 = .802853;
            double c2 = .0103328;
            double d1 = 1.432788;
            double d2 = .189269;
            double d3 = .001308;

            if (this.stdDev == 0)
            {
                return STStatus.E_NOVARIANCE;
            }

            // check for trivial results
            this.pr = pr;
            if (this.pr <= STStatus.NN_PRCSN)
            {
                this.xx = -27.0;
                return STStatus.SUCCESS;
            }
            if (this.pr >= (1 - STStatus.NN_PRCSN))
            {
                this.xx = 27.0;
                return STStatus.SUCCESS;
            }

            t = Math.Sqrt(Math.Log(1.0 / Math.Pow(this.pr, 2)));
            u1 = c0 + (c1 * t) + (c2 * Math.Pow(t, 2));
            u2 = 1 + (d1 * t) + (d2 * Math.Pow(t, 2)) + (d3 * Math.Pow(t, 3));
            this.xx = (t - (u1 / u2));

            return STStatus.SUCCESS;
        }

        public long TTest(double target, double conf)
        {
            long n, degsf = 0, confn = 0;
            double[] a_conf = { .25, .10, .05, .025, .01, .005 };
            double[,] a_tt = {       
									{1.0,3.078,6.314,12.706,31.821,63.657},
									{1.0,3.078,6.314,12.706,31.821,63.657},
									{.816,1.886,2.920,4.303,6.965,9.925},
									{.765,1.638,2.353,3.182,4.541,5.841},
									{.741,1.533,2.132,2.776,3.747,4.604},
									{.727,1.476,2.015,2.571,3.365,4.032},
									{.718,1.440,1.943,2.447,3.143,3.707},
									{.711,1.415,1.895,2.365,2.998,3.499},
									{.706,1.397,1.860,2.306,2.896,3.355},
									{.703,1.383,1.833,2.262,2.821,3.250},
									{.700,1.372,1.812,2.228,2.764,3.169},
									{.697,1.363,1.796,2.201,2.718,3.106},
									{.695,1.356,1.782,2.179,2.681,3.055},
									{.694,1.350,1.771,2.160,2.650,3.012},
									{.692,1.345,1.761,2.145,2.624,2.977},
									{.691,1.341,1.753,2.131,2.602,2.947},
									{.690,1.337,1.746,2.120,2.583,2.921},
									{.689,1.333,1.740,2.110,2.567,2.898},
									{.688,1.330,1.734,2.101,2.552,2.878},
									{.688,1.328,1.729,2.093,2.539,2.861},
									{.687,1.325,1.725,2.086,2.528,2.845},
									{.686,1.323,1.721,2.080,2.518,2.831},
									{.686,1.321,1.717,2.074,2.508,2.819},
									{.685,1.319,1.714,2.069,2.500,2.807},
									{.685,1.318,1.711,2.064,2.492,2.797},
									{.684,1.316,1.708,2.060,2.485,2.787},
									{.684,1.315,1.706,2.056,2.479,2.779},
									{.684,1.314,1.703,2.052,2.473,2.771},
									{.683,1.313,1.701,2.048,2.467,2.763},
									{.683,1.311,1.699,2.045,2.462,2.756},
									{.683,1.310,1.697,2.042,2.457,2.750},
									{.681,1.303,1.684,2.021,2.423,2.704},
									{.679,1.296,1.671,2.000,2.390,2.660},
									{.677,1.289,1.658,1.980,2.358,2.617},
									{.674,1.282,1.645,1.960,2.326,2.576 } };

            if (this.stdDev == 0)
            {
                return STStatus.E_NOVARIANCE;
            }

            // determine degs of fredom
            degsf = this.observations - 1;
            if (degsf < 1)
            {
                return (STStatus.NONE);
            }

            // get confidence level if suppled, otherwise default to .250 (0 pos)
            for (n = 0; n < 6; n++)
            {
                if (conf == a_conf[n])
                {
                    confn = n;
                    break;
                }
            }
            this.tconfLevel = a_conf[confn];

            // calculate t value
            this.tcalc = (this.mean - target) / (this.stdDev / Math.Sqrt((double)this.observations));

            // interpolate t table value per degs of freedon & conf interval... 
            if (degsf <= 30)
                this.ttable = a_tt[degsf, confn];

            if (degsf > 30 && degsf <= 40)
                this.ttable = Calcs.Interpolate(30.0, (double)a_tt[30, confn],
                    40.0, (double)a_tt[31, confn], (double)degsf);
            if (degsf > 40 && degsf <= 60)
                this.ttable = Calcs.Interpolate(40.0, (double)a_tt[31, confn],
                    60.0, (double)a_tt[32, confn], (double)degsf);
            if (degsf > 60 && degsf <= 120)
                this.ttable = Calcs.Interpolate(60.0, (double)a_tt[32, confn],
                    120.0, (double)a_tt[33, confn], (double)degsf);
            if (degsf > 120)
                this.ttable = a_tt[34, confn];

            // adjust sign for ease of display if negative result
            if (this.tcalc < 0)
            {
                this.ttable *= (-1);
            }

            return STStatus.SUCCESS;
        }


        public long Chi2Test(double target, double conf)
        {
            long n, degsf = 0, confn = 0;
            double[] a_conf = { .995, .990, .975, .950, .050, .025, .010, .005 };
            double[,] a_xt = {
			{.0000393, .00016, .00098, .00393, 3.8414, 5.0239, 6.635, 7.879},
		{.0000393, .00016, .00098, .00393, 3.8414, 5.0239, 6.635, 7.879},
		{.0100251, .02010, .05063, .10259, 5.9915, 7.3777, 9.2103, 10.5966},
		{.0717212, .11483, .21579, .35184, 7.8147, 9.3484, 11.3449, 12.8381},
		{.20699,  .29711, .48442, .71072, 9.4877, 11.1433, 13.2767, 14.8602},
		{.41174, .55430, .831211, 1.14547, 11.0705, 12.8325, 15.0863, 16.7496},
		{.67573, .87208, 1.2373, 1.6354, 12.5916, 14.4494, 16.8119, 18.5476},
		{.98926, 1.2390, 1.6899, 2.1673, 14.0671, 16.0128, 18.4753, 20.2777},
		{1.3444, 1.6464, 2.1797, 2.7426, 15.5073, 17.5346, 20.0902, 21.9550},
		{1.7349, 2.0880, 2.7004, 3.3251, 16.9190, 19.0228, 21.6660, 23.5893},
		{2.1558, 2.5582, 3.2469, 3.9403, 18.3070, 20.4831, 23.2093, 25.1882},
		{2.6032, 3.0535, 3.8157, 4.5748, 19.6751, 21.9200, 24.7250, 26.7569},
		{3.0738, 3.5705, 4.4038, 5.2260, 21.0261, 23.3367, 26.2170, 28.2995},
		{3.5650, 4.1070, 5.0087, 5.8918, 22.3621, 24.7356, 27.6883, 29.8194},
		{4.0747, 4.6604, 5.6287, 6.5706, 23.6848, 26.1190, 29.1413, 31.3193},
		{4.6009, 5.2293, 6.2621, 7.2609, 24.9958, 27.4884, 30.5780, 32.8013},
		{5.1422, 5.8122, 6.9076, 7.9616, 26.2962, 28.8454, 31.9999, 34.2672},
		{5.6972, 6.4077, 7.5642, 8.6717, 27.5871, 30.1910, 33.4087, 35.7185},
		{6.2648, 7.0149, 8.2307, 9.3904, 28.8693, 31.5264, 34.8053, 37.1564},
		{6.8439, 7.6327, 8.9065, 10.1170, 30.1435, 32.8523, 36.1908, 35.5822},
		{7.4338, 8.2604, 9.5908, 10.8508, 31.4104, 34.1696, 37.5662, 39.997},
		{8.0337, 8.8972, 10.2829, 11.5913, 32.6705, 35.4789, 38.9321, 41.401},
		{8.6427, 9.5425, 10.9823, 12.3380, 33.9244, 36.7807, 40.2894, 42.7956},
		{9.2604, 10.1956, 11.6885, 13.905, 35.1725, 38.0757, 41.6384, 44.1813},
		{9.8862, 10.8564, 12.4011, 13.8484, 36.4151, 39.3641, 42.9798, 45.5585},
		{10.5197, 11.524, 13.1197, 14.6114, 37.6525, 40.6465, 44.3141, 46.9278},
		{11.1603, 12.1981, 13.844, 15.3791, 38.8852, 41.9232, 45.6417, 48.290},
		{11.8076, 12.8786, 14.5733, 16.1513, 40.1133, 43.1944, 46.963, 49.6449},
		{12.4613, 13.5648, 15.3079, 16.9279, 41.3372, 44.4607, 48.2782, 50.9933},
		{13.1211, 14.2565, 16.0471, 17.7083, 42.557, 45.7222, 49.5879, 52.3356},
		{13.7867, 14.9535, 16.7908, 18.4926, 43.7729, 46.9792, 50.8922, 53.6720},
		{20.7065, 22.1643, 24.4331, 26.5093, 55.7585, 59.3417, 63.6907, 66.7659},
		{27.9907, 29.7067, 32.3574, 34.7642, 67.5048, 71.4202, 76.1539, 79.490},
		{35.5346, 37.4848, 40.4817, 43.1879, 79.0819, 83.2976, 88.3794, 91.9517},
		{43.2752, 45.4418, 48.7576, 51.7393, 90.5312, 95.0231, 100.425, 104.215},
		{51.1720, 53.540, 57.1532, 60.3915, 101.879, 106.629, 112.329, 116.321},
		{59.1963, 61.7541, 65.6466, 69.126, 113.145, 118.136, 124.116, 128.299},
		{67.3276, 70.0648, 74.2219, 77.9295, 124.342, 129.561, 135.807, 140.169} };

            if (this.stdDev == 0)
            {
                return STStatus.E_NOVARIANCE;
            }

            // determine degs of fredom
            degsf = this.observations - 1;
            if (degsf < 1)
            {
                return (STStatus.NONE);
            }

            // get confidence level if suppled, otherwise default to .250 (0 pos)
            for (n = 0; n < 8; n++)
            {
                if (conf == a_conf[n])
                {
                    confn = n;
                    break;
                }
            }
            this.x2confLevel = a_conf[confn];

            this.x2calc = (degsf * Math.Pow(this.stdDev, 2)) / Math.Pow(target, 2);

            // interpolate X2 table value per degs of freedon & conf interval...
            if (degsf <= 30)
                x2table = (double)a_xt[degsf, confn];
            if (degsf > 30 && degsf <= 40)
                x2table = Calcs.Interpolate(30.0, (double)a_xt[30, confn],
                    40.0, (double)a_xt[31, confn], (double)degsf);
            if (degsf > 40 && degsf <= 50)
                x2table = Calcs.Interpolate(40.0, (double)a_xt[31, confn],
                    50.0, (double)a_xt[32, confn], (double)degsf);
            if (degsf > 50 && degsf <= 60)
                x2table = Calcs.Interpolate(50.0, (double)a_xt[32, confn],
                    60.0, (double)a_xt[33, confn], (double)degsf);
            if (degsf > 60 && degsf <= 70)
                x2table = Calcs.Interpolate(60.0, (double)a_xt[33, confn],
                    70.0, (double)a_xt[34, confn], (double)degsf);
            if (degsf > 70 && degsf <= 80)
                x2table = Calcs.Interpolate(70.0, (double)a_xt[34, confn],
                    80.0, (double)a_xt[35, confn], (double)degsf);
            if (degsf > 80 && degsf <= 90)
                x2table = Calcs.Interpolate(80.0, (double)a_xt[35, confn],
                    90.0, (double)a_xt[36, confn], (double)degsf);
            if (degsf > 90)
                x2table = Calcs.Interpolate(90.0, (double)a_xt[36, confn],
                    100.0, (double)a_xt[37, confn], (double)degsf);

            // accept the hypothesis 
            if (x2calc <= x2table)
            {
                this.acceptHyp = true;
            }
            else
            {
                this.acceptHyp = false;
            }

            return STStatus.SUCCESS;
        }

    }

    // --------------------------------------------------------------------
    // ---------------------  Control Methods -----------------------------
    // --------------------------------------------------------------------

    class CTLStatus
    {
        public const long MAX_TESTWIN = 20;
        public const long TEST_ZONES = 3;
        public const long TESTWIN_LOADED = 10;
        public const long TESTWIN_APPENDED = 11;
        public const long TESTWIN_SLV_ONLY = 12;
        public const long CALC_INCOMPLETE = -2222;
    }

    public enum ProcessControlDataType
    {
        Undefined = 0,
        Variables,
        Defects,
        Defectives
    }

    public enum ControlType
    {
        Default = 0,
        XMovingRange = 1,
        XBarRange = 2,
        XBarSigma = 3,
        CDefects = 4,
        UDefects = 5,
        PDefectives = 6,
        NPDefectives = 7
    }

    class CTLType
    {
        public const long X_MRANGE = 1;
        public const long XB_RANGE = 2;
        public const long XB_SIGMA = 3;
        public const long C_DEFECTS = 4;
        public const long U_DEFECTS = 5;
        public const long P_DEFECTIVES = 6;
        public const long NP_DEFECTIVES = 7;
        public const long SPECS = 10;
        public const long TARGETS = 11;
    }

    class CTLTest
    {
        public const long SPEC_LIMIT_VIOLATION = 1;
        public const long CTL_LIMIT_VIOLATION = 2;
        public const long SHIFT_ZONE_C = 3;
        public const long SHIFT_ZONE_B = 7;
        public const long SHIFT_ZONE_A = 6;
        public const long TREND = 4;
        public const long SYSTEMATIC_VARIABLE = 5;
        public const long STRATIFICATION = 8;
        public const long MIXTURE = 9;
    }


    public struct S_ctlpoint
    {
        // control point structure

        public long pointStatus;
        public long calcStatus;
        public long sampleCount;
        public double[] a_calc;
        public double sumx;
        public double sumx2;
        public double sumz;
        public double minimum;
        public double maximum;
        public long slVioCount;

        public S_ctlpoint(long pointStatus)
        {
            this.pointStatus = pointStatus;
            this.calcStatus = STStatus.E_NORESULT;
            this.sampleCount = slVioCount = 0;
            this.sumx = this.sumx2 = this.sumz = this.minimum = this.maximum = 0;
            this.a_calc = new double[2];
            this.a_calc[0] = this.a_calc[1] = STStatus.NOVALUE;
        }
    }


    public struct S_ctllimit
    {
        // control limits structure 

        public long limitStatus;
        public long calcStatus;
        public long axisSets;
        public double[] a_ctl1;
        public long[] a_ctlInd1;
        public double[] a_ctl2;
        public long[] a_ctlInd2;
        public long pointCount;
        public double sumx;
        public double sumx2;
        public double sumv;
        public double sumz;

        public S_ctllimit(long limitStatus, long controlType,
                           double lcl1, double cl1, double ucl1,
                           double lcl2, double cl2, double ucl2)
        {
            this.limitStatus = limitStatus;
            this.calcStatus = STStatus.E_NORESULT;
            this.axisSets = Controls.AxisSets(controlType);
            this.a_ctl1 = new double[3];
            this.a_ctlInd1 = new long[3];
            this.a_ctl2 = new double[3];
            this.a_ctlInd2 = new long[3];
            this.sumx = this.sumx2 = this.sumv = this.sumz = 0;
            this.pointCount = 0;
            this.a_ctl1[0] = lcl1;
            this.a_ctl1[1] = cl1;
            this.a_ctl1[2] = ucl1;
            this.a_ctl2[0] = lcl2;
            this.a_ctl2[1] = cl2;
            this.a_ctl2[2] = ucl2;
        }
    }

    public struct S_testwin
    {
        // control point structure

        public long state;
        public long pointCount;
        public long testZones;
        public double[] a_pt;
        public double[] a_lpt;
        public long[] a_slv;
        public long[,] a_slope;
        public long[,] a_zone;

        public S_testwin(long pointCount)
        {
            this.state = 0;
            this.pointCount = pointCount;
            this.testZones = CTLStatus.TEST_ZONES;
            this.a_pt = new double[2];
            this.a_lpt = new double[2];
            this.a_slv = new long[CTLStatus.MAX_TESTWIN];
            this.a_slope = new long[2, CTLStatus.MAX_TESTWIN];
            this.a_zone = new long[2, CTLStatus.MAX_TESTWIN];
        }
    }


    /// <summary>
    /// Summary description for ControlCalc.
    /// </summary>
    public class ControlCalc
    {
        protected long dataType;
        protected long controlType;
        protected long status;
        protected string description;
        protected string uom;
        protected double lsl;
        protected long lslInd;
        protected double target;
        protected long targetInd;
        protected double usl;
        protected long uslInd;
        protected double sizen;
        protected long seriesCount;
        protected long pointCount;
        protected double[] ptmaxs = { 0, 0 };
        protected double[] ptmins = { 0, 0 };
        protected string[] seriesBuf = { " ", " " };
        protected S_ctlpoint pt;
        protected S_ctlpoint lpt;
        protected S_ctllimit cl;
        protected S_testwin tw;

        public ControlCalc(long controlType, string description, double sizen,
                        double lsl, long lslInd,
                        double target, long targetInd,
                        double usl, long uslInd, long seriesCount)
        {
            this.controlType = controlType;
            this.description = description;
            this.sizen = sizen;
            this.lsl = lsl;
            this.lslInd = lslInd;
            this.target = target;
            this.targetInd = targetInd;
            this.usl = usl;
            this.uslInd = uslInd;
            this.status = 0;
            this.pointCount = 0;
            this.seriesCount = seriesCount;
            this.pt = new S_ctlpoint(0);
            this.lpt = new S_ctlpoint(0);
            this.cl = new S_ctllimit(0, controlType, 0, 0, 0, 0, 0, 0);
            this.tw = new S_testwin(0);
        }

        protected ControlCalc()
        {

        }

        public long Status
        {
            get
            {
                return this.status;
            }
            set
            {
                this.status = value;
            }
        }

        public long DataType
        {
            get
            {
                return this.dataType;
            }
            set
            {
                this.dataType = value;
            }
        }

        public long ControlType
        {
            get
            {
                return this.controlType;
            }
            set
            {
                this.controlType = value;
                this.cl.axisSets = Controls.AxisSets(this.ControlType);
            }
        }

        public string Description
        {
            get
            {
                return this.description;
            }
            set
            {
                this.description = value;
            }
        }

        public string UOM
        {
            get
            {
                return this.uom;
            }
            set
            {
                this.uom = value;
            }
        }

        public double LSL
        {
            get
            {
                return this.lsl;
            }
            set
            {
                this.lsl = value;
            }
        }
        public long LSLInd
        {
            get
            {
                return this.lslInd;
            }
            set
            {
                this.lslInd = value;
            }
        }

        public double Target
        {
            get
            {
                return this.target;
            }
            set
            {
                this.target = value;
            }
        }
        public long TargetInd
        {
            get
            {
                return this.targetInd;
            }
            set
            {
                this.targetInd = value;
            }
        }

        public double USL
        {
            get
            {
                return this.usl;
            }
            set
            {
                this.usl = value;
            }
        }
        public long USLInd
        {
            get
            {
                return this.uslInd;
            }
            set
            {
                this.uslInd = value;
            }
        }

        public double SizeN
        {
            get
            {
                return this.sizen;
            }
            set
            {
                this.sizen = value;
            }
        }

        public long PointCount
        {
            get
            {
                return this.pointCount;
            }
            set
            {
                this.pointCount = value;
            }
        }

        public long PointCalcStatus
        {
            get
            {
                return this.pt.calcStatus;
            }
            set
            {
                this.pt.calcStatus = value;
            }
        }

        public double GetPointValue(long num)
        {
            return this.pt.a_calc[num];
        }
        public void SetPointValue(long num, double val)
        {
            this.pt.a_calc[num] = val;
        }

        public double GetPointSumz()
        {
            return this.pt.sumz;
        }

        public long SpecVioCount
        {
            get
            {
                return this.pt.slVioCount;
            }
            set
            {
                this.pt.slVioCount = value;
            }
        }

        public string GetSeriesBuffer(long num)
        {
            return this.seriesBuf[num];
        }
        public void SetSeriesBuffer(long num, string buf)
        {
            this.seriesBuf[num] = buf;
        }

        public long LimitStatus
        {
            get
            {
                return this.cl.limitStatus;
            }
            set
            {
                this.cl.limitStatus = value;
            }
        }
        public long LimitCalcStatus
        {
            get
            {
                return this.cl.calcStatus;
            }
            set
            {
                this.cl.calcStatus = value;
            }
        }

        public double GetCLLimit(long axisNo, long limitNo)
        {
            switch (axisNo)
            {
                case 1:
                    return this.cl.a_ctl2[limitNo];
                default:
                    return this.cl.a_ctl1[limitNo];
            }
            return STStatus.NOVALUE;
        }

        public void SetCLLimit(long axisNo, long limitNo, double clVal)
        {
            switch (axisNo)
            {
                case 1:
                    this.cl.a_ctl2[limitNo] = clVal;
                    break;
                default:
                    this.cl.a_ctl1[limitNo] = clVal;
                    break;
            }
        }

        public double PTMinValue(long axisNo)
        {
            return this.ptmins[axisNo];
        }

        public double PTMaxValue(long axisNo)
        {
            return this.ptmaxs[axisNo];
        }

        public void ResetPoint()
        {
            this.pt.pointStatus = 0;
            this.pt.calcStatus = STStatus.E_NORESULT;
            this.pt.sampleCount = this.pt.slVioCount = 0;
            this.pt.sumx = this.pt.sumx2 = this.pt.sumz = 0;
            this.pt.minimum = this.pt.maximum = 0;
            this.pt.a_calc[0] = this.pt.a_calc[1] = 0;
        }

        public long SetLastPointValues(long status, long sampleCount, double sumz,
                                       double calcVal1, double calcVal2)
        {
            // populate prior point values for use by 'moving' chart types
            this.lpt.pointStatus = status;
            this.lpt.calcStatus = STStatus.SUCCESS;
            this.lpt.sampleCount = sampleCount;
            this.lpt.sumz = sumz;
            this.lpt.a_calc[0] = calcVal1;
            this.lpt.a_calc[1] = calcVal2;

            return this.lpt.pointStatus;
        }

        public long SumPointSamples(double x, double sampleN)
        {
            // accumulate sample reading comprising the current control point
            this.pt.sampleCount++;
            this.pt.sumx += x;
            this.pt.sumx2 += Math.Pow(x, 2);
            this.pt.sumz += sampleN;

            if (this.pt.sampleCount == 1)
            {
                this.pt.maximum = this.pt.minimum = x;
            }
            else
            {
                this.pt.minimum = Math.Min(x, this.pt.minimum);
                this.pt.maximum = Math.Max(x, this.pt.maximum);
            }

            // keep tally of spec violations within the subgroup
            this.pt.slVioCount = TestForSLViolation(x);

            return this.pt.sampleCount;
        }

        public long SetPointSums(long samples, double sumx, double sumx2, double sumz,
            double minval, double maxval, long sumVio)
        {
            // accumulate sample reading comprising the current control point
            this.pt.sampleCount = samples;
            this.pt.sumx = sumx;
            this.pt.sumx2 = sumx2;
            this.pt.sumz = sumz;
            this.pt.slVioCount = sumVio;
            this.pt.maximum = maxval;
            this.pt.minimum = minval;
            return 0;
        }

        public void SetCLLimits(long limitStatus, double LCL_1, double CL_1, double UCL_1,
            double LCL_2, double CL_2, double UCL_2)
        {
            this.LimitStatus = limitStatus;
            this.SetCLLimit(0, 0, LCL_1);
            this.SetCLLimit(0, 1, CL_1);
            this.SetCLLimit(0, 2, UCL_1);
            this.SetCLLimit(1, 0, LCL_2);
            this.SetCLLimit(1, 1, CL_2);
            this.SetCLLimit(1, 2, UCL_2);
        }

        public long TestForSLViolation(double val)
        {
            long slVio = 0;

            // keep tally of spec violations within the subgroup
            if (this.lslInd > 0 && val < this.lsl) slVio++;
            if (this.uslInd > 0 && val > this.usl) slVio++;

            return slVio;
        }

        public long CalculatePoint()
        {
            // calculate current control point (i.e. subgroup) values 

            this.pt.calcStatus = Controls.CalculatePoint(this.controlType, this.sizen, this.target, this.pt, this.lpt);
            if (this.pt.calcStatus == STStatus.SUCCESS)
            {
                this.pointCount++;
                if (this.pointCount == 1)
                {
                    this.ptmins[0] = this.ptmaxs[0] = this.pt.a_calc[0];
                    this.ptmins[1] = this.ptmaxs[1] = this.pt.a_calc[1];
                }
                else
                {
                    this.ptmaxs[0] = Math.Max(this.ptmaxs[0], this.pt.a_calc[0]);
                    this.ptmins[0] = Math.Min(this.ptmins[0], this.pt.a_calc[0]);
                    this.ptmaxs[1] = Math.Max(this.ptmaxs[1], this.pt.a_calc[1]);
                    this.ptmins[1] = Math.Min(this.ptmins[1], this.pt.a_calc[1]);
                }
            }

            return this.pt.calcStatus;
        }


        public long SumCLPointValues(double x, double v, double sizen)
        {
            // summarize control points for control limit calculations
            this.cl.sumx += x;
            this.cl.sumv += v;
            this.cl.sumz += sizen;
            this.cl.pointCount++;

            return cl.pointCount;
        }

        public long SetCLPointSums(long pointCount, double sumx, double sumv, double sumz)
        {
            // set point summations for control limit calculations
            // used when performing on-line calcs based on proces state record accumulations
            this.cl.sumx = sumx;
            this.cl.sumv = sumv;
            this.cl.sumz = sumz;
            this.cl.pointCount = pointCount;

            return cl.pointCount;
        }

        public long AppendPointToCLWindow(bool resetpt)
        // add current control point to limit calc window
        {
            long status;

            status = SumCLPointValues(this.pt.a_calc[0], this.pt.a_calc[1],
                                      this.pt.sumz);
            if (resetpt)
            {
                this.lpt.calcStatus = this.pt.calcStatus;
                this.lpt.pointStatus = this.pt.pointStatus;
                this.lpt.sampleCount = this.pt.sampleCount;
                this.lpt.a_calc[0] = this.pt.a_calc[0];
                this.lpt.a_calc[1] = this.pt.a_calc[1];
                this.ResetPoint();
            }
            return status;
        }


        public long CalculateCLLimits(long limitStatus)
        {
            // statistical factors for estimating control limits per ..
            // .. chart type and sample sizes (ref: Grant & Leavenworth)

            this.cl.calcStatus = Controls.CaclulateLimits(this.controlType, limitStatus, this.sizen,
                            this.lsl, this.target, this.usl, this.cl);
            // if new limits ...
            // reset the series test buffers and point counts
            if (this.cl.calcStatus == STStatus.SUCCESS)
            {
                this.RestoreSeries("", "", this.pt.a_calc[0], this.pt.a_calc[1]);
                this.LimitStatus = limitStatus;
            }

            return this.cl.calcStatus;
        }

        public long AddSeriesPoint(double pt1Val, double pt2Val,
                                   double lpt1Val, double lpt2Val,
                                   long slv)
        {
            // add control point to test series window 
            // points may be comprised of 1 or 2 sub-points ..
            // .. representing mean and variance control chart results

            // check if limits are defined for this stream
            // limits must exist to calculate series zones 

            long k, n, islope, izone;
            double temp, zs_upr, zs_lwr;

            //  scroll points to append current if > max series window size
            if (this.tw.pointCount >= CTLStatus.MAX_TESTWIN)
            {
                for (n = 0; n < this.tw.pointCount - 1; n++)
                {
                    this.tw.a_zone[0, n] = this.tw.a_zone[0, n + 1];
                    this.tw.a_zone[1, n] = this.tw.a_zone[1, n + 1];
                    this.tw.a_slope[0, n] = this.tw.a_slope[0, n + 1];
                    this.tw.a_slope[1, n] = this.tw.a_slope[1, n + 1];
                    this.tw.a_slv[n] = this.tw.a_slv[n + 1];
                }
            }
            else
            {
                this.tw.pointCount++;
            }

            n = this.tw.pointCount - 1;      // simple ref to array position
            // set current point values
            this.tw.a_pt[0] = pt1Val;
            this.tw.a_pt[1] = pt2Val;
            // set previous point values 
            this.tw.a_lpt[0] = lpt1Val;
            this.tw.a_lpt[1] = lpt2Val;
            // set spec violation count for the subgroup
            this.tw.a_slv[n] = slv;

            // CALCULATE SLOPE OF NEW POINT
            // note: previous point values must be supplied or set prior 
            // default to slope = 0 
            this.tw.a_slope[0, n] = this.tw.a_slope[1, n] = 0;
            if (this.tw.pointCount > 1)
            {
                // for axis (chart) 1
                if (Math.Round(this.tw.a_pt[0], 6) < Math.Round(this.tw.a_lpt[0], 6)) this.tw.a_slope[0, n] = -1;
                if (Math.Round(this.tw.a_pt[0], 6) > Math.Round(this.tw.a_lpt[0], 6)) this.tw.a_slope[0, n] = 1;

                // for axis (chart) 2
                if (Math.Round(this.tw.a_pt[1], 6) < Math.Round(this.tw.a_lpt[1], 6)) this.tw.a_slope[1, n] = -1;
                if (Math.Round(this.tw.a_pt[1], 6) > Math.Round(this.tw.a_lpt[1], 6)) this.tw.a_slope[1, n] = 1;
            }

            // CALC ZONE SIZES ABOVE & BELOW CENTRAL LIMITS 
            this.tw.a_zone[0, n] = this.tw.a_zone[1, n] = 0;

            // for axis (chart) 1
            if (this.cl.limitStatus > 0 && this.cl.a_ctl1[2] != this.cl.a_ctl1[0])
            {
                zs_upr = (this.cl.a_ctl1[2] - this.cl.a_ctl1[1]) / this.tw.testZones;
                zs_lwr = (this.cl.a_ctl1[1] - this.cl.a_ctl1[0]) / this.tw.testZones;
                // diff from CL line 
                temp = Math.Round((pt1Val - this.cl.a_ctl1[1]), 6);
                if (temp > 0)    // in a zone above CL
                {
                    if (Math.Round(pt1Val, 6) == Math.Round(this.cl.a_ctl1[2], 6))
                    {
                        this.tw.a_zone[0, n] = this.tw.testZones;
                    }
                    else
                    {
                        this.tw.a_zone[0, n] = (long)(temp / zs_upr) + 1;
                    }
                }
                if (temp < 0)          // in a zone below CL
                {
                    if (Math.Round(pt1Val, 6) == Math.Round(this.cl.a_ctl1[0], 6))
                    {
                        this.tw.a_zone[0, n] = this.tw.testZones * -1;
                    }
                    else
                    {
                        this.tw.a_zone[0, n] = (long)(temp / zs_lwr) - 1;
                    }
                }
            }

            // for axis (chart) 2
            if (this.cl.limitStatus > 0 && this.cl.axisSets == 2 &&
                this.cl.a_ctl2[2] != this.cl.a_ctl2[0])
            {
                zs_upr = (this.cl.a_ctl2[2] - this.cl.a_ctl2[1]) / this.tw.testZones;
                zs_lwr = (this.cl.a_ctl2[1] - this.cl.a_ctl2[0]) / this.tw.testZones;
                // diff from CL line 
                temp = Math.Round((pt2Val - this.cl.a_ctl2[1]), 6);
                if (temp > 0)    // in a zone above CL
                {
                    if (Math.Round(pt2Val, 6) == Math.Round(this.cl.a_ctl2[2], 6))
                    {
                        this.tw.a_zone[1, n] = this.tw.testZones;
                    }
                    else
                    {
                        this.tw.a_zone[1, n] = (long)(temp / zs_upr) + 1;
                    }
                }
                if (temp < 0)            // in a zone below CL
                {
                    if (Math.Round(pt2Val, 6) == Math.Round(this.cl.a_ctl2[0], 6))
                    {
                        this.tw.a_zone[1, n] = this.tw.testZones * -1;
                    }
                    else
                    {
                        this.tw.a_zone[1, n] = (long)(temp / zs_lwr) - 1;
                    }
                }
            }

            // Create test series buffers for rapid dabase update & load
            for (k = 0; k < 2; k++)
            {
                this.seriesBuf[k] = "";
                for (n = 0; n < this.tw.pointCount; n++)
                {
                    izone = this.tw.a_zone[k, n];     // (-4 -3 -2 -1 0 +1 +2 +3 +4)
                    if (izone > 4) izone = 4;
                    if (izone < -4) izone = -4;
                    string str = Calcs.ZeroFill(izone);
                    islope = this.tw.a_slope[k, n];   // (-1 0 +1)
                    str = str + Calcs.ZeroFill(islope);
                    this.seriesBuf[k] = this.seriesBuf[k] + str;
                }
            }

            return this.tw.pointCount;
        }


        public long AppendSeriesPoint(bool resetpt)
        {
            // add current control point to the test window series
            long status;

            status = this.AddSeriesPoint(this.pt.a_calc[0], this.pt.a_calc[1],
                                         this.lpt.a_calc[0], this.lpt.a_calc[1],
                                         this.pt.slVioCount);
            this.tw.state = CTLStatus.TESTWIN_APPENDED;
            if (resetpt)
            {
                this.lpt.calcStatus = this.pt.calcStatus;
                this.lpt.pointStatus = this.pt.pointStatus;
                this.lpt.sampleCount = this.pt.sampleCount;
                this.lpt.a_calc[0] = this.pt.a_calc[0];
                this.lpt.a_calc[1] = this.pt.a_calc[1];
                this.ResetPoint();
            }
            return status;
        }

        public long InsertSeriesSLV(long specVio, long seriesPos)
        {
            // insert spec violation count into test series 
            // current series < 0 > specific series position
            long n;

            if (seriesPos > 0)
            {
                n = Math.Max(0, seriesPos - 1);
            }
            else
            {
                n = Math.Max(0, this.tw.pointCount - 1);
            }

            this.tw.a_slv[n] = specVio;
            this.tw.state = CTLStatus.TESTWIN_SLV_ONLY;

            return n;
        }

        public long RestoreSeries(string seriesBuffer1, string seriesBuffer2,
                                  double lpt1Val, double lpt2Val)
        {
            // decode test series buffers to formulate zone & slope indicators
            int i, k, n, len;

            this.seriesBuf[0] = seriesBuffer1;
            this.seriesBuf[1] = seriesBuffer2;
            this.tw.a_lpt[0] = lpt1Val;
            this.tw.a_lpt[1] = lpt2Val;
            this.tw.pointCount = 0;
            for (n = 0; n < CTLStatus.MAX_TESTWIN; n++)
            {
                this.tw.a_zone[0, n] = this.tw.a_zone[1, n] = 0;
                this.tw.a_slope[0, n] = this.tw.a_slope[1, n] = 0;
                this.tw.a_slv[n] = 0;
            }

            for (k = 0; k < 2; k++)
            {
                n = 0;
                if ((len = this.seriesBuf[k].Length) > 1)
                {
                    for (i = 0; i < len; i += 4)
                    {
                        this.tw.a_zone[k, n] = long.Parse(this.seriesBuf[k].Substring(i, 2));
                        this.tw.a_slope[k, n] = long.Parse(this.seriesBuf[k].Substring(i + 2, 2));
                        n++;
                        if (k == 0) this.tw.pointCount++;
                    }
                }
            }

            this.tw.state = CTLStatus.TESTWIN_LOADED;
            return this.tw.pointCount;
        }

        /*
        public bool EvaluateTest(ControlTest tst, long testAxis)
        {
            // evaluate test condidtion 
            bool testTriggered = false;

            // do not exec tests for undefinded axis (chart)
            if ((testAxis > this.cl.axisSets) ||
                (testAxis == 0 && tst.TestType > 1))
            {
                return false;
            }

            long n, seedval = 0, pattern_tally = 0, startZone = 0;
            long axisNum = testAxis - 1;
            long start_point = Math.Max(0, this.tw.pointCount - 1);
            long end_point = Math.Max(0, (start_point - tst.SeriesPoints) + 1);

            // note: if the tw.state = TESTWIN_SLV_ONLY, perform spec vio tests only

            switch (tst.TestType)
            {
                case CTLTest.SPEC_LIMIT_VIOLATION:
                    // LSL or USL violation  (1)
                    // only applied to test axis 0
                    if (testAxis == 0)
                    {
                        for (n = start_point; n >= end_point && n >= 0; n--)
                        {
                            if (this.tw.a_slv[n] > 0)
                            {
                                ++pattern_tally;
                            }
                        }
                    }
                    break;
                case CTLTest.CTL_LIMIT_VIOLATION:
                    // control limit violation  (2)
                    if (this.cl.limitStatus > 0 && this.tw.state != CTLStatus.TESTWIN_SLV_ONLY)
                    {
                        for (n = start_point; n >= end_point && n >= 0; n--)
                        {
                            if (Math.Abs(this.tw.a_zone[axisNum, n]) > this.tw.testZones)
                            {
                                ++pattern_tally;
                            }
                        }
                    }
                    break;
                case CTLTest.SHIFT_ZONE_C:
                    // Shift n of N pts in zone C or beyond  (3)
                    if (this.cl.limitStatus > 0 && this.tw.state != CTLStatus.TESTWIN_SLV_ONLY)
                    {
                        startZone = Calcs.PosNeg(this.tw.a_zone[axisNum, start_point]);
                        for (n = start_point; n >= end_point && n >= 0; n--)
                        {
                            if (Math.Abs(this.tw.a_zone[axisNum, n]) >= 1 &&
                                Calcs.PosNeg(this.tw.a_zone[axisNum, n]) == startZone)
                            {
                                ++pattern_tally;
                            }
                        }
                    }
                    break;
                case CTLTest.SHIFT_ZONE_B:
                    // Shift - n of N pts in zone B or beyond (7)
                    if (this.cl.limitStatus > 0 && this.tw.state != CTLStatus.TESTWIN_SLV_ONLY)
                    {
                        startZone = Calcs.PosNeg(this.tw.a_zone[axisNum, start_point]);
                        for (n = start_point; n >= end_point && n >= 0; n--)
                        {
                            if (Math.Abs(this.tw.a_zone[axisNum, n]) >= 2 &&
                                Calcs.PosNeg(this.tw.a_zone[axisNum, n]) == startZone)
                            {
                                ++pattern_tally;
                            }
                        }
                    }
                    break;
                case CTLTest.SHIFT_ZONE_A:
                    // Shift - n of Npts in zone A or beyond (6)
                    if (this.cl.limitStatus > 0 && this.tw.state != CTLStatus.TESTWIN_SLV_ONLY)
                    {
                        startZone = Calcs.PosNeg(this.tw.a_zone[axisNum, start_point]);
                        for (n = start_point; n >= end_point && n >= 0; n--)
                        {
                            if (Math.Abs(this.tw.a_zone[axisNum, n]) >= 3 &&
                                Calcs.PosNeg(this.tw.a_zone[axisNum, n]) == startZone)
                            {
                                ++pattern_tally;
                            }
                        }
                    }
                    break;
                case CTLTest.TREND:
                    // N consecutive pts steadily increasing/decreasing  (4)
                    // Trend 
                    if (this.tw.state != CTLStatus.TESTWIN_SLV_ONLY)
                    {
                        ++pattern_tally;   // count the 1st point 
                        seedval = this.tw.a_slope[axisNum, start_point];
                        for (n = start_point; n >= end_point && n >= 0; n--)
                        {
                            if (seedval != 0 && this.tw.a_slope[axisNum, n] == seedval)
                            {
                                ++pattern_tally;
                            }
                            else   // break eval loop when pattern is broken
                            {
                                break;
                            }
                        }
                    }
                    break;
                case CTLTest.SYSTEMATIC_VARIABLE:
                    // Systematic Variable - n consecutive pts alternating up and down (5)
                    if (this.tw.state != CTLStatus.TESTWIN_SLV_ONLY)
                    {
                        seedval = 99;  // set arbitrary seed number > than series window
                        for (n = start_point; n >= end_point && n >= 0; n--)
                        {
                            if (this.tw.a_slope[axisNum, n] != seedval &&
                                this.tw.a_slope[axisNum, n] != 0)
                            {
                                ++pattern_tally;
                                seedval = this.tw.a_slope[axisNum, n];
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    break;
                case CTLTest.STRATIFICATION:
                    // Stratification - n consecutive pts in zone C above & below centerline (8)
                    if (this.cl.limitStatus > 0 && this.tw.state != CTLStatus.TESTWIN_SLV_ONLY)
                    {
                        for (n = start_point; n >= end_point && n >= 0; n--)
                        {
                            if (Math.Abs(this.tw.a_zone[axisNum, n]) < 2)
                                ++pattern_tally;
                        }
                    }
                    break;
                case CTLTest.MIXTURE:
                    // Mixture -consecutive pts beyond zone C  (9)
                    if (this.cl.limitStatus > 0 && this.tw.state != CTLStatus.TESTWIN_SLV_ONLY)
                    {
                        for (n = start_point; n >= end_point && n >= 0; n--)
                        {
                            if (Math.Abs(this.tw.a_zone[axisNum, n]) > 1)
                                ++pattern_tally;
                        }
                    }
                    break;
                default:
                    pattern_tally = -1;
                    break;
            }

            // was process test triggered ? 
            if (pattern_tally >= tst.TriggerPoints)
            {
                testTriggered = true;
            }

            return testTriggered;
        }
        */
    }

    // 
    // control types classes 
    //
    class Controls
    {

        public static long AxisSets(long controlType)
        {
            // get number of axis (chart) sets per the control type

            long axisSets;

            switch (controlType)
            {
                case CTLType.X_MRANGE:   // x & moving range
                case CTLType.XB_RANGE:   // xbar & range
                case CTLType.XB_SIGMA:   // xbar & sigma
                    axisSets = 2;
                    break;
                case CTLType.C_DEFECTS:  // c chart
                case CTLType.U_DEFECTS:   // u chart
                case CTLType.P_DEFECTIVES:   // p chart
                case CTLType.NP_DEFECTIVES:   // np chart
                    axisSets = 1;
                    break;
                case CTLType.SPECS:   // spec chart
                case CTLType.TARGETS:  // target chart
                    axisSets = 1;
                    break;
                default:  // catch any undefined control types
                    axisSets = 1;
                    break;
            }
            return axisSets;
        }

        public static long CalculatePoint(long controlType, double sizen, double target,
                                          S_ctlpoint pt, S_ctlpoint lpt)
        {
            // calculate current control point (i.e. subgroup) values 
            double mean, range, sdev = 0, vars = 0, temp;

            pt.calcStatus = CTLStatus.CALC_INCOMPLETE;

            // calculate basic statistics for the point 

            // use constant sgrp size if specified 
            if (sizen > 0)
            {
                pt.sumz = sizen;
            }

            mean = pt.sumx / pt.sumz;
            range = Math.Abs(pt.maximum - pt.minimum);
            temp = pt.sumx2 - (Math.Pow(pt.sumx, 2) / pt.sumz);
            vars = temp / (Math.Max(1, pt.sumz - 1));
            if (vars != 0) sdev = Math.Sqrt(vars);

            switch (controlType)
            {
                case CTLType.X_MRANGE:  // x and moving range
                    pt.a_calc[0] = mean;
                    if (lpt.sampleCount == 0)
                    {
                        pt.a_calc[1] = 0;
                    }
                    else
                    {
                        pt.a_calc[1] = Math.Abs(mean - lpt.a_calc[0]);
                    }
                    pt.calcStatus = STStatus.SUCCESS;
                    break;
                case CTLType.XB_RANGE:  // xbar and range 
                    if (pt.sumz > 1)
                    {
                        pt.a_calc[0] = mean;
                        pt.a_calc[1] = range;
                        pt.calcStatus = STStatus.SUCCESS;
                    }
                    else
                    {
                        // calc as moving range if sample size = 1
                        pt.a_calc[0] = mean;
                        if (lpt.sampleCount == 0)
                        {
                            pt.a_calc[1] = 0;
                        }
                        else
                        {
                            pt.a_calc[1] = Math.Abs(mean - lpt.a_calc[0]);
                        }
                        pt.calcStatus = STStatus.SUCCESS;
                    }
                    break;
                case CTLType.XB_SIGMA:  // xbar and sigma
                    if (pt.sumz > 1)
                    {
                        pt.a_calc[0] = mean;
                        pt.a_calc[1] = sdev;
                        pt.calcStatus = STStatus.SUCCESS;
                    }
                    else
                    {
                        // calc as moving range if sample size = 1
                        pt.a_calc[0] = mean;
                        if (lpt.sampleCount == 0)
                        {
                            pt.a_calc[1] = 0;
                        }
                        else
                        {
                            pt.a_calc[1] = Math.Abs(mean - lpt.a_calc[0]);
                        }
                        pt.calcStatus = STStatus.SUCCESS;
                    }
                    break;
                case CTLType.C_DEFECTS:  // count of defects (c)
                    pt.a_calc[0] = pt.sumx;
                    pt.a_calc[1] = 0;
                    pt.calcStatus = STStatus.SUCCESS;
                    break;
                case CTLType.U_DEFECTS:  // avg # of defects (u)
                    pt.a_calc[0] = mean;
                    pt.a_calc[1] = 0;
                    pt.calcStatus = STStatus.SUCCESS;
                    break;
                case CTLType.P_DEFECTIVES:  // avg # of defectives (p)
                    pt.a_calc[0] = mean * 100;
                    pt.a_calc[1] = 0;
                    pt.calcStatus = STStatus.SUCCESS;
                    break;
                case CTLType.NP_DEFECTIVES:  // count of defectives (np)
                    pt.a_calc[0] = pt.sumx;
                    pt.a_calc[1] = 0;
                    pt.calcStatus = STStatus.SUCCESS;
                    break;
                case CTLType.SPECS:  // spec chart
                    pt.a_calc[0] = mean;
                    pt.a_calc[1] = 0;
                    pt.calcStatus = STStatus.SUCCESS;
                    break;
                case CTLType.TARGETS:  // dev from target
                    pt.a_calc[0] = mean - target;
                    pt.a_calc[1] = 0;
                    pt.calcStatus = STStatus.SUCCESS;
                    break;
                default:  // undefined
                    pt.a_calc[0] = mean;
                    pt.a_calc[1] = 0;
                    pt.calcStatus = STStatus.SUCCESS;
                    break;
            }

            return pt.calcStatus;
        }


        public static long CaclulateLimits(long controlType, long limitStatus, double sizen,
                                           double lsl, double target, double usl,
                                           S_ctllimit cl)
        {
            // statistical factors for estimating control limits per ..
            // .. chart type and sample sizes (ref: Grant & Leavenworth)
            double[] a_a2 = { 0, 2.66, 1.88, 1.023, 0.729, 0.577, 0.483, 0.419, 0.373, 0.337, 0.308, 0.285, 0.266, 0.249, 0.235, 0.223, 0.212, 0.203, 0.194, 0.187, 0.180 };
            double[] a_a3 = { 0, 1.0, 2.659, 1.954, 1.628, 1.427, 1.287, 1.182, 1.099, 1.032, 0.975, 0.927, 0.886, 0.850, 0.817, 0.789, 0.763, 0.739, 0.718, 0.698, 0.680 };
            double[] a_c2 = {0, 0.5642, 0.5642, 0.7236, 0.7979, 0.8407, 0.8686, 0.8882, 0.9027, 0.9139, 0.9227, 0.930, 0.9359, 0.9410, 0.9453, 0.9490, 0.9523, 0.9551, 
								 0.9576, 0.960, 0.9619, 0.9638, 0.9655, 0.967, 0.9684, 0.9696, 0.9748, 0.9784, 0.9811, 0.9832, 0.9849};
            double[] a_c4 = {0, 1.0, 0.7979, 0.8862, 0.9213, 0.9400, 0.9515, 0.9594, 0.9650, 0.9693, 0.9727, 0.9754, 0.9776, 0.9794, 
								 0.9810, 0.9823, 0.9835, 0.9845, 0.9854, 0.9862, 0.9869, 0.9876, 0.9882, 0.9887, 0.9892, 0.9896, 0.9903, 0.9909, 0.9914, 0.9918, 0.9923};
            double[] a_d2 = {0, 1.128, 1.128, 1.693, 2.059, 2.326, 2.534, 2.704, 2.847, 2.97, 3.078, 3.173, 3.258, 3.336, 3.407, 3.472, 
								 3.532, 3.588, 3.64, 3.69, 3.735, 3.778, 3.819, 3.858, 3.895, 3.931, 4.086, 4.213, 4.322, 4.415, 4.498};
            double[] a_d3 = {0, 1.0, 0.853, 0.888, 0.880, 0.864, 0.848, 0.833, 0.820, 0.808, 0.797, 0.787, 0.778, 0.770, 0.763, 0.756, 
								 0.750, 0.744, 0.739, 0.734, 0.729, 0.724, 0.720, 0.716, 0.712, 0.708};
            double[] a_ld3 = {0, 0, 0, 0, 0, 0, 0, 0.076, 0.136, 0.184, 0.223, 0.256, 0.284, 0.308, 0.329, 0.348, 0.364, 0.379, 0.392, 
								  0.404, 0.414, 0.425, 0.434, 0.443, 0.451, 0.459, 0.5108, 0.5483, 0.5808, 0.6095, 0.6352};
            double[] a_ld4 = {0, 3.267, 3.267, 2.575, 2.282, 2.115, 2.0, 1.924, 1.864, 1.816, 1.777, 1.744, 1.716, 1.692, 1.671, 1.652, 
								  1.636, 1.621, 1.61, 1.596, 1.586, 1.575, 1.566, 1.557, 1.548, 1.541, 1.4884, 1.4526, 1.4223, 1.3961, 1.3731};
            double[] a_lb3 = {0, 0, 0, 0, 0, 0, 0.03, 0.118, 0.185, 0.239, 0.284, 0.321, 0.354, 0.382, 0.406, 0.428, 0.448, 0.466, 0.482, 
								  0.497, 0.510, 0.523, 0.534, 0.545, 0.555, 0.565, 0.630, 0.6762, 0.7165, 0.7520, 0.7837};
            double[] a_lb4 = {0, 1.0, 3.267, 2.568, 2.266, 2.09, 1.97, 1.882, 1.815, 1.761, 1.716, 1.679, 1.646, 1.618, 1.594, 1.572, 
								  1.552, 1.534, 1.518, 1.503, 1.49, 1.477, 1.466, 1.455, 1.445, 1.435, 1.3765, 1.3353, 1.3, 1.2707, 1.2445};

            long ns;
            double temp = 0, avmean = 0, avvars = 0, avsizen = 0;

            cl.calcStatus = STStatus.E_NORESULT;

            // cannot calculate limits if no variance 
            if (cl.pointCount < 2 || controlType < 1)
            {
                return cl.calcStatus;
            }

            // common calculations
            avmean = cl.sumx / (long)cl.pointCount;
            avvars = cl.sumv / (long)cl.pointCount;
            avsizen = cl.sumz / (long)cl.pointCount;

            // determine variables datatype factors index if sample size is variable 
            if ((ns = (long)sizen) == 0)
            {
                ns = (long)cl.sumz / cl.pointCount;
            }
            ns = Math.Min(ns, 20);

            switch (controlType)
            {
                case CTLType.X_MRANGE:  // x and moving range
                    if (cl.sumv > 0)
                    {
                        cl.a_ctl1[2] = avmean + (2.66 * avvars);
                        cl.a_ctl1[1] = avmean;
                        cl.a_ctl1[0] = avmean - (2.66 * avvars);
                        cl.a_ctl2[2] = avvars * 3.267;
                        cl.a_ctl2[1] = avvars;
                        cl.a_ctl2[0] = 0;
                        cl.calcStatus = STStatus.SUCCESS;
                    }
                    break;
                case CTLType.XB_RANGE:  // xbar and  range
                    if (cl.sumv > 0)
                    {
                        cl.a_ctl1[2] = avmean + (a_a2[ns] * avvars);
                        cl.a_ctl1[1] = avmean;
                        cl.a_ctl1[0] = avmean - (a_a2[ns] * avvars);
                        cl.a_ctl2[2] = avvars * a_ld4[ns];
                        cl.a_ctl2[1] = avvars;
                        cl.a_ctl2[0] = avvars * a_ld3[ns];
                        cl.calcStatus = STStatus.SUCCESS;
                    }
                    break;
                case CTLType.XB_SIGMA:  // xbar and sdev
                    if (cl.sumv > 0)
                    {
                        cl.a_ctl1[2] = avmean + (a_a3[ns] * avvars);
                        cl.a_ctl1[1] = avmean;
                        cl.a_ctl1[0] = avmean - (a_a3[ns] * avvars);
                        cl.a_ctl2[2] = avvars * a_lb4[ns];
                        cl.a_ctl2[1] = avvars;
                        cl.a_ctl2[0] = avvars * a_lb3[ns];
                        cl.calcStatus = STStatus.SUCCESS;
                    }
                    break;
                case CTLType.C_DEFECTS:  // count of defects (c)
                    if (cl.sumx > 0)
                    {
                        cl.a_ctl1[2] = avmean + (3 * Math.Sqrt(avmean));
                        cl.a_ctl1[1] = avmean;
                        cl.a_ctl1[0] = avmean - (3 * Math.Sqrt(avmean));
                        cl.a_ctl1[0] = Math.Max(cl.a_ctl1[0], 0);
                        cl.a_ctl1[0] = Math.Max(cl.a_ctl1[0], 0);
                        cl.a_ctl2[2] = 0;
                        cl.a_ctl2[1] = 0;
                        cl.a_ctl2[0] = 0;
                        cl.calcStatus = STStatus.SUCCESS;
                    }
                    break;
                case CTLType.U_DEFECTS:  // avg # of defects (u)
                    if (cl.sumx > 0 && cl.sumz > 0)
                    {
                        cl.a_ctl1[2] = avmean + (3 * Math.Sqrt(avmean / avsizen));
                        cl.a_ctl1[1] = avmean;
                        cl.a_ctl1[0] = avmean - (3 * Math.Sqrt(avmean / avsizen));
                        cl.a_ctl1[0] = Math.Max(cl.a_ctl1[0], 0);
                        cl.a_ctl2[2] = 0;
                        cl.a_ctl2[1] = 0;
                        cl.a_ctl2[0] = 0;
                        cl.calcStatus = STStatus.SUCCESS;
                    }
                    break;
                case CTLType.P_DEFECTIVES:  // avg # of defectives (p)
                    if (cl.sumx > 0 && cl.sumz > 0)
                    {
                        avmean = cl.sumx / cl.sumz;
                        temp = avmean * ((1 - avmean) / avsizen);
                        cl.a_ctl1[2] = avmean + (3 * Math.Sqrt(temp));
                        cl.a_ctl1[1] = avmean;
                        cl.a_ctl1[0] = avmean - (3 * Math.Sqrt(temp));
                        cl.a_ctl2[2] = 0;
                        cl.a_ctl2[1] = 0;
                        cl.a_ctl2[0] = 0;
                        cl.calcStatus = STStatus.SUCCESS;
                    }
                    break;
                case CTLType.NP_DEFECTIVES:  // count of defectives (np)
                    if (cl.sumx > 0)
                    {
                        avvars = avmean / Math.Max(1, sizen);
                        temp = avmean * (1 - avvars);
                        cl.a_ctl1[2] = avmean + (3 * Math.Sqrt(temp));
                        cl.a_ctl1[1] = avmean;
                        cl.a_ctl1[0] = avmean - (3 * Math.Sqrt(temp));
                        cl.a_ctl1[0] = Math.Max(cl.a_ctl1[0], 0);
                        cl.a_ctl2[2] = 0;
                        cl.a_ctl2[1] = 0;
                        cl.a_ctl2[0] = 0;
                        cl.calcStatus = STStatus.SUCCESS;
                    }
                    break;
                case CTLType.SPECS:  // spec chart
                    cl.a_ctl1[0] = lsl;
                    cl.a_ctl1[1] = target;
                    cl.a_ctl1[2] = usl;
                    cl.calcStatus = STStatus.SUCCESS;
                    break;
                case CTLType.TARGETS:  // dev from target
                    cl.a_ctl1[0] = target + target;
                    cl.a_ctl1[1] = target;
                    cl.a_ctl1[2] = target - target;
                    cl.calcStatus = STStatus.SUCCESS;
                    break;
            }

            // assign 'new' limit status if calculated successfully
            if (cl.calcStatus == STStatus.SUCCESS)
            {
                cl.limitStatus = limitStatus;
                cl.axisSets = Controls.AxisSets(controlType);
            }

            return cl.calcStatus;
        }
    }


}