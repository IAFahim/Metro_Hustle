// <copyright file="BaseScreen.cs" company="BovineLabs">
//     Copyright (c) BovineLabs. All rights reserved.
// </copyright>

using System.Linq;
using UnityEngine;

namespace BovineLabs.Sample.UI.Views
{
    using BovineLabs.Anchor;
    using Unity.AppUI.Navigation;
    using UnityEngine.UIElements;

    [IsService]
    public abstract class BaseScreen<T> : NavigationScreen
    {
        public const string UssMenuClassName = "bl-screen";

        protected BaseScreen(T viewModel)
        {
            this.AddToClassList(UssMenuClassName);
            this.ViewModel = viewModel;
            this.contentContainer.StretchToParentSize();

            this.pickingMode = PickingMode.Ignore;
            this.contentContainer.pickingMode = PickingMode.Ignore;
            this.scrollView.pickingMode = PickingMode.Ignore;
            this.scrollView.Q<VisualElement>(className: ScrollView.contentAndVerticalScrollUssClassName).pickingMode = PickingMode.Ignore;
        }

        public T ViewModel { get; }

        // Just sealing to remove warnings of virtual
        public sealed override VisualElement contentContainer => this.scrollView.contentContainer;

        protected override void OnEnter(NavController controller, NavDestination destination, Argument[] args)
        {
            base.OnEnter(controller, destination, args);

            Debug.Log($"{destination.name} {string.Join(",", args.Select(a => $"{a.name}:{a.value}"))}");
        }
    }
}
