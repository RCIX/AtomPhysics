using System;
using System.Collections.Generic;
using System.Diagnostics;
using AtomPhysics;
using AtomPhysics.Collisions;
using AtomPhysics.Constraints;
using AtomPhysics.Factories;
using AtomPhysics.Interfaces;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace SimpleSample
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        PrimitiveBatch batch;
        Texture2D atomTexture;
        AtomPhysicsSim sim;
        Nucleus n1;
        Nucleus n2;
        NucleusChain nc1;
        NucleusChain nc2;
        NucleusChain nc3;
        NonlinearSpaceConstraint cons;
        List<IConstraint> ConstraintPile;
        Stopwatch sw;
        LineDrawer lines;
        InputHelper input;
        bool grid = false;
        private Vector2 LastPosition = Vector2.Zero;
        PixelDrawer px;

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            sim = new AtomPhysicsSim(new SweepAndPruneBroadPhase(new SimpleNarrowPhase()));
            graphics.PreferredBackBufferHeight = 500;
            graphics.PreferredBackBufferWidth = 500;
            IsMouseVisible = true;
            this.IsFixedTimeStep = true;
            this.TargetElapsedTime = new TimeSpan(0, 0, 0, 0, 16);
        }

        protected override void Initialize()
        {
            ConstraintPile = new List<IConstraint>();
            cons = ConstraintFactory.Instance.CreateNonlinearSpaceConstraint(sim, Vector2.Zero, 125, 125, 4, 1f);
            n1 = new Nucleus
            {
                Radius = 8,
                Mass = 0.5f,
                Position = new Vector2(240, 310),
                LinearDrag = 0.05f,
                RestitutionCoefficient = 1f
            };
            sim.Add(n1);
            n2 = new Nucleus
            {
                Radius = 8,
                Mass = 0.5f,
                Position = new Vector2(251, 326),
                LinearDrag = 0.05f,
                RestitutionCoefficient = 1f,
                //NonLinearSpace = cons,
                VelocityReferenceFrame = n1
            };
            sim.Add(n2);
            nc1 = ChainFactory.Instance.CreateStraightChain(sim, NucleusInfo.ExtractInfo(n1), new Vector2(0, 0), new Vector2(500, 500), true, true, LinkType.BondConstraint);
            //nc2 = ChainFactory.Instance.CreateStraightChain(sim, NucleusInfo.ExtractInfo(n1), new Vector2(0, 250), new Vector2(250, 500), true, true, LinkType.BondConstraint);
            //nc3 = ChainFactory.Instance.CreateStraightChain(sim, NucleusInfo.ExtractInfo(n1), new Vector2(250, 0), new Vector2(500, 250), true, true, LinkType.BondConstraint);
            foreach (Nucleus n in nc1.Nuclei)
            {
                n.NonLinearSpace = cons;
            }
            ConstraintPile.Add(ConstraintFactory.Instance.CreateAreaConstraint(sim, new Vector2(0), new Vector2(500)));
            //ConstraintPile.Add(ConstraintFactory.Instance.CreateDirectionalGravityConstraint(sim, new Vector2(0, 0.098f)));
            sw = new Stopwatch();
            input = new InputHelper();
            sim.Iterations = 8;
            base.Initialize();
        }
        protected override void LoadContent()
        {
            spriteBatch = new SpriteBatch(GraphicsDevice);
            batch = new PrimitiveBatch(GraphicsDevice);
            atomTexture = DrawingHelper.CreateCircleTexture(GraphicsDevice, 8, 1, Color.Purple, Color.Black);
            lines = new LineDrawer(GraphicsDevice);
            LastPosition = new Vector2(Window.ClientBounds.X, Window.ClientBounds.Y);
            px = new PixelDrawer(spriteBatch, GraphicsDevice);
        }

        protected override void UnloadContent()
        {
        }

        protected override void Update(GameTime gameTime)
        {
            input.Update();
            if (input.ExitRequested)
                this.Exit();
            if (input.IsCurPress(Keys.Up))
                n1.ApplyForce(new Vector2(0, -0.3f));
            if (input.IsCurPress(Keys.Down))
                n1.ApplyForce(new Vector2(0, 0.3f));
            if (input.IsCurPress(Keys.Left))
                n1.ApplyForce(new Vector2(-0.3f, 0));
            if (input.IsCurPress(Keys.Right))
                n1.ApplyForce(new Vector2(0.3f, 0));
            if (input.IsCurPress(Keys.Q))
                n1.ApplyForce(n1.Velocity * -0.1f);
            if (input.IsCurPress(Keys.Z))
                cons.ApplyExplosion(1f, 25, n1.Position);
            if (input.IsCurPress(Keys.X))
                cons.ApplyImplosion(1f, 25, n1.Position);
            if (input.IsCurPress(Keys.A))
                cons.ApplyForce(50, n1.Position, n1.Velocity);
            if (input.IsCurPress(Keys.S))
                cons.ApplyHealing(50, 1f, n1.Position);
            if (input.IsNewPress(Keys.C))
                grid = !grid;

            if (input.IsCurPress(Keys.D))
            {
                Vector2 n1Direction = n1.Velocity;
                if (n1Direction.Length() > 0)
                {
                    n1Direction.Normalize();
                }
                else
                {
                    n1Direction = new Vector2(0, -1);
                }

                Vector2 inFrontOfAtomPos = n1.Position + (n1Direction * 25);
                Vector2 behindAtomPos = n1.Position + (n1Direction * -25);
                cons.ApplyImplosion(1f, 25f, inFrontOfAtomPos);
                cons.ApplyExplosion(0.5f, 25f, behindAtomPos);
            }
            Vector2 currentPosition = new Vector2(Window.ClientBounds.X, Window.ClientBounds.Y);
            foreach (Nucleus n in sim.AtomList)
            {
                n.Position -= currentPosition - LastPosition;
            }
            LastPosition = currentPosition;
            sw.Start();
            sim.Update(0.016f);
            sw.Stop();
            float elapsed = (float)sw.ElapsedTicks / Stopwatch.Frequency;
            float fps = 1 / elapsed;
            Window.Title = "I am running at  " + fps.ToString() + " FPS";
            sw.Reset();
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CornflowerBlue);
            if (grid)
            {
                lines.Prepare();
                for (int x = 0; x < cons.RestPositions.GetLength(0); x++)
                {
                    for (int y = 0; y < cons.RestPositions.GetLength(1); y++)
                    {
                        lines.QueueLine(new Line2D
                        {
                            Start = cons.RestPositions[x, y],
                            End = cons.RestPositions[(int)MathHelper.Clamp(x + 1, 0, cons.RestPositions.GetLength(0) - 1), y],
                            Color = new Color(1f, 0f, 0f, 1f)
                        });
                        lines.QueueLine(new Line2D
                        {
                            Start = cons.RestPositions[x, y],
                            End = cons.RestPositions[x, (int)MathHelper.Clamp(y + 1, 0, cons.RestPositions.GetLength(1) - 1)],
                            Color = new Color(1f, 0f, 0f, 1f)
                        });
                        lines.QueueLine(new Line2D
                        {
                            Start = cons.DeformedPositions[x, y],
                            End = cons.DeformedPositions[(int)MathHelper.Clamp(x + 1, 0, cons.RestPositions.GetLength(0) - 1), y],
                            Color = new Color(0f, 1f, 0f, 1f)
                        });
                        lines.QueueLine(new Line2D
                        {
                            Start = cons.DeformedPositions[x, y],
                            End = cons.DeformedPositions[x, (int)MathHelper.Clamp(y + 1, 0, cons.RestPositions.GetLength(1) - 1)],
                            Color = new Color(0f, 1f, 0f, 1f)
                        });
                    }
                }
                //lines.QueueLine(new Line2D
                //{
                //    Start = n1.Position - new Vector2(atomTexture.Width / 2, atomTexture.Height / 2),
                //    End = (n1.Position - new Vector2(atomTexture.Width / 2, atomTexture.Height / 2)) + n1.Velocity,
                //    Color = Color.White
                //});
                lines.DoDraw();
            }
            px.Prepare();
            px.QueuePixel(new Pixel2D { X = 1, Y = 1, PixelColor = Color.Red });
            px.QueuePixel(new Pixel2D { X = 2, Y = 1, PixelColor = Color.Green });
            px.QueuePixel(new Pixel2D { X = 3, Y = 1, PixelColor = Color.Blue });
            px.DoDraw();
            spriteBatch.Begin();
            spriteBatch.Draw(atomTexture, n1.Position - new Vector2(atomTexture.Width / 2, atomTexture.Height / 2), Color.White);
            spriteBatch.Draw(atomTexture, n2.Position - new Vector2(atomTexture.Width / 2, atomTexture.Height / 2), Color.White);
            foreach (Nucleus n in nc1.Nuclei)
            {
                spriteBatch.Draw(atomTexture, n.NonLinearPosition - new Vector2(atomTexture.Width / 2, atomTexture.Height / 2), Color.White);
            }
            //foreach (Nucleus n in nc2.Nuclei)
            //{
            //    spriteBatch.Draw(atomTexture, n.Position - new Vector2(atomTexture.Width / 2, atomTexture.Height / 2), Color.White);
            //}
            //foreach (Nucleus n in nc3.Nuclei)
            //{
            //    spriteBatch.Draw(atomTexture, n.Position - new Vector2(atomTexture.Width / 2, atomTexture.Height / 2), Color.White);
            //}
            spriteBatch.End();
            base.Draw(gameTime);
        }
    }
}
