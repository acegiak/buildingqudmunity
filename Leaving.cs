using System;
using XRL.World.Parts;

namespace XRL.World.Effects
{
	[Serializable]
	public class acegiak_Leaving : Effect
	{
		public acegiak_Leaving()
		{
			base.DisplayName = "&RLeaving";
		}

		public acegiak_Leaving(int _Duration)
			: this()
		{
			Duration = _Duration;
		}

		public override string GetDetails()
		{
			return "Off to find new horizons.";
		}

		public override bool CanApplyToStack()
		{
			return true;
		}

		public override bool Apply(GameObject Object)
		{
			return true;
		}

		public override void Register(GameObject Object)
		{
            Object.RegisterEffectEvent(this, "EndTurn");

			base.Register(Object);
		}

		public override void Unregister(GameObject Object)
		{
            Object.UnregisterEffectEvent(this, "EndTurn");

			base.Unregister(Object);
		}

		public override bool FireEvent(Event E)
		{
            if(E.ID == "EndTurn"){
                Cell c = Object.pPhysics.CurrentCell;
                if(c.X == 1 || c.Y ==1 || c.X == c.ParentZone.Width-1 || c.Y == c.ParentZone.Height-1){
                    Object.Destroy("Left town",true,true);
                }
            }
			return true;
		}
	}
}
