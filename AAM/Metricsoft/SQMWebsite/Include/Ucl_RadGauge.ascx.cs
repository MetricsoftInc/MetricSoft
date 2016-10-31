using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Charting;
using Telerik.Web.UI;
using Telerik.Web.UI.HtmlChart;

namespace SQM.Website
{

    public enum SQMChartType {None, Section=1, Table=5, Linear=10, 
                        BarGraph=11, BarGraphStacked=12, BarGraphGrouped=13, BarGraphStackedInv=14, BarGraphPareto=15,
                        Column=20, ColumnChart=21, ColumnChartStacked=22, ColumnChartGrouped=23, ColumnChartStackedInv=24, ColumnChartPareto=25,
                        MultiLine=32, MultiLineInv=34, PieChart=50, Radial=60, SpiderChart = 70};

    public class GaugeDefinition
    {
        public int ControlType
        {
            get;
            set;
        }
        public string Orientation
        {
            get;
            set;
        }
        public string Position
        {
            get;
            set;
        }
        public bool NewRow
        {
            get;
            set;
        }
        public int Height
        {
            set;
            get;
        }
        public int Width
        {
            get;
            set;
        }
        public decimal? ScaleMin
        {
            get;
            set;
        }
        public decimal ScaleMax
        {
            get;
            set;
        }
        public decimal Unit
        {
            get;
            set;
        }
		public bool ScaleToMax
		{
			get;
			set;
		}
        public decimal? Multiplier
        {
            get;
            set;
        }
        public string ItemVisual
        {
            get;
            set;
        }
        public bool TotalIndicator
        {
            get;
            set;
        }
        public bool MinorTics
        {
            get;
            set;
        }
        public string Title
        {
            get;
            set;
        }
        public string LabelV    // variable axis label
        {
            get;
            set;
        }
        public string LabelA    // ordinal axis label
        {
            get;
            set;
        }
        public Dictionary<string, string> LabelSubList
        {
            get;
            set;
        }
        public bool DisplayTitle 
        {
            get;
            set;
        }
        public bool DisplayLabel
        {
            get;
            set;
        }
        public bool DisplayLegend
        {
            get;
            set;
        }
		public int TopN
		{
			get;
			set;
		}
		public ChartLegendPosition LegendPosition { get; set; }
		public Color LegendBackgroundColor { get; set; }
		public bool DisplayTooltip { get; set; }
		public Color TooltipBackgroundColor { get; set; }
        public string ValueFormat
        {
            get;
            set;
        }
        public string DefaultValueFormat
        {
            get;
            set;
        }
        public string ColorPallete
        {
            get;
            set;
        }
        public string DefaultScaleColor
        {
            get;
            set;
        }
        public string DefaultPointColor
        {
            get;
            set;
        }
        public List<GaugeIndicator> IndicatorList
        {
            get;
            set;
        }
        public int SeqNum
        {
            get;
            set;
        }
        public int Grouping
        {
            get;
            set;
        }
        public int OverlaySeries  // series number to overlay as line graph
        {
            get;
            set;
        }
        public PERSPECTIVE_TARGET Target
        {
            get;
            set;
        }
        public int ContainerHeight
        {
            get;
            set;
        }
        public int ContainerWidth
        {
            get;
            set;
        }
		public string OnLoad { get; set; }

		public GaugeDefinition()
		{
			this.IndicatorList = new List<GaugeIndicator>();
			this.DefaultScaleColor = "#20B2AA"; // "#008B8B"; // "#191970";
			this.DefaultPointColor = "#2F4F4F"; //"#CC6600";
			this.DefaultValueFormat = "#.#";
			// this.ColorPallete = "chartSeriesColor";
			this.MinorTics = true;
			this.DisplayTitle = false;
			this.DisplayLegend = true;
			this.LegendPosition = ChartLegendPosition.Bottom;
			this.LegendBackgroundColor = Color.White;
			this.DisplayTooltip = true;
			this.TooltipBackgroundColor = Color.White;
			this.NewRow = false;
			this.DisplayLabel = true;
			this.Target = null;
			this.LabelSubList = new Dictionary<string, string>();
			this.TotalIndicator = false;
			this.OverlaySeries = 0;
			this.ContainerHeight = this.ContainerWidth = 0;
			this.OnLoad = null;
			this.ScaleToMax = false;
			this.ItemVisual = "";
			this.TopN = 0;
		}

		public GaugeDefinition Initialize()
        {
			return new GaugeDefinition();
        }

		public GaugeDefinition InitializeAll()
		{
			this.IndicatorList = new List<GaugeIndicator>();
			this.DefaultScaleColor = "#20B2AA"; // "#008B8B"; // "#191970";
			this.DefaultPointColor = "#2F4F4F"; //"#CC6600";
			this.DefaultValueFormat = "#.#";
			// this.ColorPallete = "chartSeriesColor";
			this.MinorTics = true;
			this.DisplayTitle = false;
			this.DisplayLegend = true;
			this.LegendPosition = ChartLegendPosition.Bottom;
			this.LegendBackgroundColor = Color.White;
			this.DisplayTooltip = true;
			this.TooltipBackgroundColor = Color.White;
			this.NewRow = false;
			this.DisplayLabel = true;
			this.Target = null;
			this.LabelSubList = new Dictionary<string, string>();
			this.TotalIndicator = false;
			this.OverlaySeries = 0;
			this.ContainerHeight = this.ContainerWidth = 0;
			this.OnLoad = null;
			this.ScaleToMax = false;
			this.ItemVisual = "";
			this.ScaleMin = this.ScaleMax = 0;
			this.TopN = 0;

			return this;
		}

        public int AddIndicator(GaugeIndicator rgi)
        {
            this.IndicatorList.Add(rgi);
            return this.IndicatorList.Count;
        }

        public GaugeDefinition ConfigureControl(PERSPECTIVE_VIEW_ITEM vi, TargetMgr targetCtl, string addTitle, bool forceNewRow, int containerWidth, int containerHeight)
        {
			this.InitializeAll();

			if (vi.ITEM_SEQ == 82)
			{
				bool dbg = true;
				bool dd = dbg;
			}

            this.ControlType = vi.CONTROL_TYPE;
            this.Height = vi.ITEM_HEIGHT;
            this.Width = vi.ITEM_WIDTH;

            if (vi.ITEM_WIDTH == 0 && containerWidth > 0)
                this.Width = containerWidth;
            else if (vi.ITEM_WIDTH < 0)
            {
                decimal pct = Math.Abs(Convert.ToDecimal(vi.ITEM_WIDTH)) / 100;
                this.Width = Convert.ToInt32(containerWidth * pct);
            }

            if (vi.SCALE_MIN.HasValue)
                this.ScaleMin = (decimal)vi.SCALE_MIN;
            if (vi.SCALE_MAX.HasValue)
                this.ScaleMax = (decimal)vi.SCALE_MAX;
            if (vi.SCALE_UNIT.HasValue)
                this.Unit = (decimal)vi.SCALE_UNIT;

            this.Multiplier = vi.MULTIPLIER;

            this.Title = vi.TITLE;
            if (!string.IsNullOrEmpty(addTitle))
                this.Title += (" " + addTitle);

            this.Position = "left";
            if (forceNewRow)
                this.NewRow = true;
            else
                this.NewRow = vi.NEW_ROW;

            // allow series item label substitution local to the view item
            if (!string.IsNullOrEmpty(vi.A_LABEL))
            {
                string[] lbls = vi.A_LABEL.Split('|');
                this.LabelA = lbls[0];
                if (lbls.Length > 1)
                {
                    string[] pairs = lbls[1].Split(',');
                    foreach (string sp in pairs)
                    {
                        string[] args = sp.Split('=');
                        if (args.Length > 1)
                        {
                            this.LabelSubList.Add(args[0], args[1]);
                        }
                    }
                }
            }
            else
            {
                this.LabelA = "";
            }

            this.LabelV = vi.SCALE_LABEL;

            if (!string.IsNullOrEmpty(vi.COLOR_PALLETE))
                this.ColorPallete = vi.COLOR_PALLETE;

            // parse any custom display options
            if (!string.IsNullOrEmpty(vi.OPTIONS))
            {
                this.ItemVisual = vi.OPTIONS;
				foreach (string s in WebSiteCommon.SplitString(vi.OPTIONS, ','))
                {
					if (s.Contains("#"))        // label and value number format
					{
						this.ValueFormat = s;
					}
                    if (s.Contains("TOTIND"))
                        this.TotalIndicator = true;
                    if (s.Contains("OVER"))
                        this.OverlaySeries = 2;
					if (s.Contains("SCALETOMAX"))
						this.ScaleToMax = true;
					if (s.Contains("TOP5"))
					{
						this.TopN = 5;
					}
					if (s.Contains("TOP10"))
					{
						this.TopN = 10;
					}
                }
            }

            if (vi.INDICATOR_1_VALUE.HasValue)
            {
                this.AddIndicator(new GaugeIndicator().CreateNew(this.ScaleMin.HasValue ? this.ScaleMin.Value : 0, (decimal)vi.INDICATOR_1_VALUE, "", vi.INDICATOR_1_COLOR));
                if (vi.INDICATOR_2_VALUE.HasValue)
                {
                    this.AddIndicator(new GaugeIndicator().CreateNew((decimal)vi.INDICATOR_1_VALUE, (decimal)vi.INDICATOR_2_VALUE, "", vi.INDICATOR_2_COLOR));
                    if (vi.INDICATOR_3_VALUE.HasValue)
                        this.AddIndicator(new GaugeIndicator().CreateNew((decimal)vi.INDICATOR_2_VALUE, this.ScaleMax, "", vi.INDICATOR_3_COLOR));
                }
            }

            this.SeqNum = vi.ITEM_SEQ;

            if (vi.CONTROL_TYPE == 10 || vi.CONTROL_TYPE == 20 || vi.CONTROL_TYPE == 60)
                this.DisplayLabel = true;

			// get company-level target for this metric
			// this may be overridden later if BU or plant level targets are specified for this view_item
            if (vi.DISPLAY_TARGET_ID > 0  &&  targetCtl != null)
            {
                this.Target = targetCtl.GetTarget(vi.PERSPECTIVE_VIEW.PERSPECTIVE, vi.CALCS_SCOPE, vi.CALCS_STAT, 1, 0, 0, DateTime.UtcNow.Year);
            }

            return this;
        }
    }

    public class GaugeIndicator
    {
        public decimal FromValue
        {
            get;
            set;
        }
        public decimal ToValue
        {
            get;
            set;
        }
        public string Color
        {
            get;
            set;
        }
        public string Indicator
        {
            get;
            set;
        }
        public GaugeIndicator CreateNew(decimal fromValue, decimal toValue, string indicator, string color)
        {
            this.FromValue = fromValue;
            this.ToValue = toValue;
            this.Indicator = indicator;
            this.Color = color;

            return this;
        }
    }

	public class GaugeSeriesItem
	{
		public int Series
		{
			get;
			set;
		}
		public int Item
		{
			get;
			set;
		}
		public decimal XValue
		{
			get;
			set;
		}
		public decimal? YValue
		{
			get;
			set;
		}
		public string Text
		{
			get;
			set;
		}
		public string Group
		{
			get;
			set;
		}
		public string Color
		{
			get;
			set;
		}
		public bool? Exploded { get; set; }

		public GaugeSeriesItem()
		{
		}

		public GaugeSeriesItem(int series, int item, string itemText, CalcsResult calcResult)
		{
			this.Series = series;
			this.Item = item;
			this.YValue = calcResult.Result;
			if (calcResult.ValidResult2)
				this.XValue = calcResult.Result2;
			this.Text = itemText;
			this.Color = "#191970";
			this.Group = "";
		}

		public GaugeSeriesItem(int series, int item, decimal xvalue, decimal? yvalue, string itemText)
		{
			this.Series = series;
			this.Item = item;
			this.XValue = xvalue;
			this.YValue = yvalue;
			this.Text = itemText;
			this.Color = "#191970";
			this.Group = "";
		}

