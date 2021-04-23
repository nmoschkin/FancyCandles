﻿/* 
    Copyright 2019 Dennis Geller.

    This file is part of FancyCandles.

    FancyCandles is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    FancyCandles is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with FancyCandles.  If not, see<https://www.gnu.org/licenses/>. */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Media;
using System.Collections.ObjectModel;

namespace FancyCandles
{

    class CrossPriceColorConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            Brush std = (Brush)values[0];
            Brush bearbr = (Brush)values[1];
            Brush bullbr = (Brush)values[2];
            
            var candles = (ObservableCollection<ICandle>)values[3];
            bool mo = (bool)values[4];

            if (mo || candles == null || candles.Count == 0) return std;

            else
            {
                var lc = candles.LastOrDefault();

                if (lc.O > lc.C) return bearbr;
                else return bullbr;
            }

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //*******************************************************************************************************************************************************************
    class CrossPriceMarginConverter : IMultiValueConverter
    {
        // values[0] - Point CurrentMousePosition
        // values[1] - double PriceTickTextHeight
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || (values[0]).GetType() != typeof(Point) || (values[1]).GetType() != typeof(double))
                return new Thickness(0, 0, 0, 0);

            Point currentMousePosition = (Point)values[0];
            double priceTickTextHeight = (double)values[1];

