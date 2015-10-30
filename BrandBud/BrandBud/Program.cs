using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK;
using EloBuddy.SDK.Events;
using EloBuddy.SDK.Enumerations;
using EloBuddy.SDK.Events;
using SharpDX;
using Color = System.Drawing.Color;

namespace BrandBud
{
    class Program
    {
        static AIHeroClient player { get { return ObjectManager.Player; } }
        static Spell.Skillshot Q, W;
        static Spell.Targeted E, R;
        public static int[] WDamages = new[] { 75, 120, 165, 210, 255 };
        public static int[] QDamages = new[] { 80, 120, 160, 200, 240 };
        public static int[] EDamages = new[] { 75, 105, 140, 175, 210 };
        public static int[] RDamages = new[] { 150, 250, 350 };

        static void Main(string[] args)
        {
            Loading.OnLoadingComplete += Loading_OnLoadingComplete;
        }

        static void Loading_OnLoadingComplete(EventArgs args)
        {
            Q = new Spell.Skillshot(SpellSlot.Q, 1050, SkillShotType.Linear, 250, 1600, 60);
            W = new Spell.Skillshot(SpellSlot.W, 900, SkillShotType.Circular,850, int.MaxValue, 240);
            E = new Spell.Targeted(SpellSlot.E, 625);
            R = new Spell.Targeted(SpellSlot.R, 750);

            MenuManager.LoadMenu(args);

            Game.OnTick += Game_OnTick;
            Drawing.OnDraw += Drawing_OnDraw;
            Gapcloser.OnGapcloser += Gapcloser_OnGapcloser;
            Interrupter.OnInterruptableSpell += Interrupter_OnInterruptableSpell;
        }

        static void Interrupter_OnInterruptableSpell(Obj_AI_Base sender, EloBuddy.SDK.Events.Interrupter.InterruptableSpellEventArgs e)
        {
            if (MenuManager.GetCheckBox("Mixed.I"))
            {
                if (E.IsReady() && sender.IsValidTarget(E.Range))
                    E.Cast(sender);

                if (Q.IsReady() && sender.IsValidTarget(Q.Range))
                {
                    var qPre = Q.GetPrediction(sender);

                    if (qPre.HitChance == HitChance.High && qPre.CollisionObjects.Length < 1)
                        Q.Cast(qPre.CastPosition);
                }
            }
        }

        static void Gapcloser_OnGapcloser(AIHeroClient sender, EloBuddy.SDK.Events.Gapcloser.GapcloserEventArgs e)
        {
            if (sender.IsMe || sender.IsAlly)
                return;

            if (MenuManager.GetCheckBox("Mixed.Gap"))
            {
                if (E.IsReady() && sender.IsValidTarget(E.Range))
                    E.Cast(sender);

                if(Q.IsReady() && sender.IsValidTarget(Q.Range))
                {
                    var qPre = Q.GetPrediction(sender);

                    if (qPre.HitChance == HitChance.High && qPre.CollisionObjects.Length < 1)
                        Q.Cast(qPre.CastPosition);
                }
            }
        }

        static void Drawing_OnDraw(EventArgs args)
        {
            if (MenuManager.GetCheckBox("Draw.Q"))
                Drawing.DrawCircle(player.Position, Q.Range, Color.Blue);

            if (MenuManager.GetCheckBox("Draw.W"))
                Drawing.DrawCircle(player.Position, W.Range, Color.Green);

            if (MenuManager.GetCheckBox("Draw.E"))
                Drawing.DrawCircle(player.Position, E.Range, Color.White);

            if (MenuManager.GetCheckBox("Draw.R"))
                Drawing.DrawCircle(player.Position, R.Range, Color.Red);
        }

