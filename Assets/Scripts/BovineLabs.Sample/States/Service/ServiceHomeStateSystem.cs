// // <copyright file="ServiceHomeStateSystem.cs" company="BovineLabs">
// //     Copyright (c) BovineLabs. All rights reserved.
// // </copyright>
//
// using BovineLabs.Core;
//
// namespace BovineLabs.Sample.States.Service
// {
//     using BovineLabs.Anchor;
//     using BovineLabs.Core.States;
//     using BovineLabs.Sample.UI.ViewModels.Menu;
//     using Unity.AppUI.MVVM;
//     using Unity.Burst;
//     using Unity.Entities;
//
//     public partial struct ServiceHomeStateSystem : ISystem, ISystemStartStop
//     {
//         private UIHelper<HomeViewModel, HomeViewModel.Data> helper;
//
//         /// <inheritdoc/>
//         [BurstCompile]
//         public void OnCreate(ref SystemState state)
//         {
//         }
//
//         /// <inheritdoc/>
//         public void OnStartRunning(ref SystemState state)
//         {
//             // Allow the splash screen to close if it's still visible
//             // App.current.services.GetRequiredService<SplashViewModel>().IsInitialized = true;
//
//             // this.helper.Bind();
//         }
//
//         /// <inheritdoc/>
//         public void OnStopRunning(ref SystemState state)
//         {
//             // this.helper.Unbind();
//         }
//
//         [BurstCompile]
//         public void OnUpdate(ref SystemState state)
//         {
//             // ref var binding = ref this.helper.Binding;
//             // var bl = SystemAPI.GetSingleton<BLDebug>();
//             // if (binding.Play.TryConsume())
//             // {
//                 // bl.Debug("Play OnUpdate");
//             // }
//         }
//
//         private struct StateHome : IComponentData
//         {
//         }
//     }
// }