		public GaugeSeriesItem CreateNew(int series, int item, string itemText, CalcsResult calcResult)
        {
			return new GaugeSeriesItem(series, item, itemText, calcResult);
        }

        public GaugeSeriesItem CreateNew(int series, int item, decimal xvalue, decimal yvalue, string itemText)
        {
			return new GaugeSeriesItem(series, item, xvalue, yvalue, itemText);
        }
    }

    public class GaugeSeries
    {
        public int SeriesNum
        {
            get;
            set;
        }
        public string Name
        {
            get;
            set;
        }
        public string Color
        {
            get;
            set;
        }
		public int SeriesType
		{
			get;
			set;
		}
        public int YAxisCount
        {
            get;
            set;
        }
        public List<GaugeSeriesItem> ItemList
        {
            get;
            set;
        }
        public object ObjData
        {
            get;
            set;
        }
		public bool DisplayLabels { get; set; }

		public GaugeSeries()
		{
			this.ItemList = new List<GaugeSeriesItem>();
		}

		public GaugeSeries(int seriesNum, string seriesName, string color) : this()
		{
			this.SeriesNum = seriesNum;
			this.Name = seriesName;
			this.Color = color;
			this.YAxisCount = 1;
			this.ObjData = null;
			this.DisplayLabels = false;
			this.SeriesType = 0;
		}

		public GaugeSeries CreateNew(int seriesNum, string seriesName, string color)
        {
			return new GaugeSeries(seriesNum, seriesName, color);
        }

        public GaugeSeriesItem AddItem(int itemNum, string itemText, CalcsResult calcResult)
        {
            GaugeSeriesItem seriesItem = new GaugeSeriesItem().CreateNew(this.SeriesNum, itemNum, itemText, calcResult);
            this.ItemList.Add(seriesItem);
            if (calcResult.ValidResult2)
                this.YAxisCount = 2;

            return seriesItem;
        }
    }

    public partial class Ucl_RadGauge : System.Web.UI.UserControl
    {
        static decimal telerikBugValue = .000001m;    // telerik has documented bug regarding the display of zero value cells
        static Dictionary<string, string> colorList;
        static string currentPallete;
        static bool newRow;
        System.Web.UI.HtmlControls.HtmlGenericControl sectionDiv;

 
        public int CreateControl(SQMChartType controlType, GaugeDefinition ggCfg, CalcsResult results, string containerName)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl container = (System.Web.UI.HtmlControls.HtmlGenericControl)this.Parent.FindControl(containerName);
            if (container != null)
                return CreateControl(controlType, ggCfg, results, container);
            else
                return -100;
        }

        public int CreateControl(SQMChartType controlType, GaugeDefinition ggCfg, CalcsResult results, System.Web.UI.HtmlControls.HtmlGenericControl container)
        {
            if (results == null || !results.ValidResult)
                return -1;

            int status = 0;

            CreateSeriesItemLabels(ggCfg, results);

            switch (controlType)
            {
                case SQMChartType.Section:
                    CreateSection(ggCfg, container);
                    break;
                case SQMChartType.Table:
                    status = CreateTable(ggCfg, results, container);
                    break;
                case SQMChartType.Linear:
					ggCfg.DefaultPointColor = "#191970";	// navy blue
					//ggCfg.DefaultScaleColor = "darkslategray";
                    ggCfg.DisplayTitle = true;
                    status = CreateLinear(ggCfg, results, container);
                    break;
                case SQMChartType.BarGraph:
                    status = CreateBarGraph(ggCfg, results.metricSeries, container);
                    break;
                case SQMChartType.BarGraphStacked:
                    ggCfg.Grouping = 2;  // stacked
                    status = CreateStackedBarGraph(ggCfg, results.metricSeries, container);
                    break;
                case SQMChartType.BarGraphGrouped:
                    ggCfg.Grouping = 1;  // grouped
                    status = CreateStackedBarGraph(ggCfg, results.metricSeries, container);
                    break;
                case SQMChartType.BarGraphStackedInv:
                    ggCfg.Grouping = 2;  // stacked and inverted series
                    status = CreateStackedBarGraph(ggCfg, InvertSeries(results.metricSeries), container);
                    break;
                case SQMChartType.BarGraphPareto:
                    status = CreateBarParetoChart(ggCfg, results.metricSeries, container);
                    break;
                case SQMChartType.Column:
                    ggCfg.DefaultPointColor = "cyan";
                    ggCfg.DisplayTitle = true;
                    status = CreateColumn(ggCfg, results, container);
                    break;
                case SQMChartType.ColumnChart:
                    status = CreateColumnChart(ggCfg, results.metricSeries, container);
                    break;
                case SQMChartType.ColumnChartStacked:
                    ggCfg.Grouping = 2;  // stacked
                    status = CreateStackedColumnChart(ggCfg, results.metricSeries, container);
                    break;
                case SQMChartType.ColumnChartGrouped:
                    ggCfg.Grouping = 1;  // grouped
                    status = CreateStackedColumnChart(ggCfg, results.metricSeries, container);
                    break;
                case SQMChartType.ColumnChartStackedInv:
                    ggCfg.Grouping = 2;  // stacked and series inverted
                    status = CreateStackedColumnChart(ggCfg, InvertSeries(results.metricSeries), container);
                    break;
                case SQMChartType.ColumnChartPareto:
                    status = CreateColumnParetoChart(ggCfg, results.metricSeries, container);
                    break;
                case SQMChartType.MultiLine:
                    status = CreateMultiLineChart(ggCfg, results.metricSeries, container);
                    break;
                case SQMChartType.MultiLineInv:  // inverted point series
                    status = CreateMultiLineChart(ggCfg, InvertSeries(results.metricSeries), container);
                    break;
                case SQMChartType.PieChart:
					status = CreatePieChart(ggCfg, results.metricSeries, container);
                    break;
                case SQMChartType.Radial:
                    ggCfg.DisplayTitle = true;
                    status = CreateRadial(ggCfg, results, container);
                    break;
				case SQMChartType.SpiderChart:
					status = CreateSpiderChart(ggCfg, results.metricSeries, container);
					break;
            }

            return status;
        }

        #region common

        private System.Drawing.Color GetColor(string palleteName, int colorNum)
        {
            System.Drawing.Color color = new System.Drawing.Color();

            if (currentPallete == null || currentPallete != palleteName)
            {
                colorList = WebSiteCommon.GetXlatList(palleteName, "", "short");
                currentPallete = palleteName;
            }
			if (colorNum < colorList.Count)
				//color = System.Drawing.Color.FromName(colorList[colorNum.ToString()]);
               color = System.Drawing.ColorTranslator.FromHtml(colorList[colorNum.ToString()]);

            return color;
        }

        private decimal Convert(decimal valueIn, decimal? multiplier)
        {
            return multiplier.HasValue && multiplier != 0 && multiplier != 1 ? (valueIn * (decimal)multiplier) : valueIn;
        }

        private string SetValueFormat(GaugeDefinition rgCfg, string defaultFmt)
        {
            string fmt = defaultFmt;

            if (!string.IsNullOrEmpty(rgCfg.ValueFormat))
                fmt = rgCfg.ValueFormat;
            else if (string.IsNullOrEmpty(defaultFmt))
                fmt = rgCfg.DefaultValueFormat;

            return fmt;
        }

        private int SetValuePrecision(GaugeDefinition rgCfg, string defaultFmt)
        {
            string fmt = defaultFmt;
            int digits = 0;

            if (!string.IsNullOrEmpty(rgCfg.ValueFormat))
                fmt = rgCfg.ValueFormat;

            string[] args = fmt.Split('.');
            if (args.Length > 1)
            {
                digits = args[1].Length;
            }

            return digits;
        }


        private CalcsResult CreateSeriesItemLabels(GaugeDefinition ggCfg, CalcsResult results)
        {
            if (results != null)
            {
                if (!string.IsNullOrEmpty(ggCfg.LabelV) && ggCfg.LabelV.EndsWith("/") && !string.IsNullOrEmpty(results.FactorText))
                {
                    ggCfg.LabelV = ggCfg.LabelV + results.FactorText;
                }

                if (ggCfg.LabelSubList.Count > 0)
                {
                    foreach (GaugeSeries series in results.metricSeries)
                    {
                        foreach (GaugeSeriesItem item in series.ItemList)
                        {
                            try
                            {
                                string lbl = ggCfg.LabelSubList.FirstOrDefault(l => l.Key == item.Text).Value;
                                if (!string.IsNullOrEmpty(lbl))
                                    item.Text = lbl;
                            }
                            catch { }
                        }
                    }
                }
            }

            return results;
        }

        private List<GaugeSeries> InvertSeries(List<GaugeSeries> gaugeSeries)
        {
            // invert the telerik series and series items list
            // by default, telerik renders series on the y axis and items on the x axis
            // invoke this method for graph presentations that prefer stacking of items vs series
            List<GaugeSeries> outSeries = new List<GaugeSeries>();

            var hash = new HashSet<string>();
            foreach (GaugeSeries series in gaugeSeries)
            {
                foreach (GaugeSeriesItem item in series.ItemList)
                    hash.Add(item.Text);               
            }

            int nSeries = -1;
            foreach (string seriesName in hash)
            {
                GaugeSeries newSeries = new GaugeSeries().CreateNew(++nSeries, seriesName, "");
                outSeries.Add(newSeries);
            }

            nSeries = -1;
            foreach (GaugeSeries series in gaugeSeries)
            {
                foreach (GaugeSeries series0 in outSeries)
                {
                    GaugeSeriesItem newItem = new GaugeSeriesItem();
                    GaugeSeriesItem item0 = series.ItemList.Where(l => l.Text == series0.Name).FirstOrDefault();
                    if (item0 == null)
                        newItem.CreateNew(++nSeries, 0, 0, 0, series.Name);
                    else
                        newItem.CreateNew(++nSeries, 0, item0.XValue, item0.YValue ?? 0, series.Name);
                    series0.ItemList.Add(newItem);
                }
            }
  
            return outSeries;
        }

        private System.Web.UI.HtmlControls.HtmlGenericControl CreateContainer(GaugeDefinition rgCfg)
        {
            return CreateContainer(rgCfg, "");
        }

        private System.Web.UI.HtmlControls.HtmlGenericControl CreateContainer(GaugeDefinition rgCfg, string detailHTML)
        {
            System.Web.UI.HtmlControls.HtmlGenericControl div = new System.Web.UI.HtmlControls.HtmlGenericControl();
            //div.Style.Add("TEXT-ALIGN", "center");

            //if (sectionDiv != null && rgCfg.ItemVisual.Contains("CENTER"))
			if (rgCfg.ItemVisual.Contains("CENTER"))
            {
                div.Style.Add("text-align", "center");
				if (!rgCfg.NewRow)
					div.Style.Add("DISPLAY", "INLINE-BLOCK");
            }
            else
            {
                div.Style.Add("FLOAT", rgCfg.Position);
            }
           
            if (rgCfg.NewRow  ||  (newRow != null  &&  newRow))
                div.Style.Add("CLEAR", "both"); 

            if (!rgCfg.NewRow)
                div.Style.Add("margin-left","10px");

            newRow = false;
            if (rgCfg.DisplayTitle)
            {
                Label label = new Label();
                label.Text = rgCfg.Title;
                label.CssClass = "prompt";
                label.Style.Add("margin-bottom", "0");
                div.Controls.Add(label);
            }

            if (!string.IsNullOrEmpty(detailHTML))
            {
                LinkButton lnk = new LinkButton();
                lnk.Text = "Details";
                lnk.OnClientClick = "OpenDetailWindow('" + detailHTML + "'); return false;";
                div.Controls.Add(lnk);
            }

            return div;
        }