        static void Game_OnTick(EventArgs args)
        {
            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Combo))
                Combo();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.Harass))
                Harass();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LastHit))
                Farm();

            if (Orbwalker.ActiveModesFlags.HasFlag(Orbwalker.ActiveModes.LaneClear))
                LC();
        }

        static void Combo()
        {
            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (target == null)
                return;

            if (player.Distance(target) <= E.Range)
            {
                if (E.IsReady() && MenuManager.GetCheckBox("Combo.E") && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }

                if(Q.IsReady() && MenuManager.GetCheckBox("Combo.Q") && target.IsValidTarget(Q.Range))
                {
                    if(target.HasBuff("brandablaze"))
                    {
                        var qPre = Q.GetPrediction(target);

                        if (qPre.HitChance == HitChance.High && qPre.CollisionObjects.Length < 1)
                        {
                            Q.Cast(qPre.CastPosition);
                        }
                    }
                }

                if (W.IsReady() && MenuManager.GetCheckBox("Combo.W") && target.IsValidTarget(W.Range))
                {
                    var wPre = W.GetPrediction(target);

                    if (wPre.HitChance == HitChance.High)
                    {
                        W.Cast(wPre.CastPosition);
                    }
                }

                if (R.IsReady() && MenuManager.GetCheckBox("Combo.R") && target.IsValidTarget(R.Range))
                {
                    R.Cast(target);
                }
            }

            else
            {
                if (W.IsReady() && MenuManager.GetCheckBox("Combo.W") && target.IsValidTarget(W.Range))
                {
                    var wPre = W.GetPrediction(target);

                    if (wPre.HitChance == HitChance.High)
                    {
                        W.Cast(wPre.CastPosition);
                    }
                }

                if (Q.IsReady() && MenuManager.GetCheckBox("Combo.Q") && target.IsValidTarget(Q.Range))
                {
                    if (target.HasBuff("brandablaze"))
                    {
                        var qPre = Q.GetPrediction(target);

                        if (qPre.HitChance == HitChance.High && qPre.CollisionObjects.Length < 1)
                        {
                            Q.Cast(qPre.CastPosition);
                        }
                    }
                }

                if (E.IsReady() && MenuManager.GetCheckBox("Combo.E") && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }

                if (R.IsReady() && MenuManager.GetCheckBox("Combo.R") && target.IsValidTarget(R.Range))
                {
                    R.Cast(target);
                }
            }
        }

        static void Harass()
        {
            if (player.ManaPercent < MenuManager.GetSlider("Harass.MinMana"))
                return;

            var target = TargetSelector.GetTarget(E.Range, DamageType.Magical);

            if (target == null)
                return;

            if (player.Distance(target) <= E.Range)
            {
                if (E.IsReady() && MenuManager.GetCheckBox("Harass.E") && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }

                if (Q.IsReady() && MenuManager.GetCheckBox("Harass.Q") && target.IsValidTarget(Q.Range))
                {
                    if (target.HasBuff("brandablaze"))
                    {
                        var qPre = Q.GetPrediction(target);

                        if (qPre.HitChance == HitChance.High && qPre.CollisionObjects.Length < 1)
                        {
                            Q.Cast(qPre.CastPosition);
                        }
                    }
                }

                if (W.IsReady() && MenuManager.GetCheckBox("Harass.W") && target.IsValidTarget(W.Range))
                {
                    var wPre = W.GetPrediction(target);

                    if (wPre.HitChance == HitChance.High)
                    {
                        W.Cast(wPre.CastPosition);
                    }
                }              
            }

            else
            {
                if (W.IsReady() && MenuManager.GetCheckBox("Harass.W") && target.IsValidTarget(W.Range))
                {
                    var wPre = W.GetPrediction(target);

                    if (wPre.HitChance == HitChance.High)
                    {
                        W.Cast(wPre.CastPosition);
                    }
                }

                if (Q.IsReady() && MenuManager.GetCheckBox("Harass.Q") && target.IsValidTarget(Q.Range))
                {
                    if (target.HasBuff("brandablaze"))
                    {
                        var qPre = Q.GetPrediction(target);

                        if (qPre.HitChance == HitChance.High && qPre.CollisionObjects.Length < 1)
                        {
                            Q.Cast(qPre.CastPosition);
                        }
                    }
                }

                if (E.IsReady() && MenuManager.GetCheckBox("Harass.E") && target.IsValidTarget(E.Range))
                {
                    E.Cast(target);
                }               
            }
        }

        static void Farm()
        {
            if (player.ManaPercent < MenuManager.GetSlider("Farm.MinMana"))
                return;

            var minions = EntityManager.MinionsAndMonsters
                .GetLaneMinions(EntityManager.UnitTeam.Enemy, player.Position, E.Range);

            if(W.IsReady() && MenuManager.GetCheckBox("Farm.W"))
            {
                var wFramPos = EntityManager.MinionsAndMonsters.GetCircularFarmLocation(minions, 60, (int)W.Range);

                if (wFramPos.HitNumber > 2)
                {
                    W.Cast(wFramPos.CastPosition);
                }
            }

            foreach(var minion in minions.Where(m => m.IsValidTarget(Q.Range) && m.Health < GetSpellDamage(SpellSlot.Q, m) * 0.9))
            {
                if (Q.IsReady() && MenuManager.GetCheckBox("Farm.Q") && minion.IsValidTarget(Q.Range))
                {
                    var qPre = Q.GetPrediction(minion);

                    if (qPre.HitChance == HitChance.High && qPre.CollisionObjects.Length < 1)
                    {
                        Q.Cast(qPre.CastPosition);
                    }
                }
            }

            foreach (var minion in minions.Where(m => m.IsValidTarget(E.Range) && m.Health < GetSpellDamage(SpellSlot.E, m) * 0.9))
            {
                if (E.IsReady() && MenuManager.GetCheckBox("Farm.E") && minion.IsValidTarget(E.Range))
                {
                    E.Cast(minion);
                }
            }
        }

        static void LC()
        {
            if (player.ManaPercent < MenuManager.GetSlider("LaneClean.MinMana"))
                return;

            var minions = EntityManager.MinionsAndMonsters
                .GetLaneMinions(EntityManager.UnitTeam.Enemy, player.Position, E.Range);

            var kappa = minions.First();

            if (Q.IsReady() && MenuManager.GetCheckBox("LaneClean.Q") && kappa.IsValidTarget(Q.Range))
            {
                Q.Cast(kappa);
            }

            if (W.IsReady() && MenuManager.GetCheckBox("LaneClean.W") && kappa.IsValidTarget(W.Range))
            {
                W.Cast(kappa);
            }

            if (E.IsReady() && MenuManager.GetCheckBox("LaneClean.E") && kappa.IsValidTarget(E.Range))
            {
                E.Cast(kappa);
            }
        }

        static void KS()
        {
            foreach(AIHeroClient target in EntityManager.Heroes.Enemies)
            {
                if (E.IsReady() && MenuManager.GetCheckBox("KillSteal.E") && target.IsValidTarget(E.Range) && target.Health < GetSpellDamage(SpellSlot.E, target) * 0.9)
                {
                    E.Cast(target);
                }

                if (Q.IsReady() && MenuManager.GetCheckBox("KillSteal.Q") && target.IsValidTarget(Q.Range) && target.Health < GetSpellDamage(SpellSlot.Q, target) * 0.9)
                {
                   
                        var qPre = Q.GetPrediction(target);

                        if (qPre.HitChance == HitChance.High && qPre.CollisionObjects.Length < 1)
                        {
                            Q.Cast(qPre.CastPosition);
                        }
                    
                }

                if (W.IsReady() && MenuManager.GetCheckBox("KillSteal.W") && target.IsValidTarget(W.Range) && target.Health < GetSpellDamage(SpellSlot.W, target) * 0.9)
                {
                    var wPre = W.GetPrediction(target);

                    if (wPre.HitChance == HitChance.High)
                    {
                        W.Cast(wPre.CastPosition);
                    }
                }
            }
        }

        static float GetSpellDamage(SpellSlot spell, Obj_AI_Base target)
        {
            float damage = new float();

            switch (spell)
            {
                case SpellSlot.Q:
                    damage = Damage.CalculateDamageOnUnit(player, target, DamageType.Magical, QDamages[Q.Level - 1] + (player.FlatMagicDamageMod + player.BaseAbilityDamage) * 0.65f);
                    break;

                case SpellSlot.W:
                    damage = Damage.CalculateDamageOnUnit(player, target, DamageType.Magical, WDamages[W.Level - 1] + (player.FlatMagicDamageMod + player.BaseAbilityDamage) * 0.60f);
                    break;

                case SpellSlot.E:
                    damage = Damage.CalculateDamageOnUnit(player, target, DamageType.Magical, EDamages[E.Level - 1] + (player.FlatMagicDamageMod + player.BaseAbilityDamage) * 0.55f);
                    break;

                case SpellSlot.R:
                    damage = Damage.CalculateDamageOnUnit(player, target, DamageType.Magical, RDamages[R.Level - 1] + (player.FlatMagicDamageMod + player.BaseAbilityDamage) * 0.50f);
                    break;

                default:
                    damage = 0;
                    break;
            }

            return damage;
        }
    }
}
