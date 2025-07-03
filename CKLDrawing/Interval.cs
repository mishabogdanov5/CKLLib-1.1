using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CKLLib;

namespace CKLDrawing
{
    public class Interval : Button // ��������� ��������� ���������� ������������ �������
    {
        public TimeInterval CurrentInterval { 
            get { return _interval; } 
        }
        public bool IsActive { get => _isActive; }
       
        
        private TimeInterval _interval;
        private bool _isActive;

        new public Chain? Parent { get => _parent;  }
        private Chain _parent;

        internal void AddParent(Chain parent) { _parent = parent; }

        private void SetDefault()
        {
            Background = Constants.DefaultColors.INTERVAL_ITEM_COLOR;
            _isActive = false;
            BorderBrush = Constants.DefaultColors.INTERVAL_ITEM_BORDER_COLOR;
			BorderThickness = new Thickness(0);
			
            Click += (object sender, RoutedEventArgs e) => 
            {
                if (!_isActive)
                {
                    Background = Constants.DefaultColors.INTERVAL_ITEM_ACTIVE_COLOR;
                    BorderThickness = Constants.Dimentions.INTERVAL_BORDER_SIZE;
                }
                else 
                {
					Background = Constants.DefaultColors.INTERVAL_ITEM_COLOR;
                    BorderThickness = new Thickness(0);
				}

                _isActive = !_isActive;
            };
		}

        private void UpdateInterval(TimeInterval interval) 
        {
            
        }

        public void Select() 
        {
			Background = Constants.DefaultColors.INTERVAL_ITEM_ACTIVE_COLOR;
			BorderThickness = Constants.Dimentions.INTERVAL_BORDER_SIZE;
            _isActive = true;
		}

        public void Unselect() 
        {
			Background = Constants.DefaultColors.INTERVAL_ITEM_COLOR;
			BorderThickness = new Thickness(0);
            _isActive = false;
            
		}

        public Interval(TimeInterval interval) : base() 
        {
            _interval = interval;
            SetDefault();
        }

    }
}
