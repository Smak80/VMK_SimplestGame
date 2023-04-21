namespace SimplestGame
{
    public partial class Form1 : Form
    {
        private Ball b;
        private BufferedGraphics bg;
        private float fallHeight = 0f;
        public Form1()
        {
            InitializeComponent();
            b = new Ball(panel1.Size);
            bg = BufferedGraphicsManager.Current.Allocate(
                panel1.CreateGraphics(), new Rectangle(new Point(0, 0), panel1.Size)
            );
        }

        private void PaintScene()
        {
            bg.Graphics.Clear(Color.White);
            b.Paint(bg.Graphics);
            bg.Render();
        }
        private void timer1_Tick(object sender, EventArgs e)
        {
            PaintScene();
            if (b.Move()) timer1.Stop();
        }

        private void panel1_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                b.Target = e.Location;
                b.ResetMovement();
                timer1.Start();
            }
        }

        private PointF? mousePos = null;
        private void panel1_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right
                && b.Contains(e.Location)
                )
            {
                timer1.Stop();
                mousePos = e.Location;
            }
        }

        private void panel1_MouseUp(object sender, MouseEventArgs e)
        {
            mousePos = null;
        }

        private void panel1_MouseMove(object sender, MouseEventArgs e)
        {
            if (mousePos != null)
            {
                var currPos = e.Location;
                var shift = new SizeF(currPos.X - (mousePos?.X ?? 0f), currPos.Y - (mousePos?.Y ?? 0f));
                b.MoveTo(new PointF(b.Position.X + shift.Width, b.Position.Y + shift.Height));
                PaintScene();
                mousePos = currPos;
            }
        }

        private void Form1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == ' ' || e.KeyChar == 'G' || e.KeyChar == 'g')
            {
                fallHeight = Math.Max(panel1.Height - b.Position.Y, 0f);
                b.ResetMovement();
                b.Target = new PointF(b.Target.X, panel1.Height);
                timer2.Start();
                timer1.Stop();
            }
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            PaintScene();
            if (b.Fall())
            {
                PaintScene();
                if (fallHeight / 2f >= b.Size)
                {
                    if (b.Position.Y >= panel1.Height - b.Size / 2f - 1)
                    {
                        // Шар внизу, отскок
                        b.Target = new PointF(b.Target.X, panel1.Height - fallHeight / 2f - b.Size / 2f);
                        b.ResetMovement();
                    }
                    else
                    {
                        // Шар наверху, снова падение
                        fallHeight = Math.Max(panel1.Height - b.Position.Y, 0f);
                        b.Target = new PointF(b.Target.X, panel1.Height - b.Size / 2f);
                        b.ResetMovement();
                    }
                }
                else
                {
                    timer2.Stop();
                }
            }
        }
    }
}