// Fill out your copyright notice in the Description page of Project Settings.

#include "WeaponWheelBase.h"
#include "Kismet/GameplayStatics.h"
#include "Kismet/KismetMathLibrary.h"
#include "Blueprint/WidgetLayoutLibrary.h"

void UWeaponWheelBase::GetCurrentSelected()
{
	/* IF USING MOUSE */
	float targetX = WheelOrigin.X;
	float targetY = WheelOrigin.Y;

	//UE_LOG(LogTemp, Warning, TEXT("before (%f , %f) >>> (%f , %f)"), WheelOrigin.X, WheelOrigin.Y, targetX, targetY);
	UWidgetLayoutLibrary::GetMousePositionScaledByDPI(UGameplayStatics::GetPlayerController(GetWorld(), 0), targetX, targetY);

	//UE_LOG(LogTemp, Warning, TEXT("after (%f , %f) >>> (%f , %f)"), WheelOrigin.X, WheelOrigin.Y, targetX, targetY);
	float Angle = UKismetMathLibrary::FindLookAtRotation(FVector(WheelOrigin.X, WheelOrigin.Y, 0.0f), FVector(targetX, targetY, 0.0f)).Yaw + 90.0f;

	SelectButton(Angle);
}

void UWeaponWheelBase::SelectButton(float p_angle)
{
	float AngleInterval = 360.0f / bSelectedButton.Num(); // Number of buttons
	float LowerBounds = (AngleInterval / 2) - AngleInterval; // Halved for the two boundaries of the button
	for (int32 i = 0; i < bSelectedButton.Num(); i++)
	{
		for (int32 btn_index = 0; btn_index < bSelectedButton.Num(); btn_index++)
		{
			//UE_LOG(LogTemp, Warning, TEXT("lower bounds: %f"), LowerBounds);
			//UE_LOG(LogTemp, Warning, TEXT("higher boundsis: %f"), LowerBounds + AngleInterval);
			//UE_LOG(LogTemp, Warning, TEXT("angle is: %f"), p_angle);
			bSelectedButton[i] = (LowerBounds <= p_angle && LowerBounds + AngleInterval > p_angle);
			if (p_angle < -45.0f && p_angle >= -90.0f)
			{
				bSelectedButton[i] = (LowerBounds <= p_angle + 360.0f && LowerBounds + AngleInterval > p_angle + 360.0f);
			}
		}
		if (bSelectedButton[i])
			CurrentSelectedIndex = i;
		LowerBounds += AngleInterval; // Lower bounds
	}
}



