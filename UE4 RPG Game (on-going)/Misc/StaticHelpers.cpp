// Fill out your copyright notice in the Description page of Project Settings.

#include "StaticHelpers.h"
#include "Kismet/KismetMathLibrary.h"

FRotator UStaticHelpers::ConvertNormalToRotator(FVector p_normal)
{
	float rotX = UKismetMathLibrary::DegAtan2(p_normal.Y, p_normal.Z);
	float rotY = UKismetMathLibrary::DegAtan2(p_normal.X, p_normal.Z) * -1.0f;
	return FRotator(rotY, 0.0f, rotX);
}

