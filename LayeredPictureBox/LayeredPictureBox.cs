using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System;
using System.ComponentModel;

namespace LayeredPictureBox
{
    public partial class LayeredPictureBox : Panel
    {
        const string CanvasCategory = "Canvas";

        #region layers
        [Category(CanvasCategory)]
        public int LayerCount => Layers.Count;
        [Category(CanvasCategory)]
        public List<Layer<Image>> Layers { get; } = new List<Layer<Image>>();
        public Layer<Image> CreateLayer()
        {
            var layer = new Layer<Image>();
            AddLayer(layer);
            return layer;
        }
        public void CreateLayer(out Layer<Image> layer)
        {
            AddLayer(layer = new Layer<Image>());
        }
        public void AddLayer(Image layer)
        {
            AddLayer(layer, new Point(0, 0));
        }
        public void AddLayer(Image layer, Point location)
        {
            AddLayer(new Layer<Image>(layer, location));
        }
        public void AddLayer(Layer<Image> layer)
        {
            Layers.Add(layer);
            //if the layer is hiding itself, we gotta update the canvas n stuff ONCE...
            layer.PropertyChanging += (o,e) => { if(e.PropertyName == nameof(Layer<Image>.Shown) && layer.Shown) FindNewCanvasSizeLeader(); };
            //...because it will no longer meet this check anymore
            layer.PropertyChanged += delegate { if (layer.Shown) FindNewCanvasSizeLeader(); };
            FindNewCanvasSizeLeader();
        }
        public void AddLayers(int count)
        {
            for (int i = 0; i < count; i++)
                AddLayer(new Layer<Image>(null, new Point(0, 0)));
        }
        public void RemoveLayer(int index)
        {
            RemoveLayer(Layers[index]);
        }
        public void RemoveLayer(Layer<Image> layer)
        {
            Layers.Remove(layer);
            if (WidthLeader == layer)
            {
                WidthLeader = null;
                FindNewWidthLeader();
            }
            if(HeightLeader == layer)
            {
                HeightLeader = null;
                FindNewHeightLeader();
            }
            UpdateCanvasSize();
        }

        #endregion

        int canvasScale = 1;
        [Category(CanvasCategory), DefaultValue(1)]
        public int CanvasScale
        {
            get => canvasScale;
            set
            {
                if (canvasScale != value && value > 0)
                {
                    canvasScale = value;
                    UpdateCanvasSize();
                }
            }
        }

        #region size leader
        /// <summary>
        /// Updates the leaders in width and height so the canvas can be resized to the right size
        /// </summary>
        void FindNewCanvasSizeLeader()
        {
            if (!CanvasSizeLocked)
            {
                FindNewWidthLeader();
                FindNewHeightLeader();
            }
            UpdateCanvasSize();
        }

        Layer<Image> WidthLeader = null;
        [Category(CanvasCategory)]
        public int LeaderWidth { get => (WidthLeader != null && WidthLeader.Shown) ? WidthLeader.TotalWidth : 0; }
        void FindNewWidthLeader()
        {
            foreach(var contender in Layers)
                if (contender.Shown && contender.TotalWidth > LeaderWidth)
                    WidthLeader = contender;
        }

        Layer<Image> HeightLeader = null;
        [Category(CanvasCategory)]
        public int LeaderHeight { get => (HeightLeader != null && HeightLeader.Shown) ? HeightLeader.TotalHeight : 0; }

        void FindNewHeightLeader()
        {
            foreach (var contender in Layers)
                if (contender.Shown && contender.TotalHeight > LeaderHeight)
                    HeightLeader = contender;
        }
        #endregion

        #region max size and locking
        Size maxCanvasSize = new Size(0, 0);
        [Category(CanvasCategory)]
        public Size MaxCanvasSize { get => maxCanvasSize; set { maxCanvasSize = value; UpdateCanvasSize(); } }

        [Category(CanvasCategory), DefaultValue(0)]
        public int MaxCanvasWidth { get => MaxCanvasSize.Width; set { maxCanvasSize.Width = value; UpdateCanvasSize(); } }

        [Category(CanvasCategory), DefaultValue(0)]
        public int MaxCanvasHeight { get => MaxCanvasSize.Height; set { maxCanvasSize.Height = value; UpdateCanvasSize(); } }

