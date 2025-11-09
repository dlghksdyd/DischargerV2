using System;
using System.Collections.Generic;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Forms;
using System.Windows.Input;

namespace MExpress.Mex
{
    public class MexCalendar : Calendar
    {
        #region Property
        public static readonly DependencyProperty CornerRadiusProperty;

        public CornerRadius CornerRadius
        {
            get { return (CornerRadius)GetValue(CornerRadiusProperty); }
            set { SetValue(CornerRadiusProperty, value); }
        }
        #endregion

        static MexCalendar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(MexCalendar), new FrameworkPropertyMetadata(typeof(MexCalendar)));

            CornerRadiusProperty = DependencyProperty.Register("CornerRadius", typeof(CornerRadius), typeof(MexCalendar), new FrameworkPropertyMetadata(new CornerRadius(8)));
        }

        public MexCalendar()
        {
            Loaded += MexCalendar_Loaded;
        }

        private void MexCalendar_Loaded(object sender, RoutedEventArgs e)
        {
            this.ApplyTemplate();
            StackPanel root = this.GetTemplateChild("PART_Root") as StackPanel;

            CalendarItem calendarItem = root.Children[0] as CalendarItem;

            calendarItem.ApplyTemplate();
            Grid monthView = calendarItem.Template.FindName("PART_MonthView", calendarItem) as Grid;

            var count = monthView.Children.Count;

            for (int i = 0; i < count; i++)
            {
                if (monthView.Children[i] is CalendarDayButton calendarDayButton)
                {
                    calendarDayButton.PreviewMouseLeftButtonUp += CalendarDayButton_PreviewMouseLeftButtonUp;
                }
            }
        }

        private void CalendarDayButton_PreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            // UpdateSource
            BindingExpression be = this.GetBindingExpression(Calendar.SelectedDateProperty);
            be.UpdateSource();
        }
    }
}
