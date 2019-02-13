// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Items/Weapons/BaseWeapon.h"
#include "RangedWeapon.generated.h"

/**
 * 
 */
UCLASS()
class THIRDPERSONRPG_API ARangedWeapon : public ABaseWeapon
{
	GENERATED_BODY()
	
private:

	// Firing mode; if automatic, hits will be dependent on line traces, if not, spawn projectiles for hit detection
	UPROPERTY(EditAnywhere, Category = "Ranged Weapon")
		bool bAutomatic;

	UPROPERTY(EditAnywhere, Category = "Ranged Weapon")
		int32 AmmoCapacity;

	// Bullet spawn point
	UPROPERTY(EditAnywhere, Category = Mesh)
		class USceneComponent* GunMuzzle;

public:
	ARangedWeapon();

	FVector TraceEnd;

	UPROPERTY(EditAnywhere, Category = "Ranged Weapon")
		float ZoomMultiplier;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Ranged Weapon")
		TSubclassOf<class ABaseProjectile> WeaponProjectile;

	// Trace projectile path from Player's camera to get exact end point
	UFUNCTION(BlueprintCallable, Category = "Ranged Weapon")
		void TraceProjectilePath(class UCameraComponent* p_camera);
	
	// If Firing doesn't simulate projectiles, use this
	UFUNCTION(BlueprintCallable, Category = "Base Weapon")
		void TraceAttack() override;

	// Spawn projectile from gun's muzzle
	UFUNCTION(BlueprintCallable, Category = "Ranged Weapon")
		void FireProjectile();

	UFUNCTION(BlueprintCallable, Category = "Ranged Weapon")
		void Reload();

	UFUNCTION(BlueprintCallable, Category = "Ranged Weapon")
		void SetLaserSight(UParticleSystem* p_particle);

	UFUNCTION(BlueprintCallable, Category = "Ranged Weapon")
		void UpdateLaserSightPosition();

protected:
	virtual void Tick(float DeltaTime) override;

	UParticleSystemComponent* LaserSight;

};
