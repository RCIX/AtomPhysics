using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SimpleSample
{
    public class PixelDrawer
    {
        private Texture2D _whitePixel;
        private List<Pixel2D> _pixelsToDraw;
        private SpriteBatch _batch;
        private bool _begun = false;
        public PixelDrawer(SpriteBatch batch, GraphicsDevice device)
        {
            _batch = batch;
            _pixelsToDraw = new List<Pixel2D>();
            _whitePixel = new Texture2D(device, 1, 1);
            _whitePixel.SetData<Color>(new Color[] { Color.White } );
        }
        public void Prepare()
        {
            if (_begun)
                throw new InvalidOperationException("Prepare cannot be called while a draw operation is in progress.");
            _begun = true;
        }
        public void QueuePixel(Pixel2D pixel)
        {
            if (!_begun)
                throw new InvalidOperationException("QueueLine cannot be called unless Prepare has been called first.");
            _pixelsToDraw.Add(pixel);
        }

        public void DoDraw()
        {
            _batch.Begin();
            foreach (Pixel2D px in _pixelsToDraw)
            {
                _batch.Draw(_whitePixel, new Vector2(px.X, px.Y), px.PixelColor);
            }
            _batch.End();
            _begun = false;
        }
    }
    public struct Pixel2D
    {
        public int X { get; set; }
        public int Y { get; set; }
        public Color PixelColor { get; set; }
    }
}
