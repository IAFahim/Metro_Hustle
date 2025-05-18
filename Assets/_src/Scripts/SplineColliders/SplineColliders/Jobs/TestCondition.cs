using _src.Scripts.SplineColliders.SplineColliders.Data;
using BovineLabs.Reaction.Conditions;
using BovineLabs.Reaction.Data.Conditions;
using Unity.Entities;
using UnityEngine;

namespace _src.Scripts.SplineColliders.SplineColliders.Jobs
{
    [WithAll(typeof(SplineMainColliderTag))]
    public partial struct TestCondition : IJobEntity
    {
        public ConditionEventWriter.Lookup Lookup;
        public void Execute(Entity entity)
        {
            Debug.Log("I RAN");
            var conditionKey = new ConditionKey
            {
                Value = 2
            };
            Lookup[entity].Trigger(conditionKey, 10);
        }
    }
}