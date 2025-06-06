// <copyright file="HomeView.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

using _src.Scripts.Missions.Missions.Data;
using _src.Scripts.UiServices.Missions.Service;
using _src.Scripts.UiServices.UXMLs.Service;
using BovineLabs.Core;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace BovineLabs.Sample.UI.Views.Menu
{
    using BovineLabs.Anchor;
    using BovineLabs.Sample.UI.ViewModels.Menu;
    using Unity.AppUI.Navigation;
    using Unity.AppUI.UI;
    using UnityEngine.UIElements;

    public class HomeView : MenuBaseView<HomeViewModel>
    {
        public const string UssHomeClassName = "bl-home-view";
        public const string LeftClassName = UssHomeClassName + "__left";
        public const string RightClassName = UssHomeClassName + "__right";

        public const string ButtonClassName = UssHomeClassName + "__button";

        private const string ContinueText = "@UI:continue";
        private const string PlayText = "@UI:play";
        private const string OptionsText = "@UI:options";
        private const string QuitText = "@UI:quit";
        private const string QuitTitleText = "@UI:quitTitle";
        private const string QuitDescriptionText = "@UI:quitDescription";
        private const string QuitCancelText = "@UI:quitCancel";

        private readonly IUxmlService _uxmlService;
        private readonly IMissionsService _missionsService;

        public HomeView(HomeViewModel viewModel, IUxmlService uxmlService, IMissionsService missionsService)
            : base(viewModel)
        {
            _uxmlService = uxmlService;
            _missionsService = missionsService;
            var assetReferenceMissions = missionsService.GetCurrent();
            var asyncOperationHandle = assetReferenceMissions.LoadAssetAsync();
            asyncOperationHandle.Completed += OnMissionLoadComplete;
        }

        private void OnMissionLoadComplete(AsyncOperationHandle<MissionsSettings> obj)
        {
            var root = _uxmlService.GetAsset("Mobile").Instantiate().ElementAt(0);
            Add(root);
            AddToClassList(".mobile__full");
            var missionTemplate = _uxmlService.GetAsset("Mission");
            var screen = root.Q<VisualElement>("Screen");

            var missionTemplateContainer = InstantiateMission(missionTemplate);
            screen.Add(missionTemplateContainer);
            
            var button = root.Q<ActionButton>("PlayButton");
            button.RegisterCallback<ClickEvent>(_ => Play());
        }

        private static TemplateContainer InstantiateMission(VisualTreeAsset missionTemplate)
        {
            var missionTemplateContainer = missionTemplate.Instantiate();
            var missionNumberLabel = missionTemplateContainer.Q<Label>("mission_text_number");
            var missionNumberLabelFormat = missionNumberLabel.text;
            missionNumberLabel.text = string.Format(missionNumberLabelFormat, 1);
            return missionTemplateContainer;
        }

#if UNITY_STANDALONE
        private static void Quit(EventBase evt)
        {
            if (evt.target is ExVisualElement btn)
            {
                var dialog = new AlertDialog
                {
                    description = QuitDescriptionText,
                    variant = AlertSemantic.Destructive,
                    title = QuitTitleText,
                };

                dialog.SetPrimaryAction(99, QuitText, () =>
                {
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
                    UnityEngine.Application.Quit();
#endif
                });

                dialog.SetCancelAction(1, QuitCancelText);

                var modal = Modal.Build(btn, dialog);
                modal.Show();
            }
        }

#endif


        private void Play()
        {
            BovineLabsBootstrap.Instance.CreateGameWorld();
            ToGoToGame();
        }

        private void ToGoToGame()
        {
            this.Navigate(Actions.go_to_game);
        }

        private void Load()
        {
        }

        private void Options()
        {
            this.Navigate(Actions.go_to_options);
        }
    }
}