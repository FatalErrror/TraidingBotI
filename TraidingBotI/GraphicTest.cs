using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;

using Skender.Stock.Indicators;



namespace TraidingBotI
{
    public partial class GraphicTest : Form
    {
        long time;
        Bitmap bmt;
        Canvas canvas;

        List<decimal> emag;
        List<decimal> demag;
        Random r;
        

        public GraphicTest()
        {
            InitializeComponent();

            bmt = new Bitmap(pictureBox1.Width, pictureBox1.Height);
            canvas = new Canvas(Graphics.FromImage(bmt), bmt.Width, bmt.Height);
            
            r = new Random();
            
            emag = new List<decimal>();
            demag = new List<decimal>();


            pictureBox1.Image = bmt;
            //timer1.Start();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            time += timer1.Interval;
            label1.Text = "Time: " + time;
            canvas.SetTime(time);
            Draw();
            pictureBox1.Image = bmt;
        }

        private void Draw()
        {
            canvas.Clear();

            canvas.Drawsh();

            canvas.DrawSr(demag, System.Drawing.Drawing2D.DashStyle.Dash);
            canvas.DrawSr(emag, System.Drawing.Drawing2D.DashStyle.Solid);
            pictureBox1.Image = bmt;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Cursor = Cursors.WaitCursor;

            var dt = dateTimePicker1.Value;//DateTime.Parse("2022-01-14 23:45:00");

            var candles = new InvestingCom().GetCandles(Symbol.MRVL, CandlesResolution.Minute5, dt.AddHours(-24).ToUNIX(), dt.ToUNIX());
            int count = 50; 
            var ema = candles.GetEma(count);
            var dema = candles.GetDoubleEma(count);

            decimal? max = 0, min = 1000000000000;
            int emacount = 0;
            int demacount = 0;

            int iterator;

            foreach (var item in ema) emacount++;
            foreach (var item in dema) demacount++;

            iterator = emacount;
            foreach (var item in ema)
            {
                iterator--;
                if (iterator < count)
                {
                    if (item.Ema > max) max = item.Ema;
                    if (item.Ema < min) min = item.Ema;
                }
            }
            iterator = demacount;
            foreach (var item in dema)
            {
                iterator--;
                if (iterator < count)
                {
                    if (item.Dema > max) max = item.Dema;
                    if (item.Dema < min) min = item.Dema;
                }
            }

            decimal? dif = max - min;
            decimal? lastCorrect = 0;

            emag.Clear();
            iterator = emacount;
            foreach (var item in ema)
            {
                if (item.Ema != null) lastCorrect = item.Ema;
                iterator--;
                if (iterator < count)
                {
                    emag.Add((decimal)((item.Ema - min) / (dif)));
                }
            }
            demag.Clear();
            iterator = demacount;
            foreach (var item in dema)
            {
                if (item.Dema != null) lastCorrect = item.Dema;
                iterator--;
                if (iterator < count)
                {
                    demag.Add((decimal)((item.Dema - min) / (dif)));
                }
            }

            Draw();

            Cursor = Cursors.Default;
        }
    }

   
}
