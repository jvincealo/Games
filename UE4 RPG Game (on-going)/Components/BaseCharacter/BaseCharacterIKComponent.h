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

	UFUNCTION(BlueprintCallable, Category = "Base Character Foot IK")
		void ProcessFootIK(float p_deltaTime);

protected:
	// Called when the game starts
	virtual void BeginPlay() override;

	//UPROPERTY(BlueprintReadOnly, VisibleAnywhere, Category = "Base Character Foot IK")
	ACharacter* OwningCharacter;

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
		FRotator RightRotationAnim;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		FRotator LeftRotationAnim;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float TraceDistance = 55.0f;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float RightFootOffset;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float LeftFootOffset;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float HipOffset;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		FRotator RightRotation;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		FRotator LeftRotation;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float AdjustFootOffset = 2.0f;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float InterpSpeed = 13.0f;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float InterpSpeedHip = 7.0f;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Base Character Foot IK")
		float DeltaTimeSecondsIK;

	void UpdateFootIK();
	void ResetFootIKVariables();
	float FootTraceIK(FName p_socketName, FRotator& out_footRotation);
	void UpdateFootRotation(FRotator p_targetValue, FRotator& out_rotator);
	void UpdateFootOffset(float p_targetValue, float& out_effector, float p_interpSpeed);
	void UpdateCapsuleHalfHeight(float p_hipShift, bool p_resetValue);
	void UpdateAnimParams();

};
