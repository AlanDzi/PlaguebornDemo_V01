using System;
using UnityEngine;

namespace HolenderGames.UpgradesTree
{
    
    [CreateAssetMenu(fileName = "UpgradeData", menuName = "Create UpgradeData", order = 0)]
    public class UpgradeData : ScriptableObject
    {
        public string UpgradeName;
        public string[] Descriptions; 
        public int[] Costs; 
        public float[] Values; 
        public Sprite Icon;

        public bool UnlockWhenParentMaxed = true;
        [Range(1, 7)]
        public int UnlockAtParentLevel = 1; 
        public UpgradeData ParentUpgrade; 
       
        public int CurrentLevel { get; private set; }
        public bool IsMaxLevel { get { return CurrentLevel >= Descriptions.Length; } }
        public int Cost { get { return Costs[IsMaxLevel ? CurrentLevel - 1 : CurrentLevel]; } }
        public string Description { get { return Descriptions[IsMaxLevel ? CurrentLevel - 1 : CurrentLevel].ToString(); } }
        public float Value { get { return Values[CurrentLevel]; } }

        public enum StatType
        {
            MaxHealth,
            Damage,
            Stamina,
            MoveSpeed
        }



        public StatType statType;


        public void ResetUpgrade()
        {
            CurrentLevel = 0;
        }
        public void LevelUp()
        {
            if (IsMaxLevel)
                return;

            float value = Values[CurrentLevel];

            ApplyEffect(value);

            CurrentLevel++;
        }
        void ApplyEffect(float value)
        {
            PlayerStats stats = GameObject.FindFirstObjectByType<PlayerStats>();

            if (stats == null) return;

            switch (statType)
            {
                case StatType.MaxHealth:
                    stats.maxHealth += Mathf.RoundToInt(value);
                    stats.currentHealth = stats.maxHealth;
                    break;

                case StatType.Damage:
                    stats.currentDamage += Mathf.RoundToInt(value);
                    break;

                case StatType.Stamina:
                    stats.maxStamina += Mathf.RoundToInt(value);
                    stats.currentStamina = stats.maxStamina;
                    break;

                case StatType.MoveSpeed:
                    PlayerController pc = stats.GetComponent<PlayerController>();
                    if (pc != null)
                        pc.walkSpeed += value;
                    break;
            }
        }

        public override string ToString()
        {
            string desc;
            desc = name + ":\n";
            for (int i = 0; i < Descriptions.Length; i++)
            {
                desc += "Level" + (i + 1).ToString() + ": ";
                desc += Descriptions[i].ToString();
                desc += $", (cost: {Costs[i]} coins)";
                if (ParentUpgrade != null)
                {
                    desc += ", required upgrade: " + ParentUpgrade.name;

                }
                desc += "\n";
            }
            return desc;
        }

    }
}