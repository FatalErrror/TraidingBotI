using System.Collections.Generic;
using System.Drawing;




namespace TraidingBotI
{
    public class Canvas 
    {
        private Graphics g;
        private int _width;
        private int _height;

        private long _time;

        public Canvas(Graphics g, int width, int height)
        {
            this.g = g;
            _width = width;
            _height = height;
        }

        public void SetTime(long t)
        {
            _time = t;
        }

        public void Clear()
        {
            g.Clear(Color.White);
        }

        public void DrawCandle(Candle candle, int posX, int posY)
        {

            SolidBrush brush;
            Pen pen;
            if (candle.Open < candle.Close)
            {
                brush = new SolidBrush(Color.Green);
                pen = new Pen(Color.Green, 2);
            }
            else
            {
                brush = new SolidBrush(Color.Red);
                pen = new Pen(Color.Red, 2);
            }

            /*int stp = Math.Min(posY, posY - (candle.outValue - candle.inValue));
            g.DrawLine(pen, posX + 5, stp, posX + 5, posY - (candle.max - candle.inValue));
            stp = Math.Max(posY, posY - (candle.outValue - candle.inValue));
            g.DrawLine(pen, posX + 5, stp, posX + 5, posY - (candle.min - candle.inValue));
            */
            /*int stp = Math.Min(posY, posY - (candle.close - candle.open));
            g.FillRectangle(brush, posX, stp, 10, Math.Abs(candle.close - candle.open));


            g.DrawLine(pen, posX + 5, posY + (candle.open-candle.low), posX + 5, posY - (candle.high - candle.open));
           */
            
        }

        public void DrawSr(List<decimal> points, System.Drawing.Drawing2D.DashStyle dashStyle)
        {
            Pen pen = new Pen(Color.Green, 2);
            pen.DashStyle = dashStyle;

            var onepx = (_width - 20) / (float)points.Count;
            var onepy = (_height) / (decimal)102;


            for (int i = 0; i < points.Count-1; i++)
            {
                g.DrawLine(pen, (int)(onepx*i), (int)(_height - onepy - points[i] * (_height-2*onepy)), (int)(onepx*(i+1)), (int)(_height - onepy - points[i+1] * (_height - 2 * onepy)));
            }
        }

        public void Drawsh()
        {
            Pen pen = new Pen(Color.Gray, 1);

            int y;


            for (int i = 1; i < 102; i+=5)
            {
                y = _height * i / 102;
                g.DrawLine(pen, 0, y, _width, y);
            }
        }

    }

   
}
