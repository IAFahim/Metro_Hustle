// In your UI assembly (e.g., YourProject.UI)

using System;
using System.ComponentModel;
using BovineLabs.Anchor;
using BovineLabs.Anchor.Contracts;
using BovineLabs.Core;
using Unity.Entities;
using Unity.Properties;
using UnityEngine;

namespace _src.Scripts.Healths.Healths.Data
{
    public partial class GameModel : SystemObservableObject<GameModel.Data>
    {
        [CreateProperty(ReadOnly = true)] public int CurrentHealth => this.Value.CurrentHealth;
        [CreateProperty(ReadOnly = true)] public int TotalDistance => this.Value.TotalDistance;

        [CreateProperty(ReadOnly = true)] public int MaxHealth => this.Value.MaxHealth;

        public string HealthText => $"{this.CurrentHealth} / {this.MaxHealth}";
        
        [CreateProperty(ReadOnly = true)] 
        public float HealthNormalized => this.Value.HealthNormalized;

        [CreateProperty] public bool ExitGame => Value.ExitGame;
        
        // [CreateProperty]
        // public bool ExitGame
        // {
        //     get => BovineLabsBootstrap.GameWorld == null;
        //     set => this.SetProperty(this.ExitGame, value, value =>
        //     {
        //         if (value)
        //         {
        //             BovineLabsBootstrap.Instance.DestroyGameWorld();
        //         }
        //         else
        //         {
        //             BovineLabsBootstrap.Instance.CreateGameWorld();
        //         }
        //     });
        // }

        [Serializable]
        public partial struct Data: IComponentData
        {
            [SystemProperty] [SerializeField] private int totalDistance;
            [SystemProperty] [SerializeField] private int currentHealth;
            [SystemProperty] [SerializeField] private int maxHealth;
            [SystemProperty] [SerializeField] private float healthNormalized;
            [SystemProperty] [SerializeField] private bool exitGame;
        }
    }
}