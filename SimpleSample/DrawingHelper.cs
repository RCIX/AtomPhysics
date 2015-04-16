using System;
using System.Collections.Generic;
using System.Text;
using System.Diagnostics;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SimpleSample {
    public class DrawingHelper {
        public static Texture2D CreateLineTexture(GraphicsDevice graphicsDevice, int lineThickness, Color color) {
            Texture2D texture2D = new Texture2D(graphicsDevice, 2, lineThickness + 2, 1, TextureUsage.None, SurfaceFormat.Color);

            //Texture2D texture2D = new Texture2D(graphicsDevice, 2, lineWidth + 2);
            int count = 2 * (lineThickness + 2);
            Color[] colorArray = new Color[count];
            colorArray[0] = Color.TransparentWhite;
            colorArray[1] = Color.TransparentWhite;

            for (int i = 0; i < count; i++) {
                int y = i / 2;
                colorArray[i] = color;
            }

            colorArray[count - 2] = Color.TransparentWhite;
            colorArray[count - 1] = Color.TransparentWhite;
            texture2D.SetData<Color>(colorArray);
            return texture2D;
        }

        public static Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int radius, Color color) {
            return CreateCircleTexture(graphicsDevice, radius, 0, 0, 2, color, color);
        }

        public static Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int radius, Color color, Color borderColor) {
            return CreateCircleTexture(graphicsDevice, radius, 1, 1, 1, color, borderColor);
        }

        public static Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int radius, int borderWidth, Color color, Color borderColor) {
            return CreateCircleTexture(graphicsDevice, radius, borderWidth, 1, 2, color, borderColor);
        }

        public static Texture2D CreateCircleTexture(GraphicsDevice graphicsDevice, int radius, int borderWidth, int borderInnerTransitionWidth, int borderOuterTransitionWidth, Color color, Color borderColor) {
            int x = 0;
            int y = -1;
            int diameter = (radius + 2) * 2;
            //Vector2 center = new Vector2(0, 0);
            Vector2 center = new Vector2((diameter - 1) / 2f, (diameter - 1) / 2f);

            Texture2D circle = new Texture2D(graphicsDevice, diameter, diameter, 1, TextureUsage.None, SurfaceFormat.Color);
            Color[] colors = new Color[diameter * diameter];

            for (int i = 0; i < colors.Length; i++) {
                if (i % diameter == 0) { y += 1; }
                x = i % diameter;

                Vector2 distance = new Vector2(Math.Abs(center.X - x), Math.Abs(center.Y - y));

                Vector2 diff = new Vector2(x, y) - center;
                float length = diff.Length();// distance.Length();

                if (length > radius) {
                    colors[i] = Color.TransparentBlack;
                }
                else if (length >= radius - borderOuterTransitionWidth) {

                    float transitionAmount = (length - (radius - borderOuterTransitionWidth)) / borderOuterTransitionWidth;
                    transitionAmount = 255 * (1 - transitionAmount);
                    colors[i] = new Color(borderColor.R, borderColor.G, borderColor.B, (byte)transitionAmount);
                }
                else if (length > radius - (borderWidth + borderOuterTransitionWidth)) {
                    colors[i] = borderColor;
                }
                else if (length >= radius - (borderWidth + borderOuterTransitionWidth + borderInnerTransitionWidth)) {
                    float transitionAmount = (length - (radius - (borderWidth + borderOuterTransitionWidth + borderInnerTransitionWidth))) / (borderInnerTransitionWidth + 1);
                    colors[i] = new Color((byte)MathHelper.Lerp(color.R, borderColor.R, transitionAmount), (byte)MathHelper.Lerp(color.G, borderColor.G, transitionAmount), (byte)MathHelper.Lerp(color.B, borderColor.B, transitionAmount));
                }
                else {
                    colors[i] = color;
                }
            }
            circle.SetData<Color>(colors);
            return circle;
        }

        public static Texture2D CreateRectangleTexture(GraphicsDevice graphicsDevice, int width, int height, Color color) {
            return CreateRectangleTexture(graphicsDevice, width, height, 0, 0, 2, color, color);
        }

        public static Texture2D CreateRectangleTexture(GraphicsDevice graphicsDevice, int width, int height, Color color, Color borderColor) {
            return CreateRectangleTexture(graphicsDevice, width, height, 1, 1, 2, color, borderColor);
        }

        public static Texture2D CreateRectangleTexture(GraphicsDevice graphicsDevice, int width, int height, int borderWidth, Color color, Color borderColor) {
            return CreateRectangleTexture(graphicsDevice, width, height, borderWidth, 1, 2, color, borderColor);
        }

        public static Texture2D CreateRectangleTexture(GraphicsDevice graphicsDevice, int width, int height, int borderWidth, int borderInnerTransitionWidth, int borderOuterTransitionWidth, Color color, Color borderColor) {
            Texture2D texture2D = new Texture2D(graphicsDevice, width, height, 1, TextureUsage.None, SurfaceFormat.Color);

            //Texture2D texture2D = new Texture2D(graphicsDevice, 2, lineWidth + 2);
            int x;
            int y = -1;
            int j = 0;
            int count = width * height;
            Color[] colorArray = new Color[count];
            Color[] shellColor = new Color[borderWidth + borderOuterTransitionWidth + borderInnerTransitionWidth];
            float transitionAmount = 0;

            for (j = 0; j < borderOuterTransitionWidth; j++) {
                transitionAmount = (float)(j) / (float)(borderOuterTransitionWidth);
                shellColor[j] = new Color(borderColor.R, borderColor.G, borderColor.B, (byte)(255 * transitionAmount));
                //shellColor[j] = Color.Red;
            }
            for (j = borderOuterTransitionWidth; j < borderWidth + borderOuterTransitionWidth; j++) {
                shellColor[j] = borderColor;
                //shellColor[j] = Color.Green;
            }
            for (j = borderWidth + borderOuterTransitionWidth; j < borderWidth + borderOuterTransitionWidth + borderInnerTransitionWidth; j++) {
                transitionAmount = 1 - (float)(j - (borderWidth + borderOuterTransitionWidth) + 1) / (float)(borderInnerTransitionWidth + 1);
                shellColor[j] = new Color((byte)MathHelper.Lerp(color.R, borderColor.R, transitionAmount), (byte)MathHelper.Lerp(color.G, borderColor.G, transitionAmount), (byte)MathHelper.Lerp(color.B, borderColor.B, transitionAmount));
            }


            for (int i = 0; i < count; i++) {
                if (i % width == 0) { y += 1; }
                x = i % width;

                //check if pixel is in one of the rectangular border shells
                bool isInShell = false;
                for (int k = 0; k < shellColor.Length; k++) {
                    if (InShell(x, y, width, height, k)) {
                        colorArray[i] = shellColor[k];
                        isInShell = true;
                        break;
                    }
                }
                //pixel is not in shell so it is in the center
                if (!isInShell) {
                    colorArray[i] = color;
                }
            }

            texture2D.SetData<Color>(colorArray);
            return texture2D;
        }

        private static bool InShell(int x, int y, int width, int height, int shell) {
            //check each line of rectangle.
            if ((x == shell && IsBetween(y, shell, height - 1 - shell)) || (x == width - 1 - shell && IsBetween(y, shell, height - 1 - shell))) {
                return true;
            }
            else if ((y == shell && IsBetween(x, shell, width - 1 - shell)) || (y == height - 1 - shell && IsBetween(x, shell, width - 1 - shell))) {
                return true;
            }
            else {
                return false;
            }
        }

        private static bool IsBetween(float value, float min, float max) {
            if (value >= min && value <= max) {
                return true;
            }
            else {
                return false;
            }
        }

    }
}
