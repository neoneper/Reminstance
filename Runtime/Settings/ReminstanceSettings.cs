
using GameCreator.Runtime.Common;

namespace GameCreator.Runtime.Reminstance
{
    public class ReminstanceSettings : AssetRepository<ReminstanceRepository>
    {
        public override IIcon Icon => new IconComputer(ColorTheme.Type.TextLight);
        public override string Name => "Reminstance";
        
    }
}