        private void BindToContainer(System.Web.UI.HtmlControls.HtmlGenericControl container, System.Web.UI.HtmlControls.HtmlGenericControl div)
        {
            // bind div containing rad control to either the graphing area container (div) or within a section (graphtype = 1)
            if (sectionDiv != null)
                //  sectionDiv.Controls.Add(div);
                sectionDiv.Controls.AddAt(sectionDiv.Controls.Count-1, div);
            else
                container.Controls.Add(div);
        }
        
        private System.Web.UI.HtmlControls.HtmlGenericControl CreateSection(GaugeDefinition rgCfg, System.Web.UI.HtmlControls.HtmlGenericControl container)
        {
            sectionDiv = new System.Web.UI.HtmlControls.HtmlGenericControl();
            sectionDiv.Style.Add("FLOAT", "LEFT");
            if (rgCfg.NewRow)
            {
                sectionDiv.Style.Add("WIDTH", "99.5%");
				sectionDiv.Style.Add("text-align", "center");
            }
            if (!rgCfg.ItemVisual.Contains("NOBORDER"))
            {
                sectionDiv.Attributes["class"] = "separatorBar";
                sectionDiv.Style.Add("MARGIN-BOTTOM", "10px");
            }

            sectionDiv.Controls.Add(new LiteralControl("<center>"));
 
            System.Web.UI.HtmlControls.HtmlGenericControl labelDiv = new System.Web.UI.HtmlControls.HtmlGenericControl();
            if (!string.IsNullOrEmpty(rgCfg.Title))
            {
                labelDiv.Style.Add("MARGIN", "8px");
                Label label = new Label();
                label.Text = rgCfg.Title;
                label.CssClass = "refTextSmall";

                labelDiv.Controls.Add(label);
                labelDiv.Controls.Add(new LiteralControl("<br />"));
                labelDiv.Controls.Add(new LiteralControl("<br />"));
                sectionDiv.Controls.Add(labelDiv);
            }

            sectionDiv.Controls.Add(new LiteralControl("</center>"));
            container.Controls.Add(sectionDiv);
            newRow = true;

            return sectionDiv;
        }

        private void AddResults(System.Web.UI.HtmlControls.HtmlGenericControl div, GaugeDefinition rgCfg, string resultText, DataTable summaryTable)
        {
            if (!string.IsNullOrEmpty(resultText))
            {
                Label label = new Label();
                label.CssClass = "labelIndicator";
                label.Text = resultText;
                div.Style.Add("MARGIN-BOTTOM", "10PX");
                div.Style.Add("MARGIN-TOP", "0");
                div.Controls.Add(label);
            }

            if (summaryTable != null && summaryTable.Rows.Count > 0)
            {
                Table tb = new Table();
                tb.CssClass = "Grid";
                int r = 0;

                div.Style.Add("MARGIN-RIGHT", "10PX");
                div.Style.Add("MARGIN-TOP", "0");

                foreach (DataRow dr in summaryTable.Rows)
                {
                    ++r;
                    TableRow tr = new TableRow();
                    for (int c = 0; c < summaryTable.Columns.Count; c++)
                    {
                        TableCell td = new TableCell();
                        td.CssClass = r == 1 ? "HeadingCellText" : "DataCellBold";
                        td.Text = dr[c].ToString();
                        tr.Cells.Add(td);
                    }
                    tb.Rows.Add(tr);
                }
                div.Controls.Add(tb);
            }
        }

        private LineSeries IndicatorLine(decimal indValue, string indText, int numPoints, RadHtmlChart rad)
        {
            LineSeries indLine = null;
 
            indLine = new LineSeries();
            string fmt = indText + " = {0}";
            indLine.TooltipsAppearance.DataFormatString = fmt;
            indLine.TooltipsAppearance.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("white");
            indLine.LabelsAppearance.Visible = false;
            indLine.MarkersAppearance.Visible = false;
            indLine.Appearance.FillStyle.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("red");
            for (int n = 1; n <= numPoints; n++)
            {
                indLine.SeriesItems.Add(new CategorySeriesItem(indValue));
            }

            rad.PlotArea.Series.Add(indLine);
 
            return indLine;
        }

        private LineSeries IndicatorLine(GaugeDefinition rgCfg, int numPoints, RadHtmlChart rad)
        {
            if (rgCfg.Target != null)
            {
                return IndicatorLine((decimal)rgCfg.Target.TARGET_VALUE, rgCfg.Target.DESCR_SHORT, numPoints, rad);
            }
            else
                return null;
        }

        private int  OverlayLine(RadHtmlChart rad, GaugeDefinition rgCfg, GaugeSeries gaugeSeries, string title, string colorValue, bool addAxis)
        {
            int status = 0;

            LineSeries newSeries = new LineSeries();
            newSeries.MarkersAppearance.Visible = false;
            newSeries.LabelsAppearance.DataFormatString = "{0}";
            newSeries.LabelsAppearance.Visible = false;
            newSeries.TooltipsAppearance.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("white");
            newSeries.Appearance.FillStyle.BackgroundColor = System.Drawing.ColorTranslator.FromHtml(colorValue);
            newSeries.MarkersAppearance.Visible = true;
            newSeries.MarkersAppearance.MarkersType = MarkersType.Square;
            newSeries.MarkersAppearance.Size = 3m;
            newSeries.MarkersAppearance.BorderWidth = 3;
            newSeries.LineAppearance.Width = 1;
			newSeries.Name = title.Replace("\r\n", "");

            if (addAxis)
            {
                AxisY newAxis = new AxisY()
                {
                    Name = "newAxis",
                    Visible = true
                };
                newSeries.AxisName = "newAxis";
                rad.PlotArea.AdditionalYAxes.Add(newAxis);
                rad.PlotArea.AdditionalYAxes[0].TitleAppearance.Text = title;
                rad.PlotArea.AdditionalYAxes[0].TitleAppearance.TextStyle.FontSize = 11;
                rad.PlotArea.AdditionalYAxes[0].TitleAppearance.TextStyle.Bold = true;
                rad.PlotArea.AdditionalYAxes[0].LabelsAppearance.TextStyle.FontSize = 11;
                rad.PlotArea.AdditionalYAxes[0].LabelsAppearance.DataFormatString = "{0}";
            }
          
            decimal value = 0;
            foreach (GaugeSeriesItem data in gaugeSeries.ItemList)
            {
                value = Convert(data.YValue ?? 0, rgCfg.Multiplier); // data.XValue;
                newSeries.SeriesItems.Add(new CategorySeriesItem(value));
            }
 
            rad.PlotArea.Series.Add(newSeries);

            return status;
        }

        private LineSeries OverlaySummaryLine(GaugeDefinition rgCfg, List<GaugeSeries> gaugeSeries, int axisItemsCount, bool drawMarkers, bool drawLines, bool drawLabels)
        {
            decimal axisMin = 9999999999m;
            decimal axisMax = 0;

            LineSeries newSeries = new LineSeries();
            newSeries.LabelsAppearance.DataFormatString = SetValueFormat(rgCfg, "#.#"); 
            newSeries.LabelsAppearance.Visible = drawLabels;
            newSeries.TooltipsAppearance.Visible = false;
            newSeries.MarkersAppearance.Visible = drawMarkers;
            newSeries.LineAppearance.Width = drawLines == true ? 2 : 0;
            for (int col = 0; col < axisItemsCount; col++)
            {
                decimal sum = 0;
                foreach (GaugeSeries gs in gaugeSeries)
                {
                    if (col < gs.ItemList.Count)
                        sum += Convert(gs.ItemList[col].YValue ?? 0, rgCfg.Multiplier);
                }
                newSeries.SeriesItems.Add(new CategorySeriesItem(sum));
                axisMin = Math.Min(axisMin, sum);
                axisMax = Math.Max(axisMax, sum);
            }

            return newSeries;
        }

        private RadHtmlChart PrependBarLabelChart(GaugeDefinition rgCfg, List<GaugeSeriesItem> seriesData, System.Web.UI.HtmlControls.HtmlGenericControl container, int radHeight, int legendHeight)
        {
            GaugeDefinition rgCfg0 = new GaugeDefinition();
            rgCfg0 = (GaugeDefinition)SQMModelMgr.CopyObjectValues(rgCfg0, rgCfg, false);
            rgCfg0.Height = radHeight;
            rgCfg0.Title = "&nbsp;";
            RadHtmlChart rad0 = new RadHtmlChart();
            rad0.Skin = "Metro";
 
            rad0.ChartTitle.Appearance.TextStyle.FontSize = 12;
            rad0.ChartTitle.Appearance.TextStyle.Bold = true;
            rad0.ChartTitle.Text = rgCfg0.Title;
            BarSeries series0 = new BarSeries();
            rad0.PlotArea.YAxis.MinValue = rad0.PlotArea.YAxis.MaxValue = 0;
            rad0.PlotArea.YAxis.LabelsAppearance.Visible = false;
            series0.LabelsAppearance.Visible = false;

            int maxLabelLen = 0;
            foreach (GaugeSeriesItem item in seriesData)
            {
                SeriesItem si0 = new SeriesItem();
                si0.YValue = 0;
                series0.Items.Add(si0);
                rad0.PlotArea.XAxis.Items.Add(new AxisItem(item.Text));
				if (!string.IsNullOrEmpty(item.Text))
					maxLabelLen = Math.Max(maxLabelLen, item.Text.Length);
            }
            rad0.PlotArea.Series.Add(series0);
            rad0.Height = (rgCfg0.Height - 15) - legendHeight;
           // rad0.Height = (rgCfg.Height - 15) - legendHeight;
            int w = rgCfg.LabelA.Length > 0 ? 22 : 0;
            rad0.Width = (maxLabelLen-1) * 8 + w;

            rad0.BorderWidth = 0;
            rad0.PlotArea.XAxis.MinorGridLines.Visible = false;
            rad0.PlotArea.XAxis.MajorGridLines.Visible = false;
            rad0.PlotArea.YAxis.MinorGridLines.Visible = false;
            rad0.PlotArea.YAxis.MajorGridLines.Visible = false;
            rad0.PlotArea.YAxis.MajorTickType = TickType.None;
            // rad0.PlotArea.XAxis.MajorTickType = TickType.None;
            rad0.Legend.Appearance.Visible = false;
            rad0.PlotArea.YAxis.TitleAppearance.Text = "&nbsp;";
            rad0.PlotArea.YAxis.LabelsAppearance.TextStyle.FontSize = 12; // 11;
            rad0.PlotArea.YAxis.TitleAppearance.TextStyle.FontSize = 12;
            rad0.PlotArea.YAxis.TitleAppearance.TextStyle.Bold = true;
            rad0.PlotArea.XAxis.LabelsAppearance.TextStyle.FontSize = 12; // 11;
            rad0.PlotArea.XAxis.TitleAppearance.TextStyle.FontSize = 12;
            rad0.PlotArea.XAxis.TitleAppearance.TextStyle.Bold = true;
            rad0.PlotArea.XAxis.TitleAppearance.Text = rgCfg.LabelA;
            System.Web.UI.HtmlControls.HtmlGenericControl div0 = CreateContainer(rgCfg0);

            div0.Controls.Add(rad0);
           // container.Controls.Add(div0);
            BindToContainer(container, div0);

            return rad0;
        }

 
        #endregion

        #region table
        public int CreateTable(GaugeDefinition rgCfg, CalcsResult result, System.Web.UI.HtmlControls.HtmlGenericControl container)
        {
            int status = 0;

            Table tb = new Table();
            tb.CssClass = "Grid";

            TableRow tr = new TableRow();
            TableCell td = new TableCell();
            td.CssClass = "HeadingCellText";
            td.ColumnSpan = 2;
            td.Text = result.Text;
            tr.Cells.Add(td);
            tb.Rows.Add(tr);

            tr = new TableRow();
            td = new TableCell();
            td.CssClass = "labelIndicator";
            td.Text = SQMBasePage.FormatValue(result.Result, 2);
            tr.Cells.Add(td);
            td = new TableCell();
            td.CssClass = "DataCell";
            if (rgCfg.Target != null)
               td.Text = SQMBasePage.FormatValue((decimal)rgCfg.Target.TARGET_VALUE, 2);
            tr.Cells.Add(td);
            tb.Rows.Add(tr);

            System.Web.UI.HtmlControls.HtmlGenericControl div = CreateContainer(rgCfg);
            div.Style.Remove("TEXT-ALIGN");
            div.Style.Add("TEXT-ALIGN", "left");
            div.Controls.Add(tb);
            container.Controls.Add(div);

            return status;
        }
        #endregion

