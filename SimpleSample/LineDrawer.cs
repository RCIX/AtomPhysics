using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace SimpleSample
{
    public sealed class LineDrawer
    {
        private bool _begun = false;
        private GraphicsDevice _gDevice;
        private Dictionary<int, Line2D> _linesToDraw;
        BasicEffect _effect;
        public LineDrawer(GraphicsDevice device)
        {
            _gDevice = device;
            _effect = new BasicEffect(_gDevice, null);
            _effect.VertexColorEnabled = true;
            _effect.Projection = Matrix.CreateOrthographicOffCenter(0, _gDevice.Viewport.Width, _gDevice.Viewport.Height, 0, 1, 1000);
            _effect.View = Matrix.CreateLookAt(new Vector3(0, 0, 1), Vector3.Zero, Vector3.Up);
        }
        public void Prepare()
        {
            if (_begun)
                throw new InvalidOperationException("Prepare cannot be called while a draw operation is in progress.");
            _linesToDraw = new Dictionary<int, Line2D>(1);
            _begun = true;
        }
        public void QueueLine(Line2D line)
        {
            if (!_begun)
                throw new InvalidOperationException("QueueLine cannot be called unless Prepare has been called first.");
            _linesToDraw.Add(_linesToDraw.Count, line);
        }

        public void DoDraw()
        {
            if (!_begun)
                throw new InvalidOperationException("DoDraw cannot be called unless Prepare has been called first.");
            if (_linesToDraw.Count <= 0)
                throw new InvalidOperationException("You must add at least one line to draw.");
            VertexPositionColor[] points = new VertexPositionColor[_linesToDraw.Count * 2];
            int i = 0;
            foreach (KeyValuePair<int, Line2D> kvp in _linesToDraw)
            {
                points[i] = new VertexPositionColor(new Vector3(kvp.Value.Start, 0), kvp.Value.Color);
                points[i + 1] = new VertexPositionColor(new Vector3(kvp.Value.End, 0), kvp.Value.Color);
                i += 2;
            }
            int[] indices = new int[_linesToDraw.Count * 2];
            for (int j = 0; j < _linesToDraw.Count * 2; j++)
            {
                indices[j] = j;
            }
            _gDevice.RenderState.PointSize = 1;
            _gDevice.RenderState.PointSizeMax = 1;
            _gDevice.RenderState.PointSizeMin = 1;
            VertexDeclaration tempVerDec = _gDevice.VertexDeclaration;
            _gDevice.VertexDeclaration = new VertexDeclaration(_gDevice, VertexPositionColor.VertexElements);
            _effect.Begin();
            foreach (EffectPass p in _effect.CurrentTechnique.Passes)
            {
                p.Begin();
                _gDevice.DrawUserIndexedPrimitives(PrimitiveType.LineList, points, 0, points.Length, indices, 0, _linesToDraw.Count);
                p.End();
            }
            _effect.End();
            _gDevice.VertexDeclaration = tempVerDec;
            _begun = false;
        }
    }
    public struct Line2D
    {
        public Vector2 Start { get; set; }
        public Vector2 End { get; set; }
        public Color Color { get; set; }
    }
}
