// Author: Prasanna V. Loganathar
// Created: 9:26 AM 20-01-2015
// Project: RedScooby
// License: http://www.apache.org/licenses/LICENSE-2.0

using System;
using System.Collections;
using System.Collections.Generic;
using Windows.Foundation;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Animation;

namespace RedScooby.Animations
{
    public static class Actions
    {
        public static void ChangeTextContent(UIElement obj, Storyboard s, string textContent, double duration)
        {
            var dx = new ObjectAnimationUsingKeyFrames();

            if (obj is TextBlock)
            {
                Storyboard.SetTargetProperty(dx, "(TextBlock.Text)");
            }
            else if (obj is ContentControl)
            {
                Storyboard.SetTargetProperty(dx, "(ContentControl.Content)");
            }
            else
            {
                throw new Exception("Unsupported UIElement");
            }

            Storyboard.SetTarget(dx, obj);
            var textChange = new DiscreteObjectKeyFrame();
            textChange.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(duration));
            textChange.Value = textContent;
            dx.KeyFrames.Add(textChange);

            s.Children.Add(dx);
        }

        public static void FlipY(UIElement obj, Storyboard s, IEnumerable<double> valueCollection = null,
            IEnumerable<double> durationCollection = null)
        {
            SetupDefaultValues(ref valueCollection);
            SetupDefaultDuration(ref durationCollection);

            obj.RenderTransform = new CompositeTransform();
            obj.RenderTransformOrigin = new Point(0.5, 0.5);
            var d = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(d, "(UIElement.RenderTransform).(CompositeTransform.ScaleY)");
            Storyboard.SetTarget(d, obj);

            IEnumerator value = valueCollection.GetEnumerator();
            IEnumerator duration = durationCollection.GetEnumerator();

            while ((value.MoveNext()) && (duration.MoveNext()))
            {
                var flipY = new EasingDoubleKeyFrame
                {
                    Value = (double) value.Current,
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((double) duration.Current))
                };
                d.KeyFrames.Add(flipY);
            }

            s.Children.Add(d);
        }

        public static void OpacityShift(UIElement obj, Storyboard s, IEnumerable<double> valueCollection = null,
            IEnumerable<double> durationCollection = null)
        {
            SetupDefaultValues(ref valueCollection);
            SetupDefaultDuration(ref durationCollection);

            var d = new DoubleAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(d, "(UIElement.Opacity)");
            Storyboard.SetTarget(d, obj);

            IEnumerator value = valueCollection.GetEnumerator();
            IEnumerator duration = durationCollection.GetEnumerator();


            while ((value.MoveNext()) && (duration.MoveNext()))
            {
                var opacityChange = new EasingDoubleKeyFrame
                {
                    KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds((double) duration.Current)),
                    Value = (double) value.Current,
                };
                d.KeyFrames.Add(opacityChange);
            }

            s.Children.Add(d);
        }

        public static void SetupDefaultDuration(ref IEnumerable<double> valueCollection)
        {
            if (valueCollection == null)
            {
                valueCollection = new List<double> {380, 400, 650};
            }
        }

        public static void SetupDefaultValues(ref IEnumerable<double> valueCollection)
        {
            if (valueCollection == null)
            {
                valueCollection = new List<double> {0, 0, 1};
            }
        }

        public static void ShiftFontSize(UIElement obj, Storyboard s, double fontSize, double duration)
        {
            var dx = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(dx, "(UIElement.FontSize)");
            Storyboard.SetTarget(dx, obj);
            var fontSizeChange = new DiscreteObjectKeyFrame();
            fontSizeChange.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(duration));
            fontSizeChange.Value = fontSize;
            dx.KeyFrames.Add(fontSizeChange);
            s.Children.Add(dx);
        }

        public static void ShiftHeight(UIElement obj, Storyboard s, double height, double duration)
        {
            var dx = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(dx, "(UIElement.Height)");
            Storyboard.SetTarget(dx, obj);
            var keyframe = new DiscreteObjectKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(duration)),
                Value = height
            };
            dx.KeyFrames.Add(keyframe);
            s.Children.Add(dx);
        }

        public static void ShiftMargin(UIElement obj, Storyboard s, Thickness margin, double duration)
        {
            var dx = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(dx, "(UIElement.Margin)");
            Storyboard.SetTarget(dx, obj);
            var marginChange = new DiscreteObjectKeyFrame();
            marginChange.KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(duration));
            marginChange.Value = margin;
            dx.KeyFrames.Add(marginChange);
            s.Children.Add(dx);
        }

        public static void ShiftWidth(UIElement obj, Storyboard s, double width, double duration)
        {
            var dx = new ObjectAnimationUsingKeyFrames();
            Storyboard.SetTargetProperty(dx, "(UIElement.Width)");
            Storyboard.SetTarget(dx, obj);
            var keyframe = new DiscreteObjectKeyFrame
            {
                KeyTime = KeyTime.FromTimeSpan(TimeSpan.FromMilliseconds(duration)),
                Value = width
            };
            dx.KeyFrames.Add(keyframe);
            s.Children.Add(dx);
        }
    }
}