        #region ticker
        public int CreateTicker(GaugeDefinition rgCfg, List<GaugeSeries> gaugeSeries, System.Web.UI.HtmlControls.HtmlGenericControl container)
        {
            int status = 0;

            RadTicker rad = new RadTicker();

            rad.AutoStart = true;
            rad.AutoAdvance = true;
            rad.Loop = true;
            rad.TickSpeed = (int)rgCfg.Unit;
            rad.Skin = "Metro";
            rad.CssClass = "tickerEmphasis";
            if (rgCfg.Width > 0)
            {
                rad.Width = rgCfg.Width;
            }

            foreach (GaugeSeries gs in gaugeSeries)
            {
                foreach (GaugeSeriesItem data in gs.ItemList)
                {
                    RadTickerItem ti = new RadTickerItem();
                    ti.Text = data.Text + ":  " + data.YValue.ToString();
                    ti.CssClass = "labelEmphasis";
                    rad.Items.Add(ti);
                }
            }

            container.Controls.Add(rad);

            return status;
        }

        #endregion

        #region radial

        public int CreateRadial(GaugeDefinition rgCfg, CalcsResult result, System.Web.UI.HtmlControls.HtmlGenericControl container)
        {
            int status = 0;

            RadRadialGauge rad = new RadRadialGauge();

            if (rgCfg.Height > 0)
                rad.Height = rgCfg.Height;
            if (rgCfg.Width > 0)
                rad.Width = rgCfg.Width;

			if (rgCfg.ScaleMin != rgCfg.ScaleMax)
			{
				rad.Scale.Min = rgCfg.ScaleMin.HasValue ? rgCfg.ScaleMin.Value : 0;
				rad.Scale.Max = rgCfg.ScaleMax;
				rad.Scale.MajorUnit = rgCfg.Unit;

				if (result.Result > rad.Scale.Max  &&  rgCfg.ScaleToMax == true)
				{
					rad.Scale.Max = WebSiteCommon.RoundToBaseFraction(result.Result, rgCfg.Unit);
					if ((rad.Scale.Max - rad.Scale.Min) / rgCfg.Unit > 10)
					{
						rgCfg.Unit = rgCfg.Unit * 2.0m;
					}
					rad.Scale.MajorUnit = rgCfg.Unit;
				}
			}

            rad.Scale.Labels.Visible = true;
			rad.Scale.Labels.Format = "{0} " + rgCfg.LabelV;
            rad.Scale.Labels.Position = Telerik.Web.UI.Gauge.ScaleLabelsPosition.Outside;
            rad.ToolTip = SQMBasePage.FormatValue(result.Result, SetValuePrecision(rgCfg, "#.####")) + " " + rgCfg.LabelV;
            rad.Pointer.Cap.Size = .10f;
            rad.Pointer.Color = System.Drawing.ColorTranslator.FromHtml("#191970" /*"#2F4F4F"*/);

			GaugeRange range = null;
            if (rgCfg.IndicatorList == null || rgCfg.IndicatorList.Count == 0)
            {
				if (rgCfg.Target != null)
				{
					range = new GaugeRange();
					range.From = rgCfg.ScaleMin.HasValue ? rgCfg.ScaleMin.Value : 0;
					range.To = (decimal)rgCfg.Target.VALUE - (rgCfg.Unit / 4.0m);
					range.Color = System.Drawing.ColorTranslator.FromHtml(rgCfg.DefaultScaleColor);
					rad.Scale.Ranges.Add(range);
					range = new GaugeRange();
					range.From = (decimal)rgCfg.Target.VALUE - (rgCfg.Unit / 4.0m);
					range.To = (decimal)rgCfg.Target.VALUE + (rgCfg.Unit / 4.0m);
					range.Color = System.Drawing.Color.Firebrick;
					rad.Scale.Ranges.Add(range);
					range = new GaugeRange();
					range.From = (decimal)rgCfg.Target.VALUE + (rgCfg.Unit / 4.0m);
					range.To = rgCfg.ScaleMax;
					range.Color = System.Drawing.ColorTranslator.FromHtml(rgCfg.DefaultScaleColor);
					rad.Scale.Ranges.Add(range);
				}
				else
				{
					range = new GaugeRange();
					range.From = rgCfg.ScaleMin.HasValue ? rgCfg.ScaleMin.Value : 0;
					range.To = rgCfg.ScaleMax;
					range.Color = System.Drawing.ColorTranslator.FromHtml(rgCfg.DefaultScaleColor);
					rad.Scale.Ranges.Add(range);
				}
            }
            else
            {
                foreach (GaugeIndicator rgi in rgCfg.IndicatorList)
                {
                    range = new GaugeRange();
                    range.From = rgi.FromValue;
                    range.To = rgi.ToValue;
                    if (rgi.Color.ToLower() == "default")
                        range.Color = System.Drawing.ColorTranslator.FromHtml(rgCfg.DefaultScaleColor);
                    else 
                        range.Color = System.Drawing.ColorTranslator.FromHtml(rgi.Color);
                    rad.Scale.Ranges.Add(range);
                }
            }

            rad.Pointer.Value = Convert(result.Result, rgCfg.Multiplier);

            System.Web.UI.HtmlControls.HtmlGenericControl div = CreateContainer(rgCfg); //, "<table border=\"1\"><tr><td>some results</td></tr></table>");
            rad.Style.Add("margin-left", "auto");
            rad.Style.Add("margin-right", "auto");
            rad.Style.Add("margin-top", "0");
  
            div.Controls.Add(rad);
            if (rgCfg.ItemVisual != "NR")
            {
                AddResults(div, rgCfg, SQMBasePage.FormatValue(result.Result, SetValuePrecision(rgCfg, "#.####")), result.SummaryTable);
            }
            BindToContainer(container, div);

            return status;
        }
        #endregion

        #region linear

        public int CreateLinear(GaugeDefinition rgCfg, CalcsResult result, System.Web.UI.HtmlControls.HtmlGenericControl container)
        {
            int status = 0;

            RadLinearGauge rad = new RadLinearGauge();
            rad.Scale.Vertical = false;
			/*
            if (rgCfg.Height > 0 && rgCfg.Width > 0)
            {
                rad.Height = rgCfg.Height;
                rad.Width = rgCfg.Width;
            }

            rad.Scale.Min = rgCfg.ScaleMin.HasValue ? rgCfg.ScaleMin.Value : 0;
            if (rgCfg.ScaleMax == rgCfg.ScaleMin)
                rad.Scale.Max = (result.Result * 1.10m);
            else 
                rad.Scale.Max = rgCfg.ScaleMax;

            if (rgCfg.Unit != 0)
                rad.Scale.MajorUnit = rgCfg.Unit;
			*/

			if (rgCfg.Height > 0)
				rad.Height = rgCfg.Height;
			if (rgCfg.Width > 0)
				rad.Width = rgCfg.Width;

			if (rgCfg.ScaleMin != rgCfg.ScaleMax)
			{
				rad.Scale.Min = rgCfg.ScaleMin.HasValue ? rgCfg.ScaleMin.Value : 0;
				rad.Scale.Max = rgCfg.ScaleMax;
				rad.Scale.MajorUnit = rgCfg.Unit;

				if (result.Result > rad.Scale.Max && rgCfg.ScaleToMax)
				{
					rad.Scale.Max = WebSiteCommon.RoundToBaseFraction(result.Result, rgCfg.Unit);
					if ((rad.Scale.Max - rad.Scale.Min) / rgCfg.Unit > 10)
					{
						rgCfg.Unit = rgCfg.Unit * 2.0m;
					}
					rad.Scale.MajorUnit = rgCfg.Unit;
				}
			}

			if (!rgCfg.DisplayLabel)
				rad.Scale.Labels.Color = Color.Transparent;

            rad.Scale.Labels.Format = "{0} " + rgCfg.LabelV;
            rad.ToolTip = SQMBasePage.FormatValue(result.Result, SetValuePrecision(rgCfg, "#.####")) + " " + rgCfg.LabelV;
            rad.Pointer.Color = System.Drawing.ColorTranslator.FromHtml(rgCfg.DefaultPointColor);
            rad.Pointer.Shape = Telerik.Web.UI.Gauge.PointerShape.BarIndicator;
            rad.Pointer.Size = (float?)10.0;

            if (rgCfg.IndicatorList == null || rgCfg.IndicatorList.Count == 0)
            {
                GaugeRange range = new GaugeRange();
                range.From = rgCfg.ScaleMin.HasValue ? rgCfg.ScaleMin.Value : 0;
                range.To = rgCfg.ScaleMax;
                range.Color = System.Drawing.ColorTranslator.FromHtml(rgCfg.DefaultScaleColor);
                rad.Scale.Ranges.Add(range);
            }
            else
            {
                foreach (GaugeIndicator rgi in rgCfg.IndicatorList)
                {
                    GaugeRange range = new GaugeRange();
                    range.From = rgi.FromValue;
                    range.To = rgi.ToValue;
                    if (rgi.Color.ToLower() == "default")
                        range.Color = System.Drawing.ColorTranslator.FromHtml(rgCfg.DefaultScaleColor);
                    else
                        range.Color = System.Drawing.ColorTranslator.FromHtml(rgi.Color);
                    rad.Scale.Ranges.Add(range);
                }
            }

            rad.Pointer.Value = Convert(result.Result, rgCfg.Multiplier);

            System.Web.UI.HtmlControls.HtmlGenericControl div = CreateContainer(rgCfg);
            rad.Style.Add("margin-left", "auto");
            rad.Style.Add("margin-right", "auto");
            div.Controls.Add(rad);
            AddResults(div, rgCfg, SQMBasePage.FormatValue(result.Result, SetValuePrecision(rgCfg, "#.####")), result.SummaryTable);
           // container.Controls.Add(div);
            BindToContainer(container, div);

            return status;
        }

        #endregion

        #region column

        public int CreateColumn(GaugeDefinition rgCfg, CalcsResult result, System.Web.UI.HtmlControls.HtmlGenericControl container)
        {
            int status = 0;

            RadLinearGauge rad = new RadLinearGauge();
            //  rad.Skin = "Metro";
            rad.Scale.Vertical = true;
            if (rgCfg.Height > 0)
                rad.Height = rgCfg.Height;
            if (rgCfg.Width > 0)
                rad.Width = rgCfg.Width;

            rad.Scale.Min = rgCfg.ScaleMin.HasValue ? rgCfg.ScaleMin.Value : 0;
            if (rgCfg.ScaleMax == rgCfg.ScaleMin)
                rad.Scale.Max = (result.Result * 1.10m);
            else
                rad.Scale.Max = rgCfg.ScaleMax;
            if (rgCfg.Unit != 0)
                rad.Scale.MajorUnit = rgCfg.Unit;

            rad.Scale.Labels.Format = SetValueFormat(rgCfg, "#.#");
            rad.ToolTip = SQMBasePage.FormatValue(result.Result, SetValuePrecision(rgCfg, "#.####")) + " " + rgCfg.LabelV;
            rad.Pointer.Color = System.Drawing.ColorTranslator.FromHtml(rgCfg.DefaultPointColor);
            rad.Pointer.Shape = Telerik.Web.UI.Gauge.PointerShape.BarIndicator;
            rad.Pointer.Size = (float?)10.0;

            if (rgCfg.IndicatorList == null || rgCfg.IndicatorList.Count == 0)
            {
                GaugeRange range = new GaugeRange();
                range.From = rgCfg.ScaleMin.HasValue ? rgCfg.ScaleMin.Value : 0;
                range.To = rgCfg.ScaleMax;
                range.Color = System.Drawing.ColorTranslator.FromHtml(rgCfg.DefaultScaleColor);
                rad.Scale.Ranges.Add(range);
            }
            else
            {
                foreach (GaugeIndicator rgi in rgCfg.IndicatorList)
                {
                    GaugeRange range = new GaugeRange();
                    range.From = rgi.FromValue;
                    range.To = rgi.ToValue;
                    if (rgi.Color.ToLower() == "default")
                        range.Color = System.Drawing.ColorTranslator.FromHtml(rgCfg.DefaultScaleColor);
                    else
                        range.Color = System.Drawing.ColorTranslator.FromHtml(rgi.Color);
                    rad.Scale.Ranges.Add(range);
                }
            }

            rad.Pointer.Value = Convert(result.Result, rgCfg.Multiplier);

            System.Web.UI.HtmlControls.HtmlGenericControl div = CreateContainer(rgCfg);
            rad.Style.Add("margin-left", "auto");
            rad.Style.Add("margin-right", "auto");
            div.Controls.Add(rad);
            AddResults(div, rgCfg, SQMBasePage.FormatValue(result.Result, SetValuePrecision(rgCfg, "#.####")), result.SummaryTable);
           // container.Controls.Add(div);
            BindToContainer(container, div);
  
            return status;
        }
        #endregion

