// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Animation/AnimInstance.h"
#include "PlayerAnimInstance.generated.h"

/**
 * 
 */
UCLASS()
class THIRDPERSONRPG_API UPlayerAnimInstance : public UAnimInstance
{
	GENERATED_BODY()
	
public:
	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Character Movement")
		float Speed;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Character Movement")
		float Direction;

public:
	// IK Variables
	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Foot IK")
		FVector RightEffector;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Foot IK")
		FVector LeftEffector;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Foot IK")
		FVector RightJointPos;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Foot IK")
		FVector LeftJointPos;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Foot IK")
		FVector HipTranslation;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Foot IK")
		float LeftFootAlpha;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Foot IK")
		float RightFootAlpha;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Foot IK")
		FRotator RightRotation;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Foot IK")
		FRotator LeftRotation;

};
