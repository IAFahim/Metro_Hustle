// <copyright file="HomeView.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

using System.Collections.Generic;
using _src.Scripts.Missions.Missions.Data;
using _src.Scripts.UiServices.Missions.Service;
using _src.Scripts.UiServices.UXMLs.Service;
using BovineLabs.Core;
using UnityEngine;
using UnityEngine.AddressableAssets;
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
        private MissionsSettings _missionsSettings;
        private readonly List<AsyncOperationHandle> _assetReferences;

        public HomeView(HomeViewModel viewModel, IUxmlService uxmlService, IMissionsService missionsService)
            : base(viewModel)
        {
            _assetReferences = new List<AsyncOperationHandle>();
            _uxmlService = uxmlService;
            var assetReferenceMissions = missionsService.GetCurrent();
            var asyncOperationHandle = assetReferenceMissions.LoadAssetAsync();
            _assetReferences.Add(asyncOperationHandle);
            asyncOperationHandle.Completed += OnMissionLoadComplete;
        }

        private void OnMissionLoadComplete(AsyncOperationHandle<MissionsSettings> obj)
        {
            _missionsSettings = obj.Result;
            var root = _uxmlService.GetAsset("Mobile").Instantiate().ElementAt(0);
            Add(root);
            AddToClassList(".mobile__full");
            var missionTemplate = _uxmlService.GetAsset("Mission");
            var screen = root.Q<VisualElement>("Screen");
            (MissionSchema mission, int missionNumber) = _missionsSettings.GetCurrent();
            var missionTemplateContainer = InstantiateMission(missionTemplate, mission, missionNumber);
            screen.Add(missionTemplateContainer);

            var button = root.Q<ActionButton>("PlayButton");
            button.RegisterCallback<ClickEvent>(_ => Play());
        }

        private TemplateContainer InstantiateMission(
            VisualTreeAsset missionTemplate,
            MissionSchema missionSchema,
            int missionNumber
        )
        {
            TemplateContainer container = missionTemplate.Instantiate();
            Format(container, "mission_text_number", missionNumber);
            Format(container, "label_title", missionSchema.title);
            FormatText(container, "text_description", missionSchema.description);
            Format(container, "label_destination", missionSchema.startStation, missionSchema.endStation);
            Format(container, "label_time", missionSchema.time);
            Format(container, "label_distance", missionSchema.distance);
            Format(container, "label_reward", missionSchema.money);
            var asyncOperationHandle = missionSchema.item.LoadAssetAsync();
            _assetReferences.Add(asyncOperationHandle);
            asyncOperationHandle.Completed += obj => OnItemLoadComplete(container, obj);
            return container;
        }

        private void OnItemLoadComplete(TemplateContainer container, AsyncOperationHandle<Item> obj)
        {
            var item = obj.Result;
            FormatText(container, "text_item", item.title);
            var asyncOperationHandle = item.spriteAsset.LoadAssetAsync();
            _assetReferences.Add(asyncOperationHandle);
            asyncOperationHandle.Completed += handle => SetSprite(container, handle);
        }

        private static void SetSprite(TemplateContainer container, AsyncOperationHandle<Sprite> obj)
        {
            container.Q<Icon>("item_icon").image = obj.Result.texture;
        }

        private static void Format(TemplateContainer container, string name, object args)
        {
            var labelElement = GetLabelAndOutFormat(container, name, out var format);
            labelElement.text = string.Format(format, args);
        }

        private static void Format(TemplateContainer container, string name, params object[] args)
        {
            var labelElement = GetLabelAndOutFormat(container, name, out var format);
            labelElement.text = string.Format(format, args);
        }

        private static Label GetLabelAndOutFormat(TemplateContainer container, string name, out string format)
        {
            var labelElement = container.Q<Label>(name);
            format = labelElement.text;
            return labelElement;
        }

        private static void FormatText(TemplateContainer container, string name, object args)
        {
            var labelElement = GetTextAndOutFormat(container, name, out var format);
            labelElement.text = string.Format(format, args);
        }
        
        private static Text GetTextAndOutFormat(TemplateContainer container, string name, out string format)
        {
            var labelElement = container.Q<Text>(name);
            format = labelElement.text;
            return labelElement;
        }

        protected override void OnExit(NavController controller, NavDestination destination, Argument[] args)
        {
            base.OnExit(controller, destination, args);
            foreach (var asyncOperationHandle in _assetReferences) asyncOperationHandle.Release();
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