using System;
using System.ComponentModel;
using System.Drawing;

namespace SymaCord.TryOnMirror.Core.Util.ColorConverter
{
	/// <summary>
	/// Structure to define HSL.
	/// </summary>
	public struct HSL
	{
		/// <summary>
		/// Gets an empty HSL structure;
		/// </summary>
		public static readonly HSL Empty = new HSL();

		#region Fields
		private double hue;
		private double saturation;
		private double luminance;
		#endregion

		#region Operators
		public static bool operator ==(HSL item1, HSL item2)
		{
			return (
				item1.Hue == item2.Hue 
				&& item1.Saturation == item2.Saturation 
				&& item1.Luminance == item2.Luminance
				);
		}

		public static bool operator !=(HSL item1, HSL item2)
		{
			return (
				item1.Hue != item2.Hue 
				|| item1.Saturation != item2.Saturation 
				|| item1.Luminance != item2.Luminance
				);
		}

		#endregion

		#region Accessors
		/// <summary>
		/// Gets or sets the hue component.
		/// </summary>
		[Description("Hue component"),]
		public double Hue 
		{ 
			get
			{
				return hue;
			} 
			set
			{ 
				hue = (value>360)? 360 : ((value<0)?0:value); 
			} 
		} 

		/// <summary>
		/// Gets or sets saturation component.
		/// </summary>
		[Description("Saturation component"),]
		public double Saturation 
		{ 
			get
			{
				return saturation;
			} 
			set
			{ 
				saturation = (value>1)? 1 : ((value<0)?0:value); 
			} 
		} 

		/// <summary>
		/// Gets or sets the luminance component.
		/// </summary>
		[Description("Luminance component"),]
		public double Luminance 
		{ 
			get
			{
				return luminance;
			} 
			set
			{ 
				luminance = (value>1)? 1 : ((value<0)? 0 : value); 
			} 
		}

		#endregion

		/// <summary>
		/// Creates an instance of a HSL structure.
		/// </summary>
		/// <param name="h">Hue value.</param>
		/// <param name="s">Saturation value.</param>
		/// <param name="l">Lightness value.</param>
		public HSL(double h, double s, double l) 
		{
			hue = (h>360)? 360 : ((h<0)?0:h); 
			saturation = (s>1)? 1 : ((s<0)?0:s);
			luminance = (l>1)? 1 : ((l<0)?0:l);
		}

        public RGB RGB
        {
            get
            {
                double r = 0, g = 0, b = 0;

                double temp1, temp2;

                double normalisedH = hue / 360.0;

                if (luminance == 0)
                {
                    r = g = b = 0;
                }
                else
                {
                    if (saturation == 0)
                    {
                        r = g = b = luminance;
                    }
                    else
                    {
                        temp2 = ((luminance <= 0.5) ? luminance * (1.0 + saturation) : luminance + saturation - (luminance * saturation));

                        temp1 = 2.0 * luminance - temp2;

                        double[] t3 = new double[] { normalisedH + 1.0 / 3.0, normalisedH, normalisedH - 1.0 / 3.0 };

                        double[] clr = new double[] { 0, 0, 0 };

                        for (int i = 0; i < 3; ++i)
                        {
                            if (t3[i] < 0)
                                t3[i] += 1.0;

                            if (t3[i] > 1)
                                t3[i] -= 1.0;

                            if (6.0 * t3[i] < 1.0)
                                clr[i] = temp1 + (temp2 - temp1) * t3[i] * 6.0;
                            else if (2.0 * t3[i] < 1.0)
                                clr[i] = temp2;
                            else if (3.0 * t3[i] < 2.0)
                                clr[i] = (temp1 + (temp2 - temp1) * ((2.0 / 3.0) - t3[i]) * 6.0);
                            else
                                clr[i] = temp1;

                        }

                        r = clr[0];
                        g = clr[1];
                        b = clr[2];
                    }

                }
                return new RGB((int)(255 * r), (int)(255 * g), (int)(255 * b)); 
                //Color.FromArgb((int)(255 * r), (int)(255 * g), (int)(255 * b));
            }
        }

		#region Methods
		public override bool Equals(Object obj) 
		{
			if(obj==null || GetType()!=obj.GetType()) return false;

			return (this == (HSL)obj);
		}

		public override int GetHashCode() 
		{
			return Hue.GetHashCode() ^ Saturation.GetHashCode() ^ Luminance.GetHashCode();
		}

		#endregion
	}
}
