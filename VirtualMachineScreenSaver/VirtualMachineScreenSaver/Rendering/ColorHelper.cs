using System;
using System.Drawing;

namespace VirtualMachineScreenSaver.Rendering
{
	public class ColorHelper
	{
		public static Color FromAHSB(int alpha, float hue, float saturation, float brightness)
		{
			if (alpha < 0) alpha = 0;
			if (hue < 0) hue = 0;
			if (saturation < 0) saturation = 0;
			if (brightness < 0) brightness = 0;
			if (alpha > 255) alpha = 255;
			if (hue >= 360) hue = 360.0f;
			if (saturation > 1) saturation = 1;
			if (brightness > 1) brightness = 1;

			if (0 == saturation)
			{
				return Color.FromArgb(alpha, Convert.ToInt32(brightness * 255),
				  Convert.ToInt32(brightness * 255), Convert.ToInt32(brightness * 255));
			}

			float fMax, fMid, fMin;
			int iSextant, iMax, iMid, iMin;

			if (0.5 < brightness)
			{
				fMax = brightness - (brightness * saturation) + saturation;
				fMin = brightness + (brightness * saturation) - saturation;
			}
			else
			{
				fMax = brightness + (brightness * saturation);
				fMin = brightness - (brightness * saturation);
			}

			iSextant = (int)System.Math.Floor(hue / 60f);
			if (300f <= hue)
			{
				hue -= 360f;
			}
			hue /= 60f;
			hue -= 2f * (float)System.Math.Floor(((iSextant + 1f) % 6f) / 2f);
			if (0 == iSextant % 2)
			{
				fMid = hue * (fMax - fMin) + fMin;
			}
			else
			{
				fMid = fMin - hue * (fMax - fMin);
			}

			iMax = Convert.ToInt32(fMax * 255);
			iMid = Convert.ToInt32(fMid * 255);
			iMin = Convert.ToInt32(fMin * 255);

			switch (iSextant)
			{
				case 1:
					return Color.FromArgb(alpha, iMid, iMax, iMin);
				case 2:
					return Color.FromArgb(alpha, iMin, iMax, iMid);
				case 3:
					return Color.FromArgb(alpha, iMin, iMid, iMax);
				case 4:
					return Color.FromArgb(alpha, iMid, iMin, iMax);
				case 5:
					return Color.FromArgb(alpha, iMax, iMin, iMid);
				default:
					return Color.FromArgb(alpha, iMax, iMid, iMin);
			}
		}

		public static int ToArgb(Color color)
		{
			return (color.A << 24) + (color.R << 0) + (color.G << 8) + (color.B << 16);
		}
	}
}