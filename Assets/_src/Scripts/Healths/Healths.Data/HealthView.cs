using BovineLabs.Anchor;
using Unity.AppUI.UI;
using UnityEngine.UIElements;

namespace _src.Scripts.Healths.Healths.Data
{
    [IsService]
    public class HealthView : View<HealthViewModel>
    {
        public const string UssClassName = "health-view";

        public HealthView(HealthViewModel viewModel) : base(viewModel)
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
            healthBar.SetBindingToUI(nameof(ProgressBar.value), nameof(HealthViewModel.CurrentHealth));
            healthBar.SetBindingToUI(nameof(ProgressBar.value), nameof(HealthViewModel.MaxHealth));
            
            var healthTextValue = new Text
            {
                name = "health-text-value",
                dataSource = ViewModel
            };
            healthTextValue.SetBindingToUI(nameof(healthTextValue.text), nameof(HealthViewModel.CurrentHealth));
            healthBar.Add(healthTextValue);
            Add(healthBar);
        }
    }
}