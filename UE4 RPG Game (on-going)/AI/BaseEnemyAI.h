// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "AIController.h"
#include "BaseEnemyAI.generated.h"

/**
 * 
 */
UCLASS()
class THIRDPERSONRPG_API ABaseEnemyAI : public AAIController
{
	GENERATED_BODY()
	
public:
	ABaseEnemyAI();

	virtual void Possess(APawn* Pawn) override;
	virtual void Tick(float DeltaSeconds) override;
	virtual FRotator GetControlRotation() const override;

protected:
	virtual void BeginPlay() override;

	class AEnemyCharacter* EnemyBot; // Own EnemyCharacter script reference

	class UBehaviorTreeComponent* BehaviorComp;
	class UBlackboardComponent* BlackboardComp;

	UFUNCTION()
		void OnPawnDetected(const TArray<AActor*> &DetectedPawns);

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Enemy AI")
		float AISightRadius = 500.0f;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Enemy AI")
		float AISightAge = 5.0f;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Enemy AI")
		float AILoseSightRadius = AISightRadius + 50.0f;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Enemy AI")
		float AIFieldOfView = 90.0f;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Enemy AI")
		class UAISenseConfig_Sight* SightConfig;

protected:
	int32 CurrentWaypoint;
	class APlayerCharacter* PlayerRef;

	UFUNCTION(BlueprintCallable, Category = "Enemy AI")
		void Idle();

	UFUNCTION(BlueprintCallable, Category = "Enemy AI")
		void Patrol();
	
	UFUNCTION(BlueprintCallable, Category = "Enemy AI")
		void Chase();

	UFUNCTION(BlueprintCallable, Category = "Enemy AI")
		void Investigate();

	UFUNCTION(BlueprintCallable, Category = "Enemy AI")
		void Combat();

protected:
	UFUNCTION(BlueprintCallable, Category = "Enemy AI")
		void UpdateLookDirection();

	UFUNCTION(BlueprintCallable, Category = "Enemy AI")
		AEnemyCharacter* GetControlledEnemy() const { return EnemyBot; }

	UFUNCTION(BlueprintCallable, Category = "Enemy AI")
		bool WithinCombatRange();

};
