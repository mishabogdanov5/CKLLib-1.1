using System;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;


namespace CKLDrawing
{
    public static class Constants
    {
        public static class DefaultColors 
        {
            public static readonly Brush INTERVAL_ITEM_COLOR = new SolidColorBrush(Color.FromRgb(119, 139, 235));
            public static readonly Brush INTERVAL_ITEM_ACTIVE_COLOR = new SolidColorBrush(Color.FromRgb(51, 76, 190));
            public static readonly Brush INTERVAL_ITEM_BORDER_COLOR = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            public static readonly Brush EMPTY_INTERVAL_COLOR = new SolidColorBrush(Color.FromRgb(235, 134, 134));
            public static readonly Brush EMPRY_INTERVAL_ACTIVE_COLOR = new SolidColorBrush(Color.FromRgb(219, 7, 46));
            public static readonly Brush CKL_BACKGROUND = new SolidColorBrush(Color.FromRgb(68, 68, 68));
            public static readonly Brush SECTION_COLOR = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            public static readonly Brush TIME_OX_COLOR = new SolidColorBrush(Color.FromRgb(0, 0, 0));
            public static readonly Brush VALUE_COLOR = new SolidColorBrush(Color.FromRgb(255, 255, 255));
        }

        public static class Dimentions 
        {
            public static readonly double LINE_HEIGHT = 3;
            public static readonly double EMPTY_INTERVAL_HEIGHT = 6;
            public static readonly Thickness INTERVAL_BORDER_SIZE = new Thickness(2);
            public static readonly Thickness EMPTY_INTERVAL_BORDER_SIZE = new Thickness(0,1,0,1);
            public static readonly double CHAIN_HEIGHT = 28;
            public static readonly double CKL_WIDTH = 1600;
            public static readonly Thickness CHAIN_MARGIN = new Thickness(10, 0, 0, 10);
            public static readonly Thickness TIME_OX_MARGIN = new Thickness(10, 0, 0, 5);
            public static readonly double VALUE_BOX_WIDTH = 100;
            public static readonly double SECTION_WIDTH = 2.5;
            public static readonly double SECTION_HEIGHT = 10;
            public static readonly double TIME_OX_HEIGHT = 70;
            public static readonly double DEL_WIDTH = 20;
            public static readonly double OX_FREE_INTERVAL = 40;
            public static readonly double SECTIONS_TEXT_HEIGHT = 15;
            public static readonly double TEXT_SIZE = 9;
            public static readonly double FIRST_DEL_START = 5;
            public static readonly double MAIN_VIEW_PADDING_RIGHT = 5;
            public static readonly double MAIN_VIEW_PADDING_BOTTOM = 15;
        }

		public static readonly string[] TIME_DIMENTIONS_STRINGS = new string[] {"нс", "мкс", "мс", "с",
			"м", "ч", "д", "н"};

		public static readonly int[] TIME_DIMENTIONS_CONVERT = new int[] { 1000, 1000, 1000, 60, 60, 24, 7 };

		public static int MAX_DEL_COUNT = 200;
        public static int MIN_DEL_COUNT = 5;
	}
}
