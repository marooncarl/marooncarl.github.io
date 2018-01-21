/// <summary>
/// Health.
/// 
/// Programmer: Carl Childers
/// Date: 4/27/2014
/// 
/// Keeps track of health and sends death messages.
/// </summary>

using UnityEngine;
using System.Collections;

public class Damage {
	public float Amount;
	public Defense.EDamageType Type;
	public bool IsDamageOverTime;
	
	public Damage(float inAmt, Defense.EDamageType inType, bool inDOT) {
		Amount = inAmt;
		Type = inType;
		IsDamageOverTime = inDOT;
	}
}

public class Health : MonoBehaviour
{
	public float MaxHealth;
	public bool UseInvulnerablePeriod = false;
	public bool UseInvulnOnlyForDOT = true; // if true, invulnerable period only happens for damage over time
	public float InvulnerableTime = 1.0f;

	float CurrentHealth;
	bool IsAlive;
	float InvulnCounter;


	void Awake()
	{
		CurrentHealth = MaxHealth;
		IsAlive = true;
		InvulnCounter = 0;
	}

	void Update()
	{
		InvulnCounter = Mathf.Max(InvulnCounter - Time.deltaTime, 0);
	}

	public float GetHealth()
	{
		return CurrentHealth;
	}

	// Takes in a struct containing damage amount and damage type
	// since it is called using SendMessage() and can thus only have one argument.
	// For damage over time, the damage per second should be given without multiplying by delta time,
	// and with the property "IsDamageOverTime" set to true
	public void TakeDamage(Damage inDamage) {
		if (!enabled) {
			return;
		}
		if (inDamage.Amount <= 0)
			return;
		if (IsInvulnerable())
			return;

		float finalDamage = inDamage.Amount;
		if (inDamage.IsDamageOverTime) {
			if (UseInvulnerablePeriod) {
				finalDamage *= InvulnerableTime;
			} else {
				finalDamage *= Time.deltaTime;
			}
		}

		Defense myDef = GetComponent<Defense>();
		if (myDef != null) {
			finalDamage = myDef.GetModifiedDamage(finalDamage, inDamage.Type);
		}

		// If defense made damage negative, heal from the damage
		if (finalDamage < 0) {
			Heal(Mathf.Abs(finalDamage));
			return;
		}
		if (finalDamage == 0) {
			return;
		}

		CurrentHealth -= finalDamage;
		if (CurrentHealth <= 0 && IsAlive) {
			SendMessage("Died", inDamage.Type, SendMessageOptions.DontRequireReceiver);
			IsAlive = false;
		}

		if (UseInvulnerablePeriod && (inDamage.IsDamageOverTime || !UseInvulnOnlyForDOT)) {
			InvulnCounter = InvulnerableTime;
		}

		SendMessage("PlayDamageEffects", inDamage, SendMessageOptions.DontRequireReceiver);
	}

	public void Heal(float amt)
	{
		if (!enabled) {
			return;
		}
		if (amt <= 0 || !IsAlive)
			return;
		CurrentHealth = Mathf.Min (CurrentHealth + amt, MaxHealth);
	}

	public void Respawn()
	{
		CurrentHealth = MaxHealth;
		IsAlive = true;
	}

	public bool IsInvulnerable()
	{
		return (UseInvulnerablePeriod && InvulnCounter > 0);
	}

	void Disable() {
		enabled = false;
	}
}
