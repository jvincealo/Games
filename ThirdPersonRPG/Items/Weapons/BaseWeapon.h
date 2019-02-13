// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Items/Item.h"
#include "BaseWeapon.generated.h"

/**
 * 
 */
UCLASS()
class THIRDPERSONRPG_API ABaseWeapon : public AItem
{
	GENERATED_BODY()

protected:
	// Gun skeletal mesh
	UPROPERTY(VisibleDefaultsOnly, Category = Mesh)
		class USkeletalMeshComponent* WeaponSkeletalMesh;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Weapon")
		UParticleSystem* HitEffect;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Weapon")
		float WeaponRange;
	
public:
	ABaseWeapon();

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Base Weapon")
		float Damage;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Base Weapon")
		float AttackSpeed;

	UFUNCTION(BlueprintCallable, Category = "Base Weapon")
		virtual void TraceAttack();

public:
	UFUNCTION(BlueprintCallable, Category = Mesh)
	FORCEINLINE class USkeletalMeshComponent* GetMesh() const { return WeaponSkeletalMesh; }
	
};
