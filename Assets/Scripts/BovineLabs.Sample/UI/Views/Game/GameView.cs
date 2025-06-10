// <copyright file="GameView.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

using _src.Scripts.Healths.Healths.Data;
using BovineLabs.Core;

namespace BovineLabs.Sample.UI.Views.Game
{
    public class GameView : GameBaseView<GameModel>
    {
        public GameView(HealthView healthView)
            : base(new GameModel())
        {
            
            Add(healthView);
            // var templateContainer = service.GetAsset("Mobile").Instantiate();
            // var visualElement = templateContainer.ElementAt(0);
            // Add(visualElement);
            // AddToClassList(".mobile__full");

        }
        
        public static void ExitGameWorld()
        {
            BovineLabsBootstrap.Instance.DestroyGameWorld();
        }
    }
}
