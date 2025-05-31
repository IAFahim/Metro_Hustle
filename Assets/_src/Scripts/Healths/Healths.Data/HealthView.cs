using BovineLabs.Anchor;
using Unity.AppUI.UI;
using UnityEngine;
using UnityEngine.UIElements;

namespace _src.Scripts.Healths.Healths.Data
{
    [IsService]
    public class HealthView : View<GameModel>
    {
        public const string UssClassName = "health-view";

        public HealthView(GameModel viewModel) : base(viewModel)
        {
            this.AddToClassList(UssClassName);
            this.style.flexDirection = FlexDirection.Row;

            var healthLabel = new Text
            {
                name = "health-label",
                text = "Health: "
            };
            Add(healthLabel);

            
            var healthBar = new ProgressBar
            {
                name = "health-bar", lowValue = 0, highValue = 1,
                style =
                {
                    minWidth = 100,
                    height = 15
                },
                dataSource = ViewModel
            };
            healthLabel.SetBindingToUI(nameof(ProgressBar.title), nameof(GameModel.CurrentHealth));
            healthBar.SetBindingToUI(nameof(ProgressBar.value), nameof(GameModel.HealthNormalized));
            Add(healthBar);
            
            
            var distanceLabel = new Text
            {
                name = "distance-label",
                text = "Distance:"
            };
            Add(distanceLabel);
            
            var distance = new Text
            {
                name = "distance",
                text = "",
                dataSource = ViewModel
            };
            distance.SetBindingToUI(nameof(Text.text), nameof(GameModel.TotalDistance));
            
            Add(distance);
            
            
        }
    }
}