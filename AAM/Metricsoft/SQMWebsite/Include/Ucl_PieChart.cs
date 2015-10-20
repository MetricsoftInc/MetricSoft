using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Security.Permissions;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Windows.Forms;
using Telerik.Web.UI.HtmlChart;

namespace SQM.Website
{
	[AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal),
		AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), ToolboxData("<{0}:PieChart runat=\"server\"></{0}:PieChart>")]
	public class PieChart : CompositeControl, INamingContainer
	{
		/// <summary>
		/// Stores the information about the legend.
		/// </summary>
		struct LegendInfo
		{
			public SizeF Size { get; set; }
			public float LabelWidth { get; set; }
			public PointF StartingPoint { get; set; }
			public int MaxItems { get; set; }
		}

		[Category("Appearance"), DefaultValue(""), Description("The title of the pie chart.")]
		public string Title { get; set; }

		Style titleStyle = null;

		[Category("Appearance"), DefaultValue(null), Description("The style of the title of the pie chart. (NOTE: Only the ForeColor and Font properties are being used.)"),
			DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty)]
		public Style TitleStyle
		{
			get
			{
				if (this.titleStyle == null)
					this.titleStyle = new Style();
				return this.titleStyle;
			}
		}

		[Category("Layout"), DefaultValue(typeof(Unit), "400px"), Description("The height of the control.")]
		public override Unit Width
		{
			get { return base.Width; }
			set { base.Width = value; }
		}

		[Category("Layout"), DefaultValue(typeof(Unit), "400px"), Description("The width of the control.")]
		public override Unit Height
		{
			get { return base.Height; }
			set { base.Height = value; }
		}

		[Category("Appearance"), DefaultValue(0), Description("The starting angle (in degrees) for the first slice of the pie chart.")]
		public decimal StartAngle { get; set; }

		// Private read-only property to get the start angle in radians, as that is more important in our calculations.
		decimal StartAngleRadians
		{
			get { return (decimal)((double)this.StartAngle * Math.PI) / 180; }
		}

		[Category("Appearance"), DefaultValue(70), Description("The percentage from the center of the pie chart towards the edge in which to place the value labels.")]
		public decimal LabelPercentToEdge { get; set; }

		[Category("Appearance"), DefaultValue(ChartLegendPosition.Right), Description("The position of the pie chart's legend.")]
		public ChartLegendPosition LegendPosition { get; set; }

		[Category("Appearance"), DefaultValue(false), Description("Whether or not to allow values of 0 to be rendered.")]
		public bool AllowZeroValues { get; set; }

		List<MyColor> colorScheme = null;

		[Category("Appearance"), Description("The color scheme of the pie chart."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content),
			PersistenceMode(PersistenceMode.InnerProperty), DefaultValue(null), MergableProperty(false)]
		public List<MyColor> ColorScheme
		{
			get
			{
				if (this.colorScheme == null)
					this.colorScheme = new List<MyColor>();
				return this.colorScheme;
			}
			set { this.colorScheme = value; }
		}

		List<GaugeSeriesItem> values = null;

		[Category("Data"), Description("The values of the pie chart."), DesignerSerializationVisibility(DesignerSerializationVisibility.Content), PersistenceMode(PersistenceMode.InnerProperty),
			DefaultValue(null), MergableProperty(false)]
		public List<GaugeSeriesItem> Values
		{
			get
			{
				if (this.values == null)
					this.values = new List<GaugeSeriesItem>();
				return this.values;
			}
			set { this.values = value; }
		}

		decimal PieRadius { get; set; }
		HtmlGenericControl SVG { get; set; }

		// I want this to be a div, not a span, thus the override.
		protected override HtmlTextWriterTag TagKey
		{
			get { return HtmlTextWriterTag.Div; }
		}

		public PieChart() : base()
		{
			this.ClientIDMode = ClientIDMode.Predictable;
			// Setting default color for title.
			this.TitleStyle.ForeColor = Color.Black;
			// Setting bold as default for title.
			this.TitleStyle.Font.Bold = true;
			this.Width = new Unit(400, UnitType.Pixel);
			this.Height = new Unit(400, UnitType.Pixel);
			// NOTE: Setting the default font here to Verdana because otherwise Font.Name is an empty string and converts to Microsoft Sans Serif instead of the page's font.
			this.Font.Name = "Verdana";
			// NOTE: We have to set a default font size, otherwise my GetFont() extension method throws an exception.
			this.Font.Size = new FontUnit(10, UnitType.Pixel);
			this.StartAngle = 0;
			this.LabelPercentToEdge = 70;
			this.LegendPosition = ChartLegendPosition.Right;
		}

		/// <summary>
		/// Gets the color of the color scheme, wrapped to the number of colors so we never go out of the bounds of the list.
		/// </summary>
		/// <param name="i">The position of the color we want.</param>
		/// <returns>The color for the given position.</returns>
		Color GetColorAt(int i)
		{
			// We do this here as opposed to the constructor do that way a color scheme can be defined prior to the control's child controls being created.
			if (this.ColorScheme.Count == 0)
				this.colorScheme = new List<MyColor>()
				{
					Color.Red,
					Color.Yellow,
					Color.Lime,
					Color.Cyan,
					Color.Blue,
					Color.Magenta
				};
			return this.ColorScheme[i % this.ColorScheme.Count].Color;
		}

		/// <summary>
		/// Get the width of a string with the given font.
		/// Comes from http://stackoverflow.com/a/12635970
		/// </summary>
		/// <param name="str">The string to measure the width of.</param>
		/// <param name="font">The font to measure.</param>
		/// <returns>The width of the given string with the given font.</returns>
		static float GetWidthOfString(string str, Font font)
		{
			return TextRenderer.MeasureText(str, font).Width;
		}

		/// <summary>
		/// Converts a font's size into a string.
		/// </summary>
		/// <param name="font">The Font to use.</param>
		/// <returns>The font's size as a string.</returns>
		static string FontSizeAsString(Font font)
		{
			return string.Format("{0}{1}", font.Size, font.Unit == GraphicsUnit.Inch ? "in" : (font.Unit == GraphicsUnit.Millimeter ? "mm" : (font.Unit == GraphicsUnit.Pixel ? "px" :
				font.Unit == GraphicsUnit.Point ? "pt" : "")));
		}

		/// <summary>
		/// Gets a contrasted black or white color depending on the given color.
		/// Comes from http://stackoverflow.com/a/1855903
		/// </summary>
		/// <param name="color">The background color.</param>
		/// <returns>A properly contrasted black or white color depending on the background color.</returns>
		static Color ContrastColor(Color color)
		{
			// Counting the perceptive luminance - human eye favors green color...
			if (1 - (0.299 * color.R + 0.587 * color.G + 0.114 * color.B) / 255 < 0.5)
				return Color.Black; // bright colors - black font
			else
				return Color.White; // dark colors - white font
		}

		/// <summary>
		/// Will calculate the information about the legend.
		/// </summary>
		/// <param name="def">The pie chart's defintion.</param>
		/// <param name="numberOfLabels">The number of labels the legend will contain.</param>
		/// <param name="longestLabelWidth">The width of the longest label's text.</param>
		/// <returns>The information about the legend.</returns>
		LegendInfo getLegendInfo(int numberOfLabels, int longestLabelWidth, int fontHeight)
		{
			int fullLabelWidth = 12 + longestLabelWidth;
			float halfFontHeight = fontHeight / 2f;
			float width, height;
			float labelWidth;
			float x, y;
			int maxItems;
			if (this.LegendPosition == ChartLegendPosition.Left || this.LegendPosition == ChartLegendPosition.Right)
			{
				height = this.Height.ToPixels() - fontHeight * 3;
				maxItems = (int)((float)(this.Height.ToPixels() - fontHeight * 5) / fontHeight);
				int numberOfColumns = (numberOfLabels - 1) / maxItems + 1;
				width = (numberOfColumns * (fontHeight + 2 * fullLabelWidth) + 3 * fontHeight) / 2f;

				labelWidth = fullLabelWidth;

				x = halfFontHeight;
				if (this.LegendPosition == ChartLegendPosition.Right)
					x += this.Width.ToPixels() - width;
				if (numberOfLabels >= maxItems)
					y = halfFontHeight;
				else
					y = (height - numberOfLabels * (fontHeight + 1) + 1) / 2;
				y += fontHeight * 3;
			}
			else
			{
				width = this.Width.ToPixels();
				maxItems = (int)((float)(2 * this.Width.ToPixels() - fontHeight) / (fontHeight + 2 * fullLabelWidth));
				int numberOfRows = (numberOfLabels - 1) / maxItems + 1;
				height = fontHeight * (numberOfRows + 1) + numberOfRows - 1;

				labelWidth = fullLabelWidth;
				if (numberOfLabels >= maxItems)
					labelWidth = (width - fontHeight * 2) / maxItems;

				if (numberOfLabels >= maxItems)
					x = halfFontHeight;
				else
					x = (this.Width.ToPixels() - fullLabelWidth * numberOfLabels - (numberOfLabels - 1) * halfFontHeight) / 2;
				if (this.LegendPosition == ChartLegendPosition.Bottom)
					y = this.Height.ToPixels() - height + halfFontHeight;
				else
					y = 5 * halfFontHeight;

			}
			return new LegendInfo()
			{
				Size = new SizeF(width, height),
				LabelWidth = labelWidth,
				StartingPoint = new PointF(x, y),
				MaxItems = maxItems
			};
		}

		/// <summary>
		/// Determines where the center of the pie chart will be located.
		/// </summary>
		/// <param name="legend">The information about the legend.</param>
		/// <param name="fontHeight">The height of the font being used.</param>
		/// <returns>The center point of the pie chart.</returns>
		PointF calculatePieCenter(LegendInfo legend, int fontHeight)
		{
			float x = 0, y = 0;
			switch (this.LegendPosition)
			{
				case ChartLegendPosition.Left:
					x = (this.Width.ToPixels() + legend.Size.Width) / 2;
					y = (this.Height.ToPixels() + fontHeight * 3) / 2f;
					break;
				case ChartLegendPosition.Right:
					x = (this.Width.ToPixels() - legend.Size.Width) / 2;
					y = (this.Height.ToPixels() + fontHeight * 3) / 2f;
					break;
				case ChartLegendPosition.Top:
					x = this.Width.ToPixels() / 2f;
					y = (this.Height.ToPixels() + fontHeight * 3 + legend.Size.Height) / 2;
					break;
				case ChartLegendPosition.Bottom:
					x = this.Width.ToPixels() / 2f;
					y = (this.Height.ToPixels() - legend.Size.Height + fontHeight * 3) / 2;
					break;
			}
			return new PointF(x, y);
		}

		/// <summary>
		/// Determines where the start points of each value are going to be located.
		/// </summary>
		/// <param name="pieCenter">The center point of the pie chart.</param>
		/// <param name="angles">The angles of each of the values of the pie chart.</param>
		/// <returns>A list of points where the start of each value will be located.</returns>
		List<PointF> calculatePiePoints(PointF pieCenter, List<decimal> angles)
		{
			var points = new List<PointF>();
			decimal angle = this.StartAngleRadians;
			for (int i = 0; i < angles.Count; ++i)
			{
				points.Add(new PointF((float)(pieCenter.X + (double)this.PieRadius * Math.Cos((double)angle)), (float)(pieCenter.Y + (double)this.PieRadius * Math.Sin((double)angle))));

				angle += angles[i];
			}
			return points;
		}

		/// <summary>
		/// Determines where the text labels for the pie chart will be located.
		/// </summary>
		/// <param name="pieCenter">The center point of the pie chart.</param>
		/// <param name="angles">The angles of each of the values of the pie chart.</param>
		/// <returns>A list of points where the text labels will be located.</returns>
		List<PointF> calculatePieTextLocations(PointF pieCenter, List<decimal> angles)
		{
			var points = new List<PointF>();
			decimal angle = this.StartAngleRadians;
			for (int i = 0; i < angles.Count; ++i)
			{
				angle += angles[i] / 2;

				points.Add(new PointF((float)(pieCenter.X + (double)this.PieRadius * Math.Cos((double)angle) * (double)this.LabelPercentToEdge / 100),
					(float)(pieCenter.Y + (double)this.PieRadius * Math.Sin((double)angle) * (double)this.LabelPercentToEdge / 100)));

				angle += angles[i] / 2;
			}
			return points;
		}

		/// <summary>
		/// Determines what size the pie's radius should be based on the size of the area where the pie chart will display.
		/// </summary>
		/// <param name="legend">The information about the legend.</param>
		/// <param name="fontHeight">The height of the font being used.</param>
		void calculatePieRadius(LegendInfo legend, int fontHeight)
		{
			if (this.LegendPosition == ChartLegendPosition.Left || this.LegendPosition == ChartLegendPosition.Right)
				this.PieRadius = (decimal)Math.Min(this.Width.ToPixels() - legend.Size.Width, this.Height.ToPixels() - fontHeight * 3);
			else
				this.PieRadius = (decimal)Math.Min(this.Width.ToPixels(), this.Height.ToPixels() - fontHeight * 3 - legend.Size.Height);
			this.PieRadius /= 2;
			this.PieRadius -= fontHeight;
		}

		/// <summary>
		/// Draws the legend for the pie chart.
		/// </summary>
		/// <param name="legend">The information about the legend.</param>
		/// <param name="fontHeight">The height of the font being used.</param>
		void drawLegend(LegendInfo legend, int fontHeight)
		{
			float halfFontHeight = fontHeight / 2f;
			float x = legend.StartingPoint.X;
			float y = legend.StartingPoint.Y;
			for (int i = 0; i < this.Values.Count; ++i)
			{
				// When we have too many items to fit on a single row or column (depending on the legend's position), we will move to the next row or column when we have filled one in.
				if (i > 0 && i % legend.MaxItems == 0)
				{
					if (this.LegendPosition == ChartLegendPosition.Left || this.LegendPosition == ChartLegendPosition.Right)
					{
						y = legend.StartingPoint.Y;
						x += legend.LabelWidth + halfFontHeight;
					}
					else
					{
						x = legend.StartingPoint.X;
						y += fontHeight + 1;
					}
				}

				// Creates a group for the legend, then adds the colored square and the text.
				var g = new HtmlGenericControl("g");
				g.Attributes.Add("transform", string.Format("translate({0} {1})", x, y));

				var rect = new HtmlGenericControl("rect");
				rect.Attributes.Add("x", "1");
				rect.Attributes.Add("y", (halfFontHeight - 3.5f).ToString());
				rect.Attributes.Add("width", "7");
				rect.Attributes.Add("height", "7");
				rect.Attributes.Add("fill", ColorTranslator.ToHtml(this.GetColorAt(i)));
				g.Controls.Add(rect);

				var text = new HtmlGenericControl("text")
				{
					InnerText = this.Values[i].Text
				};
				text.Attributes.Add("x", "12");
				text.Attributes.Add("y", halfFontHeight.ToString());
				text.Attributes.Add("dominant-baseline", "central");
				text.Attributes.Add("fill", "#000");
				g.Controls.Add(text);

				this.SVG.Controls.Add(g);

				// Moves the position ahead depending on the legend's position.
				if (this.LegendPosition == ChartLegendPosition.Left || this.LegendPosition == ChartLegendPosition.Right)
					y += fontHeight + 1;
				else
					x += legend.LabelWidth + halfFontHeight;
			}
		}

		/// <summary>
		/// Creates the SVG for the pie chart.
		/// </summary>
		protected override void CreateChildControls()
		{
			var font = this.Font.GetFont();

			this.Controls.Clear();

			// Create the root SVG tag.
			this.SVG = new HtmlGenericControl("svg")
			{
				ID = "svg"
			};
			this.SVG.Attributes.Add("xmlns", "http://www.w3.org/2000/svg");
			this.SVG.Attributes.Add("viewBox", string.Format("-1 -1 {0} {1}", this.Width.ToPixels() + 2, this.Height.ToPixels() + 2));
			this.SVG.Attributes.Add("width", this.Width.ToString());
			this.SVG.Attributes.Add("height", this.Height.ToString());
			this.SVG.Attributes.Add("font-family", this.Font.Name);
			this.SVG.Attributes.Add("font-size", this.Font.Size.ToString());
			this.SVG.Attributes.Add("data-creator", "SQM_PieChart");

			// Add the title.
			var title = new HtmlGenericControl("text")
			{
				InnerText = this.Title
			};
			title.Attributes.Add("x", this.Width.Divide(2).ToString());
			title.Attributes.Add("y", (font.Height * 1.5).ToString());
			title.Attributes.Add("text-anchor", "middle");
			title.Attributes.Add("fill", ColorTranslator.ToHtml(this.TitleStyle.ForeColor)); // TODO: Allow title text color to be changed.
			if (this.TitleStyle.Font.Bold)
				title.Attributes.Add("font-weight", "bold");
			if (this.TitleStyle.Font.Italic)
				title.Attributes.Add("font-style", "italic");
			if (!string.IsNullOrWhiteSpace(this.TitleStyle.Font.Name) && this.TitleStyle.Font.Name != this.Font.Name)
				title.Attributes.Add("font-family", this.TitleStyle.Font.Name);
			var textDecoration = new List<string>();
			if (this.TitleStyle.Font.Overline)
				textDecoration.Add("overline");
			if (this.TitleStyle.Font.Strikeout)
				textDecoration.Add("line-through");
			if (this.TitleStyle.Font.Underline)
				textDecoration.Add("underline");
			if (textDecoration.Count > 0)
				title.Attributes.Add("text-decoration", string.Join(" ", textDecoration));
			// NOTE: The size of the title's area is not changed by the size of the title font.
			if (!this.TitleStyle.Font.Size.IsEmpty)
				title.Attributes.Add("font-size", this.TitleStyle.Font.Size.ToString());
			this.SVG.Controls.Add(title);

			// Calculate the angles of each value.
			decimal sum = this.Values.Sum(i => (decimal?)i.YValue) ?? 1;
			var angles = this.Values.Select(v => (v.YValue / sum) * 2 * (decimal)Math.PI).ToList();

			// Get the legend information and use that to calculate the pie chart's radius.
			int longestLabelWidth = (int)(this.Values.MaxOrDefault(v => GetWidthOfString(v.Text, font), 0) + 0.5f);
			var legendInfo = this.getLegendInfo(this.Values.Count, longestLabelWidth, font.Height);
			this.calculatePieRadius(legendInfo, font.Height);

			// Get the various points for the pie chart.
			var pieCenter = this.calculatePieCenter(legendInfo, font.Height);
			var points = this.calculatePiePoints(pieCenter, angles);
			var textLocations = this.calculatePieTextLocations(pieCenter, angles);

			for (int i = 0; i < this.Values.Count; ++i)
			{
				if (!this.AllowZeroValues && this.Values[i].YValue == 0)
					continue;

				var color = this.GetColorAt(i);

				int j = i + 1;
				if (j == this.Values.Count)
					j = 0;

				// Create the arc for this pie chart slice.
				var path = new HtmlGenericControl("path")
				{
					ID = "path_" + i
				};
				path.Attributes.Add("d", string.Format("M{0} {1} L{2} {3} A{4} {4} 0 {5} 1 {6} {7} z", pieCenter.X, pieCenter.Y, points[i].X, points[i].Y, this.PieRadius,
					(double)angles[i] > Math.PI ? 1 : 0, points[j].X, points[j].Y));
				path.Attributes.Add("fill", ColorTranslator.ToHtml(color));
				path.Attributes.Add("stroke", "#000");
				path.Attributes.Add("stroke-width", "1px");
				path.Controls.Add(new HtmlGenericControl("title")
				{
					InnerText = this.Values[i].Text
				});
				this.SVG.Controls.Add(path);

				// Create the text label to go over the pie chart slice.
				var text = new HtmlGenericControl("text")
				{
					InnerText = this.Values[i].YValue.ToString()
				};
				text.Attributes.Add("x", textLocations[i].X.ToString());
				text.Attributes.Add("y", textLocations[i].Y.ToString());
				text.Attributes.Add("text-anchor", "middle");
				text.Attributes.Add("fill", ColorTranslator.ToHtml(ContrastColor(color)));
				this.SVG.Controls.Add(text);
			}

			// Add the legend.
			this.drawLegend(legendInfo, font.Height);

			this.Controls.Add(this.SVG);
		}

		protected override void OnPreRender(EventArgs e)
		{
			base.OnPreRender(e);

			// This is to set up the tooltips for the pie chart's slices.
			ScriptManager.RegisterStartupScript(this, this.GetType(), "js_" + this.ID, string.Format(@"function attachTooltip_{0}()
				{{
					$('#{0} path').tooltip({{
						track: true,
						items: ':not([disabled])',
						content: function()
						{{
							var $this = $(this);
							var title = $this.find('title');
							if (title.length > 0)
								title.attr('data-rect', $this.attr('id')).appendTo('body');
							else
								title = $('[data-rect=""' + $this.attr('id') + '""]');
							return title.html();
						}}
					}});
				}}

				Sys.Application.add_init(attachTooltip_{0});
				Sys.WebForms.PageRequestManager.getInstance().add_endRequest(attachTooltip_{0});", this.SVG.ClientID), true);
		}
	}

	public class MyColor
	{
		public Color Color { get; set; }

		public MyColor()
		{
			this.Color = Color.Transparent;
		}

		public static implicit operator MyColor(Color color)
		{
			return new MyColor()
			{
				Color = color
			};
		}
	}

	public static class Extensions
	{
		/// <summary>
		/// Divides a Unit by a value, keeping the same type.
		/// </summary>
		/// <param name="a">The Unit value.</param>
		/// <param name="b">The value to divide the Unit by.</param>
		/// <returns>A new Unit with the divided value.</returns>
		public static Unit Divide(this Unit a, double b)
		{
			return new Unit(a.Value / b, a.Type);
		}

		/// <summary>
		/// Converts a Unit value to pixels.
		/// </summary>
		/// <param name="unit">The Unit value to convert.</param>
		/// <returns>The unit's value in pixels.</returns>
		public static int ToPixels(this Unit unit)
		{
			using (var g = Graphics.FromHwnd(IntPtr.Zero))
				switch (unit.Type)
				{
					case UnitType.Cm:
						return (int)(unit.Value / 2.54 * g.DpiX);
					case UnitType.Em:
						throw new NotSupportedException("This control does not support a font size with em.");
					case UnitType.Ex:
						throw new NotSupportedException("This control does not support a font size with ex.");
					case UnitType.Inch:
						return (int)(unit.Value * g.DpiX);
					case UnitType.Mm:
						return (int)(unit.Value / 25.4 * g.DpiX);
					case UnitType.Percentage:
						throw new NotSupportedException("This control does not support a font size with a percentage.");
					case UnitType.Pica:
						return (int)(unit.Value / 6 * g.DpiX);
					case UnitType.Pixel:
						return (int)unit.Value;
					case UnitType.Point:
						return (int)(unit.Value * g.DpiX / 72);
					default:
						throw new ArgumentException("Invalid unit type on font.");
				}
		}

		/// <summary>
		/// Get a Font object from a FontInfo object's data.
		/// </summary>
		/// <param name="fontInfo">The FontInfo that stores the name and size of the font we want.</param>
		/// <returns>The corresponding Font object.</returns>
		public static Font GetFont(this FontInfo fontInfo)
		{
			double size = fontInfo.Size.Unit.Value;
			GraphicsUnit unit;
			switch (fontInfo.Size.Unit.Type)
			{
				case UnitType.Cm:
					size *= 10;
					unit = GraphicsUnit.Millimeter;
					break;
				case UnitType.Em:
					throw new NotSupportedException("This control does not support a font size with em.");
				case UnitType.Ex:
					throw new NotSupportedException("This control does not support a font size with ex.");
				case UnitType.Inch:
					unit = GraphicsUnit.Inch;
					break;
				case UnitType.Mm:
					unit = GraphicsUnit.Millimeter;
					break;
				case UnitType.Percentage:
					throw new NotSupportedException("This control does not support a font size with a percentage.");
				case UnitType.Pica:
					size /= 6;
					unit = GraphicsUnit.Inch;
					break;
				case UnitType.Pixel:
					unit = GraphicsUnit.Pixel;
					break;
				case UnitType.Point:
					unit = GraphicsUnit.Point;
					break;
				default:
					throw new ArgumentException("Invalid unit type on font.");
			}
			return new Font(fontInfo.Name, (float)size, unit);
		}

		/// <summary>
		/// Invokes a transform function on each element of a sequence and returns the maximum Double value
		/// if the sequence is not empty; otherwise returns the specified default value.
		/// Comes from http://www.telerik.com/blogs/linq-sequence-contains-no-elements-extension-methods-to-the-rescue
		/// </summary>
		/// <typeparam name="TSource">The type of the elements of source.</typeparam>
		/// <param name="source">A sequence of values to determine the maximum value of.</param>
		/// <param name="selector">A transform function to apply to each element.</param>
		/// <param name="defaultValue">The default value.</param>
		/// <returns>The maximum value in the sequence or default value if sequence is empty.</returns>
		public static double MaxOrDefault<TSource>(this IEnumerable<TSource> source, Func<TSource, double> selector, double defaultValue)
		{
			if (source.Any<TSource>())
				return source.Max<TSource>(selector);

			return defaultValue;
		}
	}
}
