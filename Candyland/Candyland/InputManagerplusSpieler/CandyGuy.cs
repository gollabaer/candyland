﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;


namespace Candyland
{
    class CandyGuy : Playable
    {
        bool istargeting;
        Vector3 target;
        Texture2D texture;
        

        public CandyGuy(Vector3 position, Vector3 direction, float aspectRatio, UpdateInfo info, BonusTracker bonusTracker)
        {
            m_updateInfo = info;
            m_bonusTracker = bonusTracker;
            this.m_position = position;
            this.direction = direction;
            this.cam = new Camera(position, MathHelper.PiOver4, aspectRatio, 0.1f, 100, m_updateInfo);
            this.currentspeed = 0;
            this.upvelocity = 0;
        }

        public override void isNotCollidingWith(GameObject obj){ }

        public override void hasCollidedWith(GameObject obj){ }

        public override void update() {
            fall();
            if (m_updateInfo.candyselected)
            cam.updatevMatrix();
        }

        public override void initialize(){ }

        public override void load(ContentManager content)
        {
            effect = content.Load<Effect>("Toon");
            texture = content.Load<Texture2D>("spielertextur");
            m_model = content.Load<Model>("spielerneu");
            calculateBoundingBox();
            minOld = m_boundingBox.Min;
            maxOld = m_boundingBox.Max;
        }

        public override void uniqueskill()
        {
             if (isonground)
            {
                upvelocity = 0.08f;
                isonground = false;
            }
        }

        public override void moveTo(Vector3 goalpoint)
        {
            istargeting = true;
            target = goalpoint;
        }

        public override void movementInput(float movex, float movey, float camx, float camy)
        {
            if (istargeting)
            {
                float dx = target.X - m_position.X;
                float dz = target.Z - m_position.Z;
                float length = (float)Math.Sqrt(dx * dx + dz * dz);
                move(0.8f * dx / length, 0.8f * dz / length);
                if (length < 1) istargeting = false;
            }
            else
            {
                move(movex, movey);
                cam.changeAngle(camx, camy);
            }
        }

        protected override void fall() 
        {

            upvelocity += GameConstants.gravity;
            if (isonground) upvelocity = 0;
            this.m_position.Y += upvelocity;
            this.m_boundingBox.Max.Y += upvelocity;
            this.m_boundingBox.Min.Y += upvelocity;
            cam.changeposition(m_position);
        }

        #region collision

        public override void collide(GameObject obj)
        {
           
            cam.collideWith(obj);

            if (obj.GetType() == typeof(Platform)) collideWithPlatform(obj);
            if (obj.GetType() == typeof(Obstacle)) collideWithObstacle(obj);
            if (obj.GetType() == typeof(ObstacleBreakable)) collideWithBreakable(obj);
            if (obj.GetType() == typeof(ObstacleMoveable)) collideWithMovable(obj);
            if (obj.GetType() == typeof(PlatformSwitchPermanent)) collideWithSwitchPermanent(obj);
            if (obj.GetType() == typeof(PlatformSwitchTemporary)) collideWithSwitchTemporary(obj);
            if (obj.GetType() == typeof(ChocoChip)) collideWithChocoChip(obj); 
        }

        private void collideWithPlatform(GameObject obj)
        {
            ContainmentType contain = obj.getBoundingBox().Contains(m_boundingBox);

            if (contain == ContainmentType.Intersects)
            {
                preventIntersection(obj);
                obj.hasCollidedWith(this);
            }
            else
            {
                isonground = isonground || false;
                obj.isNotCollidingWith(this);
            }   
        }
        private void collideWithObstacle(GameObject obj) {
            if (obj.getBoundingBox().Intersects(m_boundingBox))
            {
                preventIntersection(obj);
                obj.hasCollidedWith(this);
            }
            else
            {
                obj.isNotCollidingWith(this);
            }
        }
        private void collideWithSwitchPermanent(GameObject obj) {
            if (obj.getBoundingBox().Intersects(m_boundingBox))
            {
                preventIntersection(obj);
                obj.hasCollidedWith(this);
            }
            else
            {
                obj.isNotCollidingWith(this);
            }
        }
        private void collideWithSwitchTemporary(GameObject obj) {
            if (obj.getBoundingBox().Intersects(m_boundingBox))
            {
                preventIntersection(obj);
                obj.hasCollidedWith(this);
            }
            else
            {
                obj.isNotCollidingWith(this);
            }
        }
        private void collideWithBreakable(GameObject obj) {
            if (obj.getBoundingBox().Intersects(m_boundingBox) && !obj.isdestroyed)
            {
                preventIntersection(obj);
                obj.hasCollidedWith(this);
            }
            else
            {
                obj.isNotCollidingWith(this);
            }
        }
        private void collideWithMovable(GameObject obj) {
            if (obj.getBoundingBox().Intersects(m_boundingBox)) {
                preventIntersection(obj);
                obj.hasCollidedWith(this);
            }
            else
            {
                obj.isNotCollidingWith(this);
            }
        }

        private void collideWithChocoChip(GameObject obj) {
            if (obj.getBoundingBox().Intersects(m_boundingBox))
            {
                obj.hasCollidedWith(this);
            } else {
                obj.isNotCollidingWith(this);
            }
        }

       
        #endregion

        public override void draw()
        {
            Matrix view = m_updateInfo.viewMatrix;
            Matrix projection = m_updateInfo.projectionMatrix;
            // Copy any parent transforms.
            Matrix[] transforms = new Matrix[m_model.Bones.Count];
            m_model.CopyAbsoluteBoneTransformsTo(transforms);

            Matrix translateMatrix = Matrix.CreateTranslation(m_position);
            Matrix worldMatrix = translateMatrix;

            Matrix rotation;
                    if (direction.X > 0)
                    {
                        rotation = Matrix.CreateRotationY((float)Math.Acos(direction.Z));
                    }
                    else
                    {
                        rotation = Matrix.CreateRotationY((float)-Math.Acos(direction.Z));
                    }

            // Draw the model. A model can have multiple meshes, so loop.
            foreach (ModelMesh mesh in m_model.Meshes)
            {

                    foreach (ModelMeshPart part in mesh.MeshParts)
                    {
                        part.Effect = effect;
                        effect.Parameters["World"].SetValue(rotation*worldMatrix * mesh.ParentBone.Transform);
                        effect.Parameters["DiffuseLightDirection"].SetValue(new Vector3(rotation.M13, rotation.M23, rotation.M33));
                        effect.Parameters["View"].SetValue(view);
                        effect.Parameters["Projection"].SetValue(projection);
                        effect.Parameters["WorldInverseTranspose"].SetValue(
                        Matrix.Transpose(Matrix.Invert(worldMatrix * mesh.ParentBone.Transform)));
                        effect.Parameters["Texture"].SetValue(texture);
                    }
                    // Draw the mesh, using the effects set above.
                    mesh.Draw();
                    BoundingBoxRenderer.Render(this.m_boundingBox, m_updateInfo.graphics, view, projection, Color.White);

                }
            }
        }
}