        #region bargraph
        public int CreateBarGraph(GaugeDefinition rgCfg, List<GaugeSeries> gaugeSeries, System.Web.UI.HtmlControls.HtmlGenericControl container)
        {
            int status = 0;
            int numItems = 0;
            bool negScale = false;
			double actualMinValue = 0;

            if (gaugeSeries == null || gaugeSeries.Count == 0)
                return -1;

            List<GaugeSeriesItem> seriesData = gaugeSeries[0].ItemList;
            // gotta do this to get rad to display zero label values
            foreach (GaugeSeriesItem item in seriesData)
            {
               // if (item.YValue == 0)
               //     item.YValue = telerikBugValue;
				if (item.YValue < 0)
				{
					negScale = true;
					actualMinValue = Math.Min(actualMinValue, (double)Convert(item.YValue ?? 0, rgCfg.Multiplier));
				}
            }

            bool exploded = rgCfg.ItemVisual == "E" ? true : false;

            RadHtmlChart rad = new RadHtmlChart();
 
            if (rgCfg.Width > 0)
                rad.Width = rgCfg.Width;

            int radHeight = Math.Max(seriesData.Count *  24 + 120, rgCfg.Height);
            rad.Height = radHeight;

			if (!string.IsNullOrWhiteSpace(rgCfg.OnLoad))
				rad.ClientEvents.OnLoad = rgCfg.OnLoad;

			if (negScale)
            {
                PrependBarLabelChart(rgCfg, seriesData, container, radHeight, 0);
                rgCfg.NewRow = false;
            }

			if (rgCfg.ScaleMin.HasValue)
			{
				if (negScale)
					rad.PlotArea.YAxis.MinValue = (decimal)actualMinValue;
				else 
					rad.PlotArea.YAxis.MinValue = rgCfg.ScaleMin.Value;
			}
			if (rgCfg.ScaleMax != 0)
                rad.PlotArea.YAxis.MaxValue = rgCfg.ScaleMax;
            
            rad.ChartTitle.Text = rgCfg.Title;
            rad.ChartTitle.Appearance.TextStyle.FontSize = 12;
            rad.ChartTitle.Appearance.TextStyle.Bold = true;
            rad.Skin = "Metro";

            BarSeries series = new BarSeries();
            series.LabelsAppearance.Position = BarColumnLabelsPosition.OutsideEnd;
            series.LabelsAppearance.DataFormatString = SetValueFormat(rgCfg, "#.#");
            series.TooltipsAppearance.DataFormatString = SetValueFormat(rgCfg, "#.#");
			rad.PlotArea.YAxis.LabelsAppearance.DataFormatString = SetValueFormat(rgCfg, "#.#");
            series.TooltipsAppearance.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("white");

			
           foreach (GaugeSeriesItem item in seriesData)
           {
               SeriesItem si = new SeriesItem();
               si.YValue = Convert(item.YValue ?? 0, rgCfg.Multiplier);
			   if (!string.IsNullOrEmpty(rgCfg.ColorPallete))
				   si.BackgroundColor = GetColor(rgCfg.ColorPallete, ++numItems);
               series.Items.Add(si);
           }

            rad.PlotArea.YAxis.TitleAppearance.Text = rgCfg.LabelV;
            rad.PlotArea.YAxis.LabelsAppearance.TextStyle.FontSize = 11;
            rad.PlotArea.YAxis.TitleAppearance.TextStyle.FontSize = 12;
            rad.PlotArea.YAxis.TitleAppearance.TextStyle.Bold = true;

            if (!negScale)
            {
                rad.PlotArea.XAxis.TitleAppearance.TextStyle.FontSize = 12;
                rad.PlotArea.XAxis.TitleAppearance.TextStyle.Bold = true;
                rad.PlotArea.XAxis.TitleAppearance.Text = rgCfg.LabelA;
                rad.PlotArea.XAxis.LabelsAppearance.TextStyle.FontSize = 11;
            }

            rad.PlotArea.Series.Add(series);

            LineSeries targetLine = IndicatorLine(rgCfg, seriesData.Count(), rad);
  
            if (!negScale)
            {
                foreach (GaugeSeriesItem data in seriesData)
                {
                    rad.PlotArea.XAxis.Items.Add(new AxisItem(data.Text));
                }
            }

			rad.Legend.Appearance.Position = rgCfg.LegendPosition;
			rad.Legend.Appearance.BackgroundColor = rgCfg.LegendBackgroundColor;
			rad.Legend.Appearance.Visible = rgCfg.DisplayLegend;

            if (rgCfg.OverlaySeries > 1 && gaugeSeries.Count > 1)
            {
                series.Name = rgCfg.LabelSubList != null && rgCfg.LabelSubList.Count > 0 ? rgCfg.LabelSubList.ElementAt(0).Value : "";
                OverlayLine(rad, rgCfg, gaugeSeries.ElementAt(rgCfg.OverlaySeries - 1), rgCfg.LabelSubList != null && rgCfg.LabelSubList.Count > 1 ? rgCfg.LabelSubList.ElementAt(1).Value : "", "DarkSlateGray", false);
                series.LabelsAppearance.Visible = false;
            }

            System.Web.UI.HtmlControls.HtmlGenericControl div = CreateContainer(rgCfg);
            if (negScale)
                div.Style.Remove("margin-left");
            div.Controls.Add(rad);
            //container.Controls.Add(div);
            BindToContainer(container, div);

            return status;
        }


        public int CreateStackedBarGraph(GaugeDefinition rgCfg, List<GaugeSeries> gaugeSeries, System.Web.UI.HtmlControls.HtmlGenericControl container)
        {
            int status = 0;
            bool negScale = false;
            int axisItemsCount = 0;
            int radHeight = 0;
			double actualMinValue = 0;

            if (gaugeSeries == null || gaugeSeries.Count == 0)
                return -1;

            bool exploded = rgCfg.ItemVisual == "E" ? true : false;

            RadHtmlChart rad = new RadHtmlChart();

            if (rgCfg.Height > 0)
                rad.Height = rgCfg.Height;
            if (rgCfg.Width > 0)
                rad.Width = rgCfg.Width;

			if (!string.IsNullOrWhiteSpace(rgCfg.OnLoad))
				rad.ClientEvents.OnLoad = rgCfg.OnLoad;
          
            rad.ChartTitle.Text = rgCfg.Title;
            rad.ChartTitle.Appearance.TextStyle.FontSize = 12;
            rad.ChartTitle.Appearance.TextStyle.Bold = true;
            
            rad.Skin = "Metro";

            rad.Legend.Appearance.Position = rgCfg.LegendPosition;
			rad.Legend.Appearance.BackgroundColor = rgCfg.LegendBackgroundColor;
			rad.Legend.Appearance.Visible = true;

            rad.PlotArea.YAxis.TitleAppearance.Text = rgCfg.LabelV;
            rad.PlotArea.YAxis.LabelsAppearance.TextStyle.FontSize = 11;
            rad.PlotArea.YAxis.TitleAppearance.TextStyle.FontSize = 12;
            rad.PlotArea.YAxis.TitleAppearance.TextStyle.Bold = true;
 
            rad.PlotArea.YAxis.MinorGridLines.Visible = rgCfg.MinorTics;
            rad.PlotArea.XAxis.MinorGridLines.Visible = false;

			decimal? Ymax = null;
            foreach (GaugeSeries gs in gaugeSeries)
            {
                foreach (GaugeSeriesItem data in gs.ItemList)
                {
					if (data.YValue < 0)
					{
						negScale = true;
						actualMinValue = Math.Min(actualMinValue, (double)Convert(data.YValue ?? 0, rgCfg.Multiplier));
					}
					 if (!Ymax.HasValue || data.YValue > Ymax)
						 Ymax = data.YValue;
                }
            }

			if (rgCfg.ScaleMin.HasValue)
			{
				if (negScale)
					rad.PlotArea.YAxis.MinValue = (decimal)actualMinValue;
				else
					rad.PlotArea.YAxis.MinValue = rgCfg.ScaleMin.Value;
			}
			if (rgCfg.ScaleMax != 0)
			{
				rad.PlotArea.YAxis.MaxValue = Math.Max(rgCfg.ScaleMax, (decimal)Ymax);
			}

			rad.PlotArea.YAxis.LabelsAppearance.DataFormatString = SetValueFormat(rgCfg, "#.#");

            int numItems = 0;
            int numBars = 0;
            foreach (GaugeSeries gs in gaugeSeries)
            {
                BarSeries series = new BarSeries();
                series.Stacked = rgCfg.Grouping == 2 ? true : false;
                series.LabelsAppearance.DataFormatString = SetValueFormat(rgCfg, "#.#");
                series.TooltipsAppearance.DataFormatString = SetValueFormat(rgCfg, "#.#");
                series.TooltipsAppearance.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("white");

                series.LabelsAppearance.Visible = false;
                series.TooltipsAppearance.Visible = true;
				series.Name = gs.Name.Replace("\r\n", "");
                if (!string.IsNullOrEmpty(rgCfg.ColorPallete))
                    series.Appearance.FillStyle.BackgroundColor = GetColor(rgCfg.ColorPallete, ++numItems);

                foreach (GaugeSeriesItem data in gs.ItemList)
                {
                    CategorySeriesItem item = new CategorySeriesItem();
                    if (data.YValue == 0)
                    {
                        item.Y = telerikBugValue;
                    }
                    else
                    {
                        item.Y = Convert(data.YValue ?? 0, rgCfg.Multiplier);
                    }
                    
                    if (numItems == 1)
                    {
                        ++axisItemsCount;
                        if (!negScale)
                            rad.PlotArea.XAxis.Items.Add(new AxisItem(data.Text));
                    }

                    if ((bool)series.Stacked)
                    {
                        if (numItems == 1)
                            ++numBars;
                    }
                    else
                    {
                        ++numBars;
                    }

                    series.SeriesItems.Add(item);
                }

                series.LabelsAppearance.Visible = true;
                series.LabelsAppearance.DataFormatString = SetValueFormat(rgCfg, "#.#");

                if ((bool)series.Stacked)
                {
                    series.LabelsAppearance.Visible = false;
                }
                else
                {
                    series.LabelsAppearance.Position = BarColumnLabelsPosition.OutsideEnd;
                }

                series.TooltipsAppearance.Visible = true;
                series.TooltipsAppearance.DataFormatString = SetValueFormat(rgCfg, "#.#");
                series.TooltipsAppearance.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("white");

                rad.PlotArea.Series.Add(series);
            }


            radHeight = Math.Max(numBars * 24 + 120, rgCfg.Height);
            rad.Height = radHeight;

            if (negScale)
            {
                PrependBarLabelChart(rgCfg, gaugeSeries[0].ItemList, container, radHeight, 38);
                rgCfg.NewRow = false;
                if (rgCfg.Width > 0)
                    rad.Width = Math.Max(120,rgCfg.Width - 120);
            }
            else
            {
                rad.PlotArea.XAxis.TitleAppearance.Text = rgCfg.LabelA;
                rad.PlotArea.XAxis.LabelsAppearance.TextStyle.FontSize = 12;
                rad.PlotArea.XAxis.TitleAppearance.TextStyle.FontSize = 12;
                rad.PlotArea.XAxis.TitleAppearance.TextStyle.Bold = true;
            }
            
            System.Web.HttpBrowserCapabilities browser = (System.Web.HttpBrowserCapabilities)SessionManager.Browser;
            if (browser.Browser == "IE" && browser.Version.StartsWith("8"))
            {
                ;
            }
            else
            {
                if (rgCfg.Grouping == 2)
                {
                    LineSeries summaryLine = OverlaySummaryLine(rgCfg, gaugeSeries, axisItemsCount, false, false, true);
                    rad.PlotArea.Series.Add(summaryLine);
                }
            }
            
            LineSeries targetLine = IndicatorLine(rgCfg, gaugeSeries[0].ItemList.Count, rad);

			rad.Legend.Appearance.Position = rgCfg.LegendPosition;
			rad.Legend.Appearance.BackgroundColor = rgCfg.LegendBackgroundColor;
			rad.Legend.Appearance.Visible = rgCfg.DisplayLegend;

            System.Web.UI.HtmlControls.HtmlGenericControl div = CreateContainer(rgCfg);
            if (negScale)
                div.Style.Remove("margin-left");
            div.Controls.Add(rad);

           // container.Controls.Add(div);
            BindToContainer(container, div);

            return status;
        }
        #endregion

