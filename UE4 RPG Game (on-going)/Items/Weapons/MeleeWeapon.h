// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Items/Weapons/BaseWeapon.h"
#include "MeleeWeapon.generated.h"

/**
 * 
 */
UCLASS()
class THIRDPERSONRPG_API AMeleeWeapon : public ABaseWeapon
{
	GENERATED_BODY()
	
private:
	// Weapon/Rarity of weapon; provides combat bonuses
	UPROPERTY(EditAnywhere, Category = "Melee Weapon")
		int32 EnchantmentLevel;

	// Damage multiplier on chage attack
	UPROPERTY(EditAnywhere, Category = "Melee Weapon")
		float ChargeMultiplier; 

	// Additional effects on successful hit; depends on enchantment level
	UFUNCTION(BlueprintCallable, Category = "Melee Weapon")
		void EmitShockwave();

public:
	UFUNCTION(BlueprintCallable, Category = "Base Weapon")
		void TraceAttack() override;

protected:
	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Melee Weapon")
		float AttackRadius;
	
	
};
