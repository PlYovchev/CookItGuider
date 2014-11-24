using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CookItUniversal.AttachedProperties
{
    public class AttachedProperty
    {
        public static bool GetPlaySound(DependencyObject obj)
        {
            return (bool)obj.GetValue(PlaySoundProperty);
        }

        public static void SetPlaySound(DependencyObject obj, bool value)
        {
            obj.SetValue(PlaySoundProperty, value);
        }

        // Using a DependencyProperty as the backing store for PlaySound.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty PlaySoundProperty =
            DependencyProperty.RegisterAttached("PlaySound", typeof(bool), typeof(AttachedProperty), new PropertyMetadata(false, OnStateChange));

        private static void OnStateChange(DependencyObject source, DependencyPropertyChangedEventArgs e)
        {
            MediaElement musicElement = source as MediaElement;
            bool state = (bool)e.NewValue;
            if (state == true)
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Tick += (obj, args) =>
                {
                    musicElement.Play();
                    timer.Stop(); 
                };
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Start();   
            }
            else if (state == false)
            {
                musicElement.Stop();
            }
        }
    }
}
