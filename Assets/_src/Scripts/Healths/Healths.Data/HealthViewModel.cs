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
        
        [CreateProperty(ReadOnly = true)] 
        public float HealthNormalized => this.Value.HealthNormalized;

        [Serializable]
        public partial struct Data: IComponentData
        {
            [SystemProperty] [SerializeField] private int currentHealth;
            [SystemProperty] [SerializeField] private int maxHealth;
            [SystemProperty] [SerializeField] private float healthNormalized;
        }
    }
}