// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "Blueprint/UserWidget.h"
#include "WeaponWheelBase.generated.h"

/**
 * 
 */
UCLASS()
class THIRDPERSONRPG_API UWeaponWheelBase : public UUserWidget
{
	GENERATED_BODY()

protected:

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Weapon Wheel")
		FVector2D WheelOrigin;

	UPROPERTY(BlueprintReadWrite, EditAnywhere, Category = "Weapon Wheel")
		TArray<bool> bSelectedButton;
	
public:
	int32 CurrentSelectedIndex;

	UFUNCTION(BlueprintCallable, Category = "Weapon Wheel")
		void GetCurrentSelected();

	UFUNCTION(BlueprintCallable, Category = "Weapon Wheel")
		void SelectButton(float p_angle);
	
};
