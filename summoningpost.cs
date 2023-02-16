using System;
using XRL.Core;
using XRL.Rules;
using Qud.API;
using System.Linq;
using XRL.UI;
using XRL.World.Effects;
using XRL.World;

namespace XRL.World.Parts
{
	[Serializable]
	public class acegiak_SummoningPost : IPart
	{
		public int NumberToSpawn = 30;

		public int ChancePerSpawn = 100;

		public string TurnsPerSpawn = "15-20";

		public string NumberSpawned = "1";

		public string SpawnMessage = "A giant centipede crawls out of the nest.";

		public string CollapseMessage = "The nest collapses.";

		public string SpawnParticle = "&w.";

		public string BlueprintSpawned = "BaseMerchant";

		public int SpawnCooldown = int.MinValue;

		public override void Register(GameObject Object)
		{
			Object.RegisterPartEvent(this, "EndTurn");
			Object.RegisterPartEvent(this, "GetInventoryActions");
			Object.RegisterPartEvent(this, "InvCommandDismissMerchant");
			base.Register(Object);
		}

		public override bool FireEvent(Event E)
		{
			if (E.ID == "EndTurn")
			{

				if (SpawnCooldown == int.MinValue)
				{
					SpawnCooldown = Stat.Roll(TurnsPerSpawn);
				}
				if (ParentObject.pPhysics.CurrentCell != null)
				{
					SpawnCooldown--;
					if (SpawnCooldown <= 0)
					{
						if(ParentObject.pPhysics.CurrentCell.ParentZone.CountObjects((GameObject o) => o.GetBlueprint().InheritsFrom("BaseMerchant") && o.pBrain.StartingCell.ResolveCell().DistanceTo(ParentObject) < 1)>0){
							//IPart.AddPlayerMessage("Already Someone Here");
						}else{
							Cell randomLocalAdjacentCell = ParentObject.pPhysics.CurrentCell.ParentZone.GetEmptyCellsShuffled().FirstOrDefault(c=> c.IsReachable() && (c.X == 1 || c.Y ==1 || c.X == c.ParentZone.Width-1 || c.Y == c.ParentZone.Height-1));
							if (randomLocalAdjacentCell != null && randomLocalAdjacentCell.IsEmpty() && Stat.Random(1, 100) <= ChancePerSpawn)
							{

								GameObject gameObject = GameObject.create(EncountersAPI.GetARandomDescendentOf(BlueprintSpawned));
								randomLocalAdjacentCell.AddObject(gameObject);
								if (gameObject.pBrain != null)
								{
									gameObject.pBrain.SetFeeling(ParentObject, 200);
									gameObject.pBrain.TakeOnAttitudesOf(ParentObject);
									XRLCore.Core.Game.ActionManager.AddActiveObject(gameObject);
									if(gameObject.pBrain != null){
										gameObject.pBrain.Stay(ParentObject.CurrentCell);
									}else{
										//IPart.AddPlayerMessage("they're dumb");
									}
								}
								if (Visible())
								{
									IPart.AddPlayerMessage(SpawnMessage);
								}
								SpawnCooldown = Stat.Roll(TurnsPerSpawn);
							}
							
						}
					}
				}
				
			}
			if (E.ID == "GetInventoryActions")
			{
				if(ParentObject.pPhysics.CurrentCell.ParentZone.CountObjects((GameObject o) => o.GetBlueprint().InheritsFrom("BaseMerchant") && o.pBrain.StartingCell.ResolveCell().DistanceTo(ParentObject) < 1)>0){
					E.GetParameter<EventParameterGetInventoryActions>("Actions").AddAction("DismissMerchant", 'D', false, "&WD&yismiss merchant", "InvCommandDismissMerchant");
				}
			}
			if (E.ID == "InvCommandDismissMerchant")
			{
				GameObject GO = ParentObject.pPhysics.CurrentCell.ParentZone.GetObjects().Where((GameObject o) => o.GetBlueprint().InheritsFrom("BaseMerchant") && o.pBrain.StartingCell.ResolveCell().DistanceTo(ParentObject) < 1).FirstOrDefault<GameObject>();
				if(GO != null){
					Cell randomLocalAdjacentCell = ParentObject.pPhysics.CurrentCell.ParentZone.GetEmptyCellsShuffled().FirstOrDefault(c=> c.IsReachable() && (c.X == 1 || c.Y ==1 || c.X == c.ParentZone.Width-1 || c.Y == c.ParentZone.Height-1));
					GO.pBrain.Stay(randomLocalAdjacentCell);
					GO.ApplyEffect(new acegiak_Leaving(1000));
					IPart.XDidYToZ(E.GetGameObjectParameter("Owner"), "dismiss", GO);

					Faction.PlayerReputation.modify(GO.pBrain.GetPrimaryFaction(),-5,true);
					E.RequestInterfaceExit();
				}
			}

			return base.FireEvent(E);
		}
	}
}
