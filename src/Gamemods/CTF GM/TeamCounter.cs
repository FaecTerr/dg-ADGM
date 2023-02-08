﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DuckGame;

namespace DuckGame.C44P
{
    public class TeamCounter : Thing, IDrawToDifferentLayers
    {
        private BitmapFont _font;
        private bool _temp;
        public float[] count = new float[8];
        public int[] team = new int[8];

        public GMTimer Timer;

        public StateBinding _counter = new StateBinding("count", -1, false, false);
        public StateBinding _TeamBind = new StateBinding("team", -1, false, false);
        public TeamCounter(float xpos, float ypos, bool temp = false) : base(xpos, ypos, null)
        {
            _font = new BitmapFont("biosFont", 8, -1);
            _temp = temp;
            depth = 0.9f;
        }

        public void OnDrawLayer(Layer pLayer)
        {
            if(pLayer == Layer.Foreground)
            {
                if (Timer != null)
                {
                    Vec2 camPos = new Vec2(Level.current.camera.position.x, Level.current.camera.position.y);
                    Vec2 camSize = new Vec2(Level.current.camera.width, Level.current.camera.height);
                    Vec2 drawPosition = camPos + camSize * new Vec2(0.5f, 0.015f);

                    Vec2 Unit = camSize / new Vec2(320, 180);
                    drawPosition += new Vec2(0, 14f) * Unit;

                    SpriteMap flag = new SpriteMap(GetPath("Sprites/Gamemods/CTF/Flag.png"), 27, 18);
                    flag.CenterOrigin();
                    flag.scale = Unit * 0.25f;

                    int[] sameNumber = new int[8];

                    for (int i = 0; i < Teams.active.Count; i++)
                    {
                        sameNumber[(int)count[i]]++;
                    }

                    for (int i = 0; i < Teams.active.Count; i++)
                    {
                        Vec2 originalPosition = drawPosition;
                        drawPosition.x += -camSize.x * 0.15f + camSize.x * 0.3f * (count[i] / Timer.progressTarget);

                        int lotOfTeamsIS = 4;
                        if (sameNumber[(int)count[i]] > lotOfTeamsIS)
                        {
                            drawPosition.x += ((i % 1) - 0.5f) * 8.5f * Unit.x;
                            drawPosition.y += (i) * (flag.texture.height * 0.125f) * Unit.y * 0.25f;
                            //drawPosition.y += (i / 2) * (flag.texture.height * lotOfTeamsIS - 2) / sameNumber[(int)count[i]] * Unit.y;
                        }
                        else
                        {
                            drawPosition.y += (i) * (flag.texture.height * 0.125f) * Unit.y * 0.25f;
                            //drawPosition.y += (i) * (flag.texture.height / (lotOfTeamsIS - 1)) * ((lotOfTeamsIS - 1) / sameNumber[(int)count[i]]) * Unit.y;
                        }

                        flag.frame = team[i];
                        Graphics.Draw(flag, drawPosition.x, drawPosition.y);

                        //Restoring position for next flag
                        drawPosition = originalPosition;
                    }
                }
            }
        }
    }
}