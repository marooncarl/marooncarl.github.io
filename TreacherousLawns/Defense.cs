/// <summary>
/// Defense
/// 
/// Programmer: Carl Childers
/// Date: 7/11/2015
/// 
/// Can modify incoming damage, depending on damage type and the defense script's
/// resistance to that damage type
/// </summary>

using UnityEngine;
using System.Collections;

public class Defense : MonoBehaviour {

	public enum EDamageType
	{
		DT_Normal,
		DT_Fire,
		DT_Poison,
		DT_Water,
		DT_Wind,
		DT_Lightning
	};

	public float DmgMult_Normal = 1.0f;
	public float DmgMult_Fire = 1.0f;
	public float DmgMult_Poison = 1.0f;
	public float DmgMult_Water = 1.0f;
	public float DmgMult_Wind = 1.0f;
	public float DmgMult_Lightning = 1.0f;

	// Resistance: 1.0 means normal damage, 2.0 means double damage (weakness), 0.0 means immunity, -1.0 means absorption (heal from damage)
	float[] Resistance /*= new float[] {1.0f, 1.0f, 1.0f, 1.0f, 1.0f}*/;


	void Awake() {
		Resistance = new float[6];
		Resistance[0] = DmgMult_Normal;
		Resistance[1] = DmgMult_Fire;
		Resistance[2] = DmgMult_Poison;
		Resistance[3] = DmgMult_Water;
		Resistance[4] = DmgMult_Wind;
		Resistance[5] = DmgMult_Lightning;
	}

	public float GetModifiedDamage(float incomingDamage, EDamageType inDamageType) {
		byte dmgTypeByte = (byte)inDamageType;
		if (dmgTypeByte >= Resistance.Length) {
			return incomingDamage;
		}
		return (incomingDamage * Resistance[dmgTypeByte]);
	}
}
