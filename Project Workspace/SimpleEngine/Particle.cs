using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Xna.Framework;

namespace CPI411.SimpleEngine
{
    public class Particle
    {
        public Vector3 Position { get; set; }
        public Vector3 Velocity { get; set; }
        public Vector3 Acceleration { get; set; }
        public float Age { get; set; }
        public float MaxAge { get; set; }
        public Vector3 Color { get; set; }
        public float Size { get; set; }
        public float SizeVelocity { get; set; }
        public float SizeAcceleration { get; set; }
        public Particle() { Age = -1; }
        public bool Update(float ElapsedGameTime)
        {
            if (Age < 0) return false;
            Velocity += Acceleration * ElapsedGameTime;
            Position += Velocity * ElapsedGameTime;
            SizeVelocity += SizeAcceleration * ElapsedGameTime;
            Size += SizeVelocity * ElapsedGameTime;
            Age += ElapsedGameTime;
            if (Age > MaxAge)
            {
                Age = -1;
                return false;
            }
            return true;
        }

        //#################################################################################################################################
        //  ASSIGNMENT 4 - Fountain Medium
        //#################################################################################################################################
        public bool UpdateMedium(float ElapsedGameTime)
        {
            if (Age < 0) return false;
            if (Position.Y > 1)
            {
                Velocity = new Vector3(
                Velocity.X,
                Velocity.Y * Age + 0.5f * (-9.8f) * (Age * Age),
                Velocity.Z);
            }
            else
            {
                Velocity = new Vector3(Velocity.X, 0, Velocity.Z);
            }
            Velocity += Acceleration * ElapsedGameTime;
            Position += Velocity * ElapsedGameTime;
            SizeVelocity += SizeAcceleration * ElapsedGameTime;
            Size += SizeVelocity * ElapsedGameTime;
            Age += ElapsedGameTime;
            if (Age > MaxAge)
            {
                Age = -1;
                return false;
            }
            return true;
        }

        //#################################################################################################################################
        //  ASSIGNMENT 4 - Fountain Advanced
        //#################################################################################################################################
        public bool UpdateAdvanced(float ElapsedGameTime, float resil, float frict)
        {
            if (Age < 0) return false;
            if (Position.Y > 0)
            {
                Velocity = new Vector3(
                Velocity.X,
                Velocity.Y * Age + 0.5f * (-9.8f) * (Age * Age),
                Velocity.Z);
            }
            else
            {
                Velocity = new Vector3(
                Velocity.X - frict,
                -Velocity.Y * resil,
                Velocity.Z - frict);
            }
            Velocity += Acceleration * ElapsedGameTime;
            Position += Velocity * ElapsedGameTime;
            SizeVelocity += SizeAcceleration * ElapsedGameTime;
            Size += SizeVelocity * ElapsedGameTime;
            Age += ElapsedGameTime;
            if (Age > MaxAge)
            {
                Age = -1;
                return false;
            }
            return true;
        }

        public bool IsActive() { return Age < 0 ? false : true; }
        public void Activate() { Age = 0; }
        public void Init()
        {
            Age = 0; Size = 1; SizeVelocity = SizeAcceleration = 0;
        }
    }
}
