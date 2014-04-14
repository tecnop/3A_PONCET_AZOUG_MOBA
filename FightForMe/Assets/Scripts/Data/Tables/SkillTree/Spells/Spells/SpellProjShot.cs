using UnityEngine;
using System.Collections;

public class SpellProjShot : SpellProj
{
	private uint projID;
	private uint amount;
	private uint impactSpellOverride;
	private bool doThrow;

	public SpellProjShot(Metadata metadata, uint projID, uint amount, uint impactSpellOverride = 0, bool doThrow = false, SpellCostType costType = SpellCostType.NONE, float spellCost = 0.0f)
		: base(metadata, costType, spellCost)
	{
		this.projID = projID;
		this.amount = amount;
		this.impactSpellOverride = impactSpellOverride;
		this.doThrow = doThrow;
	}

	protected override void _Execute(CharacterManager inflictor, Vector3 position, CharacterManager target)
	{
		ProjectileScript proj = inflictor.GetCombatScript().CreateProjectile(projID, impactSpellOverride);
		if (doThrow)
		{
			proj.ThrowAt(position);
		}
	}

	public override bool CastingCondition(CharacterManager caster)
	{ // TODO: Bow only
		return true;
	}
}