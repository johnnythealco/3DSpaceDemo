using UnityEngine;
using System.Collections;

[System.Serializable]
public class UnitState :  System.Object
{
	public string dsiplayName;
	public string shipClass;

	public float hull;
	public float weaponPower;
	public float shieldPower;
	public float enginePower;
	public float evasion;
	public float accuracy;

	public float currentHull{ get; set;}
	public float currentWeaponPower{ get; set;}
	public float currentShieldPower{ get; set;}
	public float currentEnginePower{ get; set;}
	public float currentEvasion{ get; set;}
	public float currentAccuracy{ get; set;}

	#region Percentage Values
	public float heathPercentage()
	{
		return (currentHull / hull) * 100;
	}
	public float weaponPowerPercentage()
	{
		return (currentWeaponPower / weaponPower) * 100;
	}
	public float shieldPowerPercentage()
	{
		return (currentShieldPower / shieldPower) * 100;
	}
	public float enginePowerPercentage()
	{
		return (currentEnginePower / enginePower) * 100;
	}
	#endregion

	public void Reset()
	{
		currentHull = hull;
		currentWeaponPower = weaponPower;
		currentShieldPower = shieldPower;
		currentEnginePower = enginePower;
		currentEvasion = evasion;
		currentAccuracy = accuracy;
	}

	#region Buffs &  DeBuffs
	public void takeHullDamage (float dmg)
	{
		currentHull = currentHull - dmg;
	}

	public void takeWeaponDamage(float dmg)
	{
		currentWeaponPower = currentWeaponPower - dmg;
	}

	public void takeShieldDamage(float dmg)
	{
		currentShieldPower = currentShieldPower - dmg;
	}

	public void takeEngineDamage(float dmg)
	{
		currentEnginePower = currentEnginePower - dmg;
	}

	public void lowerEvasion(float dmg)
	{
		currentEvasion = currentEvasion - dmg;
	}

	public void lowerAccuracy(float dmg)
	{
		currentAccuracy = currentAccuracy - dmg;
	}

	public void repairHull (float f)
	{
		currentHull = currentHull + f;
	}

	public void increaseWeaponPower(float f)
	{
		currentWeaponPower = currentWeaponPower + f;
	}

	public void increaseShieldPower(float f)
	{
		currentShieldPower = currentShieldPower + f;
	}

	public void increaseEnginePower(float f)
	{
		currentEnginePower = currentEnginePower + f;
	}

	public void raiseEvasion(float f)
	{
		currentEvasion = currentEvasion + f;
	}

	public void raiseAccuracy(float f)
	{
		currentAccuracy = currentAccuracy + f;
	}
	#endregion



}
