﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using ZongziTEK_Blackboard_Sticker.Helpers;

namespace ZongziTEK_Blackboard_Sticker
{
    /// <summary>
    /// TimetableNotificationWindow.xaml 的交互逻辑
    /// </summary>
    public partial class TimetableNotificationWindow : Window
    {
        public TimetableNotificationWindow(string title, string subtitle, double time, bool isTextTimeVisible)
        {
            InitializeComponent();

            Width = SystemParameters.WorkArea.Width;

            totalTime = TimeSpan.FromSeconds(time);
            timeLeft = totalTime;
            timeToHide = DateTime.Now.TimeOfDay + timeLeft;

            TextTitle.Text = title;
            TextSubtitle.Text = subtitle;

            timeTimer.Tick += Timer_Tick;
            timeTimer.Start();

            GridNotification.Opacity = 0;

            if (!isTextTimeVisible)
            {
                TextTime.Visibility = Visibility.Hidden;
                isTimeHidden = true;
            }

            TextTime.Text = (timeLeft.TotalSeconds - 1).ToString("00");
        }

        private TimeSpan totalTime;
        private TimeSpan timeLeft;
        private TimeSpan timeToHide;
        private bool isTimeHidden = false;
        private bool isNotificationHidden = false;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //Timer_Tick(null, null);
            ShowNotification();

            DoubleAnimation barWidthAnimation = new()
            {
                From = BorderNotification.ActualWidth,
                To = 0,
                Duration = totalTime
            };
            RectangleProgressBar.BeginAnimation(WidthProperty, barWidthAnimation);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            timeTimer.Stop();
            timeTimer.Tick -= Timer_Tick;
        }

        private DispatcherTimer timeTimer = new()
        {
            Interval = TimeSpan.FromMilliseconds(10)
        };

        private void Timer_Tick(object sender, EventArgs e)
        {
            timeLeft = timeToHide - DateTime.Now.TimeOfDay;
            TextTime.Text = timeLeft.TotalSeconds.ToString("00");

            if (timeLeft.TotalSeconds <= 1)
            {
                if (!isTimeHidden)
                    HideTime();
            }
            if (timeLeft <= TimeSpan.Zero)
            {
                if (!isNotificationHidden)
                    HideNotification();
            }
        }

        private void ShowNotification()
        {
            DoubleAnimation opacityAnimation = new()
            {
                From = 0,
                To = 1,
                Duration = TimeSpan.FromMilliseconds(750),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
            };

            /*ThicknessAnimation marginAnimation = new()
            {
                From = new Thickness(0, -160, 0, 0),
                To = new Thickness(0),
                Duration = TimeSpan.FromMilliseconds(750),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
            };*/

            DoubleAnimation topAnimation = new()
            {
                From = -Height,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(750),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseOut }
            };

            GridNotification.BeginAnimation(OpacityProperty, opacityAnimation);
            //GridNotification.BeginAnimation(MarginProperty, marginAnimation);
            BeginAnimation(TopProperty, topAnimation);
        }

        private async void HideNotification()
        {
            isNotificationHidden = true;

            DoubleAnimation opacityAnimation = new()
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(750),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn }
            };

            /*ThicknessAnimation marginAnimation = new()
            {
                From = new Thickness(0),
                To = new Thickness(0, -160, 0, 0),
                Duration = TimeSpan.FromMilliseconds(750),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn }
            };*/

            DoubleAnimation topAnimation = new()
            {
                From = 0,
                To = -Height,
                Duration = TimeSpan.FromMilliseconds(750),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn }
            };

            GridNotification.BeginAnimation(OpacityProperty, opacityAnimation);
            //GridNotification.BeginAnimation(MarginProperty, marginAnimation);
            BeginAnimation(TopProperty, topAnimation);

            await Task.Delay(750);
            Close();
        }

        private void HideTime()
        {
            isTimeHidden = true;

            DoubleAnimation opacityAnimaion = new()
            {
                From = 1,
                To = 0,
                Duration = TimeSpan.FromMilliseconds(750),
                EasingFunction = new CubicEase() { EasingMode = EasingMode.EaseIn }
            };

            TextTime.BeginAnimation(OpacityProperty, opacityAnimaion);
        }

        private void BorderNotification_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!isNotificationHidden)
            {
                HideNotification();
            }
        }
    }
}
