// Fill out your copyright notice in the Description page of Project Settings.

#include "BaseCharacterIKComponent.h"
#include "BaseCharacter.h"
#include "GameFramework/Actor.h"
#include "GameFramework/Character.h"
#include "Components/SkeletalMeshComponent.h"
#include "Components/CapsuleComponent.h"
#include "Kismet/GameplayStatics.h"
#include "Kismet/KismetMathLibrary.h"
#include "Animation/PlayerAnimInstance.h"

#include "Misc/StaticHelpers.h"

#include "Runtime/Engine/Public/DrawDebugHelpers.h"

// Sets default values for this component's properties
UBaseCharacterIKComponent::UBaseCharacterIKComponent()
{
	// Set this component to be initialized when the game starts, and to be ticked every frame.  You can turn these features
	// off to improve performance if you don't need them.
	PrimaryComponentTick.bCanEverTick = true;

}


// Called when the game starts
void UBaseCharacterIKComponent::BeginPlay()
{
	Super::BeginPlay();
	OwningCharacter = Cast<ACharacter>(GetOwner());
	if (OwningCharacter) {
		CapsuleHalfHeightFootIK = OwningCharacter->GetCapsuleComponent()->GetScaledCapsuleHalfHeight();
	}
}


// Called every frame
void UBaseCharacterIKComponent::TickComponent(float DeltaTime, ELevelTick TickType, FActorComponentTickFunction* ThisTickFunction)
{
	Super::TickComponent(DeltaTime, TickType, ThisTickFunction);

	// ...
}

#pragma region IK
void UBaseCharacterIKComponent::ProcessFootIK(float p_deltaTime)
{
	if (!OwningCharacter)
	{
		return;
	}

	DeltaTimeSecondsIK = p_deltaTime;

	UPlayerAnimInstance* animInstance = Cast<UPlayerAnimInstance>(OwningCharacter->GetMesh()->GetAnimInstance());
	if (animInstance && animInstance->Speed > 0.0f)
	{
		ResetFootIKVariables();
	}
	else
	{
		UpdateFootIK();
	}
}

void UBaseCharacterIKComponent::ResetFootIKVariables()
{
	UpdateFootOffset(0.0f, LeftEffectorAnim, InterpSpeed);
	UpdateFootOffset(0.0f, RightEffectorAnim, InterpSpeed);
	UpdateFootRotation(FRotator::ZeroRotator, LeftRotationAnim);
	UpdateFootRotation(FRotator::ZeroRotator, RightRotationAnim);
	UpdateFootOffset(0.0f, HipOffsetAnim, InterpSpeedHip);
	UpdateCapsuleHalfHeight(0.0f, true);

	UpdateAnimParams();
}

float UBaseCharacterIKComponent::FootTraceIK(FName p_socketName, FRotator& out_footRotation)
{
	FVector actorLoc = OwningCharacter->GetActorLocation();
	FVector socketLoc = OwningCharacter->GetMesh()->GetSocketLocation(p_socketName);

	FVector startPos = FVector(socketLoc.X, socketLoc.Y, actorLoc.Z);
	FVector endPos = FVector(socketLoc.X, socketLoc.Y, actorLoc.Z - (TraceDistance + CapsuleHalfHeightFootIK));

	UWorld* World = GetWorld();
	if (World)
	{
		DrawDebugLine(World, startPos, endPos, FColor(0, 0, 255), true, 5.0f);
		FHitResult* HitResult = new FHitResult();
		if (World->LineTraceSingleByChannel(*HitResult, startPos, endPos, ECC_Visibility))
		{
			out_footRotation = UStaticHelpers::ConvertNormalToRotator(HitResult->Normal);
			return (HitResult->Location - HitResult->TraceEnd).Size() - TraceDistance + AdjustFootOffset;
		}
		else
		{
			return 0.0f;
		}
	}

	return 0;
}

void UBaseCharacterIKComponent::UpdateFootIK()
{
	UPlayerAnimInstance* animInstance = Cast<UPlayerAnimInstance>(OwningCharacter->GetMesh()->GetAnimInstance());

	if (animInstance)
	{
		// Feet line trace to get offset
		RightFootOffset = FootTraceIK(FName("RightFootSocket_IK"), RightRotation);
		LeftFootOffset = FootTraceIK(FName("LeftFootSocket_IK"), LeftRotation);

		// Update feet rotation
		UpdateFootRotation(LeftRotation, LeftRotationAnim);
		UpdateFootRotation(RightRotation, RightRotationAnim);
		LeftRotationAnim = LeftRotation;
		RightRotationAnim = RightRotation;

		//Update Hip offset
		HipOffset = FMath::Max(LeftFootOffset, RightFootOffset);
		HipOffset = HipOffset > 0 ? 0 : HipOffset; // Set to 0 if larger than 0
		UpdateFootOffset(HipOffset, HipOffsetAnim, InterpSpeedHip);
		UpdateCapsuleHalfHeight(HipOffset, false);

		// Update feet based on hip offset
		UpdateFootOffset(LeftFootOffset - HipOffset, LeftEffectorAnim, InterpSpeed);
		UpdateFootOffset(RightFootOffset - HipOffset, RightEffectorAnim, InterpSpeed);

		UpdateAnimParams();
	}
}

void UBaseCharacterIKComponent::UpdateFootRotation(FRotator p_targetValue, FRotator& out_rotator)
{
	out_rotator = FMath::RInterpTo(out_rotator, p_targetValue, DeltaTimeSecondsIK, InterpSpeed);
}

void UBaseCharacterIKComponent::UpdateFootOffset(float p_targetValue, float& out_effector, float p_interpSpeed)
{
	out_effector = FMath::FInterpTo(out_effector, p_targetValue, DeltaTimeSecondsIK, p_interpSpeed);
}

void UBaseCharacterIKComponent::UpdateCapsuleHalfHeight(float p_hipShift, bool p_resetValue)
{
	float heightTarget = p_resetValue ? CapsuleHalfHeightFootIK : CapsuleHalfHeightFootIK - (FMath::Abs(p_hipShift) / 2.0f);
	float nextVal = FMath::FInterpTo(OwningCharacter->GetCapsuleComponent()->GetScaledCapsuleHalfHeight(), heightTarget, DeltaTimeSecondsIK, InterpSpeedHip);
	OwningCharacter->GetCapsuleComponent()->SetCapsuleHalfHeight(nextVal);
}

void UBaseCharacterIKComponent::UpdateAnimParams()
{
	UPlayerAnimInstance* animInstance = Cast<UPlayerAnimInstance>(OwningCharacter->GetMesh()->GetAnimInstance());

	// Set anim instance values
	animInstance->LeftEffector = FVector(LeftEffectorAnim, 0.0f, 0.0f);;
	animInstance->RightEffector = FVector(RightEffectorAnim * -1.0f, 0.0f, 0.0f);
	animInstance->HipTranslation = FVector(0.0f, 0.0f, HipOffsetAnim);
	animInstance->LeftRotation = LeftRotationAnim;
	animInstance->RightRotation = RightRotationAnim;
}
#pragma endregion
