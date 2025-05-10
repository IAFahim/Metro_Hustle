// <copyright file="GameView.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

using _src.Scripts.Healths.Healths.Data;

namespace BovineLabs.Sample.UI.Views.Game
{
    using BovineLabs.Sample.UI.ViewModels.Game;

    public class GameView : GameBaseView<HealthViewModel>
    {
        public GameView(HealthView healthView)
            : base(new HealthViewModel())
        {
            Add(healthView);
        }
    }
}
