// <copyright file="OptionsGraphicsViewModel.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

namespace BovineLabs.Sample.UI.ViewModels.Option
{
    using BovineLabs.Sample.UI.ViewModels.Option.Options.Graphics;

    public class OptionsGraphicsViewModel : OptionTabViewModel
    {
        public readonly ResolutionOption Resolution;

        public OptionsGraphicsViewModel()
        {
            this.CreateOption(out this.Resolution);
        }
    }
}