        bool canvasSizeLocked = false;
        [Category(CanvasCategory), DefaultValue(false)]
        public bool CanvasSizeLocked
        {
            get => canvasSizeLocked;
            set
            {
                if(canvasSizeLocked != value)
                {
                    canvasSizeLocked = value;
                    FindNewCanvasSizeLeader();
                }
            }
        }
        /// <summary>
        /// Locks the control's size at whatever size it currently is, allowing for scale changes
        /// </summary>
        public void LockCanvasSize()
        {
            MaxCanvasSize = CurrentCanvasSize;
            CanvasSizeLocked = true;
        }
        /// <summary>
        /// Unlocks the control's size, allowing it to resize naturally
        /// </summary>
        public void UnlockCanvasSize()
        {
            maxCanvasSize = new Size(0, 0);
            CanvasSizeLocked = false;
        }
        #endregion

        [Category(CanvasCategory), ReadOnly(true)]
        Size CurrentCanvasSize => new Size(CurrentCanvasWidth, CurrentCanvasHeight);
        [Category(CanvasCategory), ReadOnly(true)]
        public int CurrentCanvasWidth { get; set; } = 0;
        [Category(CanvasCategory), ReadOnly(true)]
        public int CurrentCanvasHeight { get; set; } = 0;

        /// <summary>
        /// Resizes the control to be the right size to hold the entire image
        /// </summary>
        void UpdateCanvasSize()
        {
            if (AutoSize)
            {
                if (!CanvasSizeLocked)
                {
                    //if the new width is smaller than the current one, we have to be ok with shrinking
                    var newWidth = MaxCanvasWidth != 0 ? Math.Min(LeaderWidth, MaxCanvasWidth) : LeaderWidth;
                    if (newWidth < Width && AutoSizeMode != AutoSizeMode.GrowAndShrink)
                        newWidth = Width;
                    CurrentCanvasWidth = newWidth;

                    var newHeight = MaxCanvasHeight != 0 ? Math.Min(LeaderHeight, MaxCanvasHeight) : LeaderHeight;
                    if (newHeight < Height && AutoSizeMode != AutoSizeMode.GrowAndShrink)
                        newHeight = Height;
                    CurrentCanvasHeight = newHeight;
                }

                Width = CurrentCanvasWidth * CanvasScale;
                Height = CurrentCanvasHeight * CanvasScale;

                //Invalidate();

                OnSizeChanged(new EventArgs());
            }
        }

        private void LayeredPicturePanel_SizeChanged(object sender, EventArgs e)
        {
            Invalidate();
        }

        public Point PointToImage(Point p)
        {
            return new Point(p.X / CanvasScale, p.Y / CanvasScale);
        }

        public LayeredPictureBox()
        {
            InitializeComponent();

            //this is important
            DoubleBuffered = true;
        }

        [DefaultValue(true)]
        public override bool AutoSize { get; set; } = true;

        [DefaultValue(AutoSizeMode.GrowAndShrink)]
        public override AutoSizeMode AutoSizeMode { get; set; } = AutoSizeMode.GrowAndShrink;

        public Image Flatten()
        {
            var rect = new Rectangle(0, 0, CurrentCanvasWidth, CurrentCanvasHeight);
            var output = new Bitmap(CurrentCanvasWidth, CurrentCanvasHeight);
            using(var g = Graphics.FromImage(output))
            {
                var args = new PaintEventArgs(g, rect);
                OnPaintBackground(args);
                OnPaint(args);
            }
            return output;
        }

        #region painting
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            PaintLayers(e);
            base.OnPaint(e);
        }

        private void PaintLayers(PaintEventArgs e)
        {
            //clear the area to redraw
            //e.Graphics.CompositingMode = CompositingMode.SourceCopy;
            //e.Graphics.DrawRectangle(Pens.Transparent, e.ClipRectangle);

            //the clipping rectangle is based off of the DISPLAYED image
            //which means we need to size it down to account for any scaling
            var realClip = e.ClipRectangle.ExpandDivide(canvasScale);

            e.Graphics.PixelOffsetMode = PixelOffsetMode.Half;
            e.Graphics.CompositingMode = CompositingMode.SourceOver;
            e.Graphics.InterpolationMode = InterpolationMode.NearestNeighbor;
            for (int i = 0; i < LayerCount; i++)
            {
                if (Layers[i].Shown)
                {
                    //source is the overlap of the clipping rect and where the source image is rendered on the canvas
                    //then subtract the Location so that the src actually points to a spot on the stored image
                    var src = Rectangle.Intersect(Layers[i].DrawRect, realClip).NegativeOffset(Layers[i].Location);

                    if (!src.IsEmpty)
                    {
                        //destination is where ever we just were (add the offset back) multiplied to fit the scale again
                        var dest = src.PositiveOffset(Layers[i].Location).Multiply(CanvasScale);
                        e.Graphics.DrawImage(Layers[i].Image, dest, src, GraphicsUnit.Pixel);
                    }
                }
            }
        }
        #endregion
    }
}
