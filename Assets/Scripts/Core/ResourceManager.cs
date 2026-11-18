using System;
using UnityEngine;
using Rumors.Core;

namespace Rumors.Core
{
    public class ResourceManager : MonoBehaviour
    {
        public static ResourceManager Instance { get; private set; }

        [SerializeField] private int _gold = 200;
        [SerializeField] private int _mana = 60;
        [SerializeField] private int _food = 40;
        [SerializeField] private int _ore  = 30;

        public int MaxGold { get; private set; } = 999;
        public int MaxMana { get; private set; } = 500;
        public int MaxFood { get; private set; } = 200;
        public int MaxOre  { get; private set; } = 300;

        public int Gold => _gold;
        public int Mana => _mana;
        public int Food => _food;
        public int Ore  => _ore;

        // daily trackers
        public int TodayGoldEarned  { get; private set; }
        public int TodayGoldSpent   { get; private set; }
        public int TodayManaEarned  { get; private set; }
        public int TodayFoodProd    { get; private set; }
        public int TodayFoodConsumed{ get; private set; }

        public event Action<int,int,int,int> OnResourceChanged;

        private void Awake()
        {
            if (Instance != null) { Destroy(gameObject); return; }
            Instance = this;
        }

        public bool CanAfford(int g, int m=0, int f=0, int o=0)
            => _gold>=g && _mana>=m && _food>=f && _ore>=o;

        public bool Spend(int g, int m=0, int f=0, int o=0, bool daily=false)
        {
            if (!CanAfford(g,m,f,o)) return false;
            _gold-=g; _mana-=m; _food-=f; _ore-=o;
            if (daily) { TodayGoldSpent+=g; TodayFoodConsumed+=f; }
            Notify(); return true;
        }

        public void Add(int g, int m=0, int f=0, int o=0, bool daily=false)
        {
            _gold = Mathf.Min(_gold+g, MaxGold);
            _mana = Mathf.Min(_mana+m, MaxMana);
            _food = Mathf.Min(_food+f, MaxFood);
            _ore  = Mathf.Min(_ore +o, MaxOre);
            if (daily) { TodayGoldEarned+=g; TodayManaEarned+=m; TodayFoodProd+=f; }
            Notify();
        }

        public void SetAll(int g, int m, int f, int o)
        {
            _gold=Mathf.Min(g,MaxGold); _mana=Mathf.Min(m,MaxMana);
            _food=Mathf.Min(f,MaxFood); _ore=Mathf.Min(o,MaxOre);
            Notify();
        }

        public void ExpandCapacity(int dg=0,int dm=0,int df=0,int dor=0)
        {
            MaxGold+=dg; MaxMana+=dm; MaxFood+=df; MaxOre+=dor;
        }

        public void ResetDailyTracker()
        {
            TodayGoldEarned=TodayGoldSpent=TodayManaEarned=TodayFoodProd=TodayFoodConsumed=0;
        }

        private void Notify() => OnResourceChanged?.Invoke(_gold,_mana,_food,_ore);
    }
}
