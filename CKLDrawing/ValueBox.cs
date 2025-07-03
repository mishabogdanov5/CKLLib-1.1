using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using CKLLib;

namespace CKLDrawing
{
    public class ValueBox : Button
    {
        public Chain? CurrentChain => _chain;
        public bool IsActive => _isActive;
        public RelationItem? Item => _item;
        public object Info => _info;
        public TextBox InnerTextBox { get; private set; }

        private Chain _chain;
        private bool _isActive;
        private RelationItem _item;
        private object _info;
        private bool _isEditing = false;

        public event EventHandler<bool> SelectionChanged;

        public ValueBox(RelationItem item, Chain chain) : base()
        {
            _item = item;
            _chain = chain;
            _isActive = false;

            SetUp();
        }

        public ValueBox(object info) : base()
        {
            _info = info;
            SetUp();
        }

        private void SetUp()
        {
            InnerTextBox = new TextBox
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                HorizontalContentAlignment = HorizontalAlignment.Stretch,
                VerticalContentAlignment = VerticalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Stretch,
                Background = Constants.DefaultColors.CKL_BACKGROUND,
                Foreground = Constants.DefaultColors.VALUE_COLOR,
                BorderThickness = new Thickness(0),
                TextAlignment = TextAlignment.Center,
                IsReadOnly = true,
                Cursor = Cursors.Hand,
                IsHitTestVisible = false 
            };

            InnerTextBox.Text = _item?.Value.ToString() ?? _info.ToString();

            Content = InnerTextBox;
            Width = Constants.Dimentions.VALUE_BOX_WIDTH;
            HorizontalContentAlignment = HorizontalAlignment.Stretch;
            VerticalContentAlignment = VerticalAlignment.Stretch;
            Height = Constants.Dimentions.CHAIN_HEIGHT;
            Background = Constants.DefaultColors.CKL_BACKGROUND;
            Foreground = Constants.DefaultColors.VALUE_COLOR;
            BorderThickness = new Thickness(Constants.Dimentions.SECTION_WIDTH);
            BorderBrush = Constants.DefaultColors.INTERVAL_ITEM_BORDER_COLOR;
            Margin = Constants.Dimentions.CHAIN_MARGIN;
            Cursor = Cursors.Hand;

            Click += (sender, e) =>
        {
            if (_isEditing) 
            {
                EndEditing();
            }
            else
            {
                ToggleSelection();
            }
        };

        MouseDoubleClick += (sender, e) =>
        {
            if (_isActive && !_isEditing)
            {
                StartEditing();
                InnerTextBox.CaretIndex = InnerTextBox.Text.Length;
                e.Handled = true;
            }
        };

        KeyDown += (sender, e) =>
        {
            if (e.Key == Key.Enter)
            {
                if (_isEditing)
                {
                    EndEditing();
                }
                else if (_isActive)
                {
                    StartEditing();
                    InnerTextBox.CaretIndex = InnerTextBox.Text.Length;
                }
                e.Handled = true;
            }
        };

        InnerTextBox.LostFocus += (sender, e) => 
        {
            if (_isEditing)
                EndEditing();
        };

        InnerTextBox.KeyDown += (sender, e) =>
        {
            if (e.Key == Key.Enter)
            {
                EndEditing();
                e.Handled = true;
            }
        };
        }

        public void Select()
        {
            if (_isActive) return;

            _isActive = true;
            Background = Constants.DefaultColors.TIME_OX_COLOR;
            InnerTextBox.Background = Constants.DefaultColors.TIME_OX_COLOR;
            SelectionChanged?.Invoke(this, true);

            if (_item?.Info != null)
                MessageBox.Show(_item.Info.ToString());
        }

        public void Unselect()
        {
            if (!_isActive) return;

            _isActive = false;
            Background = Constants.DefaultColors.CKL_BACKGROUND;
            InnerTextBox.Background = Constants.DefaultColors.CKL_BACKGROUND;
            SelectionChanged?.Invoke(this, false);
        }

        public void ToggleSelection()
        {
            if (_isActive)
                Unselect();
            else
                Select();
        }

        private void StartEditing()
        {
            InnerTextBox.IsReadOnly = false;
            InnerTextBox.Focus();
            InnerTextBox.SelectAll();
        }

        private void EndEditing()
        {
            InnerTextBox.IsReadOnly = true;
            _isActive = false;

            if (_item != null)
            {
                var a = InnerTextBox.Text.Split(';');
                Pair p = new Pair();
                for (int i = 0; i < a.Length; i++)
                {
                    p.Values.Add(a[i]);
                }
                try
                { 
                    _item.Value = p;
                }
                catch (Exception ex) 
                {
                    MessageBox.Show($"Ошибка {ex.Message}");
                }
            }
                
            else
                _info = InnerTextBox.Text;
        }
    }
}