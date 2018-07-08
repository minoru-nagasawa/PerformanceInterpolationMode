using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PerformanceInterpolationMode
{
    class PictureBox2 : PictureBox
    {
        private Image m_image;
        private InterpolationMode[] m_interpolations;
        private Dictionary<InterpolationMode, long> m_totalTicks;
        private int m_count;
        private Timer m_timer;
        public PictureBox2()
        {
            m_image = new Bitmap(640, 480);
            m_interpolations = new InterpolationMode[]
            {
                InterpolationMode.Default,
                InterpolationMode.Low,
                InterpolationMode.High,
                InterpolationMode.Bilinear,
                InterpolationMode.Bicubic,
                InterpolationMode.NearestNeighbor,
                InterpolationMode.HighQualityBilinear,
                InterpolationMode.HighQualityBicubic
            };
            m_totalTicks = new Dictionary<InterpolationMode, long>();
            foreach (var v in m_interpolations)
            {
                m_totalTicks[v] = 0;
            }
            m_timer = new Timer();
            m_timer.Interval = 500;
            m_timer.Tick += (s, e) =>
            {
                this.Refresh();
            };
            m_timer.Start();

            this.Disposed += (s, e) =>
            {
                using (var writer = new StreamWriter(@"D:\performance.txt"))
                {
                    writer.WriteLine($"{m_count}");
                    foreach (var v in m_totalTicks)
                    {
                        writer.WriteLine($"{v.Key}\t{v.Value}");
                    }
                }
            };
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            var shuffled = m_interpolations.OrderBy(i => Guid.NewGuid());
            foreach (var v in shuffled)
            {
                var sw = new Stopwatch();
                sw.Start();
                e.Graphics.InterpolationMode = v;
                e.Graphics.DrawImage(m_image, 0, 0, this.Width, this.Height);
                sw.Stop();
                m_totalTicks[v] = m_totalTicks[v] + sw.ElapsedTicks;
            }
            ++m_count;
        }
    }
}
