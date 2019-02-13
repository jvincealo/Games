// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "BaseCharacter.h"
#include "EnemyCharacter.generated.h"

UENUM(BlueprintType)
enum class EnemyState : uint8 {
	IDLE,
	PATROL,
	CHASE,
	INVESTIGATE,
	COMBAT
};

/**
 * 
 */
UCLASS()
class THIRDPERSONRPG_API AEnemyCharacter : public ABaseCharacter
{
	GENERATED_BODY()

public:
	AEnemyCharacter();

	UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Category = "Enemy AI")
		EnemyState CurrentState = EnemyState::IDLE;

	UPROPERTY(EditDefaultsOnly, Category = "Enemy AI")
		class UBehaviorTree* BehaviorTree;

	UPROPERTY(EditAnywhere, Category = "Enemy AI")
		TArray <AActor*> Waypoints;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Enemy AI")
		float EnemySightRadius;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Enemy AI")
		float EnemyAttackRange;

protected:
	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Enemy Character")
		float ExpReward;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Enemy Character")
		float GoldReward;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Enemy Character")
		float Damage;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Enemy Character")
		float AttackSpeed;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Enemy Character")
		float AttackRadius;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Enemy Character")
		bool bBoss = false;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Enemy Character")
		bool bDoingCombat = false; // Is the enemy exceuting a combat animation

protected:
	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Enemy Character")
		bool bPredictAttackActive = false;

	UFUNCTION(BlueprintCallable, Category = "Enemy Combat")
		class APlayerCharacter* TraceAttack(); // Trace for player hit

	UFUNCTION(BlueprintCallable, Category = "Enemy Combat")
		void Attack() override; // Damage player

	UFUNCTION(BlueprintCallable, Category = "Enemy Combat")
		void PredictAttackPath();

public:
	void Tick(float DeltaTime) override;

	UFUNCTION(BlueprintCallable, Category = "Enemy Character")
		void SetDoingCombat(bool p_isDoingCombat) { bDoingCombat = p_isDoingCombat; }

	UFUNCTION(BlueprintCallable, Category = "Enemy Character")
		bool IsDoingCombat() const { return bDoingCombat; }
	
	UFUNCTION(BlueprintCallable, Category = "Enemy Character")
		float GetAttackRadius() const { return AttackRadius; }

protected:
	virtual void BeginPlay() override;

	void InitializeEnemy();

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Enemy VFX")
		UParticleSystem* LaserSight;


};
