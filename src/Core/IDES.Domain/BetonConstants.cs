using System.Collections.Generic;

namespace IDES.Domain
{
    public static class BetonConstants
    {
        public static List<string> ClassesResistance { get; } = new List<string>
        {
            "C12/15",
            "C16/20",
            "C20/25",
            "C25/30",
            "C30/37",
            "C35/45",
            "C40/50",
            "C45/55",
            "C50/60",
            "C55/67",
            "C60/75",
            "C70/85",
            "C80/95",
            "C90/105"
        };
    }
}
