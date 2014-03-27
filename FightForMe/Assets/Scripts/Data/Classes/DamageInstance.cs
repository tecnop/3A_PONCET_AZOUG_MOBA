using UnityEngine;
using System.Collections;

public class DamageInstance
{ // Basically an "AppliedSpell", used for combat logging
	private CharacterManager inflictor;		// If null, damage was inflicted by the environment (I guess)
	private CharacterManager target;		// If null, the damage has not been applied yet (I don't think this really happens anymore)

	private Spell spell;					// Spell that dealt the damage

	public DamageInstance(CharacterManager inflictor, CharacterManager target, Spell spell)
	{
		this.inflictor = inflictor;
		this.target = target;
		this.spell = spell;
	}

	public override string ToString()
	{
		string inflictorName = (this.inflictor != null) ? this.inflictor.name : "<world>";
		return inflictorName + " hit  " + this.target.name + " using " + this.spell.GetName();
	}
}
