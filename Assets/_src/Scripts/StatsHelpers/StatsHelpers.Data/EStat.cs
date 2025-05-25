using System;

namespace _src.Scripts.StatsHelpers.StatsHelpers.Data
{
    public enum EStat : byte
    {
        MaxHealth = 0,
        MoveSpeed = 1,
        Strength = 2,
        Money = 3,
        MetroCoin = 4,
        TrainFront = 5,
        Fall = 6,
        Slide = 7,
        Jump = 8,
    }
    
    public enum EIntrinsic : byte
    {
        MaxHealth = 0,
        MoveSpeed = 1,
        Strength = 2,
        Money = 3,
        MetroCoin = 4,
        Train = 5,
        Fall = 6,
        Slide = 7,
        Jump = 8,
    }

    public static class EStatExt
    {
        public static string GetGuid(this EStat stat)
        {
            return stat switch
            {
                EStat.MaxHealth => "f2a5bb79645d8574b93210ebb308facd",
                EStat.MoveSpeed => "594b6d0f6224dd0419b6b6924d2961f4",
                EStat.Strength => "52f42f8ea006b604eb7a14cb260e86e1",
                EStat.Money => "1c491e6e7e1566e47b937a7392c8ec0e",
                EStat.MetroCoin => "e2b2dd5551bc4b7459f9fa48b92202f5",
                EStat.TrainFront => "2ac3c31d19138d445a95f0be3099ccd5",
                EStat.Fall => "cdaa5be8a103f7e4e9c4eadd5d19aca0",
                EStat.Slide => "f1c6f7fd49df05e4eb7388a1d4a730d6",
                EStat.Jump => "75f985a8fc011a6449a78ca4b08bf085",
                _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
            };
        }

        public static string ToName(this EStat stat)
        {
            return stat switch
            {
                EStat.MaxHealth => "Max health",
                EStat.MoveSpeed => "Move Speed",
                EStat.Strength => "Strength",
                EStat.Money => "Money",
                EStat.MetroCoin => "Metro Coin",
                EStat.TrainFront => "Train Front",
                EStat.Fall => "Fall",
                EStat.Slide => "Slide",
                EStat.Jump => "Jump",
                _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, null)
            };
        }
    }
}