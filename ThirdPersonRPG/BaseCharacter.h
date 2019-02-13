// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "GameFramework/Character.h"
#include "BaseCharacter.generated.h"

UCLASS()
class THIRDPERSONRPG_API ABaseCharacter : public ACharacter
{
	GENERATED_BODY()

	// For IK
	UPROPERTY(VisibleAnywhere, BlueprintReadWrite, Category = "Base Character", meta = (AllowPrivateAccess = "true"))
		class UBaseCharacterIKComponent* BaseIKComp;

public:
	// Sets default values for this character's properties
	ABaseCharacter();

	UFUNCTION(BlueprintCallable, Category = "Base Character")
		virtual void UpdateHealth(float p_health);

	// Checks if Character is moving
	UPROPERTY(BlueprintReadWrite, VisibleAnywhere, Category = "Base Character")
		bool bIsMoving;

protected:
	// Called when the game starts or when spawned
	virtual void BeginPlay() override;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Base Character")
		float Health = 100; // Current health

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Base Character")
		float MaxHealth = 100;

	UPROPERTY(BlueprintReadOnly, Category = "Base Character")
		bool bAlive = true;

	UFUNCTION(BlueprintCallable, Category = "Base Character")
		virtual void Attack();

public:	
	// Called every frame
	virtual void Tick(float DeltaTime) override;

	// Called to bind functionality to input
	virtual void SetupPlayerInputComponent(class UInputComponent* PlayerInputComponent) override;

protected:
	UPROPERTY(EditAnywhere, Category = "Base Character")
		TArray <TSubclassOf<class ABaseWeapon> > WeaponList;

	UPROPERTY(BlueprintReadOnly, EditAnywhere, Category = "Base Character")
		class ABaseWeapon* CurrentWeapon;
	
};
