﻿using System;
using System.Linq;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace QKit.Controls
{
    public class UniformWrapGrid : Panel
    {
        #region DependencyProperties
        public static readonly DependencyProperty ColumnMinWidthProperty = DependencyProperty.Register(
            nameof(ColumnMinWidth),
            typeof(double),
            typeof(UniformWrapGrid),
            new PropertyMetadata(default(double)));
        #endregion

        #region Properties
        public double ColumnMinWidth
        {
            get { return (double)GetValue(ColumnMinWidthProperty); }
            set { SetValue(ColumnMinWidthProperty, value); }
        }
        #endregion

        #region Methods
        private int GetDesiredColumnCount(Size size)
        {
            return ColumnMinWidth == 0 ? 1 : (int)Math.Floor(size.Width / ColumnMinWidth);
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            var columnCount = GetDesiredColumnCount(availableSize);
            var columnWidth = availableSize.Width / columnCount;

            var height = 0d;
            for (int i = 0; i < Children.Count; i += columnCount)
            {
                var childrenForCurrentRow = Children.Skip(i).Take(columnCount);

                foreach (var child in childrenForCurrentRow)
                {
                    child.Measure(new Size(columnWidth, availableSize.Height));
                }

                height += childrenForCurrentRow.Select(c => c.DesiredSize.Height).Max();
            }
            return new Size(availableSize.Width, height);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            var columnCount = GetDesiredColumnCount(finalSize);
            var columnWidth = finalSize.Width / columnCount;

            var horizontalOffset = 0d;
            var verticalOffset = 0d;

            for (int i = 0; i < Children.Count; i += columnCount)
            {
                var childrenForCurrentRow = Children.Skip(i).Take(columnCount).Cast<FrameworkElement>();
                var rowMaxHeight = childrenForCurrentRow.Select(c => c.DesiredSize.Height).Max();

                foreach (var child in childrenForCurrentRow)
                {
                    child.Arrange(new Rect(new Point(horizontalOffset, verticalOffset), new Size(columnWidth, rowMaxHeight)));
                    horizontalOffset += columnWidth;
                }

                horizontalOffset = 0;
                verticalOffset += rowMaxHeight;
            }

            return new Size(finalSize.Width, verticalOffset);
        }
        #endregion
    }
}
