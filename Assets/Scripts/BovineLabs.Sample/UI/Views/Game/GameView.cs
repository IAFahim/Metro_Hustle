// <copyright file="GameView.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

using _src.Scripts.Healths.Healths.Data;
using _src.Scripts.UXMLs.UXMLs.Data;
using UnityEngine.UIElements;

namespace BovineLabs.Sample.UI.Views.Game
{
    public class GameView : GameBaseView<HealthViewModel>
    {
        public GameView(HealthView healthView, IUxmlService service)
            : base(new HealthViewModel())
        {
            Add(healthView);
            // var templateContainer = service.GetAsset("Mobile").Instantiate();
            // var visualElement = templateContainer.ElementAt(0);
            // Add(visualElement);
            // AddToClassList(".mobile__full");

        }
    }
}
