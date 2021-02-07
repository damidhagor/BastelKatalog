using System;
using System.Collections.Generic;
using System.Globalization;
using BastelKatalog.Models;
using Xamarin.Forms;
using Xamarin.Forms.Shapes;

namespace BastelKatalog.Views.Converter
{
    /// <summary>
    /// Creates a StackLayout of indentation icons for categories.
    /// </summary>
    public class SubCategoryIndentConverter : IValueConverter
    {
        private static readonly Geometry MiddlePathGeometry = (Geometry)new PathGeometryConverter().ConvertFromInvariantString("M 0,0 V 50 M 0,25 H 25");
        private static readonly Geometry EndPathGeometry = (Geometry)new PathGeometryConverter().ConvertFromInvariantString("M 0,0 V 25 M 0,25 H 25");
        private static readonly Geometry ParentMiddlePathGeometry = (Geometry)new PathGeometryConverter().ConvertFromInvariantString("M 0,0 V 50");
        private static readonly Geometry EmptyPathGeometry = (Geometry)new PathGeometryConverter().ConvertFromInvariantString("M 0,0 H 50 V 50 H 0 V 0");


        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            Brush brush;
            List<Path> paths = new List<Path>();

            if (!(value is CategoryWrapper category))
                return new StackLayout();

            // Get Color or Brush for path from parameter
            if (parameter is Color color)
                brush = new SolidColorBrush(color);
            else if (parameter is Brush brushParameter)
                brush = brushParameter;
            else
                brush = new SolidColorBrush(Application.Current.Resources["Primary"] as Color? ?? Color.White);

            // Add categories own indent path
            if (category.SubCategoryPosition == SubCategoryPosition.Start || category.SubCategoryPosition == SubCategoryPosition.Middle)
                paths.Add(CreateMiddlePath(brush));
            else if (category.SubCategoryPosition == SubCategoryPosition.End)
                paths.Add(CreateEndPath(brush));

            // Add indents for parent categories (right to left)
            CategoryWrapper? parent = category.ParentCategory;
            while (parent != null)
            {
                if (parent.SubCategoryPosition == SubCategoryPosition.Start || parent.SubCategoryPosition == SubCategoryPosition.Middle)
                    paths.Add(CreateParentMiddlePath(brush));
                else if (parent.SubCategoryPosition == SubCategoryPosition.End)
                    paths.Add(CreateEmptyPath());

                parent = parent.ParentCategory;
            }

            // Add paths to StackLayout
            StackLayout stackLayout = new StackLayout { Orientation = StackOrientation.Horizontal };
            for (int i = paths.Count; i > 0; i--)
                stackLayout.Children.Add(paths[i - 1]);

            return stackLayout;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();


        /// <summary>
        /// Creates t-shaped path for first or middle sub-categories
        /// </summary>
        /// <param name="brush"></param>
        /// <returns></returns>
        private Path CreateMiddlePath(Brush brush)
        {
            return new Path
            {
                Aspect = Stretch.Uniform,
                Stroke = brush,
                Data = MiddlePathGeometry,
                Margin = new Thickness(25, 0, 0, 0)
            };
        }

        /// <summary>
        /// Creates L-shaped path for last (or only) sub-category
        /// </summary>
        /// <param name="brush"></param>
        /// <returns></returns>
        private Path CreateEndPath(Brush brush)
        {
            return new Path
            {
                Aspect = Stretch.Uniform,
                Stroke = brush,
                Data = EndPathGeometry,
                Margin = new Thickness(25, 0, 0, 25)
            };
        }

        /// <summary>
        /// Creates a vertical path for parent categories
        /// </summary>
        /// <param name="brush"></param>
        /// <returns></returns>
        private Path CreateParentMiddlePath(Brush brush)
        {
            return new Path
            {
                Aspect = Stretch.Uniform,
                Stroke = brush,
                Data = ParentMiddlePathGeometry,
                WidthRequest = 11,
                Margin = new Thickness(20, 0, 20, 0)
            };
        }

        /// <summary>
        /// Creates an empty path with no path.
        /// </summary>
        private Path CreateEmptyPath()
        {
            return new Path
            {
                Aspect = Stretch.Uniform,
                Stroke = Brush.Transparent,
                Data = EmptyPathGeometry
            };
        }
    }
}
