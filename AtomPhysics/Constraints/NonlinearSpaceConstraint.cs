using AtomPhysics.Interfaces;
using Microsoft.Xna.Framework;

namespace AtomPhysics.Constraints
{
    //This is the class that contains pretty much everything related to the non-linear space code
    public class NonlinearSpaceConstraint : IConstraint
    {
        //implementation stuff. Ignore for now.
        public AtomPhysicsSim Sim { get; set; }
        public bool IsBroken { get { return false; } }
        public bool NeedsIterations { get { return false; } }

        //This is a 2-dimensional array of 2 component vectors which are used as reference points for linear space.
        private Vector2[,] _restPositions;
        public Vector2[,] RestPositions { get { return _restPositions; } }

        //this is the same as _restPositions, only this grid's positions are deformed to produce the effects.
        private Vector2[,] _deformedPositions;
        public Vector2[,] DeformedPositions { get { return _deformedPositions; } }

        //This is how stiff the non-linear space is. The higher this value is, the faster space will reform to normal.
        public float DeformationStiffness { get; set; }

        //usd for bounds checking.
        private Vector2 _min;
        private Vector2 _max;

        private int _gridSpacing;

        public NonlinearSpaceConstraint(
            AtomPhysicsSim sim,
            Vector2 topLeft,
            int sizeX,
            int sizeY,
            int gridSpacing,
            float deformationStiffness)
        {
            //values of anything much above 0.1 are unbearably stiff so i scale the input to be able to take a range of 0-1.
            DeformationStiffness = MathHelper.Clamp(deformationStiffness / 10, 0,1);

            //rest is initialization of internal state
            _restPositions = new Vector2[sizeX, sizeY];
            _deformedPositions = new Vector2[sizeX, sizeY];
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    Vector2 vector = new Vector2(topLeft.X + (x * gridSpacing), topLeft.Y + (y * gridSpacing));
                    _restPositions[x, y] = vector;
                    _deformedPositions[x, y] = vector;
                }
            }
            _min = topLeft;
            _max = new Vector2(topLeft.X + ((sizeX - 1) * gridSpacing), topLeft.Y + ((sizeY - 1) * gridSpacing));
            _gridSpacing = gridSpacing;
        }

        public void Update()
        {
            if (DeformationStiffness > 0)
            {
                for (int x = 0; x < _restPositions.GetLength(0); x++)
                {
                    for (int y = 0; y < _restPositions.GetLength(1); y++)
                    {
                        Vector2 deformedPositionDelta = _restPositions[x, y] - _deformedPositions[x, y];
                        float deformedPositionDeltaLength = deformedPositionDelta.Length();
                        if (deformedPositionDeltaLength > 0)
                        {
                            float movementMult = deformedPositionDeltaLength / (1 / DeformationStiffness);
                            deformedPositionDelta.Normalize();
                            deformedPositionDelta *= movementMult;
                            //This moves the non-linear space grid elements back toward their
                            //linear counterparts at a rate defined by DeformationStiffness.
                            _deformedPositions[x, y] = _deformedPositions[x, y] + deformedPositionDelta;
                        }
                    }
                }
            }
        }
        public void ApplyExplosion(float strength, float radius, Vector2 position)
        {
            //cap the strength to a positive value or 0
            strength = MathHelper.Clamp(strength, 0, float.MaxValue);
            for (int x = 0; x < _restPositions.GetLength(0); x++)
            {
                for (int y = 0; y < _restPositions.GetLength(1); y++)
                {
                    //get and check the distance between this non-linear space grid element and the specified position
                    float distance = (_deformedPositions[x, y] - position).Length();
                    if (distance < radius)
                    {
                        //get the direction going from the specified position to the non-linear space grid element
                        Vector2 posDifference = _deformedPositions[x, y] - position;
                        Vector2 posNormal = posDifference;
                        posNormal.Normalize();

                        //add a displacement to that non-linear space grid element scaling for distance from specified
                        //position and strength
                        _deformedPositions[x, y] += Vector2.Multiply(posNormal, (radius / distance) * strength);
                    }
                }
            }
        }

        public void ApplyImplosion(float strength, float radius, Vector2 position)
        {

            //clamp the strength to a negative value or 0
            strength = MathHelper.Clamp(-strength, float.MinValue, 0);
            for (int x = 0; x < _restPositions.GetLength(0); x++)
            {
                for (int y = 0; y < _restPositions.GetLength(1); y++)
                {
                    //get and check the distance between this linear space grid element and the specified position
                    float distance = (_restPositions[x, y] - position).Length();
                    if (distance < radius)
                    {
                        //get the direction going from the specified position to the non-linear space grid element
                        //associated with the linear one
                        Vector2 posDifference = _deformedPositions[x, y] - position;
                        Vector2 posNormal = posDifference;
                        posNormal.Normalize();


                        //add a displacement to that non-linear space grid element scaling for distance from specified
                        //position and strength
                        _deformedPositions[x, y] += Vector2.Multiply(posNormal, (distance / radius) * strength);
                    }
                }
            }
        }

        public void ApplyForce(float radius, Vector2 position, Vector2 force)
        {
            for (int x = 0; x < _restPositions.GetLength(0); x++)
            {
                for (int y = 0; y < _restPositions.GetLength(1); y++)
                {
                    //get and check the distance between this linear space grid element and the specified position
                    float distance = (_restPositions[x, y] - position).Length();
                    if (distance < radius)
                    {
                        float deformedDistance = (_deformedPositions[x, y] - position).Length();
                        //add the specified displacement to that non-linear space grid element scaling for distance from specified
                        //position and strength
                        _deformedPositions[x, y] += Vector2.Multiply(force,
                            (radius / deformedDistance));
                    }
                }
            }
        }

        public void ApplyHealing(float radius, float strength, Vector2 position)
        {
            for (int x = 0; x < _restPositions.GetLength(0); x++)
            {
                for (int y = 0; y < _restPositions.GetLength(1); y++)
                {
                    //get and check the distance between this linear space grid element and the specified position
                    float distance = (_restPositions[x, y] - position).Length();
                    if (distance < radius)
                    {
                        //This moves the non-linear space grid elements back toward their
                        //linear counterparts at a rate defined by strength.
                        _deformedPositions[x, y] = Vector2.Lerp(
                            _deformedPositions[x, y],
                            _restPositions[x, y],
                            MathHelper.Clamp(strength / 10f, 0, 1));
                    }
                }
            }
        }

        //This function used to be horribly slow, i asked for and got help on
        //optimizing it from http://stackoverflow.com/questions/1654041/optimizing-a-high-traffic-function-in-xna
        //This method of calculating non-linear positions is non-ideal and produces visible oscillating on anything
        //but the most highly detailed grids, but it's the only thing i could figure out how to implement
        public Vector2 GetNonlinearPosition(Vector2 linearPosition)
        {
            if (linearPosition.Y < _min.Y || linearPosition.Y > _max.Y)
            {
                return linearPosition;
            }
            if (linearPosition.X < _min.X || linearPosition.X > _max.X)
            {
                return linearPosition;
            }

            float deltaX = linearPosition.X - _min.X;
            float deltaY = linearPosition.Y - _min.Y;
            Vector2 nearestPoint3 = Vector2.Zero;
            Vector2 nearestPoint4 = Vector2.Zero;
            Vector2 nearestPoint1 = Vector2.Zero;
            Vector2 nearestPoint2 = Vector2.Zero;

            int baseGridPointX = (int)(deltaX - (deltaX % _gridSpacing)) / _gridSpacing;
            int baseGridPointY = (int)(deltaY - (deltaY % _gridSpacing)) / _gridSpacing;

            nearestPoint1 = _restPositions[baseGridPointX + 1, baseGridPointY + 1];
            nearestPoint2 = _restPositions[baseGridPointX + 1, baseGridPointY];
            nearestPoint3 = _restPositions[baseGridPointX, baseGridPointY + 1];
            nearestPoint4 = _restPositions[baseGridPointX, baseGridPointY];

            //this is the average of the four closest rest grid elements
            Vector2 averageRestVector = new Vector2((nearestPoint1.X +
                nearestPoint2.X +
                nearestPoint3.X +
                nearestPoint4.X) / 4,
                (nearestPoint1.Y +
                nearestPoint2.Y +
                nearestPoint1.Y +
                nearestPoint4.Y) / 4);
            Vector2 nearestDeformedPoint1 = _deformedPositions[baseGridPointX + 1, baseGridPointY + 1];
            Vector2 nearestDeformedPoint2 = _deformedPositions[baseGridPointX + 1, baseGridPointY];
            Vector2 nearestDeformedPoint3 = _deformedPositions[baseGridPointX, baseGridPointY + 1];
            Vector2 nearestDeformedPoint4 = _deformedPositions[baseGridPointX, baseGridPointY];

            //this is the average of the four closest deformed grid elements
            Vector2 averageDeformedVector = new Vector2(
                (nearestDeformedPoint1.X +
                nearestDeformedPoint2.X +
                nearestDeformedPoint3.X +
                nearestDeformedPoint4.X) / 4,
                (nearestDeformedPoint1.Y +
                nearestDeformedPoint2.Y +
                nearestDeformedPoint3.Y +
                nearestDeformedPoint4.Y) / 4);
            //this gets a vector which goes from the displaced grid cell average to the rest grid cell average,
            //then returns the linear position displaced by that amount
            Vector2 displacement = averageDeformedVector - averageRestVector;
            return linearPosition + displacement;
        }

    }
}