            if (values.Length > 2)
            {
                double ChartAreaHeight = (double)values[2];
                double priceLow = ((CandleExtremums)values[3]).PriceLow;
                double priceHigh = ((CandleExtremums)values[3]).PriceHigh;
                double chartTopMargin = (double)values[4];
                double chartBottomMargin = (double)values[5];

                double? rtv = values.Length > 6 ? (double?)values[6] : null;
                bool mo = values.Length > 7 ? (bool)values[7] : true;

                if (!mo && rtv != null)
                {
                    var rt = (double)rtv;
                    var cc = (chartTopMargin / 2) + ((ChartAreaHeight - chartTopMargin - chartBottomMargin) * (1 - ((Math.Max(rt, priceLow) - Math.Min(rt, priceLow)) / (priceHigh - priceLow))));

                    return new Thickness(0, Math.Round(cc - priceTickTextHeight / 2.0), 0, 0);

                }
                else
                {
                    return new Thickness(0, Math.Round(currentMousePosition.Y) - priceTickTextHeight / 2.0, 0, 0);
                }

            }
            else
            {
                return new Thickness(0, Math.Round(currentMousePosition.Y) - priceTickTextHeight / 2.0, 0, 0);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }

    }
    //*******************************************************************************************************************************************************************
    class CrossPriceValueConverter : IMultiValueConverter
    {
        // values[0] - Point CurrentMousePosition
        // values[1] - double ChartAreaHeight
        // values[2] - CandleExtremums visibleCandlesExtremums
        // values[3] - double PriceChartTopMargin
        // values[4] - double PriceChartBottomMargin
        // values[5] - int MaxNumberOfDigitsAfterPointInPrice
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if ( values == null || values.Length < 6 || (values[0]).GetType() != typeof(Point) || (values[1]).GetType() != typeof(double) || (values[2]).GetType() != typeof(CandleExtremums)
                 || (values[3]).GetType() != typeof(double) || (values[4]).GetType() != typeof(double) || (values[5]).GetType() != typeof(int))
                return true;

            Point currentMousePosition = (Point)values[0];
            double ChartAreaHeight = (double)values[1];
            double priceLow = ((CandleExtremums)values[2]).PriceLow;
            double priceHigh = ((CandleExtremums)values[2]).PriceHigh;
            double chartTopMargin = (double)values[3];
            double chartBottomMargin = (double)values[4];
            int maxNumberOfDigitsAfterPointInPrice = (int)values[5];
            string fmt = values.Length > 6 ? (string)values[6] : "";
            double? rtv = values.Length > 7 ? (double?)values[7] : null;
            bool mo = values.Length > 8 ? (bool)values[8] : true;

            double price;

            if (!mo && (rtv is double rt))
            {
                price = rt;
            }
            else 
            {
                price = priceHigh - (currentMousePosition.Y - chartTopMargin) / (ChartAreaHeight - chartTopMargin - chartBottomMargin) * (priceHigh - priceLow);
            }

            if (string.IsNullOrEmpty(fmt))
            {
                return Math.Round(price, maxNumberOfDigitsAfterPointInPrice).ToString();
            }
            else
            {
                return price.ToString(fmt);
            }

        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }

    }
    //*******************************************************************************************************************************************************************
    class CrossVolumeConverter : IMultiValueConverter
    {
        // values[0] - Point CurrentMousePosition
        // values[1] - double VolumeHistogramHeight
        // values[2] - CandleExtremums visibleCandlesExtremums
        // values[3] - double VolumeHistogramTopMargin
        // values[4] - double VolumeHistogramBottomMargin
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 5 || (values[0]).GetType() != typeof(Point) || (values[1]).GetType() != typeof(double) || (values[2]).GetType() != typeof(CandleExtremums)
                 || (values[3]).GetType() != typeof(double) || (values[4]).GetType() != typeof(double) )
                return (long)0;

            Point currentMousePosition = (Point)values[0];
            double volumeHistogramHeight = (double)values[1];
            CandleExtremums visibleCandlesExtremums = (CandleExtremums)values[2];
            double volumeHistogramTopMargin = (double)values[3];
            double volumeHistogramBottomMargin = (double)values[4];
            return ((long)((visibleCandlesExtremums.VolumeHigh - (currentMousePosition.Y - volumeHistogramTopMargin) / (volumeHistogramHeight - volumeHistogramTopMargin - volumeHistogramBottomMargin) * visibleCandlesExtremums.VolumeHigh))).ToString();
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }

    }
    //*******************************************************************************************************************************************************************
    class VerticalCrossLineVisibilityConverter : IMultiValueConverter
    {
        // values[0] - bool IsCrossLinesVisible
        // values[1] - bool IsMouseOverPriceChart
        // values[2] - bool IsMouseOverVolumeHistogram
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3 || (values[0]).GetType() != typeof(bool) || (values[1]).GetType() != typeof(bool) || (values[2]).GetType() != typeof(bool))
                return true;

            bool isCrossLinesVisible = (bool)values[0];
            bool isMouseOverPriceChart = (bool)values[1];
            bool isMouseOverVolumeHistogram = (bool)values[2];
            return (isCrossLinesVisible && (isMouseOverPriceChart || isMouseOverVolumeHistogram)) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }

    }
    //*******************************************************************************************************************************************************************
    class CandleDrawingParametersConverter : IMultiValueConverter
    {
        // values[0] - double CandleWidth
        // values[1] - double CandleGap
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            double candleW = (double)values[0];
            double candleG = (double)values[1];
            return new CandleDrawingParameters(candleW, candleG);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }

    }
    //*******************************************************************************************************************************************************************
    class IntRangeToPointConverter : IValueConverter
    {
        public object Convert(object intRange_value, Type targetType, object parameter, CultureInfo culture)
        {
            IntRange ir = (IntRange)intRange_value;
            return new Point(ir.Start_i, ir.Count);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //*******************************************************************************************************************************************************************
    class CandleExtremumsToPriceExtremumsPointConverter : IValueConverter
    {
        public object Convert(object candleExtremums_value, Type targetType, object parameter, CultureInfo culture)
        {
            CandleExtremums cndlExtr = (CandleExtremums)candleExtremums_value;
            return new Point(cndlExtr.PriceLow, cndlExtr.PriceHigh);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //*******************************************************************************************************************************************************************
    class CandleDrawingParametersToPointConverter : IValueConverter
    {
        public object Convert(object CandleDrawingParameters_value, Type targetType, object parameter, CultureInfo culture)
        {
            CandleDrawingParameters drawingParams = (CandleDrawingParameters)CandleDrawingParameters_value;
            return new Point(drawingParams.Width, drawingParams.Gap);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //*******************************************************************************************************************************************************************
    class BrushToColorConverter : IValueConverter
    {
        public object Convert(object SolidColorBrush_value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush brush = (SolidColorBrush)SolidColorBrush_value;
            return brush.Color;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //*******************************************************************************************************************************************************************
    class SquareBoolToVisibilityConverter : IMultiValueConverter
    {
        // values[0] - bool bool1
        // values[1] - bool bool2
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || (values[0]).GetType() != typeof(bool) || (values[1]).GetType() != typeof(bool))
                return true;

            bool bool0 = (bool)values[0];
            bool bool1 = (bool)values[1];
            bool bool2 = values.Length > 2 ? (values[2] != null) : false;

            return (bool0 && (bool1 || bool2)) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }

    }
    //*******************************************************************************************************************************************************************
    class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object bool_value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)bool_value) ? Visibility.Visible : Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //*******************************************************************************************************************************************************************
    // Opposite to BoolToVisibilityConverter
    class NotBoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object bool_value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)bool_value) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
    //*******************************************************************************************************************************************************************
    class IntRange_Start_i_Converter : IValueConverter
    {
        // value - IntRange visibleCandlesRange
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IntRange visibleCandlesRange = (IntRange)value;
            return visibleCandlesRange.Start_i;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int start_i = System.Convert.ToInt32((double)value);
            IntRange visibleCandlesRange = IntRange.CreateContainingOnlyStart_i(start_i);
            return visibleCandlesRange;
        }
    }
    //*******************************************************************************************************************************************************************
    class IntRange_Count_Converter : IValueConverter
    {
        // value - IntRange visibleCandlesRange
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            IntRange visibleCandlesRange = (IntRange)value;
            return visibleCandlesRange.Count;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int start_i = System.Convert.ToInt32((double)value);
            IntRange visibleCandlesRange = IntRange.CreateContainingOnlyStart_i(start_i);
            return visibleCandlesRange;
        }
    }
    //*******************************************************************************************************************************************************************
    // Возвращает максимально допустимое значение для свойства FirstCandle_csi.
    class FirstCandleMaxIndexConverter : IMultiValueConverter
    {
        // values[0] - ObservableCollection<Candle> candles
        // values[1] - IntRange VisibleCandlesRange
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || (values[1]).GetType() != typeof(IntRange))
                return 0.0;

            IList<ICandle> candles = (IList<ICandle>)values[0];
            if (candles == null)
                return (double)int.MaxValue;
            else
            {
                int candlesCount = ((IntRange)values[1]).Count;
                if (candlesCount == 0)
                    return (double)int.MaxValue;
                return (double)(candles.Count - candlesCount);
            }
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
    //*******************************************************************************************************************************************************************
    // Возвращает Margin для candlesItemsControl.
    class TopBottomMarginConverter : IMultiValueConverter
    {
        // values[0] - double topMargin
        // values[1] - double bottomMargin
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 2 || (values[0]).GetType()!=typeof(double) || (values[1]).GetType() != typeof(double))
                return new Thickness(0,0,0,0);

            double topMargin = (double)values[0];
            double bottomMargin = (double)values[1];
            return new Thickness(0, topMargin, 0, bottomMargin);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        { throw new NotImplementedException(); }
    }
    //*******************************************************************************************************************************************************************
    /*class BoolToVolumeGridRowHeightConverter : IValueConverter
    {
        public object Convert(object bool_value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)bool_value) ? new GridLength(1, GridUnitType.Star) : new GridLength(0.0);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }

    class BoolToPriceGridRowHeightConverter : IValueConverter
    {
        public object Convert(object bool_value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((bool)bool_value) ? new GridLength(4, GridUnitType.Star) : new GridLength(1,GridUnitType.Auto);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }*/
    //*******************************************************************************************************************************************************************
}
