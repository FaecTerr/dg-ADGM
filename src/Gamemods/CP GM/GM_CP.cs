﻿using System;

namespace DuckGame.C44P
{
    [EditorGroup("ADGM|GameMode CP")]
    public class GM_CP : Thing
    {
        private SpriteMap _sprite;
        public GMTimer _timer;

        public EditorProperty<float> RoundTime;
        public EditorProperty<float> ContestTime;

        public string _string;
        public float contest;
        public float time = 90f;

        public bool contesting;
        public bool uncontesting; 

        public bool Activate = false;
        public bool ctWins = false;
        public bool tWins = false;
        public bool init = false;

        public GM_CP(float xval, float yval, GMTimer gmt) : base(xval, yval)
        {
            _sprite = new SpriteMap(Mod.GetPath<C44P>("Sprites/Gamemods/GameMode.png"), 16, 16, false);
            base.graphic = _sprite;
            center = new Vec2(8f, 8f);
            collisionOffset = new Vec2(-8f, -8f);
            collisionSize = new Vec2(15f, 15f);
            graphic = _sprite;
            _visibleInGame = false;
            _timer = gmt;
            layer = Layer.Foreground;
            RoundTime = new EditorProperty<float>(90f, this, 20f, 180f, 1f, null);
            ContestTime = new EditorProperty<float>(15f, this, 5f, 30f, 5f, null) { name = "Time to capture"};

            _editorName = "GM CP";
            editorTooltip = "Don't forget about 'Contest zones'";
        }

        public override void Update()
        {
            base.Update();
            if (_timer != null && contest > 0f && contest <= ContestTime)
            {
                _timer.progress = contest / ContestTime;
            }
            if (init == false)
            {
                init = true;
                time = RoundTime;
            }
            if (contesting == true && uncontesting == false)
            {
                if (contest <= ContestTime)
                {
                    contest += 0.01666666f;
                }
            }
            if (contesting == false && tWins == false)
            {
                if (contest > 0f)
                {
                    contest -= 0.01666666f;
                }
            }
            if (_timer != null && contest <= 0f)
            {
                _timer.str = "";
            }
            if ((time >= 14.97f && time < 15f) && contest <= 0f)
            {
                SFX.Play(GetPath("15sec.wav"), 1f, 0f, 0f, false);
            }
            if ((time >= 9.97f && time < 10f) && contest <= 0f)
            {
                SFX.Play(GetPath("10sec.wav"), 1f, 0f, 0f, false);
            }
            if (((time >= 4.97f && time < 5f) || (time >= 3.97f && time < 4f) || (time >= 2.97f && time < 3f) || (time >= 1.97f && time < 2f) || (time >= 0.97f && time < 1f)) && contest <= 0f)
            {
                SFX.Play(GetPath("LastSec.wav"), 1f, 0f, 0f, false);
            }
            if (!(Level.current is Editor))
            {
                if (_timer == null && !(Level.current is Editor))
                {
                    _timer = (new GMTimer(position.x, position.y - 16f)
                    {
                        anchor = this,
                        depth = 0.95f,
                        progressBar = true
                    });
                    Level.Add(_timer);
                    Fondle(_timer);
                }
                foreach (GMTimer gm in Level.current.things[typeof(GMTimer)])
                {
                    if (gm != null && _timer != null)
                    {
                        if (gm != _timer)
                        {
                            Level.Remove(gm);
                        }
                    }
                }
                if (time > 0f)
                {
                    if (_timer != null)
                    {
                        _timer.time = time;
                        Fondle(_timer);
                    }
                    _string = Convert.ToString(time);
                    time -= 0.0166666f;
                }
                if (contest >= ContestTime && ctWins == false && tWins == false)
                {
                    TerroristWin();
                    tWins = true;
                    SFX.Play(GetPath("GameEnd.wav"), 1f, 0f, 0f, false);
                }
                else if (contest <= 0f && time <= 0f && ctWins == false && tWins == false)
                {
                    CounterTerroristWin();
                    ctWins = true;
                    SFX.Play(GetPath("GameEnd.wav"), 1f, 0f, 0f, false);
                }
            }
            if (ctWins == true)
            {
                CounterTerroristWin();
                contest = 0f;
            }
            else if (tWins == true)
            {
                TerroristWin();
            }
            contesting = false;
            uncontesting = false;
        }
        public void TerroristWin()
        {
            foreach (Duck d in Level.current.things[typeof(Duck)])
            {
                if (d != null)
                {
                    if (d._equipment != null && d.HasEquipment(typeof(CTEquipment)) == true)
                    {
                        d.Kill(new DTImpact(this));
                    }
                }
            }
            foreach (TEquipment te in Level.current.things[typeof(TEquipment)])
            {
                if (te.duck != null && te.duck.team != null)
                {
                    TeamWin(te.duck.team);
                    return;
                }
            }
        }
        public void CounterTerroristWin()
        {
            foreach (Duck d in Level.current.things[typeof(Duck)])
            {
                if (d != null)
                {
                    if (d._equipment != null && d.HasEquipment(typeof(TEquipment)) == true)
                    {
                        d.Kill(new DTImpact(this));
                    }
                }
            }
            foreach (CTEquipment cte in Level.current.things[typeof(CTEquipment)])
            {
                if (cte.duck != null && cte.duck.team != null)
                {
                    TeamWin(cte.duck.team);
                    return;
                }
            }
        }
        public void TeamWin(Team t)
        {
            foreach (Profile profile in DuckNetwork.profiles)
            {
                if (profile.team == t && !GameMode.lastWinners.Contains(profile))
                {
                    GameMode.lastWinners.Add(profile);
                }
            }
            foreach (Duck d in Level.current.things[typeof(Duck)])
            {
                if ((t == null || (t != null && !t.activeProfiles.Contains(d.profile))) && !d.dead)
                {
                    //d.Kill(new DTImpact(this));
                }
            }
            foreach (TeamRespawner respawner in Level.current.things[typeof(TeamRespawner)])
            {
                Level.Remove(respawner);
            }
        }
    }
}