        #region columnchart
        public int CreateColumnChart(GaugeDefinition rgCfg, List<GaugeSeries> gaugeSeries, System.Web.UI.HtmlControls.HtmlGenericControl container)
        {
            int status = 0;
			int numItems = 0;

            if (gaugeSeries == null || gaugeSeries.Count == 0 ||  gaugeSeries[0].ItemList.Count == 0)
                return -1;

            List<GaugeSeriesItem> seriesData = gaugeSeries[0].ItemList;
            // gotta do this to get rad to display zero label values
			/*
            foreach (GaugeSeriesItem item in seriesData)
            {
                if (item.YValue == 0)
                    item.YValue = telerikBugValue;
            }
			*/
            bool exploded = rgCfg.ItemVisual == "E" ? true : false;

            RadHtmlChart rad = new RadHtmlChart();
            if (rgCfg.Height > 0)
                rad.Height = rgCfg.Height;
            if (rgCfg.Width > 0)
                rad.Width = rgCfg.Width;

			if (!string.IsNullOrWhiteSpace(rgCfg.OnLoad))
				rad.ClientEvents.OnLoad = rgCfg.OnLoad;

			if (rgCfg.ScaleMin.HasValue)
				rad.PlotArea.YAxis.MinValue = rgCfg.ScaleMin.Value;
			if (rgCfg.ScaleMax != 0)
                rad.PlotArea.YAxis.MaxValue = rgCfg.ScaleMax;

            rad.ChartTitle.Text = rgCfg.Title;
            rad.ChartTitle.Appearance.TextStyle.FontSize = 12;
            rad.ChartTitle.Appearance.TextStyle.Bold = true;
			rad.ChartTitle.Appearance.TextStyle.Margin = "8 0 12 0";

            rad.Skin = "Metro";
            rad.Legend.Appearance.Position = rgCfg.LegendPosition;
			rad.Legend.Appearance.BackgroundColor = rgCfg.LegendBackgroundColor;
			rad.Legend.Appearance.Visible = true;

            ColumnSeries series = new ColumnSeries();
            series.LabelsAppearance.Position = BarColumnLabelsPosition.OutsideEnd;
            series.LabelsAppearance.DataFormatString = SetValueFormat(rgCfg, "#.#");
            series.TooltipsAppearance.DataFormatString = SetValueFormat(rgCfg, "#.#");
            series.LabelsAppearance.Visible = true;
            series.TooltipsAppearance.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("white");

            foreach (GaugeSeriesItem item in seriesData)
            {
                SeriesItem si = new SeriesItem();
                si.YValue = Convert(item.YValue ?? 0, rgCfg.Multiplier);
				if (!string.IsNullOrEmpty(rgCfg.ColorPallete))
					si.BackgroundColor = GetColor(rgCfg.ColorPallete, ++numItems);
                series.Items.Add(si);
            }

            rad.PlotArea.YAxis.TitleAppearance.Text = rgCfg.LabelV;
            rad.PlotArea.YAxis.LabelsAppearance.TextStyle.FontSize = 11;
            rad.PlotArea.YAxis.TitleAppearance.TextStyle.FontSize = 12;
            rad.PlotArea.YAxis.TitleAppearance.TextStyle.Bold = true;

            rad.PlotArea.XAxis.TitleAppearance.Text = rgCfg.LabelA;
            rad.PlotArea.XAxis.LabelsAppearance.TextStyle.FontSize = 11;
            rad.PlotArea.XAxis.TitleAppearance.TextStyle.FontSize = 12;
            rad.PlotArea.XAxis.TitleAppearance.TextStyle.Bold = true;

            rad.PlotArea.YAxis.MinorGridLines.Visible = false;
            rad.PlotArea.XAxis.MinorGridLines.Visible = false;

            int maxLabelLen = 0;
            foreach (GaugeSeriesItem data in seriesData)
            {
                rad.PlotArea.XAxis.Items.Add(new AxisItem(data.Text));
				if (!string.IsNullOrEmpty(data.Text))
					maxLabelLen = Math.Max(maxLabelLen, data.Text.Length);
            }

            rad.PlotArea.Series.Add(series);

            int minWidth = (maxLabelLen - 1) * 8 * seriesData.Count;
            if (rgCfg.Width > 0  &&  minWidth > rgCfg.Width)
            {
                rad.PlotArea.XAxis.LabelsAppearance.RotationAngle = 70;
                if (rgCfg.Height > 0)
                    rad.Height = rgCfg.Height + 110;
            }

            IndicatorLine(rgCfg, seriesData.Count(), rad);

            if (rgCfg.OverlaySeries > 1  &&  gaugeSeries.Count > 1)
            {
                series.Name = rgCfg.LabelSubList != null  && rgCfg.LabelSubList.Count > 0 ? rgCfg.LabelSubList.ElementAt(0).Value : "";
                OverlayLine(rad, rgCfg, gaugeSeries.ElementAt(rgCfg.OverlaySeries - 1), rgCfg.LabelSubList != null && rgCfg.LabelSubList.Count > 1 ? rgCfg.LabelSubList.ElementAt(1).Value : "", "DarkSlateGray", false);
                series.LabelsAppearance.Visible = false;
            }

            System.Web.UI.HtmlControls.HtmlGenericControl div = CreateContainer(rgCfg);
            div.Controls.Add(rad);
           // container.Controls.Add(div);
            BindToContainer(container, div);

            return status;
        }

        public int CreateStackedColumnChart(GaugeDefinition rgCfg, List<GaugeSeries> gaugeSeries, System.Web.UI.HtmlControls.HtmlGenericControl container)
        {
            int status = 0;

            if (gaugeSeries == null || gaugeSeries.Count == 0)
                return -1;

            bool exploded = rgCfg.ItemVisual == "E" ? true : false;
  
            RadHtmlChart rad = new RadHtmlChart();

            if (rgCfg.Height > 0)
                rad.Height = rgCfg.Height;
            if (rgCfg.Width > 0)
                rad.Width = rgCfg.Width;

			if (!string.IsNullOrWhiteSpace(rgCfg.OnLoad))
				rad.ClientEvents.OnLoad = rgCfg.OnLoad;

			if (rgCfg.ScaleMin.HasValue)
				rad.PlotArea.YAxis.MinValue = rgCfg.ScaleMin.Value;
            if (rgCfg.ScaleMax != 0)
                rad.PlotArea.YAxis.MaxValue = rgCfg.ScaleMax;

            rad.ChartTitle.Text = rgCfg.Title;
            rad.ChartTitle.Appearance.TextStyle.FontSize = 12;
            rad.ChartTitle.Appearance.TextStyle.Bold = true;
			rad.ChartTitle.Appearance.TextStyle.Margin = "8 0 12 0";
            //  rad.PlotArea.YAxis.MinValue = rgCfg.ScaleMin;
            rad.Skin = "Metro";

            rad.Legend.Appearance.Position = rgCfg.LegendPosition;
			rad.Legend.Appearance.BackgroundColor = rgCfg.LegendBackgroundColor;
			rad.Legend.Appearance.Visible = true;

            rad.PlotArea.YAxis.TitleAppearance.Text = rgCfg.LabelV;
            rad.PlotArea.YAxis.LabelsAppearance.TextStyle.FontSize = 11;
            rad.PlotArea.YAxis.TitleAppearance.TextStyle.FontSize = 12;
            rad.PlotArea.YAxis.TitleAppearance.TextStyle.Bold = true;

            rad.PlotArea.XAxis.TitleAppearance.Text = rgCfg.LabelA;
            rad.PlotArea.XAxis.LabelsAppearance.TextStyle.FontSize = 11;
            rad.PlotArea.XAxis.TitleAppearance.TextStyle.FontSize = 12;
            rad.PlotArea.XAxis.TitleAppearance.TextStyle.Bold = true;
            rad.PlotArea.XAxis.MinorGridLines.Visible = false;

            int numSeries = 0;
            int maxLabelLen = 0;
            int seriesItemCount = 0;
            foreach (GaugeSeries gs in gaugeSeries)
            {
                ColumnSeries series = new ColumnSeries();
                ++numSeries;
                series.Stacked = rgCfg.Grouping == 2  ? true : false;
				series.Name = gs.Name.Replace("\r\n", "");
                if (!string.IsNullOrEmpty(rgCfg.ColorPallete))
                    series.Appearance.FillStyle.BackgroundColor = GetColor(rgCfg.ColorPallete, numSeries);

                decimal sumY = 0;
                foreach (GaugeSeriesItem data in gs.ItemList)
                {
                    CategorySeriesItem item = new CategorySeriesItem();
                    if (data.YValue == 0 && !(bool)series.Stacked)
                    {
                      //  item.Y = telerikBugValue;
                    }
                    else
                    {
                        item.Y = Convert(data.YValue ?? 0, rgCfg.Multiplier);
                    }

                    sumY += Math.Abs((decimal)item.Y);
                    if (numSeries == 1)
                    {
                        rad.PlotArea.XAxis.Items.Add(new AxisItem(data.Text));
						if (!string.IsNullOrEmpty(data.Text))
							maxLabelLen = Math.Max(maxLabelLen, data.Text.Length);
                    }

                    series.SeriesItems.Add(item);
                    seriesItemCount = Math.Max(seriesItemCount, series.SeriesItems.Count);
                }

                series.LabelsAppearance.Visible = true;
                series.LabelsAppearance.DataFormatString = SetValueFormat(rgCfg, "#.#");

                if ((bool)series.Stacked)
                {
                    series.LabelsAppearance.Visible = false;
                }
                else
                {
                    series.LabelsAppearance.Position = BarColumnLabelsPosition.OutsideEnd;
                }

                series.TooltipsAppearance.Visible = true;
                series.TooltipsAppearance.DataFormatString = SetValueFormat(rgCfg, "#.#");
                series.TooltipsAppearance.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("white");

                rad.PlotArea.Series.Add(series);
            }

            int minWidth = (maxLabelLen - 1) * 8 * seriesItemCount;
            if (rgCfg.Width > 0  &&  minWidth > rgCfg.Width)
            {
                rad.PlotArea.XAxis.LabelsAppearance.RotationAngle = 70;
                if (rgCfg.Height > 0)
                    rad.Height = rgCfg.Height + 110;
            }

            if (rgCfg.Grouping == 2)
            {
                System.Web.HttpBrowserCapabilities browser = (System.Web.HttpBrowserCapabilities)SessionManager.Browser;
                if (browser.Browser == "IE" && browser.Version.StartsWith("8"))
                {
                    ;
                }
                else
                {
                    rad.PlotArea.Series.Add(OverlaySummaryLine(rgCfg, gaugeSeries, rad.PlotArea.XAxis.Items.Count, false, false, true));
                }
            }

            //rad.PlotArea.YAxis.MinorGridLines.Visible = maxY <= 20 ? false : true;
            rad.PlotArea.YAxis.MajorGridLines.Visible = false;
			rad.Legend.Appearance.Position = rgCfg.LegendPosition;
			rad.Legend.Appearance.BackgroundColor = rgCfg.LegendBackgroundColor;
			rad.Legend.Appearance.Visible = rgCfg.DisplayLegend;

           // IndicatorLine(rgCfg, gaugeSeries[0].ItemList.Count, rad);

            if (gaugeSeries[0].ObjData != null)
            {
                rad.PlotArea.XAxis.MajorGridLines.Width = 2;
                if (rgCfg.TotalIndicator == true)
                {
                    List<AttributeValue> totals = (List<AttributeValue>)gaugeSeries[0].ObjData;
                    IndicatorLine(totals[0].Value, totals[0].Key, gaugeSeries[0].ItemList.Count, rad);
                }
            }

			System.Web.UI.HtmlControls.HtmlGenericControl div = CreateContainer(rgCfg);
            div.Controls.Add(rad);
           // container.Controls.Add(div);
            BindToContainer(container, div);
 
            return status;
        }
        #endregion

