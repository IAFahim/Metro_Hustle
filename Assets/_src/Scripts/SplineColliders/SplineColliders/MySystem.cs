using BovineLabs.Core.SingletonCollection;
using Unity.Burst;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Entities;
using Unity.Jobs;

public partial struct MySystem : ISystem
{
    private SingletonCollectionUtil<Singleton, NativeList<int>> singletonCollectionUtil;

    // Note this can't be burst compiled due to RewindAllocator creation
    public void OnCreate(ref SystemState state)
    {
        this.singletonCollectionUtil = new SingletonCollectionUtil<Singleton, NativeList<int>>(ref state);
    }

    // Note this can't be burst compiled due to RewindAllocator disposable
    public void OnDestroy(ref SystemState state)
    {
        this.singletonCollectionUtil.Dispose();
    }

    [BurstCompile]
    public unsafe void OnUpdate(ref SystemState state)
    {
        var lists = this.singletonCollectionUtil.Containers;

        for (var i = 0; i < lists.Length; i++)
        {
            state.Dependency = new Job { List = lists.Ptr[i] }.Schedule(state.Dependency);
        }

        this.singletonCollectionUtil.ClearRewind();
    }

    // Define the singleton
    public struct Singleton : ISingletonCollection<NativeList<int>>
    {
        /// <inheritdoc/>
        unsafe UnsafeList<NativeList<int>>* ISingletonCollection<NativeList<int>>.Collections { get; set; }

        /// <inheritdoc/>
        Allocator ISingletonCollection<NativeList<int>>.Allocator { get; set; }
    }

    [BurstCompile]
    private struct Job : IJob
    {
        public NativeList<int> List;

        public void Execute()
        {
            // Do something
        }
    }
}