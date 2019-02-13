// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Components/ActorComponent.h"
#include "BaseCharacterIKComponent.generated.h"


UCLASS( ClassGroup=(Custom), meta=(BlueprintSpawnableComponent) )
class THIRDPERSONRPG_API UBaseCharacterIKComponent : public UActorComponent
{
	GENERATED_BODY()

public:	
	// Sets default values for this component's properties
	UBaseCharacterIKComponent();

	void SetOwningCharacter(class ACharacter* p_character);

	UFUNCTION(BlueprintCallable, Category = "Base Character Foot IK")
		void ProcessFootIK(float p_deltaTime);

	UFUNCTION(BlueprintCallable, Category = "Base Character Foot IK")
		void SetFootIKParams();

protected:
	// Called when the game starts
	virtual void BeginPlay() override;

	//UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Category = "Base Character Foot IK")
	ACharacter* OwningCharacter;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float MaxFootAlpha = 0.85f;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float MaxHeightDivider = 34.0f; // preferred constant for height checker during foot trace

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float MaxHeightClamp = 0.9f;

public:	
	// Called every frame
	virtual void TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction) override;

protected:
	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		bool bIsUsingFootIK;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float CapsuleHalfHeightFootIK;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float HipOffsetAnim;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float RightEffectorAnim;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float LeftEffectorAnim;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float TraceDistance = 150.0f;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float RightFootOffset;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float LeftFootOffset;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float HipOffset;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float AdjustFootOffset;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float InterpSpeed;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float InterpSpeedHip;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float DeltaTimeSecondsIK;

	UFUNCTION(BlueprintCallable, Category = "Base Character Foot IK")
		void UpdateFootIK();

	UFUNCTION(BlueprintCallable, Category = "Base Character Fot IK")
		void ResetFootIKVariables();

	UFUNCTION(BlueprintCallable, Category = "Base Character Foot IK")
		float FootTraceIK(FName p_socketName);

	UFUNCTION(BlueprintCallable, Category = "Base Character Foot IK")
		void UpdateFootOffset(float p_targetValue, float& out_effector, float p_interpSpeed);

	UFUNCTION(BlueprintCallable, Category = "Base Character Foot IK")
		void UpdateCapsuleHalfHeight(float p_hipShift, bool p_resetValue);


};
