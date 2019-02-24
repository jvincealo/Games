// Fill out your copyright notice in the Description page of Project Settings.

#pragma once

#include "CoreMinimal.h"
#include "UObject/NoExportTypes.h"
#include "StaticHelpers.generated.h"

/**
 * Storage class for all function helpers
 */
UCLASS()
class THIRDPERSONRPG_API UStaticHelpers : public UObject
{
	GENERATED_BODY()

	
public:
	static FRotator ConvertNormalToRotator(FVector p_normal);
	
};
