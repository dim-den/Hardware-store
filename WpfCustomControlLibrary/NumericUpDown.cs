using System;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace WpfCustomControlLibrary
{
    [TemplatePart(Name = "PART_TextBox", Type = typeof(TextBox))]
    [TemplatePart(Name = "PART_IncreaseButton", Type = typeof(RepeatButton))]
    [TemplatePart(Name = "PART_DecreaseButton", Type = typeof(RepeatButton))]
    public class NumericUpDown : Control
    {

        public static readonly DependencyProperty ValueProperty =
            DependencyProperty.Register("Value", typeof(int?), typeof(NumericUpDown),
                                        new PropertyMetadata(null, OnValueChanged, CoerceValue));

        public int? Value
        {
            get { return (int?)GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        private static void OnValueChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericUpDown)element;

            control.TextBox.UndoLimit = 0;
            control.TextBox.UndoLimit = 1;
        }

        private static object CoerceValue(DependencyObject element, object baseValue)
        {
            var control = (NumericUpDown)element;
            var value = (int?)baseValue;

            control.CoerceValueToBounds(ref value);
            control.TextBox.Text = value.ToString() ?? "";

            return value;
        }



        public static readonly DependencyProperty MaxValueProperty =
            DependencyProperty.Register("MaxValue", typeof(int), typeof(NumericUpDown), new PropertyMetadata(100000000, OnMaxValueChanged, CoerceMaxValue));

        public int MaxValue
        {
            get { return (int)GetValue(MaxValueProperty); }
            set { SetValue(MaxValueProperty, value); }
        }

        private static void OnMaxValueChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericUpDown)element;
            var maxValue = (int)e.NewValue;

            // If maxValue steps over MinValue, shift it
            if (maxValue < control.MinValue)
            {
                control.MinValue = maxValue;
            }

            if (maxValue <= control.Value)
            {
                control.Value = maxValue;
            }
        }

        private static object CoerceMaxValue(DependencyObject element, Object baseValue)
        {
            return (int?)baseValue;
        }

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register("MinValue", typeof(int), typeof(NumericUpDown), new PropertyMetadata(0, OnMinValueChanged,
                                                             CoerceMinValue));

        public int MinValue
        {
            get { return (int)GetValue(MinValueProperty); }
            set { SetValue(MinValueProperty, value); }
        }

        private static void OnMinValueChanged(DependencyObject element, DependencyPropertyChangedEventArgs e)
        {
            var control = (NumericUpDown)element;
            var minValue = (int)e.NewValue;

            // If minValue steps over MaxValue, shift it
            if (minValue > control.MaxValue)
            {
                control.MaxValue = minValue;
            }

            if (minValue >= control.Value)
            {
                control.Value = minValue;
            }
        }

        private static object CoerceMinValue(DependencyObject element, Object baseValue)
        {
            return (int)baseValue;
        }

        public static readonly DependencyProperty IsAutoSelectionActiveProperty =
            DependencyProperty.Register("IsAutoSelectionActive", typeof(Boolean), typeof(NumericUpDown),
                                        new PropertyMetadata(false));

        public Boolean IsAutoSelectionActive
        {
            get { return (Boolean)GetValue(IsAutoSelectionActiveProperty); }
            set { SetValue(IsAutoSelectionActiveProperty, value); }
        }


        public static readonly DependencyProperty IsValueWrapAllowedProperty =
            DependencyProperty.Register("IsValueWrapAllowed", typeof(Boolean), typeof(NumericUpDown),
                                        new PropertyMetadata(false));

        public Boolean IsValueWrapAllowed
        {
            get { return (Boolean)GetValue(IsValueWrapAllowedProperty); }
            set { SetValue(IsValueWrapAllowedProperty, value); }
        }


        protected readonly CultureInfo Culture;
        protected RepeatButton DecreaseButton;
        protected RepeatButton IncreaseButton;
        protected TextBox TextBox;


        private readonly RoutedUICommand _DecreaseValueCommand =
            new RoutedUICommand("DecreaseValue", "DecreaseValue", typeof(NumericUpDown));

        private readonly RoutedUICommand _IncreaseValueCommand =
            new RoutedUICommand("IncreaseValue", "IncreaseValue", typeof(NumericUpDown));

        private readonly RoutedUICommand _updateValueStringCommand =
            new RoutedUICommand("UpdateValueString", "UpdateValueString", typeof(NumericUpDown));

        private readonly RoutedUICommand _cancelChangesCommand =
            new RoutedUICommand("CancelChanges", "CancelChanges", typeof(NumericUpDown));


        static NumericUpDown()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(NumericUpDown), new FrameworkPropertyMetadata(typeof(NumericUpDown)));
        }

        public NumericUpDown()
        {
            Culture = (CultureInfo)CultureInfo.CurrentCulture.Clone();

            Loaded += OnLoaded;
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            AttachToVisualTree();
            AttachCommands();
        }

        private void TextBoxOnLostFocus(object sender, RoutedEventArgs routedEventArgs)
        {
            UpdateValue();
        }

        private void TextBoxOnPreviewMouseLeftButtonUp(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            if (IsAutoSelectionActive)
            {
                TextBox.SelectAll();
            }
        }

        private void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            InvalidateProperty(ValueProperty);
        }

        private void ButtonOnPreviewMouseRightButtonDown(object sender, MouseButtonEventArgs mouseButtonEventArgs)
        {
            Value = 0;
        }

        private void AttachToVisualTree()
        {
            AttachTextBox();
            AttachIncreaseButton();
            AttachDecreaseButton();
        }

        private void AttachTextBox()
        {
            var textBox = GetTemplateChild("PART_TextBox") as TextBox;

            // A null check is advised
            if (textBox != null)
            {
                TextBox = textBox;
                TextBox.LostFocus += TextBoxOnLostFocus;
                TextBox.PreviewMouseLeftButtonUp += TextBoxOnPreviewMouseLeftButtonUp;

                TextBox.UndoLimit = 1;
                TextBox.IsUndoEnabled = true;
            }
        }

        private void AttachIncreaseButton()
        {
            var increaseButton = GetTemplateChild("PART_IncreaseButton") as RepeatButton;
            if (increaseButton != null)
            {
                IncreaseButton = increaseButton;
                IncreaseButton.Focusable = false;
                IncreaseButton.Command = _IncreaseValueCommand;
                IncreaseButton.PreviewMouseLeftButtonDown += (sender, args) => RemoveFocus();
                IncreaseButton.PreviewMouseRightButtonDown += ButtonOnPreviewMouseRightButtonDown;
            }
        }

        private void AttachDecreaseButton()
        {
            var decreaseButton = GetTemplateChild("PART_DecreaseButton") as RepeatButton;
            if (decreaseButton != null)
            {
                DecreaseButton = decreaseButton;
                DecreaseButton.Focusable = false;
                DecreaseButton.Command = _DecreaseValueCommand;
                DecreaseButton.PreviewMouseLeftButtonDown += (sender, args) => RemoveFocus();
                DecreaseButton.PreviewMouseRightButtonDown += ButtonOnPreviewMouseRightButtonDown;
            }
        }

        private void AttachCommands()
        {
            CommandBindings.Add(new CommandBinding(_IncreaseValueCommand, (a, b) => IncreaseValue()));
            CommandBindings.Add(new CommandBinding(_DecreaseValueCommand, (a, b) => DecreaseValue()));
            CommandBindings.Add(new CommandBinding(_updateValueStringCommand, (a, b) => UpdateValue()));
            CommandBindings.Add(new CommandBinding(_cancelChangesCommand, (a, b) => CancelChanges()));

            CommandManager.RegisterClassInputBinding(typeof(TextBox),
                                                     new KeyBinding(_IncreaseValueCommand, new KeyGesture(Key.Up)));
            CommandManager.RegisterClassInputBinding(typeof(TextBox),
                                                     new KeyBinding(_DecreaseValueCommand, new KeyGesture(Key.Down)));
            CommandManager.RegisterClassInputBinding(typeof(TextBox),
                                                     new KeyBinding(_updateValueStringCommand, new KeyGesture(Key.Enter)));
            CommandManager.RegisterClassInputBinding(typeof(TextBox),
                                                     new KeyBinding(_cancelChangesCommand, new KeyGesture(Key.Escape)));
        }


        private void CoerceValueToBounds(ref int? value)
        {
            if (value < MinValue)
            {
                value = MinValue;
            }
            else if (value > MaxValue)
            {
                value = MaxValue;
            }
        }

        private void UpdateValue()
        {
            if (TextBox.Text != "")
            {
                try
                {
                    Value = Convert.ToInt32(TextBox.Text);
                }
                catch
                {
                    Value = null;
                }
            }
            else Value = null;
        }

        private void CancelChanges()
        {
            TextBox.Undo();
        }

        private void RemoveFocus()
        {
            // Passes focus here and then just deletes it
            Focusable = true;
            Focus();
            Focusable = false;
        }

        private void IncreaseValue()
        {
            if (Value == null) return;

            int? value = Convert.ToInt32(TextBox.Text);

            CoerceValueToBounds(ref value);

            value++;

            Value = value;
        }

        private void DecreaseValue()
        {
            if (Value == null) return;

            int? value = Convert.ToInt32(TextBox.Text);

            CoerceValueToBounds(ref value);

            value--;

            Value = value;
        }

    }
}