        #region lines

        public int CreateMultiLineChart(GaugeDefinition rgCfg, List<GaugeSeries> gaugeSeries, System.Web.UI.HtmlControls.HtmlGenericControl container)
        {
            int status = 0;

            if (gaugeSeries == null || gaugeSeries.Count == 0)
                return -1;

            RadHtmlChart rad = new RadHtmlChart();

            if (rgCfg.Height > 0 && rgCfg.Width > 0)
            {
                rad.Height = rgCfg.Height;
                rad.Width = rgCfg.Width;
            }

			if (!string.IsNullOrWhiteSpace(rgCfg.OnLoad))
				rad.ClientEvents.OnLoad = rgCfg.OnLoad;

			if (!string.IsNullOrEmpty(rgCfg.Title))
            {
                rad.ChartTitle.Text = rgCfg.Title;
                rad.ChartTitle.Appearance.TextStyle.FontSize = 12;
                rad.ChartTitle.Appearance.TextStyle.Bold = true;
            }

			if (rgCfg.ScaleMin.HasValue)
				rad.PlotArea.YAxis.MinValue = rgCfg.ScaleMin.Value;
			if (rgCfg.ScaleMax != 0)
				rad.PlotArea.YAxis.MaxValue = rgCfg.ScaleMax;

			rad.Skin = "Metro";

            if (rgCfg.DisplayLegend && rgCfg.ItemVisual != "L")
            {
                rad.Legend.Appearance.Position = rgCfg.LegendPosition;
				rad.Legend.Appearance.BackgroundColor = rgCfg.LegendBackgroundColor;
                rad.Legend.Appearance.Visible = true;
            }
            else
                rad.Legend.Appearance.Visible = false;

            rad.PlotArea.YAxis.TitleAppearance.Text = rgCfg.LabelV;
            rad.PlotArea.YAxis.LabelsAppearance.TextStyle.FontSize = 11;
            rad.PlotArea.YAxis.TitleAppearance.TextStyle.FontSize = 12;
            rad.PlotArea.YAxis.TitleAppearance.TextStyle.Bold = true;

            rad.PlotArea.XAxis.LabelsAppearance.Visible = rgCfg.DisplayLabel; // rad.Legend.Appearance.Visible;
            rad.PlotArea.XAxis.LabelsAppearance.TextStyle.FontSize = 11;
            rad.PlotArea.XAxis.MinorGridLines.Visible = false;

            int numItems = 0;
            decimal maxY = 0;
            decimal minY = 0;
            decimal minYValue = 999999999;
            foreach (GaugeSeries gs in gaugeSeries)
            {
                LineSeries series = new LineSeries();
                series.LabelsAppearance.DataFormatString = SetValueFormat(rgCfg, "#.#");
                series.TooltipsAppearance.DataFormatString = SetValueFormat(rgCfg, "#.#");
                series.TooltipsAppearance.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("white");
                series.MarkersAppearance.MarkersType = MarkersType.Square;
                series.MarkersAppearance.Size = 3m;
                series.MarkersAppearance.BorderWidth = 3;
                series.LabelsAppearance.Visible = gs.DisplayLabels;
				if (gaugeSeries.Count == 1  &&  !string.IsNullOrEmpty(rgCfg.LabelA)) //  &&  string.IsNullOrEmpty(gs.Name))
				{
					series.Name = rgCfg.LabelA;
				}
				else
				{
					series.Name = gs.Name.Replace("\r\n", "");
				}
                if (!string.IsNullOrEmpty(rgCfg.ColorPallete))
                    series.Appearance.FillStyle.BackgroundColor = GetColor(rgCfg.ColorPallete, ++numItems);

				if (gs.SeriesType == 9)		// totals series
				{
					series.MarkersAppearance.MarkersType = MarkersType.Cross;
					series.LineAppearance.LineStyle = Telerik.Web.UI.HtmlChart.Enums.ExtendedLineStyle.Step;
					series.Appearance.FillStyle.BackgroundColor = System.Drawing.Color.Gray;
					series.LineAppearance.Width = 2;
				}
 
                decimal sumY = 0;
                foreach (GaugeSeriesItem data in gs.ItemList)	// points
                {
                    CategorySeriesItem item = new CategorySeriesItem();
					if (data.YValue.HasValue)
						item.Y = Convert(data.YValue.Value, rgCfg.Multiplier);
                    minYValue = Math.Min(minYValue, item.Y ?? 0);
                    sumY += Math.Abs(item.Y ?? 0);
                    if (numItems == 1)
                        rad.PlotArea.XAxis.Items.Add(new AxisItem(data.Text));
                       
                    series.SeriesItems.Add(item);
                    if (series.SeriesItems.Count > 9)
                    {
                        rad.PlotArea.XAxis.LabelsAppearance.RotationAngle = 70;
                        if (rgCfg.Height > 0)
                            rad.Height = rgCfg.Height + 110;
                    }

                }
                rad.PlotArea.Series.Add(series);
                maxY = Math.Max(maxY, sumY);
                minY = Math.Min(minY, sumY);
            }

            if (minYValue < 0)
                rad.PlotArea.YAxis.AxisCrossingValue = minYValue;

            rad.PlotArea.YAxis.MinorGridLines.Visible = maxY <= 20 ? false : true;

            LineSeries targetLine = IndicatorLine(rgCfg, gaugeSeries[0].ItemList.Count, rad);

            System.Web.UI.HtmlControls.HtmlGenericControl div = CreateContainer(rgCfg);
            div.Controls.Add(rad);
           // container.Controls.Add(div);
            BindToContainer(container, div);

            return status;
        }

        #endregion

        #region pie

		public int CreatePieChart(GaugeDefinition rgCfg, List<GaugeSeries> gaugeSeries, System.Web.UI.HtmlControls.HtmlGenericControl container)
		{
			int status = 0;
			int numItems = 0;

			if (gaugeSeries == null || gaugeSeries.Count == 0 || gaugeSeries[0].ItemList.Count == 0)
				return -1;

			bool exploded = gaugeSeries[0].ItemList.Count > 1 ? true : false;

			RadHtmlChart rad = new RadHtmlChart();
			if (rgCfg.Height > 0)
				rad.Height = rgCfg.Height;
			if (rgCfg.Width > 0)
				rad.Width = rgCfg.Width;

			rad.ChartTitle.Text = rgCfg.Title;
			rad.ChartTitle.Appearance.TextStyle.FontSize = 12;
			rad.ChartTitle.Appearance.TextStyle.Bold = true;

			PieSeries series = new PieSeries();
			series.StartAngle = 45;// 90;
			series.LabelsAppearance.Position = Telerik.Web.UI.HtmlChart.PieAndDonutLabelsPosition.OutsideEnd;
			series.LabelsAppearance.DataFormatString = "{0} " + rgCfg.LabelV;
			series.TooltipsAppearance.Visible = false;

			foreach (GaugeSeriesItem data in gaugeSeries[0].ItemList)
			{
				if (!string.IsNullOrEmpty(data.Text.Trim()))
				{
					PieSeriesItem item = new PieSeriesItem();
					item.Name = data.Text.Replace("\r\n", "");
					item.Y = Math.Round(data.YValue ?? 0, 2);
					if (!string.IsNullOrEmpty(rgCfg.ColorPallete))
						item.BackgroundColor = GetColor(rgCfg.ColorPallete, ++numItems);

					item.Exploded = exploded;
					series.SeriesItems.Add(item);
				}
			}

			rad.PlotArea.Series.Add(series);

			System.Web.UI.HtmlControls.HtmlGenericControl div = CreateContainer(rgCfg);
			div.Controls.Add(rad);
			BindToContainer(container, div);

			return status;
		}


        public int CreateSpiderChart(GaugeDefinition rgCfg, List<GaugeSeries> gaugeSeries, System.Web.UI.HtmlControls.HtmlGenericControl container)
        {
            int status = 0;
            int numItems = 0;

            if (gaugeSeries == null || gaugeSeries.Count == 0  ||  gaugeSeries[0].ItemList.Count == 0)
                return -1;

            bool exploded = gaugeSeries[0].ItemList.Count > 1 ? true : false;


            RadHtmlChart rad = new RadHtmlChart();

            if (rgCfg.Height > 0)
                rad.Height = rgCfg.Height;
            if (rgCfg.Width > 0)
                rad.Width = rgCfg.Width;

			rad.ChartTitle.Text = rgCfg.Title;
            rad.ChartTitle.Appearance.TextStyle.FontSize = 12;
            rad.ChartTitle.Appearance.TextStyle.Bold = true;

			foreach (GaugeSeries gs in gaugeSeries)
			{
				RadarAreaSeries series = new RadarAreaSeries();
				series.Name = gs.Name;
				series.MissingValues = MissingValuesBehavior.Gap;
				series.Appearance.FillStyle.BackgroundColor = GetColor(rgCfg.ColorPallete, ++numItems);
				series.LineAppearance.Width = 1;
				series.MarkersAppearance.Visible = false;

				foreach (GaugeSeriesItem gi in gs.ItemList)
				{
					CategorySeriesItem item = new CategorySeriesItem();
					item.Y = gi.YValue;
					series.SeriesItems.Add(item);
				}

				rad.PlotArea.Series.Add(series);
			}

			rad.PlotArea.XAxis.Color = Color.Black;
			rad.PlotArea.XAxis.StartAngle = 0;
			rad.PlotArea.XAxis.MajorGridLines.Color = Color.BlueViolet;
			rad.PlotArea.XAxis.MajorGridLines.Width = 1;
			foreach (GaugeSeriesItem gi in gaugeSeries[0].ItemList)
			{
				AxisItem axis = new AxisItem(gi.Text);
				rad.PlotArea.XAxis.Items.Add(axis);
			}

			rad.PlotArea.YAxis.Visible = false;
			rad.PlotArea.YAxis.MajorGridLines.Color = Color.BlueViolet;
			rad.PlotArea.YAxis.LabelsAppearance.Step = 1;
			rad.PlotArea.YAxis.MaxValue = rgCfg.ScaleMax;
			rad.PlotArea.YAxis.MinValue = rgCfg.ScaleMin;

            System.Web.UI.HtmlControls.HtmlGenericControl div = CreateContainer(rgCfg);
            div.Controls.Add(rad);
            BindToContainer(container, div);

            return status;
        }
        #endregion

