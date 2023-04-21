using System;
using System.Collections.Generic;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimplestGame
{
    public class Ball
    {
        private Random rnd = new Random();
        private Size _containerSize;
        private RectangleF BallRect => new RectangleF(
            Position.X - Size / 2f,
            Position.Y - Size / 2f,
            Size, Size);
        public int Size { get; set; }
        public Color Color { get; set; }
        public PointF Position { get; set; }
        private SizeF Shift { get; set; } = new SizeF(1f, 1f);
        private PointF _target = new PointF(0f, 0f);
        private int _steps;
        public PointF Target
        {
            get => _target;
            set
            {
                _target.X = (float)Math.Round(Math.Min(Math.Max(0f, value.X), _containerSize.Width - Size / 2f), 2);
                _target.Y = (float)Math.Round(Math.Min(Math.Max(0f, value.Y), _containerSize.Height - Size / 2f), 2);
                Shift = new SizeF(_target.X - Position.X, _target.Y - Position.Y);
                _steps = (int)Math.Max(Math.Abs(Shift.Width), Math.Abs(Shift.Height));
            }
        }

        public Ball(Size containerSize)
        {
            Size = rnd.Next(25, 50);
            Color = Color.FromArgb(rnd.Next(250), rnd.Next(250), rnd.Next(250));
            _containerSize = containerSize;
            Position = new PointF(
                rnd.Next(Size/2, _containerSize.Width - Size / 2), 
                rnd.Next(Size/2, _containerSize.Height - Size / 2)
                );
        }

        public void Paint(Graphics g)
        {
            var b = new SolidBrush(Color);
            g.FillEllipse(b, BallRect);
        }

        private bool _stopX = false;
        private bool _stopY = false;

        public void ResetMovement()
        {
            _stopX = _stopY = false;
        }
        public bool Move()
        {
            var dx = Shift.Width / _steps;
            var dy = Shift.Height / _steps;
            _stopX |= (Position.X - Target.X) * (Position.X + dx - Target.X) <= 0;
            _stopY |= (Position.Y - Target.Y) * (Position.Y + dy - Target.Y) <= 0;
            var shouldStop = _stopX && _stopY;
            Position = new (Position.X + dx,Position.Y + dy);
            if (shouldStop)
            {
                Position = new PointF(Target.X, Target.Y);
            }
            return shouldStop;
        }
        
        private const float _dg = 0.1f;
        private float _g = 0;

        public bool Fall(bool startFall = false)
        {
            var dx = (float)Math.Round(Shift.Width / _steps, 2);
            var dy = (float)Math.Round(Shift.Height / _steps, 2);
            if (dy > 0) _g += _dg;
            else _g = Math.Max(0f, _g - 1.75f * _dg);
            var g = (dy < 0) ? _g * -1 : _g;
            var shouldStop = 
                (Position.Y - Target.Y) * (Position.Y + dy + g - Target.Y) <= 0
                || Math.Abs(Shift.Height) <= Size;
            _stopX |= (Position.X - Target.X) * (Position.X + dx - Target.X) <= 0;
            if (_stopX) dx = 0;
            Position = new((float)Math.Round(Position.X + dx, 2), shouldStop ? Target.Y : (float)Math.Round(Position.Y + dy + g, 2));
            return shouldStop;
        }

        public bool Contains(PointF point)
        {
            return (Position.X - point.X) * (Position.X - point.X)
                + (Position.Y - point.Y) * (Position.Y - point.Y) <= Size * Size / 4f;
        }

        public void MoveTo(PointF point)
        {
            Position = point;
        }
    }
}
