﻿using System;
using System.Collections.Generic;

namespace DuckGame.C44P
{
    [EditorGroup("ADGM|GameMode Fuse")]
    public class C4 : Holdable
    {
        public SpriteMap _sprite;
        public bool planted = false;
        public float holden = 0f;
        public float C4timer = 25f;
        public float defuse = 0f;
        public bool defused = false;
        public bool coZone = false;
        public List<Bullet> firedBullets = new List<Bullet>();

        public C4(float xval, float yval) : base(xval, yval)
        {
            center = new Vec2(8f, 6f);
            weight = 0f;
            collisionOffset = new Vec2(-4f, -6f);
            collisionSize = new Vec2(8f, 10f);
            _editorName = "Non-GM C4";
            _sprite = new SpriteMap(GetPath("Sprites/Gamemods/FuseMode/C4"), 16, 12, false);
            base.graphic = new SpriteMap(GetPath("Sprites/Gamemods/FuseMode/C4.png"), 16, 12, false);
            _sprite.AddAnimation("idle", 1f, true, new int[1]);
        }

        public override void Update()
        {
            if (position.y > Level.current.lowestPoint + 400f)
            {
                GM_Fuse fuse = Level.Nearest<GM_Fuse>(x, y);
                position = fuse.position;
            }

            if (prevOwner != null)
            {
                Duck prev = prevOwner as Duck;
                if (prev.HasEquipment(typeof(CTEquipment)))
                {
                    hSpeed = 0f;
                }
            }
            if (planted)
            {
                angleDegrees = 0;
            }

            if (holden >= 2f)
            {
                planted = true;
                if (owner != null)
                {
                    Duck d = owner as Duck;
                    d.doThrow = true;
                    SFX.Play(GetPath("SFX/bombplanted.wav"), 1f, 0f, 0f, false);
                    foreach(GM_Fuse gm in Level.current.things[typeof(GM_Fuse)])
                    {
                        if(gm != null)
                        {
                            gm.time = gm.ExplosionTime;
                        }
                    }
                }
            }
            if (planted && !defused)
            {
                if (coZone)
                {
                    if(Level.CheckRect<PlantZone>(topLeft, bottomRight) == null)
                    {
                        defused = true;
                        SFX.Play(GetPath("SFX/bombdefused.wav"), 1f, 0f, 0f, false);
                        defuse = 0f;
                    }
                    
                }
            }
            if (owner != null)
            {
                Duck d = owner as Duck;
                if (d.HasEquipment(typeof(CTEquipment)))
                {
                    d.doThrow = true;
                    hSpeed *= 0f;
                    vSpeed *= 0f;
                    velocity *= new Vec2(0f, 0f);
                }
                if (d.inputProfile.Released("SHOOT") && !planted && !d.crouch)
                {
                    holden = 0f;
                }
                if (!coZone)
                {
                    if (d.inputProfile.Down("SHOOT") && !planted && d.crouch && d.grounded)
                    {
                        d._disarmDisable = 5;
                        holden += 0.0166666f;
                        if (holden >= 2.5f)
                        {

                        }
                        if (holden % 0.3 > 0.02 && holden % 0.3 < 0.05)
                        {
                            foreach (Duck du in Level.current.things[typeof(Duck)])
                            {
                                DinamicSFX.PlayDef(du.position, position, 1f, 480, "SFX/bombbeep.wav");
                            }
                            Level.Add(new ButtonsG(x, y - 24f));
                            d._disarmDisable = 0;
                        }
                    }
                }
                else if (coZone)
                {
                    PlantZone pz = Level.CheckRect<PlantZone>(topLeft, bottomRight);
                    if (pz != null)
                    {
                        if (d.inputProfile.Down("SHOOT") && !planted && d.crouch && d.grounded && pz.Custom)
                        {
                            d._disarmDisable = 5;
                            holden += 0.0166666f;
                            if (holden % 0.3 > 0.02 && holden % 0.3 < 0.05 && holden < 3)
                            {
                                foreach (Duck du in Level.current.things[typeof(Duck)])
                                {
                                    DinamicSFX.PlayDef(du.position, position, 1f, 480, "SFX/bombbeep.wav");
                                }
                                Level.Add(new ButtonsG(x, y - 24f));
                                d._disarmDisable = 5;
                            }
                        }
                    }
                }
            }
            if (planted)
            { 
                canPickUp = false;
                C4timer -= 0.0166666f;
                if (C4timer > 0 && !defused && C4timer % 1 > 0.02f && C4timer % 1 < 0.05f)
                {
                    SFX.Play(GetPath("SFX/bombbeep.wav"), 1f, 0f, 0f, false);
                }
                if (grounded && C4timer > 0f && !defused)
                {
                    foreach (Duck du in Level.CheckRectAll<Duck>(new Vec2(position.x - 10f, position.y + 2f), new Vec2(position.x + 10f, position.y - 6f)))
                    {
                        Defuser def = Level.CheckRect<Defuser>(new Vec2(position.x - 10f, position.y + 2f), new Vec2(position.x + 10f, position.y - 6f));
                        if (du != null)
                        {
                            if (du.crouch && du.HasEquipment(typeof(CTEquipment)))
                            {
                                du.hSpeed *= 0.9f;
                                defuse += 0.0166666f;
                                Level.Add(new DefuseFore(position.x, position.y - 6f, 0.01666f, (int)defuse));
                                if (du.holdObject is Defuser)
                                {
                                    defuse += 0.0166666f;
                                }
                                if (defuse % 0.7 > 0.02 && defuse % 0.7 < 0.05 && defuse < 6)
                                {
                                    foreach (Duck duck in Level.current.things[typeof(Duck)])
                                    {
                                        DinamicSFX.PlayDef(duck.position, position, 1f, 480, "SFX/Defuse.wav");
                                    }
                                    du._disarmDisable = 5;
                                }
                                if (defuse >= 6f)
                                {
                                    defused = true;
                                    SFX.Play(GetPath("bombdefused.wav"), 1f, 0f, 0f, false);
                                    defuse = 0f;
                                }
                            }
                        }
                        else if(def == null)
                        {
                            defuse -= 0.00844444f;
                        }
                        else
                        {
                            defuse -= 0.00422222f;
                        }
                    }
                }
            }
            if(removedFromFall)
            {
                Explode();
            }
            if (defuse >= 6f)
            {
                defused = true;
                SFX.Play(GetPath("SFX/bombdefused.wav"), 1f, 0f, 0f, false);
                defuse = 0f;
            }
            if (C4timer <= 0f && !defused)
            {
                Explode();
            }
            base.Update();
        }
        public virtual void Explode()
        {
            foreach (Duck duck in Level.CheckCircleAll<Duck>(position, 160f))
            {
                duck.Kill(new DTImpact(this));
            }

            Level.Add(new ExplosionPart(position.x, position.y, true));
            int num = 6;
            if (Graphics.effectsLevel < 2)
            {
                num = 3;
            }
            for (int i = 0; i < num; i++)
            {
                float dir = (float)i * 60f + Rando.Float(-10f, 10f);
                float dist = Rando.Float(20f, 20f);
                ExplosionPart ins = new ExplosionPart(position.x + (float)(Math.Cos((double)Maths.DegToRad(dir)) * (double)dist), position.y - (float)(Math.Sin((double)Maths.DegToRad(dir)) * (double)dist), true);
                Level.Add(ins);
            }

            Graphics.FlashScreen();
            SFX.Play("explode", 1f, 0f, 0f, false);

            if (base.isServerForObject)
            {
                for (int i = 0; i < 20; i++)
                {
                    float dir = (float)i * 18f - 5f + Rando.Float(10f);
                    ATShrapnel shrap = new ATShrapnel();
                    shrap.range = 160f + Rando.Float(8f);
                    Bullet bullet = new Bullet(position.x + (float)(Math.Cos((double)Maths.DegToRad(dir)) * 6.0), position.y - (float)(Math.Sin((double)Maths.DegToRad(dir)) * 6.0), shrap, dir, null, false, -1f, false, true);
                    Level.Add(bullet);
                    firedBullets.Add(bullet);
                    if (Network.isActive)
                    {
                        NMFireGun gunEvent = new NMFireGun(null, firedBullets, 20, false, 4, false);
                        Send.Message(gunEvent, NetMessagePriority.ReliableOrdered);
                        firedBullets.Clear();
                    }
                    Level.Remove(this);
                }
            }
        }
    }
}