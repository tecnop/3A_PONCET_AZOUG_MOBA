using UnityEngine;
using System.Collections;

public abstract class Ability : WikiEntry
{
	public Ability(Metadata metadata)
		: base(metadata)
	{

	}

	public abstract void Execute();
}
