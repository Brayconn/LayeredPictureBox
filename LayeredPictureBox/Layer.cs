using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

namespace LayeredPictureBox
{
    public class Layer<T> : INotifyPropertyChanged, INotifyPropertyChanging where T : Image
    {
        T image;
        public T Image { get => image; set { if (image != value) { OnPropertyChanging(); image?.Dispose(); image = value; OnPropertyChanged(); } } }
        Point location;
        public Point Location { get => location; set { if (location != value) { OnPropertyChanging(); location = value; OnPropertyChanged(); } } }
        public Size Size => Image?.Size ?? Size.Empty;
        public Rectangle DrawRect => new Rectangle(Location, Size);

        public int TotalWidth => Location.X + Size.Width;
        public int TotalHeight => Location.Y + Size.Height;

        bool shown = true;
        //empty images are always hidden, otherwise, use the stored value
        public bool Shown { get => !Size.IsEmpty && shown; set { if (shown != value) { OnPropertyChanging(); shown = value; OnPropertyChanged(); } } }

        public Layer(T image, Point drawLocation)
        {
            Image = image;
            Location = drawLocation;
        }

        public static explicit operator Image(Layer<T> l)
        {
            return l.Image;
        }
        public static explicit operator Point(Layer<T> l)
        {
            return l.Location;
        }

        protected void OnPropertyChanging([CallerMemberName] string name = null)
        {
            PropertyChanging?.Invoke(this, new PropertyChangingEventArgs(name));
        }
        public event PropertyChangingEventHandler PropertyChanging;
        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
        public event PropertyChangedEventHandler PropertyChanged;
    }
}