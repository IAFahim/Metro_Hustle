// In your UI assembly (e.g., YourProject.UI)

using System;
using System.ComponentModel;
using BovineLabs.Anchor;
using BovineLabs.Anchor.Contracts;
using Unity.Entities;
using Unity.Properties;
using UnityEngine;

namespace _src.Scripts.Healths.Healths.Data
{
    public partial class HealthViewModel : SystemObservableObject<HealthViewModel.Data>
    {
        [CreateProperty(ReadOnly = true)] public int CurrentHealth => this.Value.CurrentHealth;

        [CreateProperty(ReadOnly = true)] public int MaxHealth => this.Value.MaxHealth;

        public string HealthText => $"{this.CurrentHealth} / {this.MaxHealth}";
        
        public float HealthNormalized => this.MaxHealth > 0 ? (float)this.CurrentHealth / this.MaxHealth : 0f;

        [Serializable]
        public partial struct Data: IComponentData
        {
            [SystemProperty] [SerializeField] private int currentHealth;
            [SystemProperty] [SerializeField] private int maxHealth;
        }
    }
}