        #region pareto

        public int CreateColumnParetoChart(GaugeDefinition rgCfg, List<GaugeSeries> gaugeSeries, System.Web.UI.HtmlControls.HtmlGenericControl container)
        {
            int status = 0;
            int numItems = 0;

            if (gaugeSeries == null || gaugeSeries.Count == 0)
                return -1;

            List<GaugeSeriesItem> seriesData = gaugeSeries[0].ItemList.OrderByDescending(l => l.YValue).ToList();
            if (seriesData.Select(l => l.YValue).Sum() == 0)
                return -1;

            bool exploded = rgCfg.ItemVisual == "E" ? true : false;

            RadHtmlChart rad = new RadHtmlChart();
            if (rgCfg.Height > 0)
                rad.Height = rgCfg.Height;
            if (rgCfg.Width > 0)
                rad.Width = rgCfg.Width;

			if (!string.IsNullOrWhiteSpace(rgCfg.OnLoad))
				rad.ClientEvents.OnLoad = rgCfg.OnLoad;

			rad.ChartTitle.Text = rgCfg.Title;
            rad.ChartTitle.Appearance.TextStyle.FontSize = 12;
            rad.ChartTitle.Appearance.TextStyle.Bold = true;
			rad.ChartTitle.Appearance.TextStyle.Margin = "8 0 12 0";

            //rad.PlotArea.YAxis.MinValue = rgCfg.ScaleMin;
            rad.Skin = "Metro";

            ColumnSeries series = new ColumnSeries();
            series.LabelsAppearance.Visible = true;

            series.LabelsAppearance.DataFormatString = SetValueFormat(rgCfg, "#");
            series.TooltipsAppearance.DataFormatString = SetValueFormat(rgCfg, "#");
            series.DataFieldY = "YValue";
            series.TooltipsAppearance.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("white");
            if (!String.IsNullOrEmpty(rgCfg.DefaultScaleColor))
                series.Appearance.FillStyle.BackgroundColor = System.Drawing.ColorTranslator.FromHtml(rgCfg.DefaultScaleColor);
            else
                series.Appearance.FillStyle.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("#CD5C5C");

			rad.DataSource = rgCfg.TopN == 0 ? seriesData : seriesData.Take(rgCfg.TopN).ToList();
            rad.DataBind();

            if (!string.IsNullOrEmpty(rgCfg.LabelV))
                rad.PlotArea.YAxis.TitleAppearance.Text = rgCfg.LabelV;
            rad.PlotArea.YAxis.LabelsAppearance.TextStyle.FontSize = 11;
            rad.PlotArea.YAxis.TitleAppearance.TextStyle.FontSize = 12;
            rad.PlotArea.YAxis.TitleAppearance.TextStyle.Bold = true;

            if (!string.IsNullOrEmpty(rgCfg.LabelA))
                 rad.PlotArea.XAxis.TitleAppearance.Text = rgCfg.LabelA;
            rad.PlotArea.XAxis.LabelsAppearance.TextStyle.FontSize = 11;
            rad.PlotArea.XAxis.TitleAppearance.TextStyle.FontSize = 12;
            rad.PlotArea.XAxis.TitleAppearance.TextStyle.Bold = true;

            rad.PlotArea.YAxis.MinorGridLines.Visible = false;
            rad.PlotArea.XAxis.MinorGridLines.Visible = false;

			foreach (GaugeSeriesItem data in rgCfg.TopN == 0 ? seriesData : seriesData.Take(rgCfg.TopN).ToList())
            {
                rad.PlotArea.XAxis.Items.Add(new AxisItem(data.Text));
            }

            rad.PlotArea.YAxis.MaxValue = seriesData.Sum(l => l.YValue);
            //series.LabelsAppearance.Position = Telerik.Web.UI.HtmlChart.BarColumnLabelsPosition.InsideEnd;
            series.LabelsAppearance.Position = BarColumnLabelsPosition.Center;
            if (seriesData.Count > 6)
            rad.PlotArea.XAxis.LabelsAppearance.RotationAngle = 70;
            rad.PlotArea.Series.Add(series);

            AxisY lorenzeAxis = new AxisY()
            {
                MinValue = 0,
                MaxValue = 100,
                Name = "Lorenze",
                Visible = true
            };

            rad.PlotArea.AdditionalYAxes.Add(lorenzeAxis);
            rad.PlotArea.AdditionalYAxes[0].TitleAppearance.TextStyle.FontSize = 11;
            rad.PlotArea.AdditionalYAxes[0].TitleAppearance.TextStyle.Bold = true;
            rad.PlotArea.AdditionalYAxes[0].LabelsAppearance.TextStyle.FontSize = 11;
            rad.PlotArea.AdditionalYAxes[0].LabelsAppearance.DataFormatString = "{0}%";

            LineSeries lorenzeSeries = new LineSeries();
            lorenzeSeries.MarkersAppearance.Visible = false;
            lorenzeSeries.LabelsAppearance.DataFormatString = "{0}%";
            lorenzeSeries.AxisName = "Lorenze";
            lorenzeSeries.Appearance.FillStyle.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("red");
            decimal total = seriesData.Select(l => l.YValue ?? 0).Sum();
            decimal totalPct = 0;
			int itemCount = 0;
            foreach (GaugeSeriesItem item in seriesData)
            {
				if (rgCfg.TopN == 0 || ++itemCount <= rgCfg.TopN)
				{
					if (total == 0)
						lorenzeSeries.SeriesItems.Add(new CategorySeriesItem((totalPct += Decimal.Round(0 * 100m, 2))));
					else
					{
						totalPct += Decimal.Round((item.YValue ?? 0) / total * 100m, 2);
						if (item == seriesData.Last())
							totalPct = 100m;
						lorenzeSeries.SeriesItems.Add(new CategorySeriesItem(totalPct));
						//lorenzeSeries.SeriesItems.Add(new CategorySeriesItem((totalPct += Decimal.Round(item.YValue / total * 100m, 2))));
					}
				}
            }

            rad.PlotArea.Series.Add(lorenzeSeries);

            System.Web.UI.HtmlControls.HtmlGenericControl div = CreateContainer(rgCfg);
            div.Controls.Add(rad);
           // container.Controls.Add(div);
            BindToContainer(container, div);

            return status;
        }

        public int CreateBarParetoChart(GaugeDefinition rgCfg, List<GaugeSeries> gaugeSeries, System.Web.UI.HtmlControls.HtmlGenericControl container)
        {
            int status = 0;
            int numItems = 0;

            if (gaugeSeries == null || gaugeSeries.Count == 0)
                return -1;

            List<GaugeSeriesItem> seriesData = gaugeSeries[0].ItemList.OrderByDescending(l => l.YValue).ToList();
            if (seriesData.Select(l => l.YValue).Sum() == 0)
                return -1;

            bool exploded = rgCfg.ItemVisual == "E" ? true : false;

            RadHtmlChart rad = new RadHtmlChart();
            if (rgCfg.Height > 0)
                rad.Height = rgCfg.Height;
            if (rgCfg.Width > 0)
                rad.Width = rgCfg.Width;

			if (!string.IsNullOrWhiteSpace(rgCfg.OnLoad))
				rad.ClientEvents.OnLoad = rgCfg.OnLoad;
           
            rad.ChartTitle.Text = rgCfg.Title;
            rad.ChartTitle.Appearance.TextStyle.FontSize = 12;
            rad.ChartTitle.Appearance.TextStyle.Bold = true;
          //  rad.PlotArea.YAxis.MinValue = rgCfg.ScaleMin;
            rad.Skin = "Metro";

            BarSeries series = new BarSeries();
            series.LabelsAppearance.Visible = true;
            series.LabelsAppearance.Position = BarColumnLabelsPosition.Center;
            series.LabelsAppearance.DataFormatString = SetValueFormat(rgCfg, "#");
            series.TooltipsAppearance.DataFormatString = SetValueFormat(rgCfg, "#");
            series.DataFieldY = "YValue";

            if (!String.IsNullOrEmpty(rgCfg.DefaultScaleColor))
                series.Appearance.FillStyle.BackgroundColor = System.Drawing.ColorTranslator.FromHtml(rgCfg.DefaultScaleColor);
            else
                series.Appearance.FillStyle.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("#CD5C5C");

            series.TooltipsAppearance.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("white");

            rad.DataSource = rgCfg.TopN == 0 ? seriesData : seriesData.Take(rgCfg.TopN).ToList();
            rad.DataBind();
            rad.PlotArea.YAxis.MaxValue = seriesData.Sum(l => l.YValue);

            rad.PlotArea.XAxis.DataLabelsField = "Text";

            rad.PlotArea.YAxis.LabelsAppearance.TextStyle.FontSize = 11;
            rad.PlotArea.YAxis.TitleAppearance.TextStyle.FontSize = 11;
            rad.PlotArea.YAxis.TitleAppearance.TextStyle.Bold = true;

            if (!string.IsNullOrEmpty(rgCfg.LabelV))
                rad.PlotArea.XAxis.TitleAppearance.Text = rgCfg.LabelV;
            rad.PlotArea.XAxis.LabelsAppearance.TextStyle.FontSize = 11;
            rad.PlotArea.XAxis.TitleAppearance.TextStyle.FontSize = 11;
            rad.PlotArea.XAxis.TitleAppearance.TextStyle.Bold = true;

            rad.PlotArea.Series.Add(series);

            AxisY lorenzeAxis = new AxisY()
            {
                MinValue = 0,
                MaxValue = 100,
                Name = "Lorenze",
                Visible = true
            };

            rad.PlotArea.AdditionalYAxes.Add(lorenzeAxis);
            rad.PlotArea.AdditionalYAxes[0].TitleAppearance.TextStyle.FontSize = 11;
            rad.PlotArea.AdditionalYAxes[0].TitleAppearance.TextStyle.Bold = true;
            rad.PlotArea.AdditionalYAxes[0].LabelsAppearance.TextStyle.FontSize = 11;
            rad.PlotArea.AdditionalYAxes[0].LabelsAppearance.DataFormatString = "{0}%";

            LineSeries lorenzeSeries = new LineSeries();
            lorenzeSeries.MarkersAppearance.Visible = false;
            lorenzeSeries.LabelsAppearance.DataFormatString = "{0}%";
            lorenzeSeries.AxisName = "Lorenze";
            lorenzeSeries.Appearance.FillStyle.BackgroundColor = System.Drawing.ColorTranslator.FromHtml("red");
            decimal total = seriesData.Select(l => l.YValue ?? 0).Sum();
            decimal totalPct = 0;
			int itemCount = 0;
            foreach (GaugeSeriesItem item in seriesData)
            {
				if (rgCfg.TopN == 0 || ++itemCount <= rgCfg.TopN)
				{
					if (total == 0)
						lorenzeSeries.SeriesItems.Add(new CategorySeriesItem((totalPct += Decimal.Round(0 * 100m, 2))));
					else
					{
						totalPct += Decimal.Round((item.YValue ?? 0) / total * 100m, 2);
						if (item == seriesData.Last())
							totalPct = 100m;
						lorenzeSeries.SeriesItems.Add(new CategorySeriesItem(totalPct));
						// lorenzeSeries.SeriesItems.Add(new CategorySeriesItem((totalPct += Decimal.Round(item.YValue / total * 100m, 2))));
					}
				}
            }

            rad.PlotArea.Series.Add(lorenzeSeries);

            rad.Legend.Appearance.Visible = false;

            System.Web.UI.HtmlControls.HtmlGenericControl div = CreateContainer(rgCfg);
            div.Controls.Add(rad);
          //  container.Controls.Add(div);
            BindToContainer(container, div);

            return status;
        }
        #endregion
    }
}