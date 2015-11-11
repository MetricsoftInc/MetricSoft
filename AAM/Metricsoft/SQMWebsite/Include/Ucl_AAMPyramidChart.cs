using System;
using System.ComponentModel;
using System.Security.Permissions;
using System.Web;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace SQM.Website
{
	[AspNetHostingPermission(SecurityAction.Demand, Level = AspNetHostingPermissionLevel.Minimal),
		AspNetHostingPermission(SecurityAction.InheritanceDemand, Level = AspNetHostingPermissionLevel.Minimal), ToolboxData("<{0}:PyramidChart runat=\"server\"></{0}:PyramidChart>")]
	public class AAMPyramidChart : CompositeControl, INamingContainer
	{
		/// <summary>The ratio of height to width in an equilateral triangle.</summary>
		static decimal heightToWidthRatio = (decimal)Math.Sqrt(0.75);

		// Width and Height are dependant on each other with height being a ratio of width, so only the width has a default value on it.

		[Category("Layout"), DefaultValue(typeof(Unit), "400px"), Description("The width of the control.")]
		public override Unit Width
		{
			get { return base.Width; }
			set
			{
				base.Width = value;
				base.Height = value.Multiply((double)heightToWidthRatio);
			}
		}

		[Category("Layout"), Description("The height of the control.")]
		public override Unit Height
		{
			get { return base.Height; }
			set
			{
				base.Height = value;
				base.Width = value.Divide((double)heightToWidthRatio);
			}
		}

		[Category("Data"), Description("The number of fatalities to show in the first (top-most) slice of the pyramid.")]
		public decimal Fatalities { get; set; }

		[Category("Data"), Description("The number of lost time cases to show in the second (from the top) slice of the pyramid.")]
		public decimal LostTimeCases { get; set; }

		[Category("Data"), Description("The number of recordable injuries to show in the third (middle) slice of the pyramid.")]
		public decimal RecordableInjuries { get; set; }

		[Category("Data"), Description("The number of first aid cases to show in the fourth (second from bottom) slice of the pyramid.")]
		public decimal FirstAidCases { get; set; }

		[Category("Data"), Description("The number of near misses to show in the fifth (bottom-most) slice of the pyramid.")]
		public decimal NearMisses { get; set; }

		HtmlGenericControl SVG { get; set; }

		// I want this to be a div, not a span, thus the override.
		protected override HtmlTextWriterTag TagKey
		{
			get { return HtmlTextWriterTag.Div; }
		}

		public AAMPyramidChart() : base()
		{
			this.ClientIDMode = ClientIDMode.Predictable;
			this.Width = new Unit(400, UnitType.Pixel);
			// NOTE: Setting the default font here to Verdana because otherwise Font.Name is an empty string and converts to Microsoft Sans Serif instead of the page's font.
			this.Font.Name = "Verdana";
			// NOTE: We have to set a default font size, otherwise my GetFont() extension method throws an exception.
			this.Font.Size = new FontUnit(10, UnitType.Pixel);
		}

		void appendTriangleSlice(Unit widthSteps, Unit heightSteps, int sliceFromBottom, string fill)
		{
			var path = new HtmlGenericControl("path");
			path.Attributes.Add("d", string.Format("M{0},{1} h{2} l-{3},-{4} h-{5} l-{3},{4} z", widthSteps.Multiply(sliceFromBottom - 1).ToPixels(),
				heightSteps.Multiply(6 - sliceFromBottom).ToPixels(), widthSteps.Multiply(2 * (6 - sliceFromBottom)).ToPixels(), widthSteps.ToPixels(), heightSteps.ToPixels(),
				widthSteps.Multiply(2 * (5 - sliceFromBottom)).ToPixels()));
			path.Attributes.Add("fill", fill);
			path.Attributes.Add("stroke", "black");
			path.Attributes.Add("stroke-linejoin", "round");
			path.Attributes.Add("stroke-width", "0.5");
			this.SVG.Controls.Add(path);
		}

		void appendTextElement(Unit x, Unit y, decimal value, string label)
		{
			var text = new HtmlGenericControl("text");
			text.Attributes.Add("font-weight", "bold");
			text.Attributes.Add("text-anchor", "middle");
			text.Attributes.Add("y", y.ToString());
			text.Attributes.Add("dominant-baseline", "middle");

			var tspan = new HtmlGenericControl("tspan")
			{
				InnerText = value.ToString()
			};
			tspan.Attributes.Add("x", x.ToString());
			tspan.Attributes.Add("text-anchor", "middle");
			text.Controls.Add(tspan);

			tspan = new HtmlGenericControl("tspan")
			{
				InnerText = label
			};
			tspan.Attributes.Add("x", x.ToString());
			tspan.Attributes.Add("text-anchor", "middle");
			tspan.Attributes.Add("dy", (this.Font.GetFont().SizeInPoints * 1.1f) + "pt");
			text.Controls.Add(tspan);

			this.SVG.Controls.Add(text);
		}

		protected override void CreateChildControls()
		{
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
			this.SVG.Attributes.Add("data-creator", "SQM_AAMPyramidChart");

			var widthSteps = this.Width.Divide(10);
			var heightSteps = this.Height.Divide(5);
			var center = this.Width.Divide(2);

			appendTriangleSlice(widthSteps, heightSteps, 5, "rgb(79,129,189)");
			appendTextElement(center, heightSteps.Divide(2), this.Fatalities, "Fatalities");
			appendTriangleSlice(widthSteps, heightSteps, 4, "rgb(255,255,204)");
			appendTextElement(center, heightSteps.Multiply(1.5), this.LostTimeCases, "Lost Time Cases");
			appendTriangleSlice(widthSteps, heightSteps, 3, "rgb(255,190,125)");
			appendTextElement(center, heightSteps.Multiply(2.5), this.RecordableInjuries, "Recordable Injuries");
			appendTriangleSlice(widthSteps, heightSteps, 2, "rgb(204,204,255)");
			appendTextElement(center, heightSteps.Multiply(3.5), this.FirstAidCases, "First Aid Cases");
			appendTriangleSlice(widthSteps, heightSteps, 1, "rgb(216,235,179)");
			appendTextElement(center, heightSteps.Multiply(4.5), this.NearMisses, "Near Misses");

			this.Controls.Add(this.SVG);
		}
	}

	public static partial class Extensions
	{
		/// <summary>
		/// Multiplies a Unit by a value, keeping the same type.
		/// </summary>
		/// <param name="a">The Unit value.</param>
		/// <param name="b">The value to mulitplu the Unit by.</param>
		/// <returns>A new Unit with the multiplied value.</returns>
		public static Unit Multiply(this Unit a, double b)
		{
			return new Unit(a.Value * b, a.Type);
		}
	}
}
