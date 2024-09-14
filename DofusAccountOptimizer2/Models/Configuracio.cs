using System;
using System.Collections.Generic;

namespace DofusAccountOptimizer2.Models
{
    public partial class Configuracio
    {
        public long Id { get; set; }
        public long UpdateIcons { get; set; }
        public string KeyCodes { get; set; } = null!;
        public long OrderWindows { get; set; }
        public string Language { get; set; } = null!;
        public long? LastCompositionId { get; set; }
        public long EnableMouse { get; set; }
        public long EnableKeyboard { get; set; }
    }
}
