using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EloBuddy;
using EloBuddy.SDK.Menu;
using EloBuddy.SDK.Menu.Values;

namespace BrandBud
{
    class MenuManager
    {
        private static Menu menu;

        public static void LoadMenu(EventArgs e)
        {
            menu = MainMenu.AddMenu("BrandBub", "BrandBud by ngothaison v1.0");
            menu.AddLabel("BrandBub");

            menu.AddGroupLabel("Combo");
            {
                AddCheckBox("Combo.Q", "Use Q Combo");
                AddCheckBox("Combo.W", "Use W Combo");
                AddCheckBox("Combo.E", "Use E Combo");
                AddCheckBox("Combo.R", "Use R Combo");
                menu.AddSeparator();
            }

            menu.AddGroupLabel("Harass");
            {
                AddCheckBox("Harass.Q", "Use Q Harass");
                AddCheckBox("Harass.W", "Use W Harass");
                AddCheckBox("Harass.E", "Use E Harass");
                AddSlider("Harass.MinMana", "Min Mana to Harass");
                menu.AddSeparator();
            }

            menu.AddGroupLabel("Farm");
            {
                AddCheckBox("Farm.Q", "Use Q Farm");
                AddCheckBox("Farm.W", "Use W Farm");
                AddCheckBox("Farm.E", "Use E Farm");
                AddSlider("Farm.MinMana", "Min Mana to Farm");
                menu.AddSeparator();
            }

            menu.AddGroupLabel("LaneClean");
            {
                AddCheckBox("LaneClean.Q", "Use Q LaneClean");
                AddCheckBox("LaneClean.W", "Use W LaneClean");
                AddCheckBox("LaneClean.E", "Use E LaneClean");
                AddSlider("LaneClean.MinMana", "Min Mana to LaneClean");
                menu.AddSeparator();
            }

            menu.AddGroupLabel("Draw");
            {
                AddCheckBox("Draw.Q", "Draw Q");
                AddCheckBox("Draw.W", "Draw W");
                AddCheckBox("Draw.E", "Draw E");
                AddCheckBox("Draw.R", "Draw R");
                menu.AddSeparator();
            }

            menu.AddGroupLabel("KillSteal");
            {
                AddCheckBox("KillSteal.Q", "Use Q KillSteal");
                AddCheckBox("KillSteal.W", "Use W KillSteal");
                AddCheckBox("KillSteal.E", "Use E KillSteal");
                AddCheckBox("KillSteal.R", "Use R KillSteal");
                menu.AddSeparator();
            }

            menu.AddGroupLabel("Mixed");
            {
                AddCheckBox("Mixed.Gap", "Use E, Q on gapcloser");
                AddCheckBox("Mixed.I", "Use E, Q to Interrupter spells");
                
                menu.AddSeparator();
            }
        }
        
        private static void AddCheckBox(string name, string displayName)
        {
            menu.Add(name, new CheckBox(displayName));
        }

        private static void AddSlider(string name, string displayName)
        {
            menu.Add(name, new Slider(displayName,20,0,100));
        }

        public static bool GetCheckBox(string name)
        {
            return menu[name].Cast<CheckBox>().CurrentValue;
        }

        public static int GetSlider(string name)
        {
            return menu[name].Cast<Slider>().CurrentValue;
        }
    }
}
