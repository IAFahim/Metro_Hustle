// <copyright file="DistortAppBuilder.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

using _src.Scripts.UiServices.Missions.Service;
using _src.Scripts.UiServices.Sprites.Service;
using _src.Scripts.UiServices.UXMLs.Service;

namespace BovineLabs.Sample.UI
{
    using BovineLabs.Anchor;
    using BovineLabs.Sample.UI.Services;
    using BovineLabs.Sample.UI.ViewModels.Option;
    using Unity.AppUI.MVVM;

    public class BovineLabsAppBuilder : AnchorAppBuilder
    {
        /// <inheritdoc/>
        protected override void OnConfiguringApp(AppBuilder builder)
        {
            base.OnConfiguringApp(builder);

            builder.services.AddSingleton<IControlService, ControlService>();
            builder.services.AddSingleton<IUxmlService, UxmlService>();
            builder.services.AddSingleton<ISpriteService, SpriteService>();
            builder.services.AddSingleton<IMissionsService, MissionsService>();
        }

        /// <inheritdoc/>
        protected override void OnAppInitialized(AnchorApp anchor)
        {
            base.OnAppInitialized(anchor);

            // Load all our options
            anchor.services.GetRequiredService<OptionsAudioViewModel>().Load();
            anchor.services.GetRequiredService<OptionsGameplayViewModel>().Load();
            anchor.services.GetRequiredService<OptionsGraphicsViewModel>().Load();
            
        }
